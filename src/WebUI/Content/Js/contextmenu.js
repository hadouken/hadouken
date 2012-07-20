var ContextMenu =
{
    mouse: { x: 0, y: 0 },
    noHide: false,
    
    // init
    init: function() {
        var self = this;
        
        $(document).mousemove(function(e) {
            self.mouse.x = e.clientX;
            self.mouse.y = e.clientY;
        });
        
        $(document).mouseup(function(e) {
            var element = $(e.target);
            
            if(e.which == 3) {
                // if right clicking in text control, allow, otherwise stop it
                if(!e.fromTextCtrl) {
                    e.stopPropagation();
                }
            } else {
                //
            }
        });
    }
};