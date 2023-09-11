namespace DiceRoller.Core.Apis
{
    public interface IRollService
    {
        /// <summary>
        /// Connect user to keep track of rolls
        /// </summary>
        /// <param name="userId"></param>
        void Connect(int userId);

        /// <summary>
        /// Roll die
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="die"></param>
        /// <returns></returns>
        int RollDie(int userId, int die);

        /// <summary>
        /// Connect user to keep track of rolls
        /// </summary>
        /// <param name="userId"></param>
        int GetRollStat(int userId, int die);
    }
}