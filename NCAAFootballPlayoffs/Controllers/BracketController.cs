using NCAAFootballPlayoffs.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft;

namespace NCAAFootballPlayoffs.Controllers
{
    public class BracketController : Controller
    {
        // GET: Bracket
        public ActionResult Index()
        {
            return View();
        }


        public string saveGame(Game gameIn)
        {
            List<string> msgs = new List<string>();

            
            using (NCAAFootballPlayoffsEntities db = new NCAAFootballPlayoffsEntities())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        Location location = db.Locations.Find(gameIn.LocationID);
                        if (location.StateID != gameIn.Location.StateID || location.City != gameIn.Location.City)
                        {
                            Location newLocation = new Location();
                            newLocation.StateID = gameIn.Location.StateID;
                            newLocation.City = gameIn.Location.City;

                            db.Locations.Add(newLocation);
                            db.SaveChanges();

                            gameIn.Location = newLocation;
                        }

                        Game game = db.Games.Find(gameIn.GameID);
                        game.GameDatetime = gameIn.GameDatetime;
                        game.GameName = gameIn.GameName;
                        game.LocationID = gameIn.Location.LocationID;
                        game.PointSpread = gameIn.PointSpread;
                        game.TVStation = game.TVStation;
                        
                        
                        db.Entry(game).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        dbContextTransaction.Commit();
                        msgs.Add("Saved Changes.");
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        msgs.Add(e.Message);
                    }
                }
            }
            
            var responseObject = new
            {
                msgs = msgs,
                success = true
            };

            var returnString = JsonConvert.SerializeObject(responseObject);
            return returnString;
        }

        public string deleteGame(int gameID)
        {
            List<string> msgs = new List<string>();

            using (NCAAFootballPlayoffsEntities db = new NCAAFootballPlayoffsEntities())
            {
                Game deleteGame = db.Games.Find(gameID);
                deleteGame.Archived = true;
                db.Entry(deleteGame).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            msgs.Add("Game has been succesfully deleted.");
            var responseObject = new
            {
                msgs = msgs,
                success = true
            };

            var returnString = JsonConvert.SerializeObject(responseObject);
            return returnString;
        }


        /// <summary>
        /// Gets JSon of all the games that aren't archived for the active season or the season sent in as a parameter
        /// </summary>
        /// <param name="seasonID">optional</param>
        /// <returns></returns>
        public string getGamesJSon(int seasonID = -1)
        {
            using (NCAAFootballPlayoffsEntities db = new NCAAFootballPlayoffsEntities())
            {
                
                int currentSeasonID = seasonID == -1 ? db.Seasons.FirstOrDefault(f => f.ActiveSeason == true).SeasonID : (int) seasonID;
                IEnumerable<Game> seasonGames = db.Games.Where(f => f.SeasonID == currentSeasonID && f.Archived == false);

                // Serializes all games into json, ignoring self referencing loops
                var json = JsonConvert.SerializeObject(seasonGames, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });

                return json;
            }
            
        }

        /// <summary>
        /// Gets all states and returns them as JSON
        /// </summary>
        /// <returns></returns>
        public string getStatesJSon()
        {
            using (NCAAFootballPlayoffsEntities db = new NCAAFootballPlayoffsEntities())
            {
                IEnumerable<State> states = db.States;

                //This selects out the items we want in order to get rid of self referencing
                states = states.Select(f => new State
                {
                    StateAbbreviation = f.StateAbbreviation,
                    StateID = f.StateID,
                    StateName = f.StateName
                });
                
                var json = JsonConvert.SerializeObject(states, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });

                return json;
            }

        }
    }
}