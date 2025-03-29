using api.Exceptions;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GeralController() : ControllerBase
    {

        [HttpGet("versao")]
        public string GetVersao()
        {
            return Global.VERSAO;
        }

        public class MapSalasRequest
        {
            public string Token { get; set; }
        }
        [HttpPost("mapsalas")]
        public List<Sala> GetSalas(MapSalasRequest request)
        {
            string token = Environment.GetEnvironmentVariable("TOKEN");
            if (token == null) throw new Exception("Token não inicializado");
            if (request.Token != token) throw new ValidacaoException("Token inválido");

            return Global.Salas.Select(x => x.Value).ToList();
        }

        public static string GenerateToken(int length = 32)
        {
            var tokenData = new byte[length];

            RandomNumberGenerator.Fill(tokenData);

            return Convert.ToBase64String(tokenData)
                         .TrimEnd('=')
                         .Replace('+', '-')
                         .Replace('/', '_');
        }
        
        public class RequestIngresso
        {
            public string Nome { get; set; }
            public string IdSala { get; set; }
        }
        public class ResponseIngresso
        {
            public string IdSala { get; set; }
            public string Token { get; set; }
        }
        [HttpPost("ingressar")]
        public async Task<ResponseIngresso> Ingressar(RequestIngresso request)
        {
            Sala sala;
            if (request.IdSala == null)
            {
                sala = new();
                if (!Global.Salas.TryAdd(sala.Id, sala))
                {
                    throw new Exception("Criada sala que já existia");
                }
            }
            else
            {
                sala = Global.FindSalaById(request.IdSala);
            }

            var response = new ResponseIngresso();

            if (sala != null)
            {
                var user = new DadosUser();
                user.Sala = sala;
                user.Uuid = Guid.NewGuid();
                user.Nome = request.Nome;
                user.Token = GenerateToken();
                user.Admin = request.IdSala == null;
                user.Conectado = false; //criar desconectado

                sala.AddUser(user);

                response.Token = user.Token;
                response.IdSala = sala.Id;

                await DbService.Gravar(request.IdSala, request.Nome);
            }
            
            return response;
        }

    }
}
