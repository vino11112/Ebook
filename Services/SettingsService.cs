using System.Text.Json;
using Ebook.Models;
using Microsoft.Maui.Storage;

namespace Ebook.Services;

public class SettingsService : ISettingsService
{
    private const string Key = "reader_settings";

    public ReaderSettings Load()
    {
        try
        {
            if (!Preferences.ContainsKey(Key))
                return new ReaderSettings();

            string json = Preferences.Get(Key, "");
            if (string.IsNullOrWhiteSpace(json))
                return new ReaderSettings();

            var settings = JsonSerializer.Deserialize<ReaderSettings>(json);
            return settings ?? new ReaderSettings();
        }
        catch
        {

            return new ReaderSettings();
        }
    }

    public void Save(ReaderSettings settings)
    {
        try
        {
            string json = JsonSerializer.Serialize(settings);
            Preferences.Set(Key, json);
        }
        catch
        {

        }
    }
}
