using Ebook.ViewModels;

namespace Ebook.Views;

public partial class ReaderPage : ContentPage
{
    private readonly ReaderViewModel _vm;

    public ReaderPage(ReaderViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}
