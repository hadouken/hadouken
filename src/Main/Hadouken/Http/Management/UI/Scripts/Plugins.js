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

                        $.post('/plugins/uninstall', { id: id }, function() {
                            content.modal('hide');
                            location.reload();
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
        $.get('/plugins/upload', function(html) {
            $(html).modal();
        });
    });
});