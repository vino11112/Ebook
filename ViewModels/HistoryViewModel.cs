using System.Collections.ObjectModel;
using System.Windows.Input;
using Ebook.Models;
using Ebook.Services;

namespace Ebook.ViewModels;

public class HistoryViewModel : BaseViewModel
{
    private readonly IBookRepository _repo;
    private readonly IPdfPageCacheService _cache;

    public ObservableCollection<Book> History { get; } = new();

    public ICommand OpenBookCommand { get; }
    public ICommand SetRatingCommand { get; }

    public HistoryViewModel(
        IBookRepository repo,
        IPdfPageCacheService cache)
    {
        _repo = repo;
        _cache = cache;

        OpenBookCommand = new Command<Book>(async b => await OpenBookAsync(b));
        SetRatingCommand = new Command<(Book, int)>(
            async pair => await SetRatingAsync(pair.Item1, pair.Item2));
    }

    // ------------------------------------------------------------
    // Загрузка истории чтения
    // ------------------------------------------------------------
    public async Task LoadHistoryAsync()
    {
        IsBusy = true;

        try
        {
            var books = await _repo.GetAllAsync();

            var openedBooks = books
                .Where(b => b.LastOpened != null)
                .OrderByDescending(b => b.LastOpened)
                .ToList();

            History.Clear();

            foreach (var book in openedBooks)
            {
                History.Add(book);
                _ = LoadCoverAsync(book);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    // ------------------------------------------------------------
    // Загрузить обложку книги
    // ------------------------------------------------------------
    private async Task LoadCoverAsync(Book book)
    {
        if (book.CoverImage != null) return;

        var cover = await _cache.GetCoverAsync(book, 300, 450);
        if (cover == null) return;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            book.CoverImage = cover;
        });
    }

    // ------------------------------------------------------------
    // Открыть выбранную книгу
    // ------------------------------------------------------------
    private async Task OpenBookAsync(Book book)
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

        await Shell.Current.GoToAsync(nameof(Ebook.Views.ReaderPage), true,
            new Dictionary<string, object>
            {
                { "BookId", book.Id }
            });
    }

    // ------------------------------------------------------------
    // Поставить рейтинг книге
    // ------------------------------------------------------------
    private async Task SetRatingAsync(Book book, int rating)
    {
        if (book == null) return;

        book.Rating = rating;
        await _repo.AddOrUpdateAsync(book);

        EventHub.RaiseBookUpdated(book);
    }
}
