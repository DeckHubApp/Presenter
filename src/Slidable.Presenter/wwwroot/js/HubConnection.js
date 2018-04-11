var Slidable;
(function (Slidable) {
    var Presenter;
    (function (Presenter) {
        document.addEventListener("DOMContentLoaded", function () {
            var list = document.querySelector("div#questions");
            var appendQuestion = function (id, user, text) {
                id = "q" + id;
                var child = list.querySelector("#" + id);
                if (!child) {
                    child = document.createElement("div");
                    child.id = id;
                    child.className = "card";
                    child.innerHTML =
                        "<div class=\"card-body\">" + text + "</div><div class=\"card-footer text-muted\">" + user + "</div>";
                    list.insertBefore(child, list.firstChild);
                }
            };
            Hub.onConnected(function () {
                Hub.hubConnection.on("question", function (q) { appendQuestion(q.id, q.user, q.text); });
            });
        });
    })(Presenter || (Presenter = {}));
})(Slidable || (Slidable = {}));
//# sourceMappingURL=presenter.js.map