$(document).ready(function () {
    $('#ajaxloader').hide();

    var timeout;
    $.ajaxSetup({
        beforeSend: function () {
            $('#ajaxloader').show();
        },
        complete: function() {
            clearTimeout(timeout);
            timeout = setTimeout(function () { $('#ajaxloader').hide(); }, 500);
        }
    });

    $.jsonRPC.setup({
        endPoint: '/jsonrpc'
    });

    // Get our version, then compare to the latest release
    $.jsonRPC.request('core.getVersion', {
        success: function(response) {
            var localVersion = response.result;

            $.jsonRPC.request('core.releases.list', {
                success: function(data) {
                    var latestRelease = data.result[0];

                    if (semver.gt(latestRelease.version, localVersion)) {
                        var text = '<strong>Update available!</strong><br/><a href="' + latestRelease.downloadUri + '">Download Hadouken v' + latestRelease.version + '</a>.';
                        $.bootstrapGrowl(text, { delay: 0 });
                    }
                }
            });
        }
    });
});