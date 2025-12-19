namespace Ebook.Services;

public class StubPdfRenderer : IPdfRenderer
{
    public Task<int> GetPageCountAsync(string filePath)
        => Task.FromResult(1);

    public Task<byte[]?> RenderPageBytesAsync(
        string filePath,
        int pageIndex,
        int targetWidth,
        int targetHeight)
        => Task.FromResult<byte[]?>(null);
}
