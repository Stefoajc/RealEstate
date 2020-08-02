alertify.paymentMethodDialog || alertify.dialog('paymentMethodDialog',
    function () {

        var $formToPay;

        $(document).on('change',
            ".payment-method",
            function () {
                if (this.id === 'pay-easy') {
                    $('.pay-easy').removeClass('hidden');
                    $('.pay-card.pay-epay').addClass('hidden');
                } else {
                    $('.pay-easy').addClass('hidden');
                    $('.pay-card.pay-epay').removeClass('hidden');
                }
            });

        $(document).on('click',
            '#reject-payment',
            function (event) {
                event.preventDefault ? event.preventDefault() : (event.returnValue = false);
                window.alertify.closeAll();
            });

        $(document).on('submit',
            '#payment-form',
            function (event) {
                event.preventDefault ? event.preventDefault() : (event.returnValue = false);

                var finalForm = $formToPay.serialize() + '&' + $(this).serialize();

                console.log(finalForm);

                $.ajax({
                    url: this.action,
                    method: this.method,
                    data: finalForm,
                    success: function(data) {
                        if (data['success'] && data['success'][0] === 'false') {
                            showErrorResponse(data, $('#payment-form'));
                        } else if (/^\d+$/.test(data)) {
                            window.alertify.closeAll();
                            window.alertify.alert('EasyPay код', 'Вашият EasyPay код е: ' + data);
                        } else {
                            var form = ($.parseHTML(data))[0];
                            document.body.append(form);
                            form.submit();
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        var titles = jqXHR.responseText.match("<title>(.*?)</title>");
                        let shownError = titles ? titles[1] : errorThrown;
                        window.alertify.error(shownError);
                    }
                });
            });

        return {
            main: function (title, url, $formData) {
                $formToPay = $formData;
                var paymentForm = '<form id="payment-form" method="post" action="' + url + '" class="payment-field" style="margin:20px 0 10px 0;"> \
                                        <div> \
                                            <div class="col-md-5 col-xs-12"> \
                                                <input id="pay-card" class="payment-method" type="radio" name="PaymentInfo.PaymentMethod" data-target=".pay-card" value="DIRECTCREDITCARD" checked=""> \
                                                <label for="pay-card"> \
                                                    <span class="pay-icon card"></span> \
                                                    Кредитна/Дебитна карта \
                                                </label> \
                                            </div> \
                                            <div class="col-md-3-5 col-xs-12"> \
                                                <input id="pay-easy" class="payment-method" type="radio" name="PaymentInfo.PaymentMethod" data-target=".pay-easy" value="EASYPAY"> \
                                                <label for="pay-easy"> \
                                                    <span class="pay-icon easy"></span> \
                                                    EasyPay \
                                                </label> \
                                            </div> \
                                            <div class="col-md-3-5 col-xs-12"> \
                                                <input id="pay-epay" class="payment-method" type="radio" name="PaymentInfo.PaymentMethod" data-target=".pay-epay" value="EPAYPAYMENT"> \
                                                <label for="pay-epay"> \
                                                    <span class="pay-icon epay"></span> \
                                                    ePay.bg \
                                                </label> \
                                            </div> \
                                        </div> \
                                        <div class="field-validation-valid text-danger" data-valmsg-for="PaymentInfo.PaymentMethod" data-valmsg-replace="true"></div> \
                                        <div class="single-payment row pay-easy hidden" style="margin-left:5px;"> \
                                            <div class="col-xs-12"> \
                                                <div class="row"> \
                                                    <div class="col-xs-12">Име:</div> \
                                                    <div class="col-xs-12"> \
                                                        <div class="col-xs-12  col-md-6" style="padding:0; margin:0;"> \
                                                            <input type="text" name="PaymentInfo.PayerName" placeholder="до 26 символа" class="form-control"> \
                                                        </div> \
                                                        <div class="field-validation-valid text-danger" data-valmsg-for="PayerName" data-valmsg-replace="true"></div> \
                                                    </div> \
                                                </div> \
                                            </div> \
                                            <div class="col col-xs-12">Тази информация няма да бъде съхранена в системата на sProperties.</div> \
                                        </div> \
                                        <div class="single-payment pay-card pay-epay row" style="margin-left:15px;"> \
                                            За да извършите плащане ще бъдете пратени към системата на ePay. \
                                        </div> \
                                        <hr style="margin:10px 0;" /> \
                                        <div class="buttons pull-right" style="margin-right:10px;">    \
                                            <div style="display:inline-block;"> \
                                                <input type="checkbox" id="AreTermsAgreed" name="AreTermsAgreed" value="true"> <label for="AreTermsAgreed"> Съгласявам се с </label> <a target="_blank" href="/Home/PaymentTerms">Общите условия</a>. \
                                                <div class="field-validation-valid text-danger" data-valmsg-for="AreTermsAgreed" data-valmsg-replace="true"></div> \
                                            </div> \
                                            <button id="accept-payment" class="btn btn-success">Потвърди</button> \
                                            <button id="reject-payment" class="btn btn-primary">Откажи</button> \
                                        </div> \
                                        <div class="clearfix"></div> \
                                    </form>';

                this.setContent(paymentForm);
                this.setting('title', title);
                this.elements.footer.style.display = "none";
            },
            setup: function () {
                return {
                    options: {
                        basic: false,
                        maximizable: false,
                        resizable: false,
                        padding: false
                    }
                };
            },
            settings: {
                selector: undefined
            }
        };
    });