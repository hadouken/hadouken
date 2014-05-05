$(document).ready(function() {
    $('#plugins-list').on('click', '.btn-plugin-details', function () {
        var id = $(this).parents('tr').attr('data-plugin-id');
        $.get('/plugins/details/' + id, function (html) {
            var content = $(html);
            $('body').append(content);

            content.modal();
            content.on('hidden.bs.modal', function() { content.remove(); });

            content.find('#btn-uninstall-plugin').on('click', function () {
                $(this).hide();

                $.get('/plugins/uninstall/' + id, function(uninstallHtml) {
                    content.find('#details-body').html(uninstallHtml);

                    var btnConfirm = content.find('#btn-confirm-uninstall');

                    btnConfirm.attr('disabled', true);
                    btnConfirm.on('click', function () {
                        btnConfirm.attr('disabled', true);

                        $.jsonRPC.request('core.plugins.uninstall', {
                            params: [id],
                            success: function() {
                                content.modal('hide');
                                location.reload();
                            }
                        });
                    });

                    content.find('#confirmed-plugin-id').on('keyup', function() {
                        btnConfirm.attr('disabled', $(this).val() !== id);
                    });
                });
            });
        });
    });

    $('.btn-upload-package').on('click', function(e) {
        e.preventDefault();
        var template = $('#upload-template').html();

        var dlg = bootbox.dialog({
            title: 'Upload plugin',
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

                        // Read file
                        var fileInput = $('#package')[0];
                        var reader = new FileReader();

                        reader.onload = function(readerArgs) {
                            $.jsonRPC.request('core.plugins.install', {
                                params: [readerArgs.target.result.split(',')[1]],
                                success: function (response) {
                                    if (response.result) {
                                        location.reload();
                                    } else {
                                        throw new Error('Could not install package. Check log.');
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
    });
});