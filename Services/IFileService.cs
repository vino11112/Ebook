public interface IFileService
{
    Task<string?> PickPdfAsync();


    Task<string?> DownloadPdfFromUrlAsync(string url);
}
