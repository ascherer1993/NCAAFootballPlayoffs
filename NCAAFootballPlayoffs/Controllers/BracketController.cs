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
        public ActionResult Index(int? usernameID)
        {
            if (usernameID == null)
            {
                return RedirectToAction("Index", "Home");
            }
            BracketViewModel bvm = new BracketViewModel();
            Username username = db.Usernames.FirstOrDefault(f => f.UsernameID == usernameID);
            if (username != null)
            {
                bvm.UsernameID = (int)usernameID;
                bvm.Username = username.UsernameText;
                bvm.SeasonID = 1;
                return View(bvm);
            }
            return RedirectToAction("Unauthorized", "Error");
        }


        /// <summary>
        /// Ajax call that saves the game received in as a parameter
        /// </summary>
        /// <param name="gameIn"></param>
        /// <returns></returns>
        [AjaxOnly]
        public string submitBracket(IEnumerable<UserPick> userPicks, IEnumerable<UserBonusQuestionPick> bonusQuestionPicks)
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
                        UserPick temp = db.UserPicks.FirstOrDefault(f => f.UsernameID == userPick.UsernameID && f.GameID == userPick.GameID);
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

                    foreach (var bonusQuestionPick in bonusQuestionPicks)
                    {
                        UserBonusQuestionPick temp = db.UserBonusQuestionPicks.FirstOrDefault(f => f.UsernameID == bonusQuestionPick.UsernameID && f.UserBonusQuestionPickID == bonusQuestionPick.UserBonusQuestionPickID);
                        if (temp == null)
                        {
                            db.UserBonusQuestionPicks.Add(bonusQuestionPick);
                        }
                        else
                        {
                            temp.SelectedAnswerID = bonusQuestionPick.SelectedAnswerID;
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
        /// Ajax call that saves the game received in as a parameter
        /// </summary>
        /// <param name="gameIn"></param>
        /// <returns></returns>
        [AjaxOnly]
        public string addQuestion(BonusQuestion questionIn, IEnumerable<QuestionAnswer> questionAnswersIn)
        {
            //Messages to return to ajax call
            List<string> msgs = new List<string>();
            bool success = false;
            BonusQuestion returnQuestion = new BonusQuestion();

            //Transaction for multiple edits of the database
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.BonusQuestions.Add(questionIn);
                    db.SaveChanges();

                    foreach(var answer in questionAnswersIn)
                    {
                        answer.BonusQuestionID = questionIn.BonusQuestionID;
                        db.QuestionAnswers.Add(answer);
                    }

                    db.SaveChanges();
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
                question = questionIn,
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
        public string saveQuestion(BonusQuestion questionIn, IEnumerable<QuestionAnswer> questionAnswersIn)
        {
            //Messages to return to ajax call
            List<string> msgs = new List<string>();
            bool success = false;
            BonusQuestion returnQuestion = new BonusQuestion();

            //Transaction for multiple edits of the database
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    //Makes changes to the game and saves the changes
                    BonusQuestion bonusQuestion = db.BonusQuestions.Find(questionIn.BonusQuestionID);
                    bonusQuestion.DisplayAsMultChoice = questionIn.DisplayAsMultChoice;
                    bonusQuestion.SeasonID = questionIn.SeasonID;
                    bonusQuestion.Text = questionIn.Text;

                    db.Entry(bonusQuestion).State = System.Data.Entity.EntityState.Modified;

                    foreach(var questionAnswerIn in questionAnswersIn)
                    {
                        QuestionAnswer questionAnswer = db.QuestionAnswers.Find(questionAnswerIn.QuestionAnswerID);
                        questionAnswer.Text = questionAnswerIn.Text;
                        questionAnswer.IsCorrectAnswer = questionAnswerIn.IsCorrectAnswer;

                        db.Entry(questionAnswer).State = System.Data.Entity.EntityState.Modified;
                    }

                    db.SaveChanges();

                    db.SaveChanges();
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
                question = questionIn,
                success = success
            };
            var json = JsonConvert.SerializeObject(responseObject, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
            return json;
        }

        [AjaxOnly]
        public string deleteQuestion(int questionID)
        {
            List<string> msgs = new List<string>();


            BonusQuestion question = db.BonusQuestions.Find(questionID);
            //question.Archived = true;
            //db.Entry(deleteGame).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            msgs.Add("Question has been succesfully deleted.");
            var responseObject = new
            {
                msgs = msgs,
                success = true
            };

            var returnString = JsonConvert.SerializeObject(responseObject);
            return returnString;
        }

        /// <summary>
        /// Gets JSon of all the information needed to load the BracketViewModel
        /// </summary>
        /// <param name="seasonID">optional</param>
        /// <returns></returns>
        [AjaxOnly]
        public string getBracketPageJSon(int seasonID = -1, int usernameID = -1)
        {
            List<Game> seasonGames = getGames(seasonID);
            List<State> states = getStates();
            List<Team> teams = getTeams();
            List<UserPick> userPicks = getPicks(usernameID, seasonID);
            List<BonusQuestion> bonusQuestions = getBonusQuestions(seasonID);
            List<UserBonusQuestionPick> userBonusQuestionPicks = getUserBonusQuestionPicks(usernameID, seasonID);

            var BracketJson = new {
                games = seasonGames,
                states = states,
                teams = teams,
                picks = userPicks,
                bonusQuestions = bonusQuestions,
                userBonusQuestionPicks = userBonusQuestionPicks
            };

            // Serializes the data into json, ignoring self referencing loops
            var json = JsonConvert.SerializeObject(BracketJson, Formatting.None,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });

            return json;
        }

        /// <summary>
        /// Gets all the games that aren't archived for the season sent in as a parameter
        /// </summary>
        /// <param name="seasonID">optional</param>
        /// <returns></returns>
        public List<Game> getGames(int seasonID = -1)
        {

            int currentSeasonID = seasonID == -1 ? db.Seasons.FirstOrDefault(f => f.ActiveSeason == true).SeasonID : (int)seasonID;
            IEnumerable<Game> seasonGames = db.Games.Where(f => f.SeasonID == currentSeasonID && f.Archived == false);

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
            });
            #endregion

            return seasonGames.ToList(); ;
        }

        /// <summary>
        /// Gets all states and returns them
        /// </summary>
        /// <returns></returns>
        public List<State> getStates()
        {
            IEnumerable<State> states = db.States;

            //This selects out the items we want in order to get rid of self referencing
            states = states.Select(f => new State
            {
                StateAbbreviation = f.StateAbbreviation,
                StateID = f.StateID,
                StateName = f.StateName
            });

            return states.ToList();
        }


        /// <summary>
        /// Gets all teams and returns them
        /// </summary>
        /// <returns></returns>
        public List<Team> getTeams()
        {
            IEnumerable<Team> teams = db.Teams.OrderBy(f => f.TeamName);

            //This selects out the items we want in order to get rid of self referencing
            teams = teams.Select(f => new Team
            {
                TeamName = f.TeamName,
                TeamNickname = f.TeamNickname,
                TeamID = f.TeamID
            });

            return teams.ToList();
        }


        /// <summary>
        /// Gets all user picks
        /// </summary>
        /// <returns></returns>
        public List<UserPick> getPicks(int usernameID, int seasonID)
        {
            IEnumerable<UserPick> userpicks = db.UserPicks.Where(f => f.UsernameID == usernameID && f.Game.SeasonID == seasonID);

            userpicks = userpicks.Select(f => new UserPick()
            {
                ChosenTeamID = f.ChosenTeamID,
                GameID = f.GameID,
                IsSurePick = f.IsSurePick
            });

            return userpicks.ToList();
        }

        /// <summary>
        /// Gets all bonus questions for a season
        /// </summary>
        /// <returns></returns>
        public List<BonusQuestion> getBonusQuestions(int seasonID)
        {
            IEnumerable<BonusQuestion> bonusQuestions = db.BonusQuestions.Where(f => f.SeasonID == seasonID);
            bonusQuestions = bonusQuestions.Select(f => new BonusQuestion()
            {
                BonusQuestionID = f.BonusQuestionID,
                SeasonID = f.SeasonID,
                Text = f.Text,
                DisplayAsMultChoice = f.DisplayAsMultChoice,
                QuestionAnswers = f.QuestionAnswers.Select(g => new QuestionAnswer()
                    {
                    BonusQuestionID = f.BonusQuestionID,
                    QuestionAnswerID = g.QuestionAnswerID,
                    Text = g.Text
                    }).ToList()
            });
            return bonusQuestions.ToList();
        }

        /// <summary>
        /// Gets all bonus question picks for a user in a season
        /// </summary>
        /// <returns></returns>
        public List<UserBonusQuestionPick> getUserBonusQuestionPicks(int usernameID, int seasonID)
        {
            IEnumerable<UserBonusQuestionPick> userBonysQuestionPicks = db.UserBonusQuestionPicks.Where(f => f.UsernameID == usernameID && f.QuestionAnswer.BonusQuestion.SeasonID == seasonID);

            userBonysQuestionPicks = userBonysQuestionPicks.Select(f => new UserBonusQuestionPick()
            {
                SelectedAnswerID = f.SelectedAnswerID,
                UserBonusQuestionPickID = f.UserBonusQuestionPickID,
                QuestionAnswer = new QuestionAnswer()
                {
                    BonusQuestionID = f.QuestionAnswer.BonusQuestionID
                }
            });
            return userBonysQuestionPicks.ToList();
        }

        #region ajaxCalls
        /// <summary>
        /// Gets all the games that aren't archived for the selected season and returns them as json.
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
        /// Gets all teams and returns them as JSON
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


        /// <summary>
        /// Gets all states and returns them as JSON
        /// </summary>
        /// <returns></returns>
        [AjaxOnly]
        public string getPicksJSon(int usernameID, int seasonID)
        {
            IEnumerable<UserPick> userpicks = db.UserPicks.Where(f => f.UsernameID == usernameID && f.Game.SeasonID == seasonID);

            userpicks = userpicks.Select(f => new UserPick()
            {
                ChosenTeamID = f.ChosenTeamID,
                GameID = f.GameID,
                IsSurePick = f.IsSurePick
            });

            var json = JsonConvert.SerializeObject(userpicks, Formatting.None,
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
        public string getBonusQuestionsJSon(int seasonID)
        {
            var json = JsonConvert.SerializeObject(null, Formatting.None,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });

            return json;
        }
        #endregion
    }
}