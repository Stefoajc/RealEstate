$(document).ready(function () {
    window.recaptchaHelpers = (function () {
        var recaptchaHelpers = {
            initRecaptcha: function (recaptchaContainer) {
                if (!(recaptchaContainer instanceof jQuery)) {
                    recaptchaContainer = $(recaptchaContainer);
                }

                if (recaptchaContainer.hasClass('not-ready')) {
                    recaptchaContainer.removeClass('not-ready');
                }

                function renderThis() {
                    for (let i = 0; i < recaptchaContainer.length; i++) {
                        recaptchaContainer.eq(i).html('').data('widgetId', grecaptcha.render(recaptchaContainer.eq(i)[0], { sitekey: '6Lers20UAAAAAPu8hl2paT_Hu6JLd3oSjnDVwL1D' }));
                        recaptchaContainer.eq(i).find('textarea').attr('name', 'reCaptcha');
                    }
                }

                if (grecaptcha.render) renderThis();
                else grecaptcha.ready(function () {
                    renderThis();
                });
            },

            resetRecaptcha: function (recaptchaContainer) {
                if (!jQuery.contains(document.documentElement, recaptchaContainer[0])) return;
                grecaptcha.reset(recaptchaContainer.data('widgetId'));
                recaptchaContainer.find('textarea').attr('name', 'reCaptcha');
            },

            createRecaptcha: function() {
                var recaptchaContainer = '<div class="g-recaptcha"></div>';
                this.initRecaptcha($(recaptchaContainer));
                return recaptchaContainer;
            }
        };

        //Init all recaptchas on the page
        recaptchaHelpers.initRecaptcha($('.g-recaptcha.not-ready').removeClass('not-ready'));

        return recaptchaHelpers;
    })();

});