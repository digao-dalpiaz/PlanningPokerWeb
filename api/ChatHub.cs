﻿using api.Exceptions;
using api.Models;
using Microsoft.AspNetCore.SignalR;

namespace api
{
    public class ChatHub : Hub
    {

        private const string DATA_IDENT = "DATA";

        private DadosUser GetByContext(bool obrigatorio = true)
        {
            var user = Context.Items[DATA_IDENT] as DadosUser;
            if (obrigatorio && user == null) throw new Exception("Usuário não encontrado");
            return user;
        }

        public class InfoUser
        {
            public Guid Uuid { get; set; }
            public bool Admin { get; set; }
        }
        public async Task<InfoUser> Entrar(string idSala, string token)
        {
            var sala = Global.FindSalaById(idSala);
            if (sala == null) throw new ValidacaoException("Sala não encontrada: " + idSala);

            var dadosUser = sala.FindUserByToken(token);
            if (dadosUser == null) throw new Exception("Usuário não encontrado pelo token");
            
            dadosUser.IdConexao = Context.ConnectionId;
            dadosUser.Conectado = true;

            Context.Items[DATA_IDENT] = dadosUser;

            await MandarParaTodosSala(sala);

            return new InfoUser
            {
                Uuid = dadosUser.Uuid,
                Admin = dadosUser.Admin
            };
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var dadosUsuario = GetByContext(obrigatorio: false);
            if (dadosUsuario != null)
            {
                dadosUsuario.Conectado = false;

                await MandarParaTodosSala(dadosUsuario.Sala);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public class Posicao
        {
            public bool EmVotacao { get; set; }
            public List<PosicaoUser> Users { get; set; }
        }
        public class PosicaoUser
        {
            public Guid Uuid { get; set; }
            public string Nome { get; set; }
            public int? Voto { get; set; }
            public string Tamanho { get; set; }
            public string Complexidade { get; set; }
            public bool Votou { get; set; }
            public bool Admin { get; set; }
            public bool Conectado { get; set; }
        }
        private async Task MandarParaTodosSala(Sala sala)
        {
            var users = sala.Users; //thread safe

            var listaUsers = users.Select(x => new PosicaoUser
            {
                Uuid = x.Uuid,
                Nome = x.Nome,
                Votou = x.Voto != null,
                Voto = sala.EmVotacao ? null : x.Voto,
                Tamanho = sala.EmVotacao ? null : x.Tamanho,
                Complexidade = sala.EmVotacao ? null : x.Complexidade,
                Admin = x.Admin,
                Conectado = x.Conectado
            }).OrderBy(x => x.Nome);

            if (!sala.EmVotacao) listaUsers = listaUsers.OrderBy(x => x.Voto);

            var posicao = new Posicao();
            posicao.Users = listaUsers.ToList();
            posicao.EmVotacao = sala.EmVotacao;
                
            var conexoes = users.Where(x => x.Conectado).Select(x => x.IdConexao);
            await Clients.Clients(conexoes).SendAsync("Posicao", posicao);

            sala.AtualizarTimestamp();
        }

        public async Task Votar(int voto, string tamanho, string complexidade)
        {
            var dadosUsuario = GetByContext();
            var sala = dadosUsuario.Sala;
            if (!sala.EmVotacao) throw new ValidacaoException("Não está em votação");

            dadosUsuario.Voto = voto;
            dadosUsuario.Tamanho = tamanho;
            dadosUsuario.Complexidade = complexidade;

            await MandarParaTodosSala(sala);
        }
        
        public async Task DefinirStatusSala(bool emVotacao)
        {
            var dadosUsuario = GetByContext();
            if (!dadosUsuario.Admin) throw new Exception("Usuário não é admin");

            var sala = dadosUsuario.Sala;
            sala.EmVotacao = emVotacao;

            if (emVotacao) sala.RemoverDesconectadosAndClearVotos();
            
            await MandarParaTodosSala(sala);
        }

    }
}
