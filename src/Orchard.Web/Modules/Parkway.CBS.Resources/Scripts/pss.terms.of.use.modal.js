$(document).ready(function () {
    if (document.cookie.search("FirstTime") < 0) {
        $("#termsModal").show();
        $("#confirmTerms").click(function () {
            document.cookie = "FirstTime=No";
            $("#termsModal").hide();
        });
    }
});