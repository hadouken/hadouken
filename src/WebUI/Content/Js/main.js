window.addEvent("domready", function() {
    setupGlobalEvents();
    
    setupTorrentListUI();

    hdknWebUI.trtTable.setConfig({ "rowMaxCount": Math.ceil(screen.height / 16) || 100, "rowMode": MODE_VIRTUAL });
    
    hdknWebUI.trtTable.requiresRefresh = true;
    hdknWebUI.trtTable.resizePads();
    hdknWebUI.trtTable.refresh();
    hdknWebUI.trtTable.calcSize();
    
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

function resizeUI() {
    hdknWebUI.trtTable.resizeTo(undefined, 600);
}