document.addEventListener('DOMContentLoaded', () => {
    // 1. Carousel Sản phẩm nổi bật
    const track = document.getElementById('featuredTrack');
    const prevBtn = document.getElementById('featuredPrev');
    const nextBtn = document.getElementById('featuredNext');

    if (track && prevBtn && nextBtn) {
        const scrollStep = () => {
            const card = track.querySelector('.product-card');
            return card ? card.offsetWidth + 24 : 300;
        };

        prevBtn.addEventListener('click', () => {
            track.scrollBy({ left: -scrollStep(), behavior: 'smooth' });
        });

        nextBtn.addEventListener('click', () => {
            track.scrollBy({ left: scrollStep(), behavior: 'smooth' });
        });
    }

    // 2. Form tư vấn nhanh (FR-11)
    const leadForm = document.getElementById('quickLeadForm');
    const leadMsg = document.getElementById('quickLeadMsg');

    if (leadForm && leadMsg) {
        leadForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            const formData = new FormData(leadForm);
            const btn = leadForm.querySelector('button[type="submit"]');
            btn.disabled = true;

            try {
                const res = await fetch(leadForm.action, {
                    method: 'POST',
                    body: formData
                });
                const data = await res.json();
                leadMsg.textContent = data.message || (res.ok
                    ? 'Đã nhận thông tin! Chúng tôi sẽ liên hệ sớm.'
                    : 'Vui lòng kiểm tra lại Email hoặc Số điện thoại.');
                if (res.ok) leadForm.reset();
            } catch {
                leadMsg.textContent = 'Có lỗi kết nối, vui lòng thử lại sau.';
            } finally {
                btn.disabled = false;
            }
        });
    }
});