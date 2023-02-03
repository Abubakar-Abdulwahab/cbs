$(document).ready(function () {

    checkWidth();

    $(window).resize(function () {
        checkWidth();
    });

    function checkWidth() {
        if ($(window).width() < 600) {
            displayPopupToggle(true);
        } else { displayPopupToggle(false); }
    }

    function displayPopupToggle(display) {
        if (display) {
            $(".service-note-toggle-container").show();
            $("#sideNoteCollapsible").closest(".row").find(".col-md-6:nth-of-type(1)").css("order", "2");
            $("#sideNoteCollapsible").closest(".row").find(".col-md-6:nth-of-type(2)").css("order", "1");
            $("#sideNoteCollapsible").addClass("collapse");

        } else {
            $(".service-note-toggle-container").hide();
            $("#sideNoteCollapsible").closest(".row").find(".col-md-6:nth-of-type(1)").css("order", "1")
            $("#sideNoteCollapsible").closest(".row").find(".col-md-6:nth-of-type(2)").css("order", "2")
            $("#sideNoteCollapsible").removeClass("collapse");
        }
    }
});