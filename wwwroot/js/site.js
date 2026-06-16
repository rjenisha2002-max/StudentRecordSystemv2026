// Auto-dismiss alerts after 5 seconds
document.addEventListener('DOMContentLoaded', function () {
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(a => {
        setTimeout(() => {
            a.style.transition = 'opacity .5s';
            a.style.opacity = '0';
            setTimeout(() => a.remove(), 500);
        }, 5000);
    });

    // Input: clamp number marks between 1-100 on blur
    document.querySelectorAll('input[type="number"].mark-input').forEach(inp => {
        inp.addEventListener('blur', function () {
            let v = parseInt(this.value);
            if (!isNaN(v)) {
                if (v < 1)   this.value = 1;
                if (v > 100) this.value = 100;
            }
        });
    });
});
