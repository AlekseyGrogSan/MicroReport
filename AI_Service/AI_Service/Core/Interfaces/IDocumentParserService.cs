namespace AI_Service.Core.Interfaces
{
    public interface IDocumentParserService
    {
        Task<string> AgreggateDocumentTextsAsync(List<string> documentS3Keys, CancellationToken cancellationToken);
    }
}