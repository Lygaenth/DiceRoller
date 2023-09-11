using DiceRollerServer.Models;

namespace DiceRoller.Core.Apis
{
    /// <summary>
    /// Map service API
    /// </summary>
    public interface IMapService
    {
        /// <summary>
        /// Get map
        /// </summary>
        /// <param name="partyId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Map GetMap(int partyId, int id);

        /// <summary>
        /// Get all maps available to user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<Map> GetMapList(int partyId);

        /// <summary>
        /// Load maps for user
        /// </summary>
        /// <param name="userId"></param>
        void LoadPartyMaps(int partyId);
    }
}