using Microsoft.Extensions.Configuration;

namespace ReadOtter.Shared.Src.Configuration;

public static class DatabaseConfigurationExtensions
{
    public static IConfigurationBuilder AddSqliteSettings(
        this IConfigurationBuilder builder,
        string connectionString)
    {
        return builder.Add(new DatabaseConfigurationSource(connectionString));
    }
}
