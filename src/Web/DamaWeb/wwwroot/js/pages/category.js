const categorySlides = document.getElementsByClassName("catalog-types");

function InitializeGlideSlides() {
    new Glide('#new-items', GetSliderBasicOptions('slider')).mount();

    for (var i = 0; i < categorySlides.length; i++) {
        new Glide(categorySlides[i], GetSliderBasicOptions('slider')).mount();
    }
}

InitializeGlideSlides();