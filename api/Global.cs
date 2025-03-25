using api.Models;
using System.Collections.Concurrent;

namespace api
{
    public class Global
    {

        public const string VERSAO = "1.2";

        public static readonly ConcurrentDictionary<string, Sala> Salas = new();

        public static Sala FindSalaById(string idSala)
        {
            if (string.IsNullOrEmpty(idSala)) throw new ArgumentNullException(nameof(idSala));

            if (!Salas.TryGetValue(idSala, out var sala)) return null;
            return sala;
        }

    }
}
