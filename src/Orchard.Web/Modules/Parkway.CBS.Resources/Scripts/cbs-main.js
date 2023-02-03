(function () {
    'use strict';
    /*jslint browser: true*/
    /*eslint-env browser*/
    /*global $*/

    /* code for menu color change on scroll */
    $(document).scroll(function () {
        if ($(this).scrollTop() < $('#main-nav').height()) {
            $('#main-nav').removeClass('scrolled');
            //            $('#main-nav').css('background-color', 'transparent');
            //            $('#main-nav').css('box-shadow', 'none');
        } else {
            $('#main-nav').addClass('scrolled');
            //            $('#main-nav').css('background-color', '#00B05F');
            //            $('#main-nav').css('box-shadow', '2px 0 5px rgba(0, 0, 0, 0.5)');

        }
    });
    /* code for menu color change on scroll */

    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    })

}());
