using Ebook.ViewModels;

namespace Ebook.Views;

public partial class ImportPage : ContentPage
{
    public ImportPage(ImportViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
