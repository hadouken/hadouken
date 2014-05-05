$(document).ready(function () {
    function loadPlugins(plugins, installedPlugins) {
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
                    bootbox.dialog({
                        title: 'Confirm installation',
                        message: 'This will install <strong>' + plugin.id + '</strong> and any dependencies it might have.',
                        buttons: {
                            main: {
                                label: 'Install ' + plugin.id,
                                className: 'btn-primary',
                                callback: function (e) {
                                    var target = $(e.target);

                                    target.prepend($('<i class="fa fa-spinner fa-spin"></i>'));
                                    target.attr('disabled', true);

                                    install(plugin.id, function () {
                                        location.reload();
                                    });

                                    return false;
                                }
                            }
                        }
                    });
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

    function install(pluginId, callback) {
        $.jsonRPC.request('core.repository.install', {
            params: [pluginId],
            success: function() {
                callback();
            }
        });
    };

    $.jsonRPC.request('core.repository.list', {
        success: function (data) {
            $.jsonRPC.request('core.plugins.list', {
                success: function(data2) {
                    loadPlugins(data.result, data2.result);
                }
            });
        }
    });
});