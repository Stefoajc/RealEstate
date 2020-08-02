$(document).ready(function () {

    (function () {

        var getTopHiddenElement = function(element) {
            while (element.parentElement.offsetParent === null) {
                element = element.parentElement;
            }
            return element;
        };

        var clampLines = function (element, lines) {
            //used for parent element which is hidden
            var elementDefaultDisplay;
            var elementToMakeBlock;

            //used when the target element is not block
            var targetElementDefaultDisplay;
            if (element.style.display !== "block") {
                targetElementDefaultDisplay = element.style.display;
                element.style.display = "block";
            }

            if (element.offsetParent === null) {
                elementToMakeBlock = getTopHiddenElement(element);
            }

            if (typeof elementToMakeBlock !== "undefined" && elementToMakeBlock !== null) {
                elementDefaultDisplay = elementToMakeBlock.style.display;
                if (elementToMakeBlock.offsetParent === null) {
                    elementToMakeBlock.style.display = "block";
                }
            }
            window.$clamp(element, { clamp: lines });
            if (typeof elementToMakeBlock !== "undefined" && elementToMakeBlock !== null) {
                elementToMakeBlock.style.display = elementDefaultDisplay;
            }
            if (targetElementDefaultDisplay) {
                element.style.display = "block";
            }
         };

        let elements = document.getElementsByClassName('clamp-one');
        for (let i = 0; i < elements.length; i++) {
            clampLines(elements[i], 1);
        }

        elements = document.getElementsByClassName('clamp-two');
        for (let i = 0; i < elements.length; i++) {
            clampLines(elements[i], 2);
        }

        elements = document.getElementsByClassName('clamp-three');
        for (let i = 0; i < elements.length; i++) {
            clampLines(elements[i], 3);
        }

        elements = document.getElementsByClassName('clamp-four');
        for (let i = 0; i < elements.length; i++) {
            clampLines(elements[i], 4);
        }

        elements = document.getElementsByClassName('clamp-five');
        for (let i = 0; i < elements.length; i++) {
            clampLines(elements[i], 5);
        }

        elements = document.getElementsByClassName('clamp-six');
        for (let i = 0; i < elements.length; i++) {
            clampLines(elements[i], 6);
        }

        elements = document.getElementsByClassName('clamp-38');
        for (let i = 0; i < elements.length; i++) {
            window.$clamp(elements[i], { clamp: '38px' });
        }
        
    })();

});