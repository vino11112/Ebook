using System.Windows.Input;
using Ebook.Models;
using Ebook.Services;

namespace Ebook.ViewModels;

public class ImportViewModel : BaseViewModel
{
    private readonly IFileService _file;
    private readonly IBookRepository _repo;
    private readonly IBackgroundRenderService _render;

    public string Title { get; set; } = "";
    public string FilePath { get; set; } = "";
    public string Url { get; set; } = "";

    public ICommand PickFileCommand { get; }
    public ICommand ImportCommand { get; }
    public ICommand ImportUrlCommand { get; }

    public ImportViewModel(
        IFileService file,
        IBookRepository repo,
        IBackgroundRenderService render)
    {
        _file = file;
        _repo = repo;
        _render = render;

        PickFileCommand = new Command(async () => await PickPdfAsync());
        ImportCommand = new Command(async () => await ImportLocalAsync());
        ImportUrlCommand = new Command(async () => await ImportFromUrlAsync());
    }

    private async Task PickPdfAsync()
    {
        var file = await _file.PickPdfAsync();
        if (file != null)
        {
            FilePath = file;
            OnPropertyChanged(nameof(FilePath));
        }
    }

    private async Task ImportLocalAsync()
    {
        if (string.IsNullOrWhiteSpace(FilePath))
        {
            await App.Current.MainPage.DisplayAlert("Ошибка", "Выберите PDF", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(Title))
        {
            await App.Current.MainPage.DisplayAlert("Ошибка", "Введите название", "OK");
            return;
        }

        var book = new Book
        {
            Title = Title.Trim(),
            FilePath = FilePath,
            IsReady = false,
            RenderProgress = 0
        };

        await _repo.AddAsync(book);

        EventHub.RaiseBookImported(book);
        _render.StartRender(book.Id);

        await Shell.Current.Navigation.PopAsync();
    }

    private async Task ImportFromUrlAsync()
    {
        if (string.IsNullOrWhiteSpace(Url))
        {
            await App.Current.MainPage.DisplayAlert("Ошибка", "Введите URL", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(Title))
        {
            await App.Current.MainPage.DisplayAlert("Ошибка", "Введите название книги", "OK");
            return;
        }

        IsBusy = true;

        try
        {
            var path = await _file.DownloadPdfFromUrlAsync(Url);

            if (path == null)
            {
                await App.Current.MainPage.DisplayAlert("Ошибка", "Не удалось скачать PDF", "OK");
                return;
            }

            var book = new Book
            {
                Title = Title.Trim(),
                FilePath = path,
                OriginalSource = Url,
                IsReady = false,
            };

            await _repo.AddAsync(book);
            EventHub.RaiseBookImported(book);

            _render.StartRender(book.Id);

            await Shell.Current.Navigation.PopAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }


}
