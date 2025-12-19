using Android.Graphics;
using Android.Graphics.Pdf;
using Android.OS;
using Ebook.Services;

[assembly: Dependency(typeof(Ebook.Platforms.Android.AndroidPdfRenderer))]
namespace Ebook.Platforms.Android;

public class AndroidPdfRenderer : IPdfRenderer
{
    public Task<int> GetPageCountAsync(string filePath)
    {
        return Task.Run(() =>
        {
            using var fd = ParcelFileDescriptor.Open(
                new Java.IO.File(filePath),
                ParcelFileMode.ReadOnly);

            using var renderer = new PdfRenderer(fd);

            return renderer.PageCount; 
        });
    }

    public Task<byte[]?> RenderPageBytesAsync(
        string filePath,
        int pageIndex,
        int targetWidth,
        int targetHeight)
    {
        return Task.Run<byte[]?>(() =>
        {
            using var fd = ParcelFileDescriptor.Open(
                new Java.IO.File(filePath),
                ParcelFileMode.ReadOnly);

            using var renderer = new PdfRenderer(fd);

            if (pageIndex < 0 || pageIndex >= renderer.PageCount)
                return null;

            using var page = renderer.OpenPage(pageIndex);

            int pdfW = page.Width;
            int pdfH = page.Height;

            if (pdfW <= 0 || pdfH <= 0)
            {
                pdfW = 1080;
                pdfH = 1600;
            }

            float scale = targetWidth > 0
                ? (float)targetWidth / pdfW
                : 1f;

            int bmpW = (int)(pdfW * scale);
            int bmpH = (int)(pdfH * scale);

            if (bmpW < 200 || bmpH < 200)
            {
                bmpW = pdfW;
                bmpH = pdfH;
            }

            using var bitmap = Bitmap.CreateBitmap(bmpW, bmpH, Bitmap.Config.Argb8888);
            var canvas = new Canvas(bitmap);
            canvas.DrawColor(global::Android.Graphics.Color.White);

            var rect = new global::Android.Graphics.Rect(0, 0, bmpW, bmpH);

            page.Render(bitmap, rect, null, PdfRenderMode.ForDisplay);

            using var ms = new MemoryStream();
            bitmap.Compress(Bitmap.CompressFormat.Png, 100, ms);

            return ms.ToArray();
        });
    }
}
