using DiceRoller.Core.Models;
using DiceRollerServer.Models;
using System.Drawing;

namespace DiceRoller.Core.Apis
{
    /// <summary>
    /// Party service API
    /// </summary>
    public interface IPartyService
    {
        /// <summary>
        /// Connect player to party
        /// </summary>
        /// <param name="partyId"></param>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        int Connect(int partyId, string name, string password);

        /// <summary>
        /// Create session by GMs for players
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Party CreateSession(string name, string password);

        /// <summary>
        /// Get party
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Party GetParty(int id);

        /// <summary>
        /// Get user name from party
        /// </summary>
        /// <param name="partyId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        string GetUserName(int partyId, int userId);

        /// <summary>
        /// Get users from party
        /// </summary>
        /// <param name="partyId"></param>
        /// <returns></returns>
        List<PartyMember> GetUsers(int partyId);

        /// <summary>
        /// Move user in party
        /// </summary>
        /// <param name="partyId"></param>
        /// <param name="userId"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        bool MoveUser(int partyId, int userId, Point position);

        /// <summary>
        /// Update user's character HP
        /// </summary>
        /// <param name="partyId"></param>
        /// <param name="userId"></param>
        /// <param name="hp"></param>
        /// <returns></returns>
        bool UpdateHp(int partyId, int userId, int hp);

        /// <summary>
        /// Get user position in party
        /// </summary>
        /// <param name="partyId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Point GetUserPosition(int partyId, int userId);
    }
}