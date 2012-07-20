window.addEvent("domready", function () {
    setupGlobalEvents();
    setupTorrentListUI();

    hdknWebUI.trtTable.setConfig({ "rowMaxCount": Math.ceil(screen.height / 16) || 100, "rowMode": MODE_VIRTUAL });

    setupToolbar();
    setupDialogManager();

    resizeUI();

    /*
    var rdata = tbl.rowData["abcd"];
    console.log(rdata);
    
    rdata.data[0] = "debian";
    
    tbl.requiresRefresh = true;
    tbl.resizePads();
    tbl.refresh();
    */

    hdknWebUI.init();
});

function setupGlobalEvents() {
    window.addEvent("resize", resizeUI);
}

function setupTorrentListUI() {
    hdknWebUI.trtTable.create("mainTorList", hdknWebUI.trtCols, Object.append({
        "format": hdknWebUI.trtFormatRow.bind(hdknWebUI),
        "sortCustom": hdknWebUI.sortCustom.bind(hdknWebUI),
        "onColReset": hdknWebUI.colReset.bind(hdknWebUI),
        "onColResize": hdknWebUI.colResize.bind(hdknWebUI),
        "onColMove": hdknWebUI.colMove.bind(hdknWebUI),
        "onColToggle": hdknWebUI.colToggle.bind(hdknWebUI),
        "onKeyDown": hdknWebUI.keyDown.bind(hdknWebUI),
        "onSort": hdknWebUI.sort.bind(hdknWebUI),
        "onSelect": hdknWebUI.trtSelect.bind(hdknWebUI),
        "onDblClick": hdknWebUI.dblClick.bind(hdknWebUI)
    }, {
        "colMask": 0x0000, // automatically calculated based on this.trtColDefs
        "colOrder": [], // automatically calculated based on this.trtColDefs
        "colWidth": [], // automatically calculated based on this.trtColDefs
        "reverse": false,
        "sIndex": -1
    }));
}

function setupDialogManager() {
    //--------------------------------------------------
    // DIALOG MANAGER
    //--------------------------------------------------

    DialogManager.init();

    ["About", "Add", "AddEditRSSFeed", "AddURL", "AddLabel", "Props", "RSSDownloader", "Delete"].each(function (k) {
        var isModal = ["Add", "AddEditRSSFeed", "Props"].contains(k);

        DialogManager.add(k, isModal, {
            "Add": function () { /* */ }
            , "AddURL": function () { utWebUI.getDirectoryList(); }
            , "RSSDownloader": function () { utWebUI.rssDownloaderShow(true); }
            // , "Settings": function() { utWebUI.stpanes.onChange(); }
        }[k]);
    });

}

function setupToolbar() {
    //--------------------------------------------------
    // TOOLBAR
    //--------------------------------------------------

//    utWebUI.updateToolbar();

    // -- Buttons

    ["add", "addurl", "setting"].each(function (act) {
        $(act).addStopEvent("click", function (ev) {
            if (ev.target.hasClass("disabled")) {
                return;
            }

            var arg;
            switch (act) {
                case "add": hdknWebUI.showAddTorrent(); break;
                case "addurl": utWebUI.showAddURL(); break;
                case "rssdownloader": utWebUI.showRSSDownloader(); break;
                case "setting": utWebUI.showSettings(); break;

                case "remove": utWebUI.removeDefault(ev.shift); break;

                case "queueup":
                case "queuedown":
                    arg = ev.shift;

                default:
                    utWebUI[act](arg);
            }
        });
    });

    // -- Toolbar Chevron

    $("tbchevron").addStopEvents({
        "mousedown": function (ev) {
            utWebUI.toolbarChevronShow(this);
        },
        "click": null
    });

    // -- Search Field

    $("query").addEvent("keydown", function (ev) {
        if (ev.key == "enter") {
            utWebUI.searchExecute();
        }
    });

    $("search").addStopEvents({
        "mousedown": function (ev) {
            if (ev.isRightClick()) {
                utWebUI.searchMenuShow(this);
            }
        },
        "click": function (ev) {
            utWebUI.searchExecute();
        }
    });

    $("searchsel").addStopEvents({
        "mousedown": function (ev) {
            utWebUI.searchMenuShow(this);
        },
        "click": null
    });
}

function resizeUI() {
    hdknWebUI.trtTable.resizeTo(undefined, 600);
}