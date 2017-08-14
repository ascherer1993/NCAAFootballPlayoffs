using NCAAFootballPlayoffs.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft;
using NCAAFootballPlayoffs.Models.ViewModels;
using System.Data.Entity;

namespace NCAAFootballPlayoffs.Controllers
{
    public class BracketController : Controller
    {
        NCAAFootballPlayoffsEntities db = new NCAAFootballPlayoffsEntities();

        /// <summary>
        /// View for bracket page
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            BracketViewModel bvm = new BracketViewModel();
            bvm.UsernameID = 1;
            bvm.Username = "ams0068";
            return View(bvm);
        }


        /// <summary>
        /// Ajax call that saves the game received in as a parameter
        /// </summary>
        /// <param name="gameIn"></param>
        /// <returns></returns>
        [AjaxOnly]
        public string submitBracket(IEnumerable<UserPick> userPicks)
        {
            //Messages to return to ajax call
            List<string> msgs = new List<string>();
            bool success = false;

            //Transaction for multiple edits of the database
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    foreach(var userPick in userPicks)
                    {
                        UserPick temp = db.UserPicks.FirstOrDefault(f => f.UserNameID == userPick.UserNameID && f.GameID == userPick.GameID);
                        if (temp == null)
                        {
                            db.UserPicks.Add(userPick);
                        }
                        else
                        {
                            temp.ChosenTeamID = userPick.ChosenTeamID;
                            temp.IsSurePick = userPick.IsSurePick;
                            db.Entry(temp).State = EntityState.Modified;
                        }
                    }

                    db.SaveChanges();
                    dbContextTransaction.Commit();
                    msgs.Add("Saved Changes.");
                    success = true;
                }
                catch (Exception e)
                {
                    dbContextTransaction.Rollback();
                    msgs.Add("There was an error saving changes. Please try again.");
                }
            }

            var responseObject = new
            {
                msgs = msgs,
                success = success
            };
            var json = JsonConvert.SerializeObject(responseObject, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
            return json;
        }

        /// <summary>
        /// Ajax call that saves the game received in as a parameter
        /// </summary>
        /// <param name="gameIn"></param>
        /// <returns></returns>
        [AjaxOnly]
        public string addGame(Game gameIn, string favoriteNameIn, string favoriteNicknameIn, string underdogNameIn, string underdogNicknameIn)
        {
            //Messages to return to ajax call
            List<string> msgs = new List<string>();
            bool success = false;
            Game returnGame = new Game();

            //Transaction for multiple edits of the database
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {

                    //This is a check to make sure that the location doesn't already exist in the db. If it does, it uses that location
                    Location locationCheck = db.Locations.FirstOrDefault(f => f.StateID == gameIn.Location.StateID && f.City == gameIn.Location.City);
                    if (locationCheck == null)
                    {
                        //New location to save
                        Location newLocation = new Location();
                        newLocation.State = db.States.Find(gameIn.Location.StateID);
                        newLocation.City = gameIn.Location.City;

                        db.Locations.Add(newLocation);
                        db.SaveChanges();

                        gameIn.Location = newLocation;
                    }
                    else
                    {
                        gameIn.Location = locationCheck;
                    }

                    if (gameIn.FavoriteID == -1)
                    {
                        Team teamCheck = db.Teams.FirstOrDefault(f => f.TeamName == favoriteNameIn);
                        if (teamCheck == null)
                        {
                            Team newTeam = new Team();
                            newTeam.TeamName = favoriteNameIn;
                            newTeam.TeamNickname = favoriteNicknameIn;
                            newTeam.Archived = false;
                            db.Teams.Add(newTeam);
                            db.SaveChanges();
                            gameIn.Favorite = newTeam;
                        }
                        else
                        {
                            gameIn.Favorite = teamCheck;
                        }
                    }
                    else
                    {
                        gameIn.Favorite = db.Teams.Find(gameIn.FavoriteID);
                    }
                    if (gameIn.UnderdogID == -1)
                    {
                        Team teamCheck = db.Teams.FirstOrDefault(f => f.TeamName == underdogNameIn);
                        if (teamCheck == null)
                        {
                            Team newTeam = new Team();
                            newTeam.TeamName = underdogNameIn;
                            newTeam.TeamNickname = underdogNicknameIn;
                            newTeam.Archived = false;
                            db.Teams.Add(newTeam);
                            db.SaveChanges();
                            gameIn.Underdog = newTeam;
                        }
                        else
                        {
                            gameIn.Underdog = teamCheck;
                        }
                    }
                    else
                    {
                        gameIn.Underdog = db.Teams.Find(gameIn.UnderdogID);
                    }

                    gameIn.SeasonID = 1;

                    db.Games.Add(gameIn);
                    db.SaveChanges();

                    #region selectedGame
                    returnGame = db.Games.Find(gameIn.GameID);
                    returnGame = new Game
                    {
                        FavoriteID = returnGame.FavoriteID,
                        Favorite = new Team
                        {
                            TeamID = returnGame.FavoriteID,
                            TeamName = returnGame.Favorite.TeamName,
                            TeamNickname = returnGame.Favorite.TeamNickname
                        },
                        UnderdogID = returnGame.UnderdogID,
                        Underdog = new Team
                        {
                            TeamID = returnGame.UnderdogID,
                            TeamName = returnGame.Underdog.TeamName,
                            TeamNickname = returnGame.Underdog.TeamNickname
                        },
                        LocationID = returnGame.LocationID,
                        Location = returnGame.LocationID != null ? new Location()
                        {
                            LocationID = returnGame.Location.LocationID,
                            City = returnGame.Location.City,
                            StateID = returnGame.Location.StateID,

                            State = returnGame.Location.StateID != null ? new State
                            {
                                StateID = returnGame.Location.State.StateID,
                                StateName = returnGame.Location.State.StateName,
                                StateAbbreviation = returnGame.Location.State.StateAbbreviation
                            } : null
                        } : null,
                        GameDatetime = returnGame.GameDatetime,
                        GameName = returnGame.GameName,
                        GameID = returnGame.GameID,
                        PointSpread = returnGame.PointSpread,
                        IsBCSBowl = returnGame.IsBCSBowl
                    };
                    #endregion
                    
                    dbContextTransaction.Commit();
                    msgs.Add("Saved Changes.");
                    success = true;
                }
                catch (Exception e)
                {
                    dbContextTransaction.Rollback();
                    msgs.Add("There was an error saving changes to the game. Please try again.");
                }
            }

            var responseObject = new
            {
                msgs = msgs,
                game = returnGame,
                success = success
            };
            var json = JsonConvert.SerializeObject(responseObject, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
            return json;
        }



        /// <summary>
        /// Ajax call that saves the game received in as a parameter
        /// </summary>
        /// <param name="gameIn"></param>
        /// <returns></returns>
        public string saveGame(Game gameIn)
        {
            //Messages to return to ajax call
            List<string> msgs = new List<string>();
            bool success = false;

            //Transaction for multiple edits of the database
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    //Finds the location of the game, and edits it if it has changed.
                    Location location = db.Locations.Find(gameIn.LocationID);
                    if (location.StateID != gameIn.Location.StateID || location.City != gameIn.Location.City)
                    {
                        //This is a check to make sure that the location doesn't already exist in the db. If it does, it uses that location
                        Location locationCheck = db.Locations.FirstOrDefault(f => f.StateID == gameIn.Location.StateID && f.City == gameIn.Location.City);
                        if (locationCheck == null)
                        {
                            //New location to save
                            Location newLocation = new Location();
                            newLocation.StateID = gameIn.Location.StateID;
                            newLocation.City = gameIn.Location.City;

                            db.Locations.Add(newLocation);
                            db.SaveChanges();

                            gameIn.LocationID = newLocation.LocationID;
                        }
                        else
                        {
                            gameIn.LocationID = locationCheck.LocationID;
                        }
                    }

                    //Makes changes to the game and saves the changes
                    Game game = db.Games.Find(gameIn.GameID);
                    game.GameDatetime = gameIn.GameDatetime;
                    game.GameName = gameIn.GameName;
                    game.LocationID = gameIn.LocationID;
                    game.PointSpread = gameIn.PointSpread;
                    game.TVStation = gameIn.TVStation;
                    game.IsBCSBowl = gameIn.IsBCSBowl;

                    db.Entry(game).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    dbContextTransaction.Commit();
                    msgs.Add("Saved Changes.");
                    success = true;
                }
                catch (Exception)
                {
                    dbContextTransaction.Rollback();
                    msgs.Add("There was an error saving changes to the game. Please try again.");
                }
            }


            var responseObject = new
            {
                msgs = msgs,
                success = success
            };

            return JsonConvert.SerializeObject(responseObject);
        }

        [AjaxOnly]
        public string deleteGame(int gameID)
        {
            List<string> msgs = new List<string>();


            Game deleteGame = db.Games.Find(gameID);
            deleteGame.Archived = true;
            db.Entry(deleteGame).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

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
        [AjaxOnly]
        public string getGamesJSon(int seasonID = -1)
        {

            int currentSeasonID = seasonID == -1 ? db.Seasons.FirstOrDefault(f => f.ActiveSeason == true).SeasonID : (int)seasonID;
            List<Game> seasonGames = db.Games.Where(f => f.SeasonID == currentSeasonID && f.Archived == false).ToList();

            #region GetGames
            seasonGames = seasonGames.Select(g => new Game
            {
                FavoriteID = g.FavoriteID,
                Favorite = new Team
                {
                    TeamID = g.FavoriteID,
                    TeamName = g.Favorite.TeamName,
                    TeamNickname = g.Favorite.TeamNickname
                },
                UnderdogID = g.UnderdogID,
                Underdog = new Team
                {
                    TeamID = g.UnderdogID,
                    TeamName = g.Underdog.TeamName,
                    TeamNickname = g.Underdog.TeamNickname
                },
                LocationID = g.LocationID,
                Location = g.LocationID != null ? new Location()
                {
                    LocationID = g.Location.LocationID,
                    City = g.Location.City,
                    StateID = g.Location.StateID,

                    State = g.Location.StateID != null ? new State
                    {
                        StateID = g.Location.State.StateID,
                        StateName = g.Location.State.StateName,
                        StateAbbreviation = g.Location.State.StateAbbreviation
                    } : null
                } : null,
                GameDatetime = g.GameDatetime,
                GameName = g.GameName,
                GameID = g.GameID,
                PointSpread = g.PointSpread,
                IsBCSBowl = g.IsBCSBowl
            }).ToList();
            #endregion

            // Serializes all games into json, ignoring self referencing loops
            var json = JsonConvert.SerializeObject(seasonGames, Formatting.None,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });

            return json;
        }

        /// <summary>
        /// Gets all states and returns them as JSON
        /// </summary>
        /// <returns></returns>
        [AjaxOnly]
        public string getStatesJSon()
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

        /// <summary>
        /// Gets all states and returns them as JSON
        /// </summary>
        /// <returns></returns>
        [AjaxOnly]
        public string getTeamsJSon()
        {
            IEnumerable<Team> teams = db.Teams.OrderBy(f => f.TeamName);

            //This selects out the items we want in order to get rid of self referencing
            teams = teams.Select(f => new Team
            {
                TeamName = f.TeamName,
                TeamNickname = f.TeamNickname,
                TeamID = f.TeamID
            });

            var json = JsonConvert.SerializeObject(teams, Formatting.None,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });

            return json;
        }


    }
}