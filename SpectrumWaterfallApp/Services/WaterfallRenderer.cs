using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SpectrumWaterfallApp.Services;

public static class WaterfallRenderer
{
    public static void RenderWaterfall(
        WriteableBitmap bitmap,
        float[] spectrum,
        float[][] powerMap,
        byte[][] colorMap,
        double zoom)
    {
        int width = bitmap.PixelWidth;
        int height = bitmap.PixelHeight;
        int stride = width * 4;

        for (int y = height - 1; y > 0; y--)
            Array.Copy(powerMap[y - 1], powerMap[y], width);

        for (int x = 0; x < width; x++)
            powerMap[0][x] = spectrum[x];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float val = powerMap[y][x];
                val = Math.Clamp((val + 120f) * (float)zoom - 120f, -120f, -20f);
                var (r, g, b) = ColorMapService.PowerToColor(val);

                int offset = x * 4;
                colorMap[y][offset + 0] = b;
                colorMap[y][offset + 1] = g;
                colorMap[y][offset + 2] = r;
                colorMap[y][offset + 3] = 255;
            }
        }

        bitmap.Lock();
        for (int y = 0; y < height; y++)
        {
            Marshal.Copy(colorMap[y], 0, bitmap.BackBuffer + y * stride, stride);
        }
        bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
        bitmap.Unlock();
    }
}