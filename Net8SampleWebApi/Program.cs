using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", () => "It's up!");

app.MapGet("config/debug", () => ((IConfigurationRoot)app.Configuration).GetDebugView());

app.MapGet("health/databases", () => app.Configuration.GetSection("ConnectionStrings").GetChildren()
    .Select(x =>
    {
        try
        {
            using var connection = new SqlConnection(x.Value);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1";
            command.ExecuteScalar();
            return (x.Key, "OK");
        }
        catch (Exception ex)
        {
            return (x.Key, ex.Message);
        }
    })
    .ToDictionary(x => x.Key, x => x.Item2));

app.Run();
