using NCAAFootballPlayoffs.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace NCAAFootballPlayoffs.Controllers
{
    public class BracketController : Controller
    {
        // GET: Bracket
        public ActionResult Index()
        {
            return View();
        }

        public string getGamesJSon(int seasonID = -1)
        {
            using (NCAAFootballPlayoffsEntities db = new NCAAFootballPlayoffsEntities())
            {
                
                int currentSeasonID = seasonID == -1 ? db.Seasons.FirstOrDefault(f => f.ActiveSeason == true).SeasonID : (int) seasonID;
                IEnumerable<Game> seasonGames = db.Games.Where(f => f.SeasonID == currentSeasonID);
                //var json = new JavaScriptSerializer().Serialize(seasonGames);
                var json = JsonConvert.SerializeObject(seasonGames, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
                return json;
            }
            
        }
    }
}