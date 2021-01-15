import Swiper from 'swiper/bundle';

if (document.getElementById("image-slider") !== null) {
    const imageSlider = new Swiper(".swiper-container",
        {
            pagination: {
                el: '.swiper-pagination',
                type: 'fraction',
            },
            navigation: {
                nextEl: '.swiper-button-next',
                prevEl: '.swiper-button-prev',
            },
        });
}