function BracketViewModel(urls) {
    var self = this;
    //Setup
    self.isLoading = ko.observable(true);
    self.games = ko.observableArray();
    self.states = [];
    self.tempState = ko.observable();

    self.loadGames = function () {
        $.get("getGamesJSon", function (data) {
            self.games = ko.mapping.fromJSON(data);

            $.each(self.games(), function (index, game) {
                game.isEditing = ko.observable(false);
                game.Location.State = ko.observable(game.Location.State)
            });

            ko.applyBindings(self);
        });
    }

    self.loadStates = function () {
        $.get("getStatesJSon", function (data) {
            self.states = ko.mapping.fromJSON(data);
        });
    }();
    
    //self.updateState = function (game) {
    //    if (self.tempState() != null)
    //    {
    //        game.Location.State.StateName(self.tempState().StateName);
    //        game.Location.State.StateID(self.tempState().StateID);
    //        game.Location.State.StateAbbreviation(self.tempState().StateAbbreviation);
    //    }
    //}

    //self.getStateName = ko.purecomputed(function (state) {
    //    return 7;
    //});

    //Utility Functions
    self.editGame = function (game) {
        game.isEditing(true);
        //self.tempState(game.Location.State)
    }
    self.saveGame = function (game) {
        game.isEditing(false);
        gameToSave = ko.toJS(game);

        gameToSave.GameDatetime = moment(gameToSave.GameDatetime).format("YYYY-MM-DD HH:mm:ss")
        $.post("/Bracket/saveGame", { gameIn: gameToSave }, function (returnedData) {
            response = JSON.parse(returnedData);
            msgs = response.msgs;
            for (i = 0; i < msgs.length; i++) {
                response.success ? alertify.success(msgs[i]) : alertify.error(msgs[i]);
            }
            if (response.success) {
            }
        })
    }

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