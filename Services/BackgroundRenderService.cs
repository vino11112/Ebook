using System.Threading;
using Ebook.Models;

namespace Ebook.Services;

public class BackgroundRenderService : IBackgroundRenderService
{
    private readonly IBookRepository _repo;
    private readonly IPdfRenderer _renderer;

    public BackgroundRenderService(IBookRepository repo, IPdfRenderer renderer)
    {
        _repo = repo;
        _renderer = renderer;
    }

    public async Task PreRenderBookAsync(
        Book book,
        IProgress<double>? progress = null,
        CancellationToken token = default)
    {
        if (book == null) return;

        var totalPages = await _renderer.GetPageCountAsync(book.FilePath);
        if (totalPages <= 0) totalPages = 1;

        book.TotalPages = totalPages;
        book.IsReady = false;
        book.RenderProgress = 0;

        await _repo.UpdateAsync(book);
        EventHub.RaiseBookUpdated(book);

        for (int i = 0; i < totalPages; i++)
        {
            token.ThrowIfCancellationRequested();

            await _renderer.RenderPageBytesAsync(book.FilePath, i, 900, 0);

            double p = (double)(i + 1) / totalPages;
            book.RenderProgress = p;

            progress?.Report(p);
            EventHub.RaiseBookUpdated(book);

            await Task.Delay(5);
        }

        book.IsReady = true;
        book.RenderProgress = 1;
        await _repo.UpdateAsync(book);

        EventHub.RaiseBookUpdated(book);
        EventHub.RaiseBookRenderFinished(book);

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Книга готова",
                    $"\"{book.Title}\" дорендерилась и готова к чтению.",
                    "OK");
            }
            catch { }
        });
    }

    public async void StartRender(int bookId)
    {
        var book = await _repo.GetByIdAsync(bookId);
        if (book == null) return;

        await PreRenderBookAsync(book);
    }
}
