using MySqlConnector;

namespace api
{
    public class DbService
    {

        private readonly static string CONNECTION_STRING = new MySqlConnectionStringBuilder
        {
            Server = Environment.GetEnvironmentVariable("DB_SERVER"),
            Port = uint.Parse(Environment.GetEnvironmentVariable("DB_PORT")),
            Database = Environment.GetEnvironmentVariable("DB_DATABASE"),
            UserID = Environment.GetEnvironmentVariable("DB_USER"),
            Password = Environment.GetEnvironmentVariable("DB_PASSWORD")
        }.ConnectionString;

        public async static Task Gravar(string sala, string nome)
        {
            using var connection = new MySqlConnection(CONNECTION_STRING);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "insert into log (data_hora, sala, nome) values (@dh, @sala, @nome)";
            
            command.Parameters.Add(new MySqlParameter("@dh", DateTime.Now));
            command.Parameters.Add(new MySqlParameter("@sala", sala));
            command.Parameters.Add(new MySqlParameter("@nome", nome));

            await command.ExecuteNonQueryAsync();
        }

    }
}
