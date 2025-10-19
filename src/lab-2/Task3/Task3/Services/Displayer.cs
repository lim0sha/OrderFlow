using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Spectre.Console;
using Task3.Models.Entities;
using Task3.Models.Enums;
using Task3.Services.Interfaces;

namespace Task3.Services;

public class Displayer : BackgroundService, IDisplayer
{
    private readonly IShower _shower;
    private readonly IOptionsMonitor<RendererBlueprint> _monitor;

    public Displayer(IShower shower, IOptionsMonitor<RendererBlueprint> monitor)
    {
        _shower = shower;
        _monitor = monitor;
    }

    public void Display(RendererBlueprint options)
    {
        switch (options.Mode)
        {
            case RenderMode.Figlet:
                _shower.ShowFiglet(options.Text);
                break;
            case RenderMode.Base64:
                _shower.ShowImageFromBase64(options.ImageBase64, options.ImageMaxWidth);
                break;
            case RenderMode.Url:
                _shower.ShowImageFromUrl(options.ImageUrl, options.ImageMaxWidth);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(options), "Unknown render mode in options");
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _monitor.OnChange(blueprint =>
        {
            AnsiConsole.Clear();
            Display(blueprint);
        });

        Display(_monitor.CurrentValue);

        while (!stoppingToken.IsCancellationRequested)
            await Task.Delay(1000, stoppingToken);
    }
}