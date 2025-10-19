namespace Task3.Services.Interfaces;

public interface IShower
{
    void ShowFiglet(string text);

    void ShowImageFromUrl(string url, int width);

    void ShowImageFromBase64(string base64, int width);
}