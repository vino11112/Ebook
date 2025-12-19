using Ebook.Models;

namespace Ebook.Services;

public interface IPdfPageCacheService
{

    Task<ImageSource?> GetPageImageAsync(Book book, int page, int width, int height);


    Task<ImageSource?> GetCoverAsync(Book book, int width, int height);


    Task PreloadAroundAsync(Book book, int centerPage, int width, int height, int radius = 2);
}
