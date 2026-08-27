namespace DocumentService.Core.DTOs
{
    public record DownloadfResult(
        Stream stream,
        string? contentType,
        string filename);
    
}
