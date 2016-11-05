function BracketViewModel(urls) {
    var self = this;
    self.isLoading = ko.observable(true);
    self.games = null;

    self.loadGames = function () {
        $.get("getGamesJSon", function (data) {
            self.games = ko.mapping.fromJSON(data);
        });
    }
}