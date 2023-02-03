$(document).ready(function () {
    var scrolling = false;

    $(".notification-message-list-item").click(function () {
        $("#notificationMessageList").hide();
        $("#notificationMessage").show();
        toggleActiveItem();
    });

    $("#backCaret").click(function () {
        $("#notificationMessageList").show();
        $("#notificationMessage").hide();
        toggleActiveItem();
    });

    $(".notification-message-list-container").on("scroll", function () {
        scrolling = true;
    });

    setInterval(function () {
        if (scrolling) {
            scrolling = false;
            if (($(".notification-message-list-container").prop("scrollHeight") - $(".notification-message-list-container").outerHeight()) == Math.floor($(".notification-message-list-container").scrollTop())) {
                //make api call to fetch items
                //$("#profileloader").show();
                //$(".notification-message-list-container").css("overflow", "hidden");
            }
        }
    }, 300);


    function toggleActiveItem() {
        $(".breadcrumb-nav .nav-link").toggleClass("active-link");
        $($(".breadcrumb-nav .nav-link").siblings("span")).toggleClass("small-active-circle small-circle");
        $(".active-center-link").html($(".active-link").html());
    }
});