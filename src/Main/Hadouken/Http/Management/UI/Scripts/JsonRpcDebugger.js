$(document).ready(function () {
    $('#btn-send-json').on('click', function(e) {
        e.preventDefault();

        var json = $('#json').val();
        var startTime = new Date().getTime();
        var data = JSON.parse(json);

        $.jsonRPC.request(data.method, {
            params: data.params,
            success: function(result) {
                var responseTime = new Date().getTime() - startTime;
                $('#responseTime').text(responseTime);
                $('#result').val(JSON.stringify(result, undefined, 2));
            }
        });
    });
});