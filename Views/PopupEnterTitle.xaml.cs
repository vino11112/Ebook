namespace Ebook.Views;

public partial class PopupEnterTitle : ContentPage
{
    private readonly TaskCompletionSource<string?> _tcs = new();

    public PopupEnterTitle()
    {
        InitializeComponent();
    }

    public Task<string?> ShowAsync()
    {
        Application.Current.MainPage.Navigation.PushModalAsync(this);
        return _tcs.Task;
    }

    private async void OnConfirmClicked(object sender, EventArgs e)
    {
        var text = EntryTitle.Text;
        await Application.Current.MainPage.Navigation.PopModalAsync();
        _tcs.SetResult(text);
    }
}
