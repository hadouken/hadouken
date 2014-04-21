$(document).ready(function () {
    var connection = $.hubConnection();
    var hub = connection.createHubProxy("events");

    hub.on('publishEvent', function (obj) {
        var name = obj.name;
        var data = obj.data;

        var row = $('<tr><td class="event-time"></td><td class="event-name"></td><td><pre class="event-data"></pre></td></tr>');

        var date = new Date();
        var time = date.getHours() + ":" + date.getMinutes() + ":" + date.getSeconds() + "." + date.getMilliseconds();

        row.find('.event-time').text(time);
        row.find('.event-name').text(name);
        row.find('.event-data').text(JSON.stringify(data));

        $('#eventsList').prepend(row);
    });

    connection.start()
        .done(function() {
            // Show green icon or something
        })
        .fail(function() {
            // Show red icon
        });
});