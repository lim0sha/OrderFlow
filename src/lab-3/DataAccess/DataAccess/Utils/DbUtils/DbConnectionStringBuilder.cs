using DataAccess.Configs;

namespace DataAccess.Utils.DbUtils;

public static class DbConnectionStringBuilder
{
    public static string Build(DbConnectionConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);
        return $"Host={config.Host};" +
               $"Port={config.Port};" +
               $"Username={config.Username};" +
               $"Password={config.Password};" +
               $"Database={config.Database};" +
               "Enlist=true;";
    }
}