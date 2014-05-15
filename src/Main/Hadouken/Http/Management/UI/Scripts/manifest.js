(function(manifest) {
    var filename = 'manifest.json';

    manifest.read = function(dataUrl, callback) {
        zip.workerScriptsPath = "/Scripts/";
        zip.createReader(new zip.Data64URIReader(dataUrl), function (reader) {
            reader.getEntries(function (entries) {
                if (entries.length) {
                    var manifest = $.grep(entries, function (ent) { return ent.filename === filename; })[0];

                    if (manifest == null) {
                        // Error
                    } else {
                        manifest.getData(new zip.TextWriter(), function (text) {
                            var json = JSON.parse(text);
                            callback(json);
                        });
                    }
                }
            });
        });
    }

})(window.manifest || (window.manifest = {}));