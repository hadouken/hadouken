$(document).ready(function () {
    rpc('core', 'core.plugins.list', null, function (result) {
        var container = $('#settings-items-list');

        var plugins = result.result;

        for (var i = 0; i < plugins.length; i++) {
            var item = $('<li><a href="#"></a></li>');
            item.find('a').attr('href', '/manage/plugins/settings/' + plugins[i]);
            item.find('a').text(plugins[i]);

            container.append(item);
        }
    });
});