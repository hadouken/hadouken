var MenuItem = new Class({
    Implements: Options,

    title: "",
    
    options: {
        onClick: Function.from()
    },

    initialize: function(title, options) {
        this.title = title;
        this.setOptions(options);
    },
    
    toElement: function() {
        var li = new Element('li');
        var a = new Element('a', {
            text: this.title,
            href: '#',
            events: {
                'click': function(e) {
                    e.preventDefault();
                    this.options.onClick();
                }.bind(this)
            }
        });

        li.adopt(a);

        return li;
    }
});

var MainMenu = {
    addItem: function(menuItem) {
        $("nav-main").adopt($(menuItem));
    }
};