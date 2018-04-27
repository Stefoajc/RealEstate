// When the user clicks the button, open the modal 
function showModal(selector) {
    let modalWindow = document.getElementById(selector);
    modalWindow.style.display = "block";
}

// When the user clicks on <span> (x), close the modal
function closeModal(selector) {
    let modalWindow = document.getElementById(selector);
    modalWindow.style.display = "none";
}


// When the user clicks anywhere outside of the modal, close it
window.onclick = function (event) {
    let modal = document.getElementById('modalRegular');
    let modalSlider = document.getElementById('modalSlider');

    if (event.target === modal) {
        modal.style.display = "none";
    }
    if (event.Target === modalSlider) {
        modalSlider.style.display = "none";
    }
}