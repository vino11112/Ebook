using SQLite;

namespace Ebook.Services;

public interface IDatabaseService
{
    SQLiteAsyncConnection GetConnection();
}
