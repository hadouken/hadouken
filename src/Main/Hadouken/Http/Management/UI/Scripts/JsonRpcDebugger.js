$(document).ready(function () {
    $('#btn-send-json').on('click', function(e) {
        e.preventDefault();

        var pluginId = $('#pluginId').val();
        var json = $('#json').val();

        var startTime = new Date().getTime();

        $.post("/manage/jsonrpc", { pluginId: pluginId, json: json }, function (result) {
            var responseTime = new Date().getTime() - startTime;
            $('#responseTime').text(responseTime);

            var jsonResult = JSON.parse(result);
            $('#result').val(JSON.stringify(jsonResult, undefined, 2));
        });
    });
});