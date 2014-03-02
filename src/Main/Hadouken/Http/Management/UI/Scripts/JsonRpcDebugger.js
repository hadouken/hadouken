$(document).ready(function () {
    $('#btn-send-json').on('click', function(e) {
        e.preventDefault();

        var pluginId = $('#pluginId').val();
        var json = $('#json').val();

        var startTime = new Date().getTime();

        var data = JSON.parse(json);
        rpc(pluginId, data.method, data.params, function(result) {
            var responseTime = new Date().getTime() - startTime;
            $('#responseTime').text(responseTime);
            $('#result').val(JSON.stringify(result, undefined, 2));
        });
    });
});