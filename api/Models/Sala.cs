namespace api.Models
{
    public class Sala
    {
        private const int INTERVALO = 1000 * 60 * 10; //5 minutos

        private DateTime _ultimoUso;
        private readonly Timer _timer;

        private readonly object _tsLock = new();

        public Sala()
        {
            _ultimoUso = DateTime.Now;
            _timer = new(TimerCallback, null, INTERVALO, INTERVALO);
        }

        public string Id { get; set; }
        public List<DadosUser> Users { get; set; }
        public bool EmVotacao { get; set; }

        private void TimerCallback(object state)
        {
            int qtdUsers;
            lock (Users) qtdUsers = Users.Count;

            if (qtdUsers == 0 && _ultimoUso.AddMinutes(5) < DateTime.Now)
            {
                Global.Salas.TryRemove(Id, out _);
            }
        }

        public void AtualizarTimestamp()
        {
            lock (_tsLock)
            {
                _ultimoUso = DateTime.Now;
            }
        }

        public DadosUser FindUserByToken(string token)
        {
            lock (Users)
            {
                return Users.FirstOrDefault(x => x.Token == token);
            }
        }

        public List<DadosUser> GetCopiaListaUsers()
        {
            lock (Users)
            {
                return Users.ToList();
            }
        }

        public void AddUser(DadosUser user)
        {
            lock (Users)
            {
                Users.Add(user);
            }
        }

        public void RemoveUser(DadosUser user)
        {
            lock (Users)
            {
                Users.Remove(user);
            }
        }

        public void RemoverDesconectadosAndClearVotos()
        {
            lock (Users)
            {
                Users.RemoveAll(x => x.Desconectado);
                Users.ForEach(x => x.Voto = null);
            }
        }

    }
}
