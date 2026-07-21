using System.Security.Cryptography;

namespace api
{
    public class RandomId
    {
        private const string Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string Generate(int length = 12)
        {
            Span<byte> bytes = stackalloc byte[length];
            RandomNumberGenerator.Fill(bytes);

            char[] result = new char[length];

            for (int i = 0; i < length; i++)
                result[i] = Chars[bytes[i] % Chars.Length];

            return new string(result);
        }

        public static string GerarIdSala()
        {
            var id = Generate(16);

            return $"{id[..4]}-{id[4..8]}-{id[8..12]}";
        }
    }
}
