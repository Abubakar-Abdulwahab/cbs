$(document).ready(function () {
    $("#allowAdditionalSplitsToggle").click(function (e) {
        if (this.checked) {
            $("#accountNumber").attr("required", false);
            $("#selectedBank").attr("required", false);
            $("#accountNumberFieldContainer").slideUp("fast", "linear", function () { });
            $("#bankFieldContainer").slideUp("fast", "linear", function () { });
            
        } else {
            $("#accountNumberFieldContainer").slideDown("fast", "linear", function () { });
            $("#bankFieldContainer").slideDown("fast", "linear", function () { });
            $("#accountNumber").attr("required", true);
            $("#selectedBank").attr("required", true);
        }
    });

});