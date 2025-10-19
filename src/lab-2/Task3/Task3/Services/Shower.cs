using Spectre.Console;
using Task3.Services.Interfaces;

namespace Task3.Services;

public sealed class Shower : IShower, IDisposable
{
    private readonly HttpClient _http = new();
    private bool _disposed;

    public void ShowFiglet(string text)
    {
        FigletText figOut = new FigletText(text).Color(Color.White);
        AnsiConsole.Write(figOut);
    }

    public void ShowImageFromUrl(string url, int width)
    {
        string temp = Path.GetTempFileName();
        File.WriteAllBytes(temp, _http.GetByteArrayAsync(url).Result);
        CanvasImage img = new CanvasImage(temp).MaxWidth(width);
        AnsiConsole.Write(img);
        File.Delete(temp);
    }

    public void ShowImageFromBase64(string base64, int width)
    {
        string clean;

        if (base64.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
        {
            int dot = base64.IndexOf(',', StringComparison.CurrentCulture);
            clean = dot >= 0 ? base64[(dot + 1)..] : base64;
        }
        else
        {
            clean = base64;
        }

        byte[] bytes = Convert.FromBase64String(clean);
        using var stream = new MemoryStream(bytes);
        CanvasImage image = new(stream)
        {
            MaxWidth = width,
        };
        AnsiConsole.Write(image);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _http.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}