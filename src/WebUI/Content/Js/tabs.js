var Tabs =
{
    tabs:
    {
        general: "General",
        files: "Files",
        trackers: "Trackers"
    },
    
    init: function()
    {
        var s = "";
        
        for(var n in this.tabs)
        {
            s += "<li id=\"tab_" + n + "\"><a href=\"#\" onmousedown=\"Tabs.show('" + n + "'); return false;\" onfocus=\"this.blur();\">" + this.tabs[n] + "</a></li>";
        }
        
        $("#tabbar").html(s);
        
        this.show("general");
    },
    
    onShow: function(id)
    {
    },
    
    show: function(id)
    {
        var p = null, l = null;
        
        for(var n in this.tabs)
        {
            if((l = $("#tab_" + n)).length && (p = $("#tab_" + n + "_content")).length)
            {
                if(n == id)
                {
                    p.show();
                    
                    var prefix = null;
                    
                    switch(n)
                    {
                        case "files":
                            prefix = "files";
                            break;
                            
                        default:
                            this.onShow(n);
                            break;
                    }
                    
                    if(prefix)
                    {
                        WebUI.getTable(prefix).refreshRows();
                    }
                    
                    WebUI.setActiveView(id);
                    
                    l.addClass("selected").css("z-index", 1);
                }
                else
                {
                    p.hide();
                    
                    l.removeClass("selected").css("z-index", 0);
                }
            }
        }
    }
};