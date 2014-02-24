$(document).ready(function() {
    $('#plugins-list').on('click', '.btn-plugin-details', function () {
        var id = $(this).parents('tr').attr('data-plugin-id');
        $.get('/management/plugins/details/' + id, function(html) {
            $(html).modal();
        });
    });
});