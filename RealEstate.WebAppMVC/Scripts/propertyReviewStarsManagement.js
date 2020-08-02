$(document).ready(function () {

    //Works
    $(document).on('click', '#create-review', function (event) {
        if (!window.$isUserAuthenticated) {
            event.preventDefault ? event.preventDefault() : (event.returnValue = false);
            alertify.error("Нужно е да влезете в системата за да дадете ревю!");
        }
    });

    //Give review to the Property
    $('.rating > input').change(function () {
        var reviewTextTemplate = `<div class="form-group">
                                    <label class="control-label" for="review-text">Въведете текст към оценката си (полето може да остане празно)</label>
                                    <textarea class="form-control" id="review-text" name="review-text" rows="5"></textarea>
                                </div>
                                <div class="col-xs-12">
                                    <div class="form-group">
                                        <button id="create-review-btn" class="btn btn-primary" style="float: right; margin-bottom: 10px;">
                                            Потвърди оценката
                                        </button>
                                    </div>
                                </div>`;
        var title = `Въведете текст към оценката.`;

        createAndShowModal({ title: title, body: reviewTextTemplate });

    });

    $(document).on('click',
        '#create-review-btn',
        function () {
            var reviewText = $('#review-text').val();
            var createReviewForm = $('#create-review');
            var form = new FormData(createReviewForm[0]);
            form.append('ReviewText', reviewText);
            form.append('__RequestVerificationToken', $('#__AjaxAntiForgeryForm input[name="__RequestVerificationToken"]').val());

            //Close the modal
            removeClosestModal(this);

            //send review
            $.ajax({
                url: createReviewForm.attr('action'),
                type: createReviewForm.attr('method'),
                data: form,
                processData: false,
                contentType: false,
                success: function (data) {
                    if (data.success) {
                        alertify.error("Грешка при създаване на ревю.");
                    } else {
                        alertify.success("Успешно направено ревю. Благодарим ви!");
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    var titles = jqXHR.responseText.match("<title>(.*?)</title>");
                    let shownError = titles ? titles[1] : errorThrown;
                    alertify.error(shownError);
                }
            });
        });
});