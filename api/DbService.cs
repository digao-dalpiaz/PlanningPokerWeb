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

        public async static Task Gravar(HttpContext context, bool modoCreate, string sala, string nome)
        {
            using var connection = new MySqlConnection(CONN_BUILDER.ConnectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "insert into log (data_hora, ip, modo, sala, nome) values (@dh, @ip, @modo, @sala, @nome)";

            var trueIp = context.Request.Headers["X-Forwarded-For"].ToString(); //StringValue retorna Empty se chave não encontrada

            command.Parameters.Add(new MySqlParameter("@dh", DateTime.Now));
            command.Parameters.Add(new MySqlParameter("@ip", !string.IsNullOrEmpty(trueIp) ? trueIp : context.Connection.RemoteIpAddress?.ToString()));
            command.Parameters.Add(new MySqlParameter("@modo", modoCreate ? "CREATE" : "JOIN"));
            command.Parameters.Add(new MySqlParameter("@sala", sala));
            command.Parameters.Add(new MySqlParameter("@nome", nome));

            await command.ExecuteNonQueryAsync();
        }

    }
}
