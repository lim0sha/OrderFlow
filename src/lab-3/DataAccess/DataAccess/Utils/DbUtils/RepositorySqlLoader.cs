using System.Reflection;

namespace DataAccess.Utils.DbUtils;

public static class RepositorySqlLoader
{
    private const string BasePath = "DataAccess.Repositories.Resources.Sql";
    private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();

    public static string Load(string fileName)
    {
        string resourceName = $"{BasePath}.{fileName}";
        using Stream stream = Assembly.GetManifestResourceStream(resourceName)
                              ?? throw new InvalidOperationException($"SQL resource not found: {resourceName}");

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}