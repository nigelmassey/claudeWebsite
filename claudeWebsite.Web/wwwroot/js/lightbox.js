(function () {
    var lightbox = document.getElementById('lightbox');
    var lbImg = document.getElementById('lb-img');
    var lbClose = document.getElementById('lb-close');
    var grid = document.querySelector('.gallery-grid');
    if (!lightbox || !grid) return;

    grid.addEventListener('click', function (e) {
        var wrap = e.target.closest('.thumb-wrap');
        if (wrap) {
            lbImg.src = wrap.dataset.fullsrc;
            lightbox.style.display = 'flex';
            document.body.style.overflow = 'hidden';
        }
    });

    lbClose.addEventListener('click', closeLightbox);

    lightbox.addEventListener('click', function (e) {
        if (e.target === lightbox) closeLightbox();
    });

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') closeLightbox();
    });

    function closeLightbox() {
        lightbox.style.display = 'none';
        lbImg.src = '';
        document.body.style.overflow = '';
    }
})();
