using Ebook.Models;
using SQLite;

namespace Ebook.Services;

public class BookRepository : IBookRepository
{
    private readonly SQLiteAsyncConnection _db;

    public BookRepository(IDatabaseService database)
    {
        _db = database.GetConnection();
        _ = InitAsync();
    }

    private async Task InitAsync()
    {
        await _db.CreateTableAsync<Book>();
    }

    public async Task<List<Book>> GetAllAsync(string search = "")
    {
        var list = await _db.Table<Book>().ToListAsync();

        if (!string.IsNullOrWhiteSpace(search))
            list = list.Where(b => b.Title.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

        list = list
            .OrderByDescending(b => b.LastOpened ?? b.CreatedAt)
            .ToList();

        return list;
    }

    public Task<Book?> GetByIdAsync(int id)
        => _db.FindAsync<Book>(id);

    public Task AddAsync(Book book)
        => _db.InsertAsync(book);

    public Task UpdateAsync(Book book)
        => _db.UpdateAsync(book);

    public Task DeleteAsync(Book book)
        => _db.DeleteAsync(book);

    public async Task AddOrUpdateAsync(Book book)
    {
        if (book.Id == 0)
            await _db.InsertAsync(book);
        else
            await _db.UpdateAsync(book);
    }
}
