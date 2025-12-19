using Ebook.Models;

namespace Ebook.Services;

public class PdfPageCacheService : IPdfPageCacheService
{
    private readonly IPdfRenderer _renderer;

    private readonly Dictionary<string, ImageSource> _cache = new();

    private readonly LinkedList<string> _lru = new();
    private readonly object _lock = new();


    private const int MaxEntries = 80;

    public PdfPageCacheService(IPdfRenderer renderer)
    {
        _renderer = renderer;
    }

    private string Key(Book b, int page, int w, int h)
        => $"{b.Id}_{page}_{w}_{h}";


    public async Task<ImageSource?> GetPageImageAsync(Book book, int page, int width, int height)
    {
        var key = Key(book, page, width, height);


        lock (_lock)
        {
            if (_cache.TryGetValue(key, out var cached))
            {
                Touch(key);
                return cached;
            }
        }


        var bytes = await _renderer.RenderPageBytesAsync(book.FilePath, page, width, height);
        if (bytes == null || bytes.Length < 100)
            return null;

        var img = ImageSource.FromStream(() => new MemoryStream(bytes));


        lock (_lock)
        {
            AddToCache(key, img);
        }

        return img;
    }

    public Task<ImageSource?> GetCoverAsync(Book book, int width, int height)
        => GetPageImageAsync(book, 0, width, height);


    public async Task PreloadAroundAsync(Book book, int centerPage, int width, int height, int radius = 2)
    {
        if (book.TotalPages <= 0)
            return;


        var pagesToLoad = new List<int>();

        for (int offset = -radius; offset <= radius; offset++)
        {
            if (offset == 0) continue;

            int p = centerPage + offset;
            if (p < 0 || p >= book.TotalPages)
                continue;

            var key = Key(book, p, width, height);

            lock (_lock)
            {
                if (_cache.ContainsKey(key))
                    continue; 
            }

            pagesToLoad.Add(p);
        }

        foreach (int p in pagesToLoad)
        {
            try
            {
                var key = Key(book, p, width, height);

                var bytes = await _renderer.RenderPageBytesAsync(book.FilePath, p, width, height);
                if (bytes == null || bytes.Length < 100)
                    continue;

                var img = ImageSource.FromStream(() => new MemoryStream(bytes));

                lock (_lock)
                {
                    AddToCache(key, img);
                }
            }
            catch
            {
              
            }
        }
    }


    private void AddToCache(string key, ImageSource img)
    {
        if (_cache.ContainsKey(key))
        {
            _cache[key] = img;
            Touch(key);
            return;
        }

        _cache[key] = img;
        _lru.AddLast(key);

        if (_lru.Count > MaxEntries)
        {

            var oldest = _lru.First!.Value;
            _lru.RemoveFirst();
            _cache.Remove(oldest);
        }
    }

    private void Touch(string key)
    {
        var node = _lru.Find(key);
        if (node == null) return;

        _lru.Remove(node);
        _lru.AddLast(node);
    }
}
