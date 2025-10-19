using System.Web;
using Task1.Clients.Interfaces;

namespace Task1.Clients.Builders;

public class QueryStringBuilder : IQueryStringBuilder
{
    public string Build(Dictionary<string, string> queryParameters)
    {
        return string.Join("&", queryParameters.Select(kvp => $"{HttpUtility.UrlEncode(kvp.Key)}={HttpUtility.UrlEncode(kvp.Value)}"));
    }
}
