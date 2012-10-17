var Plugin = new Class({
    Implements: [ Options, Events ],
    
    initialize: function(id, options) {
        this.setOptions(options);
        
        this.id = id;
    },
    
    load: function() {
        this.fireEvent("load");
    }
});

(function() {

var SettingsManager = new Class({
    Implements: [ Events ],
    
    changePane: function(id) {
        this.fireEvent("paneChanged", id);
    }
});

window.SettingsManager = new SettingsManager();

})();