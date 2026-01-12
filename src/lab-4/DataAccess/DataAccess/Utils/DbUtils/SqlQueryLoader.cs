using System.Reflection;

namespace DataAccess.Utils.DbUtils;

public static class SqlQueryLoader
{
    private const string ResourceBasePath = "DataAccess.Migrations.Scripts";
    private static readonly Assembly Assembly = Assembly.GetExecutingAssembly();

    public static string Load(string fileName)
    {
        string resourceName = $"{ResourceBasePath}.{fileName}";
        using Stream stream = Assembly.GetManifestResourceStream(resourceName)
                           ?? throw new InvalidOperationException($"SQL resource not found: {resourceName}");

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}