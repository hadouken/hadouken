define(['signals', 'hasher', 'crossroads'], function (signals, hasher, crossroads) {
    function PageManager() {
        this.currentPage = null;
    }

    PageManager.getInstance = function () {
        if (this._instance === undefined) {
            this._instance = new PageManager();
        }

        return this._instance;
    };

    PageManager.prototype.init = function() {
        if (location.hash === '') {
            hasher.setHash('dashboard');
        }

        hasher.initialized.add(this.parseHash);
        hasher.changed.add(this.parseHash);
        hasher.init();
    };

    PageManager.prototype.parseHash = function(newHash, oldHash) {
        crossroads.parse(newHash);
    };

    PageManager.prototype.addPage = function (page) {
        var that = this;
        
        if (typeof page.routes === 'undefined') {
            return;
        }
        
        for (var i = 0; i < page.routes.length; i++) {
            var route = page.routes[i];
            crossroads.addRoute(route, function () { that.route(page); });
        }
    };

    PageManager.prototype.route = function(page) {
        if (this.currentPage !== null) {
            this.currentPage.unload();
        }

        this.currentPage = page;
        this.currentPage.load(arguments);
    };

    return PageManager;
});