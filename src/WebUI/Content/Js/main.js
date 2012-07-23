$(document).ready(function() {
    loadContent();
    loadDialogs();
    loadEvents();
    loadDividers();
    
    ContextMenu.init();
    Tabs.init();
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
    
    Network.getHtml("/dialogs/label.html", function(data)
    {
        Dialogs.create("dlgLabel", "New label", data, true);
    });
    
    Network.getHtml("/dialogs/about.html", function(data)
    {
        Dialogs.create("dlgAbout", "About Hadouken", data, true);
    });
}

function loadEvents()
{
    $(".cat").mouseclick(WebUI.labelContextMenu);
}

function loadDividers()
{
    new DnD("HDivider",
    {
        left : function() { return(60); },
        right : function() { return( $(window).width()-20 ); },
        restrictY : true,
        maskId : "dividerDrag",
        onStart : function(e) { return(WebUI.settings["webui.show_cats"]); },
        onRun : function(e) { $(document.body).css( "cursor", "e-resize" ); },
        onFinish : function(e) 
        {
            var self = e.data;
            var w = self.mask.offset().left-2;
            WebUI.resizeLeft(w,null);    
            w = $(window).width()-w-11;
            WebUI.resizeTop(w,null);
            WebUI.resizeBottom(w,null);
            WebUI.setHSplitter();
            $(document.body).css( "cursor", "default" );
        }
    });
    
    new DnD("VDivider",
    {
        top : function() { return(160); },
        bottom : function() { return( $(window).height()-60 ); },
        restrictX : true,
        maskId : "dividerDrag",
        onStart : function(e) { return(WebUI.settings["webui.show_dets"]); },
        onRun : function(e) { $(document.body).css( "cursor", "n-resize" ); },
        onFinish : function(e) 
        {
            var self = e.data;
            var offs = self.mask.offset();
            WebUI.resizeTop(null,offs.top-($("#toolbar").is(":visible") ?  $("#toolbar").height() : -1)-8);
            WebUI.resizeBottom(null,$(window).height()-offs.top-$("#statusbar").height()-14);
            WebUI.setVSplitter();
            $(document.body).css( "cursor", "default" );
        }
    });

}