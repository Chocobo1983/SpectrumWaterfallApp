namespace SpectrumWaterfallApp.Services;

public class SpectrumGenerator(int width)
{
    public void Generate(float[] buffer, double time)
    {
        var rnd = Random.Shared;
        const float baseNoise = -100f;
        var center = width / 2f;

        for (int i = 0; i < buffer.Length; i++)
        {
            var noise = (float)(rnd.NextDouble() * 4 - 2);
            var signal = 90f * (float)Math.Exp(-Math.Pow((i - center) / 20f, 2));

            for (int h = 1; h <= 3; h++)
            {
                var offset = h * 160;
                var widthFactor = 25f + h * 5;
                var amplitude = 40f / h;

                signal += amplitude * (float)Math.Exp(-Math.Pow((i - (center + offset)) / widthFactor, 2));
                signal += amplitude * (float)Math.Exp(-Math.Pow((i - (center - offset)) / widthFactor, 2));
            }

            var pulse = 1.0f + 0.05f * (float)Math.Sin(time * 2.0f);
            buffer[i] = Math.Clamp(baseNoise + signal * pulse + noise, -120f, -20f);
        }
    }
}