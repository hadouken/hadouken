$(document).ready(function() {
    $('#btn-send-notif').on('click', function() {
        $.bootstrapGrowl('Notification!');
    });
});