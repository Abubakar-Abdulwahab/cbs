$(document).ready(function () {
    var toggled = true;
    rotateCaret(180);
    $(".request-details-content-2 .header").click(function () {
        if (toggled) {
            toggled = false;
            rotateCaret(0);
        } else { toggled = true; rotateCaret(180); }
    });

    $("#backCaret").click(function () { goBack(); });

    function rotateCaret(deg) {
        $("#customCaret").css({ 'transform': 'rotate('+deg+'deg)' });
    }

    function goBack() {
        window.history.back();
    }
});
