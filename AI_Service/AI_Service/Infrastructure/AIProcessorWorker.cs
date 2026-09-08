using AI_Service.Core.Models;
using System.Threading.Channels;
using AI_Service.Core.Interfaces;
using AI_Service.Core.DTOs;
using Microsoft.Extensions.Options;
using AI_Service.Core.Settings;

namespace AI_Service.Infrastructure
{
    public class AIProcessorWorker(
        Channel<RequestModel> _channel,
        IServiceScopeFactory _factory,
        ILogger<AIProcessorWorker> _logger,
        IKafkaProducerService _producer,
        IOptions<KafkaSettings> _settings
        ) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach(var request in _channel.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    _logger.LogInformation("{Format}", request.ContentType);
                    using var scope = _factory.CreateScope();

                    var documentParser = scope.ServiceProvider.GetRequiredService<IDocumentParserService>();
                    var aiService = scope.ServiceProvider.GetRequiredService<IAIService>();
                    var reportExportService = scope.ServiceProvider.GetRequiredService<IReportExporterService>();

                    var aggregateText = await documentParser.AgreggateDocumentTextsAsync(request.DocumentS3Keys, stoppingToken);

                    var result = await aiService.GenerateAIReport(request, aggregateText, stoppingToken);

                    var res = await reportExportService.ExportReportAsync(result,request.RequestId, request.ContentType, stoppingToken);

                    var completed = new ReportCompletedEventDto(request.RequestId, request.UserId, res.filename, res.format, res.s3key);

                    var topic = _settings.Value.Topics["ReportCompleted"];

                    await _producer.ProduceAsync(topic, completed, stoppingToken);

                    _logger.LogInformation("Отчет отправлен на DocService: {RequestId}", completed.RequestId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при обработке запроса {RequestId}", request.RequestId);
                }
            }
        }
    }
}
