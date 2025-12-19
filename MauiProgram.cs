using Ebook.Services;
using Ebook.ViewModels;
using Ebook.Views;

namespace Ebook;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // ---------- SERVICES ----------
        builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
        builder.Services.AddSingleton<IBookRepository, BookRepository>();
        builder.Services.AddSingleton<IFileService, FileService>();
        builder.Services.AddSingleton<IPdfPageCacheService, PdfPageCacheService>();
        builder.Services.AddSingleton<IBackgroundRenderService, BackgroundRenderService>();
        builder.Services.AddSingleton<PageCache>();
        builder.Services.AddSingleton<ScreenInfoService>();

        // Новый сервис настроек чтения
        builder.Services.AddSingleton<ISettingsService, SettingsService>();

#if ANDROID
        builder.Services.AddSingleton<IPdfRenderer, Ebook.Platforms.Android.AndroidPdfRenderer>();
#else
        builder.Services.AddSingleton<IPdfRenderer, StubPdfRenderer>();
#endif

        // ---------- VIEWMODELS ----------
        builder.Services.AddTransient<LibraryViewModel>();
        builder.Services.AddTransient<ImportViewModel>();
        builder.Services.AddTransient<ReaderViewModel>();
        builder.Services.AddTransient<HistoryViewModel>();

        // ---------- PAGES ----------
        builder.Services.AddTransient<LibraryPage>();
        builder.Services.AddTransient<ImportPage>();
        builder.Services.AddTransient<ReaderPage>();
        builder.Services.AddTransient<HistoryPage>();

        return builder.Build();
    }
}
