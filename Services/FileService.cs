using System.Net;

namespace Ebook.Services;

public class FileService : IFileService
{
    public async Task<string?> PickPdfAsync()
    {
        try
        {
            var pick = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Pdf,
                PickerTitle = "Выберите PDF файл"
            });

            return pick?.FullPath;
        }
        catch
        {
            return null;
        }
    }

    public async Task<string?> DownloadPdfFromUrlAsync(string url)
    {
        try
        {
            using var http = new HttpClient();
            byte[] bytes = await http.GetByteArrayAsync(url);

            if (bytes == null || bytes.Length < 1000)
                return null;

            string fileName = $"{Guid.NewGuid()}.pdf";

            string folder = FileSystem.AppDataDirectory;
            string path = Path.Combine(folder, fileName);

            await File.WriteAllBytesAsync(path, bytes);

            return path;
        }
        catch
        {
            return null;
        }
    }
}
