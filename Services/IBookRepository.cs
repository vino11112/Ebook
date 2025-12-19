using Ebook.Models;

namespace Ebook.Services;

public interface IBookRepository
{
    Task<List<Book>> GetAllAsync(string search = "");
    Task<Book?> GetByIdAsync(int id);

    Task AddAsync(Book book);
    Task UpdateAsync(Book book);
    Task DeleteAsync(Book book);
    Task AddOrUpdateAsync(Book book);
}
