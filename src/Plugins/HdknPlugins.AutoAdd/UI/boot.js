(function () {
    var AutoAdd = {
        "folders": {},
        "folderId": -1,
        "fldTable": new STable(),
        "config": {

        },

        "init": function () {
            var self = this;
            
            utWebUI.request("get", "action=autoadd-getwatchedfolders", null, function (data) {
                $each(data.folders, function (val) {
                    self.addFolder(val);
                });
            });
        },

        "colDefs": [
            ["path",  240, TYPE_STRING],
            ["label", 235, TYPE_STRING]
        ],

        "formatRow": function (values, index) {
            var useidx = $chk(index);

            if (useidx)
                return values[index];
            else
                return values;
        },

        "onColReset": function () {
            var config = {
                  "colMask": 0
			    , "colOrder": this.colDefs.map(function (item, idx) { return idx; })
			    , "colWidth": this.colDefs.map(function (item, idx) { return item[1]; })
            };

            this.colDefs.each(function (item, idx) { if (!!item[3]) config.colMask |= (1 << idx); });
            this.fldTable.setConfig(config);
        },

        "onSelect": function (ev, id) {
            // id is folder id
            if(has(this.folders, id)) {
                this.folderId = id;
            } else {
                this.folderId = -1;
            }
        },

        "addFolder": function(val) {
            this.fldTable.addRow([val.Path, val.Label], val.Id);

            // add to local dictionary as well
            this.folders[val.Id] = val;
        },
        
        "refreshTable": function () {
            this.fldTable.calcSize();
            this.fldTable.restoreScroll();
            this.fldTable.resizePads();
        }
    };

    var html = '<fieldset>' +
                 '<legend>Watched folders</legend>' +
                 '<div id="autoadd-Folders"></div>' +
                 '<input type="button" class="btn" value="Remove" id="autoadd_remove_folders" />' +
               '</fieldset>' +

               '<fieldset>' +
                 '<legend>Add folder</legend>' +

                 '<div class="line-cont">' +
                   '<div class="half fll">' +
                     '<label for="autoadd_add_folder_path">Path</label><br />' +
                     '<input type="text" id="autoadd_add_folder_path" class="tbox wide" />' +
                   '</div>' +
                   '<div class="half flr">' +
                     '<label for="autoadd_add_folder_label">Label</label><br />' +
                     '<input type="text" id="autoadd_add_folder_label" class="tbox" /> ' +
                     '<input type="button" class="btn" value="Add" id="autoadd_add_folder" />' +
                   '</div>' +
                 '</div>' +
               '</fieldset>';

    var settings = new Element("div", {
        id: "dlgSettings-Plugins-AutoAdd",
        "class": "settings-pane",
        html: html
    });

    settings.inject($("dlgSettings-body"));

    AutoAdd.fldTable.create("autoadd-Folders", AutoAdd.colDefs, {
        "rowMultiSelect": false,
        "format": AutoAdd.formatRow.bind(AutoAdd),
        "onColReset": AutoAdd.onColReset.bind(AutoAdd),
        "onSelect": AutoAdd.onSelect.bind(AutoAdd),
    });

    AutoAdd.fldTable.setConfig({
        "rowMaxCount": utWebUI.limits.maxVirtTableRows,
        "rowMode": MODE_VIRTUAL,
        "colText": {
            "path": "Path",
            "label": "Label"
        }
    });

    utWebUI.stpanes.addTab("dlgSettings-Plugins-AutoAdd", "AutoAdd").show("dlgSettings-General");

    var tblSize = $("dlgSettings-Plugins-AutoAdd").getDimensions({ computeSize: true });
    AutoAdd.fldTable.resizeTo(tblSize.x - 15, tblSize.y - 160);

    SettingsManager.addEvent("paneChanged", function (id) {
        if (id != "dlgSettings-Plugins-AutoAdd") return;

        AutoAdd.refreshTable();
    });

    $("autoadd_add_folder").addEvent("click", function () {
        var d = [{ "Path": $("autoadd_add_folder_path").get("value"), "Label": $("autoadd_add_folder_label").get("value")}];

        utWebUI.request("post", "action=autoadd-setwatchedfolders", d, function (folders) {
            for (var i = 0; i < folders.length; i++) {
                AutoAdd.addFolder(folders[i]);

                $("autoadd_add_folder_path").set("value", "");
                $("autoadd_add_folder_label").set("value", "");
            }

            AutoAdd.refreshTable();
        });
    });

    $("autoadd_remove_folders").addEvent("click", function() {
        if(AutoAdd.folderId === -1) {
            alert("Select folder to remove");
            return;
        }

        var d = [AutoAdd.folderId];

        utWebUI.request("post", "action=autoadd-remwatchedfolders", d, function(data) {
            for(var i = 0; i < data.removed.length; i++) {
                AutoAdd.fldTable.removeRow(data.removed[i]);
            }

            AutoAdd.refreshTable();
        });
    });

    AutoAdd.init();

    window.AutoAdd = AutoAdd;
})();