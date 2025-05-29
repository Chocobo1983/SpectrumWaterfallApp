namespace SpectrumWaterfallApp.Services;

public static class ColorMapService
{
    public static (byte R, byte G, byte B) PowerToColor(float power)
    {
        float t = (power + 120f) / 100f;
        t = Math.Clamp(t, 0f, 1f);

        return t switch
        {
            <= 0.25f => Interpolate((0, 0, 255), (0, 255, 255), t / 0.25f),
            <= 0.5f => Interpolate((0, 255, 255), (0, 255, 0), (t - 0.25f) / 0.25f),
            <= 0.75f => Interpolate((0, 255, 0), (255, 255, 0), (t - 0.5f) / 0.25f),
            _ => Interpolate((255, 255, 0), (255, 0, 0), (t - 0.75f) / 0.25f),
        };
    }

    private static (byte R, byte G, byte B) Interpolate((byte R, byte G, byte B) c1, (byte R, byte G, byte B) c2, float t)
    {
        byte r = (byte)(c1.R + (c2.R - c1.R) * t);
        byte g = (byte)(c1.G + (c2.G - c1.G) * t);
        byte b = (byte)(c1.B + (c2.B - c1.B) * t);
        return (r, g, b);
    }
}