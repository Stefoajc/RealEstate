$(document).ready(function () {

    var policyCookieName = 'policyCookie';

    if (!isCookieExisting(policyCookieName)) {

        var policyCookieForm = 
                        '<div class="center-block col-md-12"> \
                            <img class="grayscale" width="32" height="32" src="/Resources/exclamation-mark-info-128.png" alt=""> <b>sproperties.net</b> използва бисквитки. Разглеждайки този сайт, вие се съгласявате с използването на бисквитки. \
                            <a href="/Home/CookiesPolicy" target="_blank"><b>Научи повече.</b></a> \
                            <a href="#" class="cookie-policy-accept btn btn-primary">OK</a> \
                        </div>';

        $('.cookies-policy').append(policyCookieForm);

        $(document).on('click',
            '.cookie-policy-accept',
            function (event) {
                event.preventDefault ? event.preventDefault() : (event.returnValue = false);

                setCookie(policyCookieName, 1, 355);
                $(this).parent().hide();
            });
    }
});