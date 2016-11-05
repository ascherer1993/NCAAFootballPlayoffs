function BracketViewModel(urls) {
    var self = this;
    self.isLoading = ko.observable(true);
    self.games = ko.observableArray();

    self.loadGames = function () {
        $.get("getGamesJSon", function (data) {
            self.games = ko.mapping.fromJSON(data);
            ko.applyBindings(self);
        });
    }
}