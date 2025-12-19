using Microsoft.Maui.Controls;

namespace Ebook.ViewModels;

public class PdfPageViewModel : BaseViewModel
{
    private int _pageNumber;
    public int PageNumber
    {
        get => _pageNumber;
        set => SetProperty(ref _pageNumber, value);
    }

    private ImageSource? _image;
    public ImageSource? Image
    {
        get => _image;
        set => SetProperty(ref _image, value);
    }
}
