$(document).ready(function () {
    var token = $("input[name=__RequestVerificationToken]").val();


    $("#contactUsForm").on('submit', function (event) {
        event.preventDefault();
        submitForm();
    });

    $("#closeFlash").click(function (e) {
        $("#errorFlash").fadeOut("fast");
    });

    $("#closeSuccessFlash").click(function (e) {
        $("#successFlash").fadeOut("fast");
    });

    function submitForm() {

        $("#submitBtn").prop("disabled", true);
        $("#contactUsLoader").show();

        let name = $("#name").val();
        let email = $("#email").val();
        let message = $("#message").val();
        let subject = $("#subject").val();

        let url = "/p/x/contact-us";
        let data = { "__RequestVerificationToken": token, "name": name, "email": email, "message": message, "subject": subject };

        $.post(url, data, function (response) {
            $("#contactUsLoader").hide();
            $("#submitBtn").prop("disabled", false);

            if (!response.Error) {
                $("#successFlash").show();
                $('#contactUsForm')[0].reset();

            }
            else {
                $("#errorFlash").show();
                $("#responseMessage").html(`${response.ResponseObject}`);

            }
        });
    }
});
