using Ebook.Views;

namespace Ebook;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(ReaderPage), typeof(ReaderPage));
        Routing.RegisterRoute(nameof(ImportPage), typeof(ImportPage));
    }
}
