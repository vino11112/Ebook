using System.Windows.Input;
using Ebook.Models;
using Ebook.Services;

namespace Ebook.ViewModels;

[QueryProperty(nameof(BookId), "BookId")]
public class ReaderViewModel : BaseViewModel
{
    private readonly IBookRepository _repo;
    private readonly IPdfPageCacheService _cache;

    public int BookId { get; set; }
    public Book? Book { get; private set; }

    private ImageSource? _pageImage;
    public ImageSource? PageImage
    {
        get => _pageImage;
        set => SetProperty(ref _pageImage, value);
    }

    private int _currentPage;
    public int CurrentPage
    {
        get => _currentPage;
        set
        {
            if (SetProperty(ref _currentPage, value))
                OnPropertyChanged(nameof(DisplayPageNumber));
            _ = LoadPageAsync(value);
        }
    }

    public int TotalPages => Book?.TotalPages ?? 1;
   
    private bool _isUiVisible = true;
    public bool IsUiVisible
    {
        get => _isUiVisible;
        set => SetProperty(ref _isUiVisible, value);
    }

    public ICommand NextPageCommand { get; }
    public ICommand PrevPageCommand { get; }
    public ICommand ZoomInCommand { get; }
    public ICommand ZoomOutCommand { get; }
    public ICommand ToggleUiCommand { get; }

    private double _zoom = 1.0;

    public ReaderViewModel(IBookRepository repo, IPdfPageCacheService cache)
    {
        _repo = repo;
        _cache = cache;

        NextPageCommand = new Command(NextPage);
        PrevPageCommand = new Command(PrevPage);
        ZoomInCommand = new Command(() => ChangeZoom(1.25));
        ZoomOutCommand = new Command(() => ChangeZoom(0.8));
        ToggleUiCommand = new Command(() => IsUiVisible = !IsUiVisible);
    }

    public async Task LoadAsync()
    {
        Book = await _repo.GetByIdAsync(BookId);
        if (Book == null) return;

        if (Book.LastPage < 0) Book.LastPage = 0;
        if (Book.LastPage >= Book.TotalPages && Book.TotalPages > 0)
            Book.LastPage = Book.TotalPages-1 ;

        CurrentPage = Book.LastPage;
        await LoadPageAsync(CurrentPage);
    }

    public async Task LoadPageAsync(int page)
    {
        if (Book == null) return;

        if (page < 0) page = 0;
        if (Book.TotalPages > 0 && page >= Book.TotalPages)
            page = Book.TotalPages - 1;

        // размер рендера (подогнан под нормальный баланс качество/скорость)
        int width = (int)(DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density);

        int height = (int)(1800 * _zoom);

        var img = await _cache.GetPageImageAsync(Book, page, width, height);
        if (img == null)
        {
            PageImage = null;
            return;
        }

        PageImage = img;

        _ = _cache.PreloadAroundAsync(Book, page, width, height, radius: 2);

        Book.LastPage = page;
        Book.LastOpened = DateTime.Now;
        await _repo.AddOrUpdateAsync(Book);

        OnPropertyChanged(nameof(DisplayPageNumber));

        OnPropertyChanged(nameof(TotalPages));
    }
    public int DisplayPageNumber => CurrentPage + 1;

    private void ChangeZoom(double factor)
    {
        _zoom *= factor;

        if (_zoom < 1.0)
            _zoom = 1.0;
        if (_zoom > 4.0)
            _zoom = 4.0;

        _ = LoadPageAsync(CurrentPage);
    }

    private void NextPage()
    {
        if (Book == null) return;
        if (CurrentPage < Book.TotalPages -1)
            CurrentPage++;
    }

    private void PrevPage()
    {
        if (CurrentPage >0)
            CurrentPage--;
    }
}
