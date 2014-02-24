$(document).ready(function() {
    $('#plugins-list').on('click', '.btn-plugin-details', function () {
        var id = $(this).parents('tr').attr('data-plugin-id');
        $.get('/manage/plugins/details/' + id, function(html) {
            $(html).modal();
        });
    });

    $('.btn-upload-package').on('click', function(e) {
        e.preventDefault();
        $.get('/manage/plugins/upload', function(html) {
            $(html).modal();
        });
    });
});