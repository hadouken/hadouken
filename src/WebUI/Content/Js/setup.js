$(document).ready(function () {
    LoadSettings();

    $("#save").on('click', function () {
        var username = $("#http_username").val();
        var password = $("#http_password").val();

        $.ajax({
            url: '/api/config',
            type: 'POST',
            dataType: 'json',
            contentType: 'application/json',
            data: JSON.stringify({ "http.auth.username": username, "http.auth.password": password, "bt.savePath": $("#bt_savepath").val() }),
            success: function (result) {
                if (result) {
                    var url = window.location.protocol + "//" + username + ":" + password + "@" + window.location.host;
                    alert(url);
                    window.location.replace(url);
                }
            },
            error: function () {
                alert("Failed... :(");
            }
        });
    });
});

function LoadSettings() {
    $.getJSON('/api/config?key=http.auth.username&key=http.auth.password&key=bt.savePath', function (data) {
        $("#http_username").val(data["http.auth.username"]);
        $("#http_password").val(data["http.auth.password"]);
        $("#bt_savepath").val(data["bt.savePath"]);
    });
}