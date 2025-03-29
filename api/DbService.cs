using MySqlConnector;

namespace api
{
    public class DbService
    {

        private readonly static MySqlConnectionStringBuilder CONN_BUILDER = new()
        {
            Server = Environment.GetEnvironmentVariable("DB_SERVER"),
            Port = uint.Parse(Environment.GetEnvironmentVariable("DB_PORT")),
            Database = Environment.GetEnvironmentVariable("DB_DATABASE"),
            UserID = Environment.GetEnvironmentVariable("DB_USER"),
            Password = Environment.GetEnvironmentVariable("DB_PASSWORD")
        };

        public async static Task Gravar(HttpRequest request, string sala, string nome)
        {
            using var connection = new MySqlConnection(CONN_BUILDER.ConnectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "insert into log (data_hora, ip, sala, nome) values (@dh, @ip, @sala, @nome)";
            
            command.Parameters.Add(new MySqlParameter("@dh", DateTime.Now));
            command.Parameters.Add(new MySqlParameter("@ip", request.Host.Value));
            command.Parameters.Add(new MySqlParameter("@sala", sala));
            command.Parameters.Add(new MySqlParameter("@nome", nome));

            await command.ExecuteNonQueryAsync();
        }

    }
}
