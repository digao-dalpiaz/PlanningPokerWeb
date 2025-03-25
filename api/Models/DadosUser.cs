﻿using System.Text.Json.Serialization;

namespace api.Models
{
    public class DadosUser
    {
        public string IdConexao { get; set; }

        public Guid Uuid { get; set; }
        public string Token { get; set; }
        public string Nome { get; set; }

        [JsonIgnore]
        public Sala Sala { get; set; }
        public bool Admin { get; set; }

        public int? Voto { get; set; }

        public bool Conectado { get; set; }
    }
}
