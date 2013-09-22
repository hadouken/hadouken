(function ($, document) {
    $(document).ready(function () {
    });

    // Set up Pjax
    $(document).pjax('a', '#page-container');

    // Set up NProgress with Pjax
    $(document).on('pjax:send', function () {
        NProgress.start();
    });

    $(document).on('pjax:complete', function () {
        NProgress.done();
    });
})(jQuery, document);

// Real setup below
window.addEvent('domready', function () {
    $("overlay").show();
    
    Hadouken.init();

    $("overlay").hide();
});