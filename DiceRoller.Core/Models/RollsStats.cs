namespace DiceRoller.Core.Models
{
    public class RollsStats
    {
        private readonly Dictionary<int, List<int>> _results;

        public RollsStats()
        {
            _results = new Dictionary<int, List<int>>();
        }

        public void AddResults(int die, int result)
        {
            if (!_results.ContainsKey(die))
                _results[die] = new List<int>();

            _results[die].Add(result);
        }

        public int GetMean(int die)
        {
            if (!_results.ContainsKey(die))
                return -1;

            return _results[die].Sum() / _results[die].Count();
        }
    }
}
