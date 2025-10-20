namespace Task1.Clients.Interfaces;

public interface IQueryStringBuilder
{
    string Build(Dictionary<string, string> queryParameters);
}