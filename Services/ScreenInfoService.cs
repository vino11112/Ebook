using Microsoft.Maui.Devices;

namespace Ebook.Services;

public class ScreenInfoService
{
    public int ScreenWidth => (int)DeviceDisplay.MainDisplayInfo.Width;
    public int ScreenHeight => (int)DeviceDisplay.MainDisplayInfo.Height;
    public double Density => DeviceDisplay.MainDisplayInfo.Density;

    public int DpToPx(int dp) => (int)(dp * Density);
}
