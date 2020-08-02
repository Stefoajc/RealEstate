$(document).ready(function () {

    //Works
    $(document).on('click', '#create-review', function (event) {
        if (!window.g_isUserAuthenticated) {
            event.preventDefault ? event.preventDefault() : (event.returnValue = false);
            window.alertify.error("Нужно е да влезете в системата за да дадете ревю!");
        }
    });

    //Give review to the Property
    $('.rating > input').change(function (event) {
        let reviewForm = $(this).closest('form');

        $.ajax({
            url: reviewForm.attr('action'),
            type: reviewForm.attr('method'),
            data: reviewForm.serialize(),
            success: function (data, status) {
                if (data.success) {
                    showErrorResponse(data, reviewForm);
                    event.preventDefault ? event.preventDefault() : (event.returnValue = false);
                } else {
                    window.alertify.success("Успешно направено ревю. Благодарим ви!");
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                var titles = jqXHR.responseText.match("<title>(.*?)</title>");
                let shownError = titles ? titles[1] : errorThrown;
                window.alertify.error(shownError);
                event.preventDefault ? event.preventDefault() : (event.returnValue = false);
            }
        });
    });    
});