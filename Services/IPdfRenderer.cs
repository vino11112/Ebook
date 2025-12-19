namespace Ebook.Services;

public interface IPdfRenderer
{
    Task<int> GetPageCountAsync(string filePath);

    Task<byte[]?> RenderPageBytesAsync(
        string filePath,
        int pageIndex,
        int targetWidth,
        int targetHeight);
}
