namespace DiceRollerServer.Services
{
    public class RollService
    {
        private readonly Dictionary<string, string> _users;
        private readonly Random _random;

        public RollService()
        {
            _users = new Dictionary<string, string>();
            _random = new Random(DateTime.Now.Millisecond);
        }

        public void Connect(string userId, string name)
        {
            _users[userId] = name;
        }

        public int RollDie(int die)
        {
            return _random.Next(1, die+1);
        }
    }
}
