using SQLite;
using Microsoft.Maui.Controls;
using Ebook.ViewModels;

namespace Ebook.Models;

public class Book : BaseViewModel
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    private string _title = string.Empty;
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    private string? _author;
    public string? Author
    {
        get => _author;
        set => SetProperty(ref _author, value);
    }

    private string _filePath = string.Empty;
    public string FilePath
    {
        get => _filePath;
        set => SetProperty(ref _filePath, value);
    }
    private int _rating;
    public int Rating
    {
        get => _rating;
        set => SetProperty(ref _rating, value);
    }


    public string SourceType { get; set; } = "Local";

    public string? OriginalSource { get; set; }

    private DateTime? _lastOpened;
    public DateTime? LastOpened
    {
        get => _lastOpened;
        set => SetProperty(ref _lastOpened, value);
    }

    private int _lastPage = 1;
    public int LastPage
    {
        get => _lastPage;
        set => SetProperty(ref _lastPage, value);
    }

    private int _totalPages;
    public int TotalPages
    {
        get => _totalPages;
        set => SetProperty(ref _totalPages, value);
    }

    public string? Category { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    private bool _isReady = false;
    public bool IsReady
    {
        get => _isReady;
        set => SetProperty(ref _isReady, value);
    }

    private double _renderProgress = 0;
    public double RenderProgress
    {
        get => _renderProgress;
        set => SetProperty(ref _renderProgress, value);
    }

    private ImageSource? _coverImage;
    [Ignore]
    public ImageSource? CoverImage
    {
        get => _coverImage;
        set => SetProperty(ref _coverImage, value);
    }

    [Ignore]
    public double Progress => TotalPages <= 0 ? 0 : (double)LastPage / TotalPages;
}
