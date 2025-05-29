using System.Windows;
using System.Windows.Media.Imaging;

namespace SpectrumWaterfallApp.Services;

public static class SpectrumRenderer
{
    public static void RenderSpectrumLine(WriteableBitmap bitmap, float[] data, double zoom)
    {
        bitmap.Lock();

        unsafe
        {
            IntPtr buffer = bitmap.BackBuffer;
            int stride = bitmap.BackBufferStride;
            int height = bitmap.PixelHeight;
            int width = bitmap.PixelWidth;

            for (int y = 0; y < height; y++)
            {
                byte* row = (byte*)buffer + y * stride;
                for (int x = 0; x < width; x++)
                {
                    int offset = x * 4;
                    if (offset + 3 >= stride) continue;
                    row[offset + 0] = 0;
                    row[offset + 1] = 0;
                    row[offset + 2] = 0;
                    row[offset + 3] = 255;
                }
            }

            for (int dbm = -120; dbm <= -20; dbm += 10)
            {
                int y = (int)((1 - (dbm + 120f) / 100f) * (height - 1));
                y = Math.Clamp(y, 0, height - 1);

                byte* row = (byte*)buffer + y * stride;
                for (int x = 0; x < width; x++)
                {
                    int offset = x * 4;
                    if (offset + 3 < stride)
                    {
                        row[offset + 0] = 30;
                        row[offset + 1] = 30;
                        row[offset + 2] = 30;
                        row[offset + 3] = 255;
                    }
                }
            }

            for (int mhz = 90; mhz <= 110; mhz += 2)
            {
                int x = (int)((mhz - 90) / 20.0 * width);
                if (x < 0 || x >= width) continue;

                for (int y = 0; y < height; y++)
                {
                    byte* row = (byte*)buffer + y * stride;
                    int offset = x * 4;
                    if (offset + 3 >= stride) continue;
                    row[offset + 0] = 30;
                    row[offset + 1] = 30;
                    row[offset + 2] = 30;
                    row[offset + 3] = 255;
                }
            }

            int prevY = -1;
            for (int x = 0; x < width; x++)
            {
                int index = x * data.Length / width;
                float val = Math.Clamp(data[index], -120f, -20f);
                float norm = (val + 120f) / 100f;
                norm *= (float)zoom;
                int y = (int)((1 - norm) * (height - 1));
                y = Math.Clamp(y, 0, height - 1);

                if (x > 0 && prevY >= 0)
                {
                    DrawLine(buffer, stride, width, height, x - 1, prevY, x, y, 0, 255, 0);
                }

                prevY = y;
            }
        }

        bitmap.AddDirtyRect(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
        bitmap.Unlock();
    }

    private static unsafe void DrawLine(IntPtr buffer, int stride, int width, int height,
                                        int x0, int y0, int x1, int y1,
                                        byte r, byte g, byte b)
    {
        int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = -Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int err = dx + dy;

        while (true)
        {
            if ((uint)x0 < width && (uint)y0 < height)
            {
                byte* p = (byte*)buffer + y0 * stride + x0 * 4;
                p[0] = b;
                p[1] = g;
                p[2] = r;
                p[3] = 255;
            }

            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 >= dy) { err += dy; x0 += sx; }
            if (e2 <= dx) { err += dx; y0 += sy; }
        }
    }
}