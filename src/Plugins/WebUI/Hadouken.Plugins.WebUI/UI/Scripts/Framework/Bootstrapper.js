var Hadouken;
(function (Hadouken) {
    var Bootstrapper = (function () {
        function Bootstrapper() {
        }
        Bootstrapper.prototype.init = function () {
            console.log("init");
        };
        return Bootstrapper;
    })();
    Hadouken.Bootstrapper = Bootstrapper;
})(Hadouken || (Hadouken = {}));
