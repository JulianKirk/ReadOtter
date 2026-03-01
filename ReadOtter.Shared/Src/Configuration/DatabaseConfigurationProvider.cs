using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace ReadOtter.Shared.Src.Configuration;

public class DatabaseConfigurationProvider : ConfigurationProvider
{
    private readonly string connectionString;

    public DatabaseConfigurationProvider(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public override void Load()
    {
        var data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        using var checkCmd = connection.CreateCommand();
        checkCmd.CommandText =
            "SELECT name FROM sqlite_master WHERE type='table' AND name='AppSettings'";
        var tableExists = checkCmd.ExecuteScalar() != null;

        if (tableExists)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT SettingName, SettingValue FROM AppSettings";

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var name = reader.GetString(0);
                var value = reader.GetString(1);
                data[$"AppSettings:{name}"] = value;
            }
        }

        Data = data;
    }

    public void Reload()
    {
        Load();
        OnReload();
    }
}
