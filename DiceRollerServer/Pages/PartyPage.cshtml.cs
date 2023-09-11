using DiceRollerServer.Hubs;
using DiceRollerServer.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DiceRollerServer.Pages
{
    public class PartyPageModel : PageModel
    {
        private readonly RollHub _hub;

        public List<SelectListItem> Maps { get; set; }

        public PartyPageModel(RollHub hub)
        {
            _hub = hub;
            Maps = new List<SelectListItem>();
        }

        public void OnGet(string parameters)
        {
            if (!Request.Path.HasValue)
                return;

            string partyId = Request.Path.Value.Split('/')[2];
            string userId = Request.Path.Value.Split('/')[3];

            ViewData["UserId"] = userId;
            ViewData["UserName"] = "GM";
            ViewData["PartyId"] = partyId;
            ViewData["PartyName"] = _hub.GetPartyName(partyId);

            var userName = "GM";
            if (int.TryParse(userId, out _))
                userName = _hub.GetUser(partyId, userId);

            var mapList = _hub.GetMaps(Convert.ToInt32(partyId));
            var maps = mapList.Split('|');
            Maps.Clear();

            foreach(var map in maps)
            {
                var values = map.Split(";");
                Maps.Add(new SelectListItem { Value = values[0], Text = values[1] });
            }

            ViewData["UserName"] = userName;
        }
    }
}
