namespace SpectrumWaterfallApp.Models;

public class SpectrumDataModel
{
    public int Width { get; }
    public int SpectrumHeight { get; }
    public int WaterfallHeight { get; }

    public float[] CurrentSpectrum { get; }
    public float[][] WaterfallPowerMap { get; }
    public byte[][] WaterfallColorMap { get; }

    public SpectrumDataModel(int width, int spectrumHeight, int waterfallHeight)
    {
        Width = width;
        SpectrumHeight = spectrumHeight;
        WaterfallHeight = waterfallHeight;

        CurrentSpectrum = new float[width];
        WaterfallPowerMap = new float[waterfallHeight][];
        WaterfallColorMap = new byte[waterfallHeight][];

        for (int y = 0; y < waterfallHeight; y++)
        {
            WaterfallPowerMap[y] = new float[width];
            WaterfallColorMap[y] = new byte[width * 4];
            for (int x = 0; x < width; x++)
                WaterfallPowerMap[y][x] = -120f;
        }
    }
}