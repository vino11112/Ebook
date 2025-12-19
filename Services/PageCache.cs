using Ebook.Models;

namespace Ebook.Services;

public class PageCache
{
    private readonly Dictionary<string, ImageSource> _cache = new();

    private string Key(Book b, int page) => $"{b.Id}_{page}";

    public bool TryGet(Book b, int page, out ImageSource? img)
        => _cache.TryGetValue(Key(b, page), out img);

    public void Save(Book b, int page, ImageSource img)
        => _cache[Key(b, page)] = img;
}
