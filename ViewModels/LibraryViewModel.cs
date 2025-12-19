using System.Collections.ObjectModel;
using System.Windows.Input;
using Ebook.Models;
using Ebook.Services;
using Ebook.Views;

namespace Ebook.ViewModels;

public class LibraryViewModel : BaseViewModel
{
    private readonly IBookRepository _repo;
    private readonly IPdfPageCacheService _cache;
    private readonly IFileService _file; 

    public ObservableCollection<Book> Books { get; } = new();

    private List<Book> _allBooks = new();

    private string _searchQuery = string.Empty;
    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            if (SetProperty(ref _searchQuery, value))
                ApplyFilter();
        }
    }

    public ICommand ImportCommand { get; }
    public ICommand OpenBookCommand { get; }
    public ICommand DeleteBookCommand { get; }

    public LibraryViewModel(
        IBookRepository repo,
        IPdfPageCacheService cache,
        IFileService file)
    {
        _repo = repo;
        _cache = cache;
        _file = file;

        ImportCommand = new Command(async () => await OpenImportPageAsync());
        OpenBookCommand = new Command<Book>(async b => await OpenBookAsync(b));
        DeleteBookCommand = new Command<Book>(async b => await DeleteBookAsync(b));

        EventHub.BookImported += OnBookImported;
        EventHub.BookUpdated += OnBookUpdated;
        EventHub.BookRenderFinished += OnBookRenderFinished;
    }


    public async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            var list = await _repo.GetAllAsync();

            _allBooks = list
                .OrderByDescending(b => b.LastOpened ?? b.CreatedAt)
                .ToList();

            ApplyFilter();

            foreach (var b in _allBooks)
                _ = LoadCoverAsync(b);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ApplyFilter()
    {
        IEnumerable<Book> src = _allBooks;

        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            var q = SearchQuery.Trim().ToLowerInvariant();
            src = src.Where(b => (b.Title ?? string.Empty).ToLowerInvariant().Contains(q));
        }

        Books.Clear();
        foreach (var b in src)
            Books.Add(b);
    }

    private async Task LoadCoverAsync(Book b)
    {
        if (b.CoverImage != null) return;

        var cover = await _cache.GetCoverAsync(b, 300, 450);
        if (cover == null) return;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            b.CoverImage = cover;
        });
    }


    private async Task OpenImportPageAsync()
    {
        await Shell.Current.GoToAsync(nameof(ImportPage));
    }


    private async Task OpenBookAsync(Book? book)
    {
        if (book == null) return;

        if (!book.IsReady)
        {
            await App.Current.MainPage.DisplayAlert(
                "Подождите",
                "Книга ещё рендерится…",
                "ОК");
            return;
        }

        await Shell.Current.GoToAsync(nameof(ReaderPage),
            new Dictionary<string, object>
            {
                { "BookId", book.Id }
            });
    }

    private async Task DeleteBookAsync(Book? book)
    {
        if (book == null) return;

        bool confirm = await App.Current.MainPage.DisplayAlert(
            "Удалить книгу?",
            $"\"{book.Title}\" будет удалена.",
            "Удалить", "Отмена");

        if (!confirm) return;

        await _repo.DeleteAsync(book);

        if (File.Exists(book.FilePath))
        {
            try { File.Delete(book.FilePath); }
            catch { /* пофиг */ }
        }

        _allBooks.RemoveAll(b => b.Id == book.Id);
        Books.Remove(book);
    }


    private void OnBookImported(Book b)
    {
        _allBooks.Add(b);
        _allBooks = _allBooks
            .OrderByDescending(x => x.LastOpened ?? x.CreatedAt)
            .ToList();

        ApplyFilter();
        _ = LoadCoverAsync(b);
    }

    private void OnBookUpdated(Book b)
    {
        var local = _allBooks.FirstOrDefault(x => x.Id == b.Id);
        if (local == null) return;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            local.RenderProgress = b.RenderProgress;
            local.IsReady = b.IsReady;
            local.TotalPages = b.TotalPages;
            local.LastOpened = b.LastOpened;
            local.LastPage = b.LastPage;

            ApplyFilter();
        });
    }

    private void OnBookRenderFinished(Book b)
    {

        OnBookUpdated(b);
    }
}
