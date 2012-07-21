$(document).ready(function() {
    loadContent();
    loadDialogs();
    
    ContextMenu.init();
    WebUI.init();
});

function loadContent()
{
    $(document.body).append($("<iframe name='uploadfrm'/>").css({visibility: "hidden"}).attr( { name: "uploadfrm" } ).width(0).height(0).load(function()
    {
        $("#torrent_file").val("");
        $("#add_button").attr("disabled", false);
        
        var d = (this.contentDocument || this.contentWindow.document);
        
        if(d && (d.location.href != "about:blank"))
            try { eval(d.body.innerHTML); } catch(e) {}
    }));
}

function loadDialogs()
{
    Network.getHtml("/dialogs/addtorrent.html", function(data)
    {
        Dialogs.create("dlgAddTorrent", "Add torrent", data, true);
    });
}