using DiceRollerServer.Hubs;
using DiceRollerServer.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DiceRollerServer.Pages
{
    public class PartyPageModel : PageModel
    {
        private readonly RollHub _hub;

        public PartyPageModel(RollHub hub)
        {
            _hub = hub;
        }

        public void OnGet(string parameters)
        {
            if (!Request.Path.HasValue)
                return;

            string partyId = Request.Path.Value.Split('/')[2];
            string userId = Request.Path.Value.Split('/')[3];

            ViewData["UserName"] = "GM";
            ViewData["PartyId"] = partyId;
            ViewData["PartyName"] = _hub.GetPartyName(partyId);

            var userName = "";
            if (userId.ToUpper() == "GM")
                userName = "GM";
            else
                userName = _hub.GetUser(partyId, userId).Result;
            ViewData["UserName"] = userName;
        }
    }
}