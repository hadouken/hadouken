///<reference path="../hdkn.d.ts"/>

module Hadouken.Plugins.Torrents.UI {
    export class AddTorrentsDialog extends Hadouken.UI.Dialog {
        constructor() {
            super("/plugins/core.torrents/dialogs/add.html");
        }

        onShow(): void {
            this.setupEvents();
        }

        setupEvents(): void {
            var c = this.getContent();

            c.find('#btn-add-torrents').on('click', (e) => {
                e.preventDefault();
                this.addTorrents();
            });
        }

        addTorrents(): void {
            this.disableAddButton();

            var c = this.getContent();
            var fileInput = c.find('#torrent-files')[0];
            var reader = new FileReader();
            var filesAdded = 0;

            reader.onload = (e) => {
                var data = [
                    e.target.result.split(',')[1],
                    '',
                    ''
                ];

                var rpcClient = new Hadouken.Http.JsonRpcClient('/jsonrpc');
                rpcClient.callParams('torrents.addFile', data, () => {
                    filesAdded++;

                    if (filesAdded == fileInput.files.length)
                        this.enableAddButton();
                });
            };

            for (var i = 0; i < fileInput.files.length; i++) {
                reader.readAsDataURL(fileInput.files[i]);
            }
        }

        disableAddButton(): void {
            this.getContent().find('#btn-add-torrents').attr('disabled', true);
        }

        enableAddButton(): void {
            this.getContent().find('#btn-add-torrents').attr('disabled', false);
        }
    }
}