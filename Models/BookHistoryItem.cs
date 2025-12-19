using Microsoft.Maui.Controls;
using Ebook.ViewModels;

namespace Ebook.Models;

public class BookHistoryItem : BaseViewModel
{
    public Book SourceBook { get; }

    public int Id => SourceBook.Id;

    public string Title => SourceBook.Title;

    private ImageSource? _cover;
    public ImageSource? CoverImage
    {
        get => _cover;
        set => SetProperty(ref _cover, value);
    }

    public int LastPage => SourceBook.LastPage;
    public int TotalPages => SourceBook.TotalPages;

    public DateTime? LastOpened => SourceBook.LastOpened;

    public string ProgressText =>
        TotalPages > 0
            ? $"Страница {LastPage} из {TotalPages}"
            : "Прогресс неизвестен";

    public BookHistoryItem(Book book)
    {
        SourceBook = book;
    }
}
