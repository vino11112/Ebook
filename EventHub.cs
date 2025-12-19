using Ebook.Models;
namespace Ebook;

public static class EventHub
{
    public static event Action<Book>? BookUpdated;
    public static event Action<Book>? BookImported;
    public static event Action<Book>? BookRenderFinished;

    public static void RaiseBookUpdated(Book b)
        => BookUpdated?.Invoke(b);

    public static void RaiseBookImported(Book b)
        => BookImported?.Invoke(b);

    public static void RaiseBookRenderFinished(Book b)
        => BookRenderFinished?.Invoke(b);
}
