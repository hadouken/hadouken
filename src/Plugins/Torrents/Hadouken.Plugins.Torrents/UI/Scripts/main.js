console.log("Loading Torrents plugin");

var TorrentsListPage = new Class({
    Extends: Page,
    
    initialize: function() {
        this.parent("/plugins/core.torrents/list.html");
    },

    load: function() {
        console.log("TorrentsListPage::load()");
    },
    
    unload: function() {
        console.log("TorrentsListPage::unload()");
    }
});

try {

    MainMenu.addItem(
        new MenuItem("Torrents", {
            onClick: function() {
                Pages.navigateTo(new TorrentsListPage());
            }
        })
    );

} catch(e) {
    console.log(e);
}