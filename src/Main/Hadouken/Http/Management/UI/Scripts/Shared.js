$(document).ready(function () {
    rpc('core', 'core.plugins.list', null, function (result) {
        var container = $('#settings-items-list');
        var plugins = result.result;

        for (var i = 0; i < plugins.length; i++) {

            (function(plugin) {
                $.getJSON('/manage/plugins/settings/' + plugin + '/get', function () {
                    var item = $('<li><a href="#"></a></li>');
                    item.find('a').attr('href', '/manage/plugins/settings/' + plugin);
                    item.find('a').text(plugin);

                    container.append(item);
                });
            })(plugins[i]);
        }
    });
});