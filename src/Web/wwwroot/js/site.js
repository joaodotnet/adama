// Write your Javascript code.
$(document).ready(function () {
    $('body').on('mouseenter mouseleave', '.dropdown', function (e) {
        var _d = $(e.target).closest('.dropdown');
        _d.addClass('show');
        setTimeout(function () {
            _d[_d.is(':hover') ? 'addClass' : 'removeClass']('show');
        }, 1000);
    });
});
function toNumber(value) {
    return parseFloat(value).toFixed(2).replace('.', ',');
}