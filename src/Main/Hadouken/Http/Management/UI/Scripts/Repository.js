$(document).ready(function () {
    function loadPlugins(plugins) {
        $('#plugins-list').empty();

        for (var i = 0; i < plugins.length; i++) {
            (function (plugin) {

                var row = $('<tr></tr>');
                row.append($('<td></td>').text(plugin.id));
                row.append($('<td></td>').text(plugin.description));

                var latestVersion = '';

                if (typeof plugin.latestRelease === 'undefined' || plugin.latestRelease === null) {
                    return;
                }

                latestVersion = plugin.latestRelease.version;

                row.append($('<td></td>').text(latestVersion));

                var installButton = $('<button type="button" class="btn btn-primary btn-xs pull-right">Install</button>');
                installButton.on('click', function () {
                    prepareInstall(plugin.id);
                });

                var updateButton = $('<button type="button" class="btn btn-warning btn-xs pull-right">Update</button>');
                updateButton.on('click', function () {
                    $(this).attr('disabled', true);

                    install(plugin.id, function() {
                        location.reload();
                    });
                });

                var alreadyInstalled = $('<span class="pull-right"><em>(installed)</em></span>');

                $.jsonRPC.request('core.plugins.getVersion', {
                    params: [plugin.id],
                    success: function (response) {
                        var version = response.result;

                        if (installedPlugins.indexOf(plugin.id) != -1) {
                            if (semver.gt(latestVersion, version)) {
                                row.append($('<td></td>').append(updateButton)).addClass('warning');
                            } else {
                                row.append($('<td></td>').append(alreadyInstalled));
                            }
                        } else {
                            row.append($('<td></td>').append(installButton));
                        }

                        $('#plugins-list').append(row);
                    }
                });

            })(plugins[i]);
        }
    }

    function prepareInstall(pluginId) {
        $.getJSON('/api/repository/' + pluginId, function (data) {
            (function(plugin) {
                $.get('/repository/install/' + plugin.id, function(html) {
                    var content = $(html);
                    $('body').append(html);

                    content.modal();
                    content.on('hidden.bs.modal', function() { content.modal('hide'); });

                    content.find('.plugin-name').text(plugin.id);

                    content.find('#btn-confirm-install').on('click', function () {
                        content.find('#btn-confirm-install').attr('disabled', true);

                        install(plugin.id, function() {
                            content.modal('hide');
                            location.reload();
                        });
                    });
                });
            })(data);
        });
    }

    function install(pluginId, callback) {
        $.post('/api/repository/install', { id: pluginId }, function(response) {
            callback();
        });
    };

    $.getJSON('/api/repository', function (plugins) {
        loadPlugins(plugins);
    });
});