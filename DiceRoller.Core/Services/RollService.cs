using DiceRoller.Core.Apis;
using DiceRoller.Core.Exceptions;
using DiceRoller.Core.Models;

namespace DiceRollerServer.Services
{
    public class RollService : IRollService
    {
        private readonly Dictionary<int, RollsStats> _users;
        private readonly Random _random;

        /// <summary>
        /// Constructor
        /// </summary>
        public RollService()
        {
            _users = new Dictionary<int, RollsStats>();
            _random = new Random(DateTime.Now.Millisecond);
        }

        /// <summary>
        /// Connect user to keep track of rolls
        /// </summary>
        /// <param name="userId"></param>
        public void Connect(int userId)
        {
            _users[userId] = new RollsStats();
        }

        /// <summary>
        /// Roll a die
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="die"></param>
        /// <returns></returns>
        public int RollDie(int userId, int die)
        {
            var result = _random.Next(1, die + 1);
            if (_users.ContainsKey(userId))
                _users[userId].AddResults(die, result);
            return result;
        }

        /// <summary>
        /// Get roll stats for a die
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="die"></param>
        /// <returns></returns>
        /// <exception cref="UnknownIdException"></exception>
        public int GetRollStat(int userId, int die)
        {
            if (!_users.ContainsKey(userId))
                throw new UnknownIdException("RollStats", userId);

            return _users[userId].GetMean(die);
        }
    }
}
