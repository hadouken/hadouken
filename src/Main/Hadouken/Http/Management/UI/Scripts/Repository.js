$(document).ready(function () {
    function loadPlugins(plugins) {
        $('#plugins-list').empty();

        for (var i = 0; i < plugins.length; i++) {
            var plugin = plugins[i];

            var row = $('<tr></tr>');
            row.append($('<td></td>').text(plugin.id));
            row.append($('<td></td>').text(plugin.description));

            var latestVersion = '';

            if (typeof plugin.latestRelease !== 'undefined') {
                latestVersion = plugin.latestRelease.version;
            }

            row.append($('<td></td>').text(latestVersion));

            var detailsButton = $('<button type="button" class="btn btn-primary btn-xs pull-right">Details</button>');
            detailsButton.on('click', function() {
                showDetails(plugin.id);
            });

            row.append($('<td></td>').append(detailsButton));

            $('#plugins-list').append(row);
        }
    }

    function showDetails(pluginId) {
        $.getJSON('/manage/api/repository/' + pluginId, function() {

        });
    }

    $.getJSON('/manage/api/repository', function (plugins) {
        loadPlugins(plugins);
    });
});