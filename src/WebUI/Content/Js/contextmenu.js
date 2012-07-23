var CMENU_SEP = " 0";
var CMENU_CHILD = " 1";
var CMENU_SEL = " 2";

var ContextMenu =
{
    mouse: { x: 0, y: 0 },
    noHide: false,
    
    // init
    init: function()
    {
        var self = this;
        
        $(document).mousemove(function(e)
        {
            self.mouse.x = e.clientX;
            self.mouse.y = e.clientY;
        });
        
        $(document).mouseup(function(e)
        {
            var element = $(e.target);
            
            if(e.which == 3)
            {
                // if right clicking in text control, allow, otherwise stop it
                if(!e.fromTextCtrl)
                {
                    e.stopPropagation();
                }
            }
            else
            {
                if(!element.hasClass("top-menu-item") &&
                    !element.hasClass("exp") &&
                    !element.hasClass("CMenu") &&
                    !(element.hasClass("menu-cmd") && element.hasClass("dis")) &&
                    !element.hasClass("menuitem") &&
                    !element.hasClass("menu-line"))
                {
                    if(element.hasClass("menu-cmd") && self.noHide)
                    {
                        element.toggleClass("sel");
                    }
                    else
                    {
                        window.setTimeout("ContextMenu.hide()", 50);
                    }
                }
            }
        });
        
        this.obj = $("<ul>").css({ position: "absolute" }).addClass("CMenu");
        
        $(document.body).append(this.obj);
    },
    
    get: function(label)
    {
        var ret = null;
        
        $("a", this.obj).each(function(ndx, val)
        {
            if($(val).text() == label)
            {
                ret = $(val).parent();
                return false;
            }
        });
        
        return ret;
    },
    
    add: function()
    {
        var args = [];
        
        $.each(arguments, function(ndx, val)
        {
            args.push(val);
        });
        
        var aft = null;
        
        if(($type(args[0]) == "object") && args[0].hasClass && args[0].hasClass("CMenu"))
        {
            var o = args[0];
            args.splice(0, 1);
        }
        else
        {
            var o = this.obj;
        }
        
        if(($type(args[0]) == "object") && args[0].hasClass && args[0].hasClass("menuitem"))
        {
            aft = args[0];
            args.splice(0, 1);
        }
        
        var self = this;
        
        $.each(args, function(ndx, val)
        {
            if($type(val))
            {
                var li = $("<li>").addClass("menuitem");
                
                if(val[0] == CMENU_SEP)
                {
                    li.append($("<hr>").addClass("menu-line"));
                }
                else if(val[0] == CMENU_CHILD)
                {
                    li.append($("<a></a>").addClass("exp").text(val[1]));
                    
                    var ul = $("<ul>").addClass("CMenu");
                    for(var j = 0, len = val[2].length; j < len; j++)
                    {
                        self.add(ul, val[2][j]);
                    }
                    
                    li.append(ul);
                }
                else if(val[0] == CMENU_SEL)
                {
                    var a = $("<a></a>").addClass("sel menu-cmd").text(val[1]);
                    
                    switch($type(val[2]))
                    {
                        case "string":
                            a.attr("href", "#").click(function() { eval(val[2]); });
                            break;
                            
                        case "function":
                            a.attr("href", "#").click(val[2]);
                            break;
                    }
                    
                    li.append(a.focus(function() { this.blur(); }));
                }
                else
                {
                    if($type(val[0]))
                    {
                        var a = $("<a></a>").addClass("menu-cmd").text(val[0]);
                        
                        switch($type(val[1]))
                        {
                                case false:
                                    a.addClass("dis");
                                    break;
                                    
                                case "string":
                                    a.attr("href","#").click( function() { eval(val[1]) } );
                                    break;
                                    
                                case "function":
                                    a.attr("href","#").click(val[1]);
                                    break;
                        }
                        
                        li.append(a.focus( function() { this.blur(); } ));
                    }
                }
                
                if(aft)
                {
                    aft.after(li);
                }
                else
                {
                    o.append(li);
                }
            }
        });
    },
    
    clear: function()
    {
        this.obj.empty();
    },
    
    setNoHide: function()
    {
        this.noHide = true;
    },
    
    show: function(x, y)
    {
        if(x == null)
        {
            x = this.mouse.x;
        }
        
        if(y == null)
        {
            y = this.mouse.y;
        }
        
        if(x + this.obj.width() > $(window).width())
        {
            x -= this.obj.width();
        }
        
        if(y + this.obj.height() > $(window).height())
        {
            y -= this.obj.height();
        }
        
        if(y < 0)
        {
            y = 0;
        }

        this.obj.css({ left: x, top: y, "z-index": ++Dialogs.maxZ });
        
        $("ul.CMenu a.exp").hover(function()
        {
            var submenu = $(this).next();
            
            if(submenu.offset().left + submenu.width() > $(window).width())
            {
                submenu.css({ "left": -150 });
            }
            
            if(submenu.offset().top + submenu.height() > $(window).height())
            {
                submenu.css({ "top": -submenu.height() + 20 });
            }
        });
        
        this.obj.show(Dialogs.divider);
    },
    
    hide: function()
    {
        this.noHide = false;
        
        if(this.obj.is(":visible"))
        {
            this.obj.hide(Dialogs.divider);
            this.clear();
            return true;
        }
        
        return false;
    }
};