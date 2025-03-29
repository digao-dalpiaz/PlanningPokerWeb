using MySqlConnector;

namespace api
{
    public class DbService
    {

        private readonly static string CONN_STRING;
        static DbService()
        {
            CONN_STRING = Environment.GetEnvironmentVariable("DB_STRING");
            if (string.IsNullOrEmpty(CONN_STRING)) throw new Exception("Configuração de conexão do banco não definida");
        }

        private static string GetIp(HttpContext context)
        {
            var trueIp = context.Request.Headers["X-Forwarded-For"].ToString(); //StringValue retorna Empty se chave não encontrada
            return !string.IsNullOrEmpty(trueIp) ? trueIp : context.Connection.RemoteIpAddress.ToString();
        }

        public async static Task Gravar(HttpContext context, bool modoCreate, string sala, string nome)
        {
            using var connection = new MySqlConnection(CONN_STRING);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "insert into log (data_hora, ip, modo, sala, nome) values (@dh, @ip, @modo, @sala, @nome)";

            command.Parameters.Add(new MySqlParameter("@dh", DateTime.Now));
            command.Parameters.Add(new MySqlParameter("@ip", GetIp(context)));
            command.Parameters.Add(new MySqlParameter("@modo", modoCreate ? "CREATE" : "JOIN"));
            command.Parameters.Add(new MySqlParameter("@sala", sala));
            command.Parameters.Add(new MySqlParameter("@nome", nome));

            await command.ExecuteNonQueryAsync();
        }

        public async static Task GravarException(HttpContext context, Exception ex)
        {
            using var connection = new MySqlConnection(CONN_STRING);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "insert into erros (data_hora, ip, ident, detalhes) values (@dh, @ip, @ident, @detalhes)";

            command.Parameters.Add(new MySqlParameter("@dh", DateTime.Now));
            command.Parameters.Add(new MySqlParameter("@ip", GetIp(context)));
            command.Parameters.Add(new MySqlParameter("@ident", context.Connection.Id));
            command.Parameters.Add(new MySqlParameter("@detalhes", ex.ToString()));

            await command.ExecuteNonQueryAsync();
        }

    }
}
