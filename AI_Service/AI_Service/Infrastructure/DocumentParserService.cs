using AI_Service.Core.Interfaces;
using AI_Service.Core.Models;
using System.Runtime.CompilerServices;
using System.Text;
using UglyToad.PdfPig;

namespace AI_Service.Infrastructure
{
    public class DocumentParserService(IS3Service _storage) : IDocumentParserService
    {
        public async Task<string> AgreggateDocumentTextsAsync(
            List<string> documentS3Keys,
            CancellationToken cancellationToken)
        {
            var contextBuilder = new StringBuilder();

            foreach (var key in documentS3Keys)
            {
                using var fileStream = await _storage.DownloadFileAsync(key, cancellationToken);

                string text = Path.GetExtension(key).ToLower() switch
                {
                    ".pdf" => ExtractFromPdf(fileStream),
                    ".txt" or ".md" => await ExtractTextFromTextFileAsync(fileStream),
                    _ => string.Empty
                };

                contextBuilder.AppendLine($"--- ДОКУМЕНТ: {key} ---");
                contextBuilder.AppendLine(text);
                contextBuilder.AppendLine($"--- КОНЕЦ ДОКУМЕНТА ---\n");
            }

            return contextBuilder.ToString();
        }

        private string ExtractFromPdf(Stream stream)
        {
            var textBuilder = new StringBuilder();

            using var file = PdfDocument.Open(stream);
            foreach (var page in file.GetPages())
            {
                textBuilder.Append(page.Text);
            }
            return textBuilder.ToString();
        }

        private async Task<string> ExtractTextFromTextFileAsync(Stream stream)
        {
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }
    }
}
