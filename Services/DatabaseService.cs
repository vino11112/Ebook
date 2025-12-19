using SQLite;

namespace Ebook.Services;

public class DatabaseService : IDatabaseService
{
    private readonly SQLiteAsyncConnection _connection;

    public DatabaseService()
    {
        var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ebook.db");

        _connection = new SQLiteAsyncConnection(dbPath);
    }

    public SQLiteAsyncConnection GetConnection() => _connection;
}
