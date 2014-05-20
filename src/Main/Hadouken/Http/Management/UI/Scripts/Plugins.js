$(document).ready(function() {
    $('#plugins-list').on('click', '.btn-plugin-details', function () {
        var id = $(this).parents('tr').attr('data-plugin-id');

        $.jsonRPC.request('core.plugins.getDetails', {
            params: [id],
            success: function(response) {
                var plugin = response.result;
                var template = $('#details-template').html();

                var dlg = bootbox.dialog({
                    title: 'Details for "' + id + '"',
                    message: template,
                    buttons: {
                        confirmUninstall: {
                            label: 'Uninstall',
                            className: 'btn-danger btn-xs',
                            callback: function () {
                                showUninstall(dlg, id);
                                return false;
                            }
                        },
                        uninstall: {
                            label: 'Uninstall',
                            className: 'btn-danger btn-uninstall',
                            callback: function () { return false; }
                        },
                        unload: {
                            label: 'Unload',
                            className: 'btn-warning btn-unload',
                            callback: function(e) {
                                var target = $(e.target);

                                target.prepend($('<i class="fa fa-spinner fa-spin"></i>'));
                                target.attr('disabled', true);

                                unloadPlugin(id);

                                return false;
                            }
                        },
                        load: {
                            label: 'Load',
                            className: 'btn-primary btn-load',
                            callback: function (e) {
                                var target = $(e.target);

                                target.prepend($('<i class="fa fa-spinner fa-spin"></i>'));
                                target.attr('disabled', true);

                                loadPlugin(id);

                                return false;
                            }
                        }
                    }
                });

                $(dlg).find('.pluginName').text(plugin.Name);
                $(dlg).find('.pluginVersion').text(plugin.Version);
                $(dlg).find('.pluginPath').text(plugin.Path);

                $(dlg).find('.btn-uninstall').hide();

                if (plugin.State === 'Loaded') {
                    $(dlg).find('.btn-load').hide();
                } else {
                    $(dlg).find('.btn-unload').hide();
                }
            }
        });
    });

    $('.btn-upload-package').on('click', function(e) {
        e.preventDefault();
        var title = $('#upload-template').attr('data-title');
        var template = $('#upload-template').html();

        var dlg = bootbox.dialog({
            title: title,
            message: template,
            buttons: {
                cancel: {
                    label: 'Cancel'
                },
                upload: {
                    label: 'Upload',
                    className: 'btn-primary',
                    callback: function (ev) {
                        var target = $(ev.target);

                        target.prepend($('<i class="fa fa-spinner fa-spin"></i>'));
                        target.attr('disabled', true);

                        var password = $(dlg).find('#password').val();

                        // Read file
                        var fileInput = $('#package')[0];
                        var reader = new FileReader();

                        var error = function() {
                            $(dlg).modal('hide');
                            $.bootstrapGrowl('Could not install plugin. See log for details.', { type: 'danger' });
                        };

                        reader.onload = function(readerArgs) {
                            $.jsonRPC.request('core.plugins.install', {
                                params: [password, readerArgs.target.result.split(',')[1]],
                                success: function (response) {
                                    if (response.result) {
                                        location.reload();
                                    } else {
                                        error();
                                    }
                                },
                                error: function (response) {
                                    if (response.error.code === 1000) {
                                        $(dlg).find('#password').closest('.form-group').addClass('has-error');
                                        target.attr('disabled', false).find('i').remove();
                                    } else {
                                        error();
                                    }
                                }
                            });
                        };

                        reader.readAsDataURL(fileInput.files[0]);

                        return false;
                    }
                }
            }
        });

        $(dlg).find('.btn-primary').attr('disabled', true);
        $(dlg).find('.manifest-data').hide();

        $(dlg).find('#package').on('change', function(evt) {
            var files = evt.target.files;
            $(dlg).find('.btn-primary').attr('disabled', (files.length === 0));

            if (files.length == 0) {
            } else {
                var file = files[0];
                var reader = new FileReader();

                reader.onload = (function() {
                    return function (e) {
                        manifest.read(e.target.result, function(m) { showManifest(m); });
                    }
                })(file);

                reader.readAsDataURL(file);
            }
        });
    });
});

function unloadPlugin(pluginId) {
    $.jsonRPC.request('core.plugins.unload', {
        params: [pluginId],
        success: function () {
            location.reload();
        }
    });
}

function loadPlugin(pluginId) {
    $.jsonRPC.request('core.plugins.load', {
        params: [pluginId],
        success: function () {
            location.reload();
        }
    });
}

function showManifest(manifest) {
    $('.manifest-data').show();
    
    if (manifest.dependencies && manifest.dependencies.length > 0) {
        var list = $('#dependenciesList');
        list.empty();

        for (var i = 0; i < manifest.dependencies.length; i++) {
            var dependency = manifest.dependencies[i];
            var item = $('<li>', {
                text: dependency.id + ' (' + dependency.version + ')'
            });

            list.append(item);
        }
    }
}

function showUninstall(dialog, pluginId) {
    var template;

    $.jsonRPC.request('core.plugins.canUninstall', {
        params: [pluginId],
        success: function (response) {
            if (response.result.CanUninstall) {
                template = $('#uninstall-template').html();
                $(dialog).find('.btn').hide();

                $(dialog)
                    .find('.btn-uninstall')
                    .on('click', function () {
                        var password = $(dialog).find('#password').val();
                        uninstallPlugin(password, pluginId);
                    })
                    .show();

                $(dialog).find('.bootbox-body').empty().append($(template));
            } else {
                template = $('#uninstallUnvavailable-template').html();
                $(dialog).find('.bootbox-body').empty().append($(template));

                for (var i = 0; i < response.result.Dependencies.length; i++) {
                    var dep = response.result.Dependencies[i];
                    var li = $('<li>').text(dep);

                    $(template).find('#dependencies').append(li);
                }
            }
        }
    });
}

function uninstallPlugin(password, pluginId) {
    $.jsonRPC.request('core.plugins.uninstall', {
        params: [password, pluginId],
        success: function() {
            location.reload();
        }
    });
}