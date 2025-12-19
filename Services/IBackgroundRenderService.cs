using System.Threading;
using Ebook.Models;

namespace Ebook.Services;

public interface IBackgroundRenderService
{

    Task PreRenderBookAsync(
        Book book,
        IProgress<double>? progress = null,
        CancellationToken token = default);

    void StartRender(int bookId);
}
