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
        endPoint: '/manage/jsonrpc'
    });

    $.jsonRPC.request('core.plugins.list', {
        success: function(result) {
            var container = $('#settings-items-list');
            var plugins = result.result;

            for (var i = 0; i < plugins.length; i++) {

                (function (plugin) {
                    $.get('/manage/plugins/settings/' + plugin, function () {
                        var item = $('<li><a href="#"></a></li>');
                        item.find('a').attr('href', '/manage/plugins/settings/' + plugin);
                        item.find('a').text(plugin);

                        container.append(item);
                    });
                })(plugins[i]);
            }
        }
    });

    // Get our version, then compare to the latest release
    $.jsonRPC.request('core.getVersion', {
        success: function(response) {
            var localVersion = response.result;

            $.getJSON('/manage/api/releases', function(releases) {
                var latestRelease = releases[0];

                if (semver.gt(latestRelease.version, localVersion)) {
                    var text = '<strong>Update available!</strong><br/><a href="' + latestRelease.downloadUri + '">Download Hadouken v' + latestRelease.version + '</a>.';
                    $.bootstrapGrowl(text, { delay: 0 });
                }
            });
        }
    });
});