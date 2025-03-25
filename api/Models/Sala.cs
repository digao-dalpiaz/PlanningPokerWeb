namespace api.Models
{
    public class Sala
    {
        private const int INTERVALO = 1000 * 60 * 1; //1 minuto

        private readonly List<DadosUser> _users;
        private readonly Timer _timer;

        private readonly object _tsLock = new();
        private DateTime _ultimoUso;

        public Sala()
        {
            Id = Guid.NewGuid().ToString();
            _users = [];
            _ultimoUso = DateTime.Now;
            _timer = new(TimerCallback, null, INTERVALO, INTERVALO);
        }

        public string Id { get; }
        public bool EmVotacao { get; set; }
        public List<DadosUser> Users { get { lock(_users) return _users.ToList(); } }
        public DateTime UltimoUso { get => _ultimoUso; }

        public void AtualizarTimestamp()
        {
            lock (_tsLock)
            {
                _ultimoUso = DateTime.Now;
            }
        }

        private void TimerCallback(object state)
        {
            int qtdUsers;
            lock (_users) qtdUsers = _users.Count;

            if (qtdUsers == 0 && _ultimoUso.AddMinutes(2) < DateTime.Now)
            {
                Global.Salas.TryRemove(Id, out _);
            }
        }

        public DadosUser FindUserByToken(string token)
        {
            lock (_users)
            {
                return _users.FirstOrDefault(x => x.Token == token);
            }
        }

        public void AddUser(DadosUser user)
        {
            lock (_users)
            {
                _users.Add(user);
            }
        }

        public void RemoveUser(DadosUser user)
        {
            lock (_users)
            {
                _users.Remove(user);
            }
        }

        public void RemoverDesconectadosAndClearVotos()
        {
            lock (_users)
            {
                _users.RemoveAll(x => x.Desconectado);
                _users.ForEach(x => x.Voto = null);
            }
        }

    }
}
