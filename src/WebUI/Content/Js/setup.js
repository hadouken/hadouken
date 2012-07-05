$(document).ready(function () {
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