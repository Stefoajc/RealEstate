// When the user clicks the button, open the modal 
function showModal(elementId) {
    let modalWindow = document.getElementById(elementId);
    $(modalWindow).addClass('opened');
    modalWindow.style.display = "block";
}

//params: {title:string,body:string,footer:string,messageType:string}
function createAndShowModal(modalInfo) {

    //IE compatibility
    modalInfo.title = modalInfo.title || "";
    modalInfo.footer = modalInfo.footer || "";
    modalInfo.messageType = modalInfo.messageType || "success";

    if ($('#modalRegular').length) {
        removeModal('modalRegular');
    }
    let alertType = 'alert-';

    switch (modalInfo.messageType) {
        case "success":
        case "warning":
        case "danger":
            alertType += modalInfo.messageType;
            break;
        default:
            alertType += "success";
    }

    if (modalInfo.title) {
        modalInfo.title =
            '<div class="modal-header">'
                    + modalInfo.title + 
                    '<span class="close" style="margin-top: 5px;" onclick="removeClosestModal(this)">&times;</span> \
                </div>';
    }

    if (modalInfo.footer) {
        modalInfo.footer =
            '<div class="modal-footer">'
                + modalInfo.footer +
                '</div>';
    }

    var modalWindow =
        '<!-- The Modal Regular Images --> \
        <div id="modalRegular" class="modal"> \
        <!--Modal content --> \
            <div class="modal-content">'
                + modalInfo.title + 
                '<div class="modal-body"  style="margin: 0;">'
                + modalInfo.body + 
                '</div>'
                + modalInfo.footer + 
            '</div> \
        </div>';

    $("body").append(modalWindow);

    $('#modalRegular').css("display", "block");
}

// When the user clicks on <span> (x), close the modal
function closeModal() {
    let modalWindow = $('.opened');
    modalWindow.removeClass('opened');
    modalWindow.hide();
}

// hide the closest modal
function closeClosestModal(clickedItem) {
    let modalWindow = $(clickedItem).closest('.modal.opened');
    $(modalWindow).hide();
}

function removeModal(selector) {
    let modalWindow = document.getElementById(selector);
    modalWindow.remove();
}

function removeClosestModal(clickedItem) {
    let modalWindow = $(clickedItem).closest('.modal');
    modalWindow.remove();
}

// When the user clicks anywhere outside of the modal, close it
window.onclick = function (event) {
    let modal = $('.modal.opened');

    if (event.target === modal[0]) {
        modal.hide();
        modal.removeClass('opened');
    }
}