declare namespace signalR {
    export class HubConnection {
        on<T>(name: string, data: (T) => void): void;
    }
}

namespace Slidable {

    declare namespace Hub {

        export var hubConnection: signalR.HubConnection | null;

        export function onConnected(callback: Function): void;

        export function onDisconnected(callback: Function): void;

        export function connect(): void;
    }

    namespace Presenter {

        interface IQuestion {
            id: string;
            user: string;
            text: string;
        }

        document.addEventListener("DOMContentLoaded",
            () => {

                const list = document.querySelector("div#questions");
                const appendQuestion = (id, user, text) => {
                    id = `q${id}`;
                    let child = list.querySelector(`#${id}`) as HTMLDivElement;
                    if (!child) {
                        child = document.createElement("div");
                        child.id = id;
                        child.className = "card";
                        child.innerHTML =
                            `<div class="card-body">${text}</div><div class="card-footer text-muted">${user}</div>`;
                        list.insertBefore(child, list.firstChild);
                    }
                }

                Hub.onConnected(() => {
                    Hub.hubConnection.on<IQuestion>("question",
                        q => { appendQuestion(q.id, q.user, q.text); });
                });
            });
    }
}