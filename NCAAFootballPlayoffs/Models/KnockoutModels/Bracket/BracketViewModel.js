﻿function BracketViewModel(urls) {
    var self = this;
    //Setup
    self.isLoading = ko.observable(true);
    self.games = ko.observableArray();
    self.states = [];
    self.teams = [];

    //This is used for select lists for states
    self.selectState = ko.observable();
    self.selectFavorite = ko.observable();
    self.favoriteOptionValue
    self.selectUnderdog = ko.observable();

    //These are used for the new game modal
    self.newGameBowlName = ko.observable();
    self.newGameTime = ko.observable(null);
    self.newGameCity = ko.observable();
    self.newGameSelectedStateID = ko.observable();
    self.newGameFavoriteID = ko.observable();
    self.newGameUnderdogID = ko.observable();
    self.newGameSpread = ko.observable();
    self.isBCSBowl = ko.observable();

    //This applies bindings after all of the data has been loaded.
    self.loadAjax = function () {
        $.when(self.loadGames(), self.loadStates(), self.loadTeams()).done(function (a1, a2) {
            ko.applyBindings(self);
        });
    }

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
            });
        });
    };

    //Loads all states to be used for dropdown
    self.loadStates = function () {
        return $.get("/Bracket/getStatesJSon", function (data) {
            self.states = ko.mapping.fromJSON(data);
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

}