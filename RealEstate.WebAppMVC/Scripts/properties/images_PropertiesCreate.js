//-----IMAGES FUNCTIONS
$(document).ready(function () {
    var imagesRegularCount = 0;
    var imagesSliderCount = 0;

    // Initialize Slider
    let slider = $("#owl-main-slider");
    slider.owlCarousel(owlDefaultOptions);
    slider.addClass("owl-carousel-init");
    var carouselSlider = slider.data('owlCarousel');

    let sliderRegular = $("#owl-regular-slider");
    sliderRegular.owlCarousel(owlDefaultOptions);
    sliderRegular.addClass("owl-carousel-init");
    var carouselRegularSlider = sliderRegular.data('owlCarousel');


    // owlItemTemplate
    let owlItemTemplate = function (imgTag, styles) {
        $(imgTag[0]).attr('style', styles);
        return '<div class="col-md-12 animation">' +
            '<div class="pgl-property featured-item">' +
            '<div class="property-thumb-info">' +
            '<div class="property-thumb-info-image">' +
            imgTag[0].outerHTML +
            '</div>' +
            '</div>' +
            '</div>' +
            '</div>';
    }

    let appendImageTo = function (imgTag, isForSlider) {
        if (isForSlider) {
            let imageForOwl = imgTag.clone();
            $(imageForOwl).removeClass("imagePlaceholderSlider");
            let owlItemToAdd = owlItemTemplate(imageForOwl, 'width:1000px;height:400px;');
            carouselSlider.addItem(owlItemToAdd);
            let imgTagForGallerySlider = imgTag;
            $(imgTagForGallerySlider).appendTo('#gallerySlider');
        } else {
            let imageForOwl = imgTag.clone();
            $(imageForOwl).removeClass("imagePlaceholder");
            let owlItemToAdd = owlItemTemplate(imageForOwl, 'width:522px;height:348px;');
            carouselRegularSlider.addItem(owlItemToAdd);
            let imgTagForGallery = imgTag;
            $(imgTagForGallery).appendTo('#gallery');
        }

    }
    // Multiple images preview in browser
    // Array of strings which are img tags with pictures embeded
    let createImageTagsList = function (input, isForSlider) {
        if (input.files) {
            let filesAmount = input.files.length;

            for (let i = 0; i < filesAmount; i++) {
                let reader = new FileReader();

                //Create img tag with the image embeded in base64
                let imageClass = isForSlider ? 'imagePlaceholderSlider' : 'imagePlaceholder';
                let imageTag =
                    $($.parseHTML('<img class="' +
                        imageClass +
                        '"/><span class="topright"><b>x</b></span>')).data('file', input.files[i]);

                reader.onload = function (event) {
                    imageTag.attr('src', event.target.result);
                    //push the image in the result array
                    appendImageTo(imageTag, isForSlider);
                }
                reader.readAsDataURL(input.files[i]);
            }
        }
    };


    $('body').on('change',
        '#gallery-photo-add',
        function () {
            imagesRegularCount += this.files.length;
            $('#imageRegularCount').text(imagesRegularCount + ' Снимки');

            createImageTagsList(this, false);

            $(this).clone().appendTo($(this).parent()).hide();
            $(this).val('');
        });

    $('body').on('change',
        '#gallery-photo-slider-add',
        function () {
            imagesSliderCount += this.files.length;
            $('#imageSliderCount').text(imagesSliderCount + ' Снимки');

            createImageTagsList(this, true);

            $(this).clone().appendTo($(this).parent()).hide();
            $(this).val('');
        });


    $('body').on('click',
        '.topright',
        function removeImageFromGaleryAndSetImageCount() {
            //Get index of image in the gallery for carousel item deletion
            var index = Array.prototype.slice.call(this.parentElement.children).indexOf(this);
            if ($(this).prev().attr('class') === 'imagePlaceholder') {
                //Remove the item from the Carousel Preview
                carouselRegularSlider.removeItem(index - 1);
                //Remove the images count
                imagesRegularCount--;
                //Update the span with the images count
                $('#imageRegularCount').html(imagesRegularCount + ' Снимки');
            } else {
                //Remove the item from the Carousel Preview
                carouselSlider.removeItem(index - 1);
                //Remove the images count
                imagesSliderCount--;
                //Update the span with the images count
                $('#imageSliderCount').html(imagesSliderCount + ' Снимки');
            }

            //Remove the image from gallery
            $(this).prev().remove();
            //Remove the Span with the X from the gallery
            $(this).remove();
        });


    function addImageToGallery(imagePath, isForSlider)
    {
        let imageClass = isForSlider ? 'imagePlaceholderSlider' : 'imagePlaceholder';
        let imageTag =
            $($.parseHTML('<img class="' +
                imageClass +
                '"/><span class="topright"><b>x</b></span>'));

        imageTag.attr('src', imagePath);
        //push the image in the result array
        appendImageTo(imageTag, isForSlider);
    }
});
