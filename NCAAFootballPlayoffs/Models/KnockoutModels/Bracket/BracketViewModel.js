function BracketViewModel(ModelIn) {
    var self = this;
    //Setup
    self.username = ModelIn.Username;
    self.usernameID = ModelIn.UsernameID;
    self.seasonID = ko.observable(ModelIn.SeasonID);

    self.isLoaded = ko.observable(false);
    self.games = ko.observableArray();
    self.bonusQuestions = ko.observableArray();
    self.states = [];
    self.teams = [];

    //This is used for select lists for states
    self.selectState = ko.observable();
    self.newGameSelectFavorite = ko.observable();
    self.newGameSelectUnderdog = ko.observable();

    //These are used for the new game modal
    self.newGameBowlName = ko.observable();
    self.newGameDatetime = ko.observable(null);
    self.newGameCity = ko.observable();
    self.newGameSelectedStateID = ko.observable();
    self.newGameFavoriteID = ko.observable();
    self.newGameUnderdogID = ko.observable();
    self.newGamePointSpread = ko.observable();
    self.newGameIsBCSBowl = ko.observable();
    self.newGameFavoriteName = ko.observable();
    self.newGameFavoriteNickname = ko.observable();
    self.newGameUnderdogName = ko.observable();
    self.newGameUnderdogNickname = ko.observable();

    //These are used for the new bonus question modal
    self.newQuestionText = ko.observable();
    self.newQuestionMultipleChoice = ko.observable();
    self.newQuestionAnswer = ko.observable();
    self.newQuestionAnswerArray = ko.observableArray();

    self.surePickCount = ko.pureComputed(function () {
        var count = 0;
        self.games().forEach(function (game) {
            if (game.isSurePick()) {
                count++;
            }
        });
        return count;
    }, this);
    self.pickMadeCount = ko.pureComputed(function () {
        var count = 0;
        self.games().forEach(function (game) {
            if (game.teamPickID() != undefined)
            {
                count++;
            }
        });
        return count;
    }, this);

    self.loadAjax = function () {
        $.when(self.loadBracketData()).done(function (a1, a2) {
            ko.applyBindings(self);
            $("#gamesDiv").css("visibility", "visible");
            $("#loadingDiv").hide();
        });
    }

    //Loads all games for the active season
    self.loadBracketData = function () {
        return $.get("/Bracket/getBracketPageJSon",
            {
                usernameID: self.usernameID,
                seasonID: self.seasonID()
            }, function (data) {
                var returnObject = ko.mapping.fromJSON(data);

                //Games
                self.games = returnObject.games;
                $.each(self.games(), function (index, game) {
                    game.isEditing = ko.observable(false);
                    game.Location.State = ko.observable(game.Location.State)
                    game.teamPickID = ko.observable();
                    game.isSurePick = ko.observable();
                });

                //Bonus Questions
                self.bonusQuestions = returnObject.bonusQuestions;
                $.each(self.bonusQuestions(), function (index, bonusQuestion) {
                    bonusQuestion.isEditing = ko.observable(false);
                    bonusQuestion.answerPickID = ko.observable();
                });

                //Bonus Question Picks
                var userBonusQuestionPicks = returnObject.userBonusQuestionPicks;
                self.bonusQuestions().forEach(function (bonusQuestion) {
                    userBonusQuestionPicks().forEach(function (userBonusQuestionPick) {
                        //IF THEY HAVE THE SAME QUESTION, THEN THE ANSWER IS THE ONE FOR THAT ONE
                        if (bonusQuestion.BonusQuestionID() == userBonusQuestionPick.QuestionAnswer.BonusQuestionID())
                        {
                            bonusQuestion.answerPickID(userBonusQuestionPick.SelectedAnswerID());
                        }
                        //bonusQuestion.QuestionAnswers().forEach(function (questionAnswer) {
                        //    if (userBonusQuestionPick.SelectedAnswerID() == questionAnswer.QuestionAnswerID()) {
                        //        bonusQuestion.answerPickID(userBonusQuestionPick.UserBonusQuestionPickID());
                        //    }
                        //});
                    });
                });

                //States
                self.states = returnObject.states;

                //Teams
                self.teams = returnObject.teams;
                var createNewGameOption = { 'TeamName': 'Create new team', 'TeamID': -1 };
                self.teams.splice(0, 0, createNewGameOption);

                //Picks
                var picks = returnObject.picks;
                self.games().forEach(function (game) {
                    picks().forEach(function (pick) {
                        if (pick.GameID() == game.GameID()) {
                            game.teamPickID(pick.ChosenTeamID());
                            game.isSurePick(pick.IsSurePick());
                        }
                    });
                });
        });
    };

    self.loadTeams = function () {
        return $.get("/Bracket/getTeamsJSon", function (data) {
            self.teams = ko.mapping.fromJSON(data);
            var createNewGameOption = { 'TeamName': 'Create new team', 'TeamID': -1 };
            self.teams.splice(0, 0, createNewGameOption);

            //$.each(self.teams(), function (index, team) {
            //    team.isEditing = ko.observable(false);
            //    game.Location.State = ko.observable(game.Location.State)
            //});
        });
    };

    //Loads all games for the active season
    self.loadGames = function () {
        return $.get("/Bracket/getGamesJSon", function (data) {
            self.games = ko.mapping.fromJSON(data);
            $.each(self.games(), function (index, game) {
                game.isEditing = ko.observable(false);
                game.Location.State = ko.observable(game.Location.State)
                game.teamPickID = ko.observable();

                game.isSurePick = ko.observable();
            });
        });
    };

    //Loads all states to be used for dropdown
    self.loadStates = function () {
        return $.get("/Bracket/getStatesJSon", function (data) {
            self.states = ko.mapping.fromJSON(data);
        });
    };

    self.loadPicks = function () {
        return $.get("/Bracket/getPicksJSon",
            {
                usernameID: self.usernameID,
                seasonID: self.seasonID()
            },
            function (data) {
            var picks = ko.mapping.fromJSON(data);
            
            self.games().forEach(function (game) {
                picks().forEach(function (pick) {
                    if (pick.GameID() == game.GameID())
                    {
                        game.teamPickID(pick.ChosenTeamID());
                        game.isSurePick(pick.IsSurePick());
                    }
                });
            });
        });
    };


    //Loads all bonus questions for the active season
    self.loadBonusQuestions = function () {
        return $.get("/Bracket/getBonusQuestionsJSon", function (data) {
            self.bonusQuestions = ko.mapping.fromJSON(data);
            $.each(self.bonusQuestions(), function (index, question) {
                question.isEditing = ko.observable(false);
            });
        });
    };

    //Utility Functions
    self.editGame = function (game) {
        self.selectState = ko.observable(game.Location.State());

        //Selects option from dropdown that matches the state.
        //This was necessary because the correct value was not being selected.
        $(".stateSelect option").filter(function () {
            return $(this).text() == self.selectState().StateName();
        }).prop('selected', true);

        //$(".stateSelect option").filter(function () {
        //    return $(this).text() == self.selectState().StateName();
        //}).prop('selected', true);

        //Makes fields editable
        game.isEditing(true);
    }

    self.editBonusQuestion = function (bonusQuestion) {
        bonusQuestion.isEditing(true);
    }

    //This method saves the game
    self.addGame = function () {
        var newGame = {
            'GameName': self.newGameBowlName,
            'GameDatetime': moment(self.newGameTime).format("YYYY-MM-DD HH:mm:ss"),
            'Location.City': self.newGameCity,
            'Location.StateID': self.newGameSelectedStateID,
            'FavoriteID': self.newGameSelectFavorite,
            'UnderdogID': self.newGameSelectUnderdog,
            'PointSpread': self.newGamePointSpread,
            'IsBCSBowl': self.newGameIsBCSBowl
        }

        //Need to get values from the temporary state chosen from the dropdown
        //game.Location.StateID(self.selectState().StateID());
        //game.Location.State(self.selectState());
        var gameToSave = ko.toJS(newGame);

        //gameToSave.GameDatetime = moment(gameToSave.GameDatetime).format("YYYY-MM-DD HH:mm:ss")
        $.post("/Bracket/addGame", {
            gameIn: gameToSave,
            favoriteNameIn: self.newGameFavoriteName(),
            favoriteNicknameIn: self.newGameFavoriteNickname(),
            underdogNameIn: self.newGameUnderdogName(),
            underdogNicknameIn: self.newGameUnderdogNickname()
        }, function (returnedData) {
            response = JSON.parse(returnedData);
            msgs = response.msgs;
            
            for (i = 0; i < msgs.length; i++) {
                //Displays all messages
                response.success ? alertify.success(msgs[i]) : alertify.error(msgs[i]);
            }
            if (response.success) {
                var returnedGame = ko.mapping.fromJS(response.game);
                returnedGame.isEditing = ko.observable(false);
                returnedGame.Location.State = ko.observable(returnedGame.Location.State)
                returnedGame.teamPickID = ko.observable();

                returnedGame.isSurePick = ko.observable(false);
                //returnedGame.isSurePick.subscribe(function (value) {
                //    if (value) {
                //        self.surePickCount(self.surePickCount() + 1);
                //    }
                //    else {
                //        self.surePickCount(self.surePickCount() - 1);
                //    }
                //});

                self.games.push(returnedGame);
                $("#myModal").modal('hide');
                
            }
        })
    }

    //This method saves the game
    self.saveGame = function (game) {
        //Need to get values from the temporary state chosen from the dropdown
        game.Location.StateID(self.selectState().StateID());
        game.Location.State(self.selectState());
        gameToSave = ko.toJS(game);

        gameToSave.GameDatetime = moment(gameToSave.GameDatetime).format("YYYY-MM-DD HH:mm:ss")
        $.post("/Bracket/saveGame", { gameIn: gameToSave }, function (returnedData) {
            response = JSON.parse(returnedData);
            msgs = response.msgs;
            for (i = 0; i < msgs.length; i++) {
                //Displays all messages
                response.success ? alertify.success(msgs[i]) : alertify.error(msgs[i]);
            }
            if (response.success) {
                game.isEditing(false);
                
            }
        })
    }

    //Sends the id to a method that archives the selected game
    self.deleteGame = function (game) {
        alertify.confirm("Are you sure you wish to delete this game?", function (e) {
            if (e) {
                $.post("/Bracket/deleteGame", { gameID: game.GameID() }, function (returnedData) {
                    response = JSON.parse(returnedData);
                    msgs = response.msgs;
                    for (i = 0; i < msgs.length; i++)
                    {
                        response.success ? alertify.success(msgs[i]) : alertify.error(msgs[i]);
                    }
                    if (response.success)
                    {
                        self.games.remove(game);
                    }
                })
            } else {
                // user clicked "cancel"
            }
        });
    }


    //This method saves the game
    self.saveBonusQuestion = function (bonusQuestion) {
        //gameToSave = ko.toJS(game);

        //gameToSave.GameDatetime = moment(gameToSave.GameDatetime).format("YYYY-MM-DD HH:mm:ss")
        //$.post("/Bracket/saveGame", { gameIn: gameToSave }, function (returnedData) {
        //    response = JSON.parse(returnedData);
        //    msgs = response.msgs;
        //    for (i = 0; i < msgs.length; i++) {
        //        //Displays all messages
        //        response.success ? alertify.success(msgs[i]) : alertify.error(msgs[i]);
        //    }
        //    if (response.success) {
        //        game.isEditing(false);

        //    }
        //})
        bonusQuestion.isEditing(false);
    }

    //Add answer to question
    self.addQuestionAnswer = function () {
        var newQAnswer = {
            QuestionAnswerID: null,
            Text: self.newQuestionAnswer()
        }
        self.newQuestionAnswerArray.push(newQAnswer);
        self.newQuestionAnswer("");
    }

    self.removeSeat = function (answer) { self.newQuestionAnswerArray.remove(answer) }

    //This method saves the game
    self.addQuestion = function () {
        var newQuestion = {
            'Text': self.newQuestionText,
            'DisplayAsMultChoice': self.newQuestionMultipleChoice,
            'SeasonID': self.seasonID
        }

        var questionToSave = ko.toJS(newQuestion);
        var questionAnswers = ko.toJS(self.newQuestionAnswerArray);

        //gameToSave.GameDatetime = moment(gameToSave.GameDatetime).format("YYYY-MM-DD HH:mm:ss")
        $.post("/Bracket/addQuestion", {
            questionIn: questionToSave,
            questionAnswersIn: questionAnswers
        }, function (returnedData) {
            response = JSON.parse(returnedData);
            msgs = response.msgs;

            for (i = 0; i < msgs.length; i++) {
                //Displays all messages
                response.success ? alertify.success(msgs[i]) : alertify.error(msgs[i]);
            }
            if (response.success) {
                var returnedQuestion = ko.mapping.fromJS(response.question);
                returnedQuestion.isEditing = ko.observable(false);
                self.bonusQuestions.push(returnedQuestion);
                $("#myModal").modal('hide');

            }
        })
    }


    //focuses when modal opens
    $('#myModal').on('shown.bs.modal', function () {
        $('#new-bowl-name').focus();
    })

    $('#myModal2').on('shown.bs.modal', function () {
        $('#new-question-question-text').focus();
    })

    self.submitBracket = function ()
    {
        var gamePickInfo = [];
        self.games().forEach(function (game) {
            var gameInfo = {
                GameID: game.GameID(),
                ChosenTeamID: game.teamPickID(),
                IsSurePick: game.isSurePick(),
                UsernameID: self.usernameID
            };
            gamePickInfo.push(gameInfo);
        });
        var bonusQuestionPickInfo = [];
        self.bonusQuestions().forEach(function (bonusQuestion) {
            var bonusQuestionInfo = {
                UserBonusQuestionPickID: bonusQuestion.BonusQuestionID(),
                SelectedAnswerID: bonusQuestion.answerPickID(),
                UsernameID: self.usernameID
            };
            bonusQuestionPickInfo.push(bonusQuestionInfo);
        });
        $.post("/Bracket/submitBracket", {
            userPicks: gamePickInfo,
            bonusQuestionPicks: bonusQuestionPickInfo
        }, function (returnedData) {
            response = JSON.parse(returnedData);
            msgs = response.msgs;

            for (i = 0; i < msgs.length; i++) {
                //Displays all messages
                response.success ? alertify.success(msgs[i]) : alertify.error(msgs[i]);
            }
            if (response.success) {

            }
        })
    }

}