/// <reference path="./hub.ts"/>
var Slidable;
(function (Slidable) {
    var Presenter;
    (function (Presenter) {
        // ReSharper restore InconsistentNaming
        document.addEventListener("DOMContentLoaded", function () {
            var list = document.querySelector("ul#questions");
            var appendQuestion = function (id, user, text) {
                id = "q" + id;
                var li = list.querySelector("#" + id);
                if (!li) {
                    li = document.createElement("li");
                    li.id = id;
                    li.className = "list-group-item";
                    li.innerHTML =
                        "<span class=\"small\"><strong>" + user + "</strong></span><br><span>" + text + "</span>";
                    list.insertBefore(li, list.firstChild);
                }
            };
            var protocol = window.location.protocol === "https:" ? "wss:" : "ws:";
            var path = window.location.pathname.substr(8).replace(/\/[0-9]+$/, "");
            var wsUri = protocol + "//" + window.location.host + path;
            var socket = new WebSocket(wsUri);
            socket.addEventListener("message", function (e) {
                var q = JSON.parse(e.data);
                if (q.MessageType === "question") {
                    appendQuestion(q.Id, q.User, q.Text);
                }
            });
        });
    })(Presenter = Slidable.Presenter || (Slidable.Presenter = {}));
})(Slidable || (Slidable = {}));
