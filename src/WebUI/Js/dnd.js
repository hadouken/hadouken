// Drag & Drop object 
function DnD( id, options )
{
        this.obj = $('#'+id);
        var headers = this.obj.find(".dlg-header");
        var header = headers.length>0 ? $(headers[0]) : this.obj;
        this.options = options || {};
        if(!this.options.left)
                this.options.left = function() { return(0); };
        if(!this.options.top)
                this.options.top = function() { return(0); };
        if(!this.options.right)
                this.options.right = function() { return($(window).width()); };
        if(!this.options.bottom)
                this.options.bottom = function() { return($(window).height()); };
        if(!this.options.onStart)
                this.options.onStart = function() { return(true); }
        if(!this.options.onRun)
                this.options.onRun = function() {}
        if(!this.options.onFinish)
                this.options.onFinish = function() {}
        if(!this.options.maskId)
                this.options.maskId = 'dragmask';
        this.detachedMask = (this.options.maskId!=id);
        this.mask = $('#'+this.options.maskId);
        header.unbind( "mousedown" );
        header.bind( "mousedown", this, this.start );
}

DnD.prototype.start = function( e )
{
        var self = e.data;
        if(self.options.onStart(e))
        {
                var offs = self.obj.offset();
                Dialogs.bringToTop(self.obj.attr("id"))
                Dialogs.bringToTop(self.mask.attr("id"))
                if(self.detachedMask)
                {
                        self.mask.css( { left: offs.left, top: offs.top, width: self.obj.width(), height: self.obj.height() } );
                        self.mask.show();
                }
                self.delta = { x: e.clientX-offs.left, y: e.clientY-offs.top };
                $(document).bind("mousemove",self,self.run); 
                $(document).bind("mouseup",self,self.finish);
        }       
        return(false);
}

DnD.prototype.run = function( e )
{
        var self = e.data;
        if(!self.options.restrictX)
                self.mask.css( { left: Math.min(Math.max(self.options.left(), e.clientX),self.options.right())-self.delta.x } ); 
        if(!self.options.restrictY)
                self.mask.css( { top: Math.min(Math.max(self.options.top(), e.clientY),self.options.bottom())-self.delta.y } );
        self.options.onRun(e);
        return(false);
}

DnD.prototype.finish = function( e )
{
        var self = e.data;
        self.options.onFinish(e);
        if(self.detachedMask)
        {
                var offs = self.mask.offset();
                self.mask.hide();
                self.obj.css( { left: offs.left, top: offs.top } );
        }
        $(document).unbind("mousemove",self.run); 
        $(document).unbind("mouseup",self.finish);
        return(false);
}
