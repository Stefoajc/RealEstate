//responseText : Json object with ModelState Data { {key:<string> , value: <string[]>}, .. }
//element : (HtmlNode) Place where the actions will be made (ex. The form) Where to set the the validation messages
function showErrorResponse(errorResponse, element) {
    if (!element) return;
    if (typeof errorResponse === 'string') {
        let tagContent = errorResponse.match("<title>(.*?)</title>");
        let shownError = tagContent ? tagContent[1] : errorResponse;
        element.find(".text-danger").last().html(shownError);
        return;
    }
    element.find('.text-danger').html('');
    element.find('.text-danger').each(function () {
        let thisElem = $(this);
        if (errorResponse[thisElem.attr('data-valmsg-for')]) {
            for (var i = 0; i < errorResponse[thisElem.attr('data-valmsg-for')].length; i++) {
                thisElem.append(
                    '<div>' +
                    errorResponse[thisElem.attr('data-valmsg-for')][i] +
                    '</div>'
                );
            }
        }
    });
}