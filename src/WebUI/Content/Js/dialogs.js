var Dialogs =
{
    maxZ: 2000,
    visible: [],
    items: {},
    divider: 0,
    modalState: false,
    
    create: function(id, name, content, isModal, noClose)
    {   
        $(document.body).append($("<div>").attr("id", id).addClass("dlg-window").html(content).
            prepend($("<div>").attr("id", id + "-header").addClass("dlg-header").text(name)).
            prepend($("<a></a>").addClass("dlg-close")));
            
        return this.add(id, isModal, noClose);
    },
    
    add: function(id, isModal, noClose)
    {
        var obj = $("#" + id);
        
        if(!isModal)
            isModal = false;
        
        obj.css({ position: "absolute", display: "none", outline: "0px solid transparent" }).
            data("modal", isModal).data("nokeyclose", noClose);
            
        if(!noClose)
        {
            obj.find(".dlg-close").attr("href", "#").bind('click', function()
            {
                Dialogs.hide(id);
            });
        }
        
        var self = this;
        
        var checkForButtons = function me(val)
        {
            if(val.hasClass("Cancel"))
            {
                val.click(function() { Dialogs.hide(id); });
            }
            
            if(val.hasClass("Button"))
            {
                $(val).focus(function() { this.blur(); });
            }
            
            val.children().each(function(ndx, val) { me($(val)); });
        };
        checkForButtons(obj);
        
        var inScrollBarArea = function(obj, x, y)
        {
            if(obj.tagName && (/^input|textarea$/i).test(obj.tagName))
            {
                return false;
            }
            
            var c = $(obj).offset();
            
            return ((obj.scrollHeight > obj.clientHeight) && (x > obj.clientWidth + c.left));
        }
        
        obj.mousedown(function(e)
        {
            if((!browser.isOpera || !inScrollBarArea(e.target, e.clientX, e.clientY)) && !Dialogs.modalState)
            {
                self.bringToTop(this.id);
            }
        }).attr("tabindex", "0").keypress(function(e)
        {
            if((e.keyCode == 13) && !(e.target && e.target.tagName && (/^textarea$/i).test(e.target.tagName)))
            {
                $("#" + id + ".OK").click();
            }
        });
        
        this.items[id] = { beforeShow: null, afterShow: null, beforeHide: null, afterHide: null };
        
        obj.data("dnd", new DnD(id));
        
        return this;
    },
    
    center: function(id)
    {
        var obj = $("#" + id);
        obj.css( { left: Math.max(($(window).width()-obj.width())/2,0), top: Math.max(($(window).height()-obj.height())/2,0) } );
    },
    
    toggle: function(id)
    {
        var pos = $.inArray(id + "", this.visible);
        
        if(pos >= 0)
        {
            this.hide(id);
        }
        else
        {
            this.show(id);
        }
    },
    
    setEffects: function(divider)
    {
        this.divider = divider;
    },
    
    setModalState: function()
    {
        $("#modalbg").show();
        
        this.bringToTop("modalbg");
        this.modalState = true;
    },
    
    clearModalState: function()
    {
        $("#modalbg").hide();
        
        this.modalState = false;
    },
    
    show: function(id, callback)
    {
        var obj = $("#" + id);
        
        if(obj.data("modal"))
        {
            this.setModalState();
        }
        
        if($type(this.items[id]) && ($type(this.items[id].beforeShow) == "function"))
        {
            this.items[id].beforeShow(id);
        }
        
        this.center(id);
        
        obj.show(obj.data("modal") ? null : this.divider, callback);
        
        if($type(this.items[id]) && ($type(this.items[id].afterShow) == "function"))
        {
            this.items[id].afterShow(id);
        }
        
        this.bringToTop(id);
    },
    
    hide: function(id, callback)
    {
        var pos = $.inArray(id + "", this.visible);
        
        if(pos >= 0)
        {
            this.visible.splice(pos, 1);
        }
        
        var obj = $("#" + id);
        
        if($type(this.items[id]) && ($type(this.items[id].beforeHide)=="function"))
        {
            this.items[id].beforeHide(id);
        }
        
        obj.hide(this.divider,callback);
        
        if($type(this.items[id]) && ($type(this.items[id].afterHide)=="function"))
        {
            this.items[id].afterHide(id);
        }
            
        if(obj.data("modal"))
        {
            this.clearModalState();
        }
    },
    
    setHandler: function(id, type, handler)
    {
        if($type(this.items[id]))
        {
            this.items[id][type] = handler;
        }
        
        return this;
    },
    
    isModalState: function()
    {
        return this.modalState;
    },
    
    bringToTop: function(id)
    {
        if($type(this.items[id]))
        {
            var pos = $.inArray(id + "", this.visible);
            
            if(pos >= 0)
            {
                if(pos == this.visible.length - 1)
                {
                    return;
                }
                
                this.visible.splice(pos, 1);
            }
            
            this.visible.push(id);
        }
        
        $("#" + id).css("z-index", ++Dialogs.maxZ);
        
        if(!browser.isOpera)
        {
            $("#" + id).focus();
        }
    },
    
    hideTopMost: function()
    {
        if(this.visible.length && !$("#" + this.visible[this.visible.length - 1]).data("nokeyclose"))
        {
            this.hide(this.visible.pop());
            return true;
        }
        
        return false;
    }
}