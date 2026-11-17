document.addEventListener('DOMContentLoaded', () => {
    const track = document.getElementById('productSliderTrack');
    const prevBtn = document.getElementById('sliderPrev');
    const nextBtn = document.getElementById('sliderNext');
    const dots = document.querySelectorAll('#sliderDots .slider-dot');

    if (!track || dots.length === 0) return;

    let currentIndex = 0;
    const totalSlides = dots.length;

    const goToSlide = (index) => {
        currentIndex = (index + totalSlides) % totalSlides;
        track.style.transform = `translateX(-${currentIndex * 100}%)`;
        dots.forEach((dot, i) => dot.classList.toggle('is-active', i === currentIndex));
    };

    prevBtn?.addEventListener('click', () => goToSlide(currentIndex - 1));
    nextBtn?.addEventListener('click', () => goToSlide(currentIndex + 1));
    dots.forEach((dot, idx) => dot.addEventListener('click', () => goToSlide(idx)));

    if (totalSlides > 1) {
        setInterval(() => goToSlide(currentIndex + 1), 5000);
    }
});