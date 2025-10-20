using Task3.Models.Enums;

namespace Task3.Models.Entities;

public class RendererBlueprint
{
    public RenderMode Mode { get; init; } = RenderMode.Figlet;

    public string Text { get; init; } = "lim0sha's lab";

    public string ImageBase64 { get; init; } = string.Empty;

    public string ImageUrl { get; init; } = string.Empty;

    public int ImageMaxWidth { get; init; } = 40;
}