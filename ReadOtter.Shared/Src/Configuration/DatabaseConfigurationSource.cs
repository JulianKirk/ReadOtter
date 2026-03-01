using Microsoft.Extensions.Configuration;

namespace ReadOtter.Shared.Src.Configuration;

public class DatabaseConfigurationSource : IConfigurationSource
{
    private readonly string connectionString;

    public DatabaseConfigurationSource(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new DatabaseConfigurationProvider(connectionString);
    }
}
