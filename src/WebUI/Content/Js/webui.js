
var hdknWebUI = {
    version: "1.0",

    config: {
        updateInterval: 1500
    },

    updateTimeout: null,

    torrents: {},

    trtTable: new STable(),

    trtCols: [
    //[ colID, colWidth, colType, colDisabled = false, colIcon = false, colAlign = ALIGN_AUTO, colText = "" ]
          ["name", 220, TYPE_STRING]
        , ["size", 75, TYPE_NUMBER]
        , ["done", 60, TYPE_NUM_PROGRESS]
        , ["status", 100, TYPE_CUSTOM]
        , ["seeds", 60, TYPE_NUMBER]
        , ["peers", 60, TYPE_NUMBER]
        , ["downspeed", 80, TYPE_NUMBER]
        , ["upspeed", 80, TYPE_NUMBER]
        , ["eta", 60, TYPE_NUM_ORDER]
        , ["uploaded", 75, TYPE_NUMBER]
        , ["downloaded", 75, TYPE_NUMBER]
        , ["ratio", 50, TYPE_NUMBER]
    ],

    trtFormatRow: function (values, index) {
        var useidx = $chk(index);
        var len = (useidx ? (index + 1) : values.length);

        for (var i = (index || 0); i < len; i++) {
            switch (this.trtCols[i][0]) {
                case "size":
                    values[i] = values[i].toFileSize(2);
                    break;

                case "ratio":
                    values[i] = (values[i] == -1) ? "\u221E" : (values[i] / 1000).toFixedNR(3);
                    break;
            }
        }

        if (useidx) {
            return values[index];
        } else {
            return values;
        }
    },

    sortCustom: function () { console.log("sortCustom"); },

    colReset: function () { console.log("colReset"); },

    colResize: function () { console.log("colResize"); },

    colMove: function () { console.log("colMove"); },

    colToggle: function () { console.log("colToggle"); },

    keyDown: function (ev) { console.log(ev); },

    sort: function () { console.log("sort"); },

    trtSelect: function (ev, id) {
        //updateToolbar();
        console.log("trtSelect: " + ev + ", " + id);
    },

    dblClick: function () { console.log("dblclick"); },

    init: function () {
        this.update.delay(0, this);
    },

    update: function () {
        this.request("/api/torrents", "GET", (function (json) {
            this.parseTorrentList(json);
        }).bind(this));
    },

    parseTorrentList: function (torrentList) {
        var self = this;

        torrentList.each(function (item) {
            var hash = item.InfoHash;
            var row = self.trtDataToRow(item);

            console.log(row);

            if (has(self.torrents, hash)) {
                var rdata = self.trtTable.rowData[hash];

                row.each(function (v, k) {
                    if (v != rdata.data[k]) {
                        self.trtTable.updateCell(hash, k, row);
                    }
                });

            } else {
                self.trtTable.addRow(row, hash, "Status_Up", false);
                self.trtTable.requiresRefresh = true;
            }

            self.torrents[hash] = item;
        });

        this.trtTable.resizePads();
        this.trtTable.refresh();

        this.beginPeriodicUpdate();
    },

    trtDataToRow: function (data) {
        return this.trtCols.map(function (item) {
            switch (item[0]) {
                case "name":
                    return data.Name;

                case "size":
                    return data.Size;

                case "done":
                    return data.Progress + "%";

                case "status":
                    return data.State;

                case "seeds":
                    return data.Seeders;

                case "peers":
                    return data.Peers;

                case "downspeed":
                    return data.DownloadSpeed;

                case "upspeed":
                    return data.UploadSpeed;

                case "eta":
                    return calculateEta(data.DownloadSpeed, data.Size, data.Progress);

                case "uploaded":
                    return data.UploadedBytes;

                case "downloaded":
                    return data.DownloadedBytes;

                case "ratio":
                    return (data.DownloadedBytes == 0 ? 0 : data.UploadedBytes / data.DownloadedBytes);
            }
        }, this);
    },

    request: function (url, method, fn) {
        var self = this;

        var req = function () {
            try {
                new Request.JSON({
                    "url": url,
                    "method": method,
                    "onSuccess": (fn) ? fn.bind(self) : Function.from()
                }).send();
            } catch (e) {
                console.log("error: " + e);
            }
        };

        req();
    },

    beginPeriodicUpdate: function () {
        this.endPeriodicUpdate();

        this.updateTimeout = this.update.delay(this.config.updateInterval, this);
    },

    endPeriodicUpdate: function () {
        clearTimeout(this.updateTimeout);
        clearInterval(this.updateTimeout);
    },

    showAddTorrent: function () {
        DialogManager.show("Add");
    }
};