using Ebook.Models;

namespace Ebook.Services;

public interface ISettingsService
{
    ReaderSettings Load();
    void Save(ReaderSettings settings);
}
