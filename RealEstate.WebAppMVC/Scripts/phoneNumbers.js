var phoneNumberHelpers = (function () {

    var onPhoneChangeSuccess;
    var onPhoneChangeFailure;
    var onPhoneValidationSuccess;
    var onPhoneValidationFailure;

    var changePhonenumberForm =
        '<form action="/Manage/AddPhoneNumber" method="post" class="form-horizontal" role="form" id="change-phonenumber-form" style="padding: 0 40px;">'
        + '<input name="__RequestVerificationToken" type="hidden" value="">'
        + '<div class="validation-summary-valid text-danger" data-valmsg-summary="true"><ul><li style="display:none"></li></ul></div>' +
        '<div class="form-group"> \
                            <label class="control-label">Телефонен номер</label> \
                            <input name="Number" id="Number" class="form-control" /> \
                            <span class="field-validation-valid text-danger" data-valmsg-for="Number" data-valmsg-replace="true"></span> \
                        </div> \
                        <div class="form-group"> \
                            <input type="submit" class="btn btn-primary btn-block" value="Потвърди" /> \
                        </div> \
                    </form>';

    var verifyPhoneNumberCode = function (phoneNumber) {
        return '<form action="/Manage/VerifyPhoneNumber" method="post" class="form-horizontal" role="form" id="validate-phonenumber-form" style="padding: 0 40px;"> \
            <div class="validation-summary-valid text-danger" data-valmsg-summary="true"><ul><li style="display:none"></li></ul></div> \
                        <input type="hidden" name="phoneNumber" value="' +
            phoneNumber +
            '" /> \
                        <div class="form-group"> \
                                <label class="control-label">Въведете кодът изпратен на въведеният телефонен номер:</label> \
                                <input name="Code" id="Code" class="form-control" /> \
                                <span class="field-validation-valid text-danger" data-valmsg-for="Code" data-valmsg-replace="true"></span> \
                            </div> \
                            <div class="form-group"> \
                                <input type="submit" class="btn btn-primary btn-block" value="Потвърди" /> \
                            </div> \
                        </form>';
    };

    $('#change-phonenumber').click(function (event) {
        event.preventDefault ? event.preventDefault() : (event.returnValue = false);
        window.alertify.genericDialog(changePhonenumberForm)
            .set('title', 'Смяна на телефонния номер');
    });

    //Send change/add phonenumber request
    //If Agent/Administrator it will be changed right away, 
    //if client verification will be needed
    $(document).on('submit',
        '#change-phonenumber-form',
        function (event) {
            event.preventDefault ? event.preventDefault() : (event.returnValue = false);
            //Get phoneNumber from form
            var phoneNumber = this.Number.value;

            var globalAntiForgery = $('#__AjaxAntiForgeryForm>input').val();
            $(this).find('input[name="__RequestVerificationToken"]').val(globalAntiForgery);

            var thisForm = $(this);
            $.ajax({
                url: this.action,
                method: this.method,
                data: thisForm.serialize(),
                success: function (data) {
                    if (data.success) {
                        //show form validation errors
                        showErrorResponse(data, thisForm);
                    } else if (data === "STATUS_OK") {
                        //remove the modal window
                        window.alertify.genericDialog().close();
                        window.alertify.success("Успешно сменен телефонен номер!");
                        onPhoneChangeSuccess(phoneNumber);
                    } else {
                        //validate phone number if needed
                        window.alertify.genericDialog().close();
                        window.alertify.genericDialog(verifyPhoneNumberCode(phoneNumber))
                            .set('title', "Потвърждаване на телефонният номер");
                        onPhoneChangeSuccess(phoneNumber);
                    }

                },
                error: function (jqXHR, textStatus, errorThrown) {
                    //remove the modal window
                    window.alertify.genericDialog().close();
                    var titles = jqXHR.responseText.match("<title>(.*?)</title>");
                    let shownError = titles ? titles[1] : errorThrown;
                    window.alertify.error(shownError);
                    onPhoneChangeFailure();
                }

            });
        });

    $(document).on('submit',
        '#validate-phonenumber-form',
        function (event) {
            event.preventDefault ? event.preventDefault() : (event.returnValue = false);

            var globalAntiForgery = $('#__AjaxAntiForgeryForm>input').val();
            $(this).find('input[name="__RequestVerificationToken"]').val(globalAntiForgery);

            var thisForm = $(this);
            $.ajax({
                url: this.action,
                method: this.method,
                data: thisForm.serialize(),
                success: function (data) {
                    if (data === "STATUS_OK") {
                        //remove the modal window
                        window.alertify.genericDialog().close();
                        window.alertify.success("Успешно потвърден телефонен номер!");
                        onPhoneValidationSuccess();
                    } else {
                        //show form validation errors
                        showErrorResponse(data, thisForm);
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    //remove the modal window
                    window.alertify.genericDialog().close();
                    var titles = jqXHR.responseText.match("<title>(.*?)</title>");
                    let shownError = titles ? titles[1] : errorThrown;
                    window.alertify.error(shownError);
                    onPhoneValidationFailure();
                }

            });
        });

    var start = function (event,
        onChangeSuccess = (newPhoneNumber = "") => { }, onChangeFailure = () => { },
        onValidateSuccess = () => { }, onValidateFailure = () => { }) {

        if (event) {
            event.preventDefault ? event.preventDefault() : (event.returnValue = false);
        }
        
        onPhoneChangeSuccess = onChangeSuccess;
        onPhoneChangeFailure = onChangeFailure;
        onPhoneValidationSuccess = onValidateSuccess;
        onPhoneValidationFailure = onValidateFailure;

        window.alertify.genericDialog(changePhonenumberForm)
            .set('title', 'Смяна на телефонния номер');
    };

    return { trigger: start };
})();