$(document).ready(function () {

    if (document.getElementById("createAccountBtn") != null) {
        document.getElementById("createAccountBtn").addEventListener("click", submitForm, false);
        //$("#userProfileForm").on('submit', function (event) {
        //    event.preventDefault();
        //    $("#PwdModal").modal("show");
        //});

        $("#userProfileForm").on('submit', function (event) {
            event.preventDefault();
        });
    }

    $("#modalCloseBtn").click(function () {
        $("#pwd").prop("required", false);
        $("#cpwd").prop("required", false);
        $("#proceedBtn").prop("disabled", false);
        confirmModalOpen = false;
    });

    function submitForm() {
        $("#proceedBtn").click();
        let formPwdField = document.querySelector("#userProfileForm #password");
        let formCpwdField = document.querySelector("#userProfileForm #cpassword");
        let modalPwdField = document.querySelector("#PwdModal #pwd");
        let modalCpwdField = document.querySelector("#PwdModal #cpwd");
        formPwdField.value = modalPwdField.value;
        formCpwdField.value = modalCpwdField.value;
        if ($("#userProfileForm")[0].checkValidity()) {
            $("#userProfileForm").off('submit');
            $("#userProfileForm").submit();
        }
    }
});