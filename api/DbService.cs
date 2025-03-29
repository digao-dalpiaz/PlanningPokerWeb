using MySqlConnector;

namespace api
{
    public class DbService
    {

        public async static Task Gravar(string sala, string nome)
        {
            var sb = new MySqlConnectionStringBuilder
            {
                Server = Environment.GetEnvironmentVariable("DB_SERVER"),
                Database = Environment.GetEnvironmentVariable("DB_DATABASE"),
                UserID = Environment.GetEnvironmentVariable("DB_USER"),
                Password = Environment.GetEnvironmentVariable("DB_PASSWORD")
            };

            using var connection = new MySqlConnection(sb.ConnectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "insert into log (sala, nome) values (@sala, @nome)";
            
            command.Parameters.Add(new MySqlParameter("@sala", sala));
            command.Parameters.Add(new MySqlParameter("@nome", nome));

            await command.ExecuteNonQueryAsync();
        }

    }
}
