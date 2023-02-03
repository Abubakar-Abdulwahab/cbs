$(document).ready(function () {
    var token = $("input[name=__RequestVerificationToken]").val();
    var profilesObj = new Object();
    var stillChecking = false;
    var stillCheckingLogin = false;
    var fromProceed = false;
    disableCategories();


    function toggleProfileDiv(visible) {
        $("#profiles").empty();
        $("#profileData").val("");
        if (visible) {
            $("#parentProfileDiv").show();
            $("#profileData").prop("required", true);
        }
        else {
            $("#parentProfileDiv").hide();
            $("#profileData").prop("required", false);
        }
    }


    function buildDataList(profiles) {
        for (var val in profiles) {
            $('<option data-value="' + profiles[val].Id + '" value="' + profiles[val].Name + ' (' + profiles[val].Address + ')"></option>').appendTo($("#profiles"));
        }
    }

    function settingsForProfileLoading(profileLoaded) {
        if (profileLoaded) {
            //show profile loader
            $("#profileloader").hide();
        } else {
            //show profile loader
            $("#profileloader").show();
        }
    }

    $('input[type=radio][name=TaxPayerType]').change(function () {
        if (!loggedIn) {
            //check if a profile search if needed
            if (this.dataset.value == "False") { toggleProfileDiv(false); return; }
            var taxPayerType = $('input[name=TaxPayerType]:checked').val();
            toggleProfileDiv(true);
            if(taxPayerType === "2"){ $("#bzRegLink").css("display","flex"); } else { $("#bzRegLink").hide(); }
            //check if the profiles have been gotten before
            if (profilesObj[taxPayerType] != undefined) {
                buildDataList(profilesObj[taxPayerType].Profiles);
            } else {
                //get the list of entities registered under this category
                var url = "x/get-tax-entities-bycategory";
                var requestData = { "categoryId": taxPayerType, "__RequestVerificationToken": token };
                //set the form up when profiles are being fetched
                settingsForProfileLoading(false);

                $.post(url, requestData, function (data) {
                    if (!data.Error) {
                        //show the data list and bind the recods
                        profilesObj[taxPayerType] = {};
                        profilesObj[taxPayerType].Profiles = data.ResponseObject;
                        buildDataList(profilesObj[taxPayerType].Profiles);
                    } else {
                        var errmsg = data.ResponseObject;
                        $("#errorFlash").show();
                        $("#errorMsg").html(errmsg);
                        $("#loader").hide();
                        toggleInputs(true);
                        stillChecking = false;
                    }
                }).always(function () {
                    settingsForProfileLoading(true);
                });
            }
        }
    });


    function disableCategories() {
        if (!allowCategorySelect) {
            $(':radio:not(:checked)').attr('disabled', true);
        }
    }


    $("#indexForm").on('submit', function (e) {
        //data list value
        $("#loader").show();
        if (loggedIn) {
            var inputValue = $('#revenueHeadData').val();
            var profileDataValue = $('#profileData').val();
            var revenueHeadValueIdentifier = $('#revenue-heads option[value="' + inputValue + '"]').attr('data-value');
            //var revenueHeadValueIdentifier = $("#revenue-heads option[value='" + inputValue + "']").attr('data-value');
            var profileIdentifierValue = $("#profiles option[value='" + profileDataValue + "']").attr('data-value');
            $('#revenueHeadIdentifier').val(revenueHeadValueIdentifier);
            $('#profileIdentifier').val(profileIdentifierValue);
            $('#taxCategory').val($('input[name=TaxPayerType]:checked').val());
            toggleInputs(false);
            return true;
        }

        e.preventDefault();
        if (!stillChecking) {
            var inputValue = $('#revenueHeadData').val();
            var profileDataValue = $('#profileData').val();
            var revenueHeadValueIdentifier = $('#revenue-heads option[value="' + inputValue + '"]').attr('data-value');
            var profileIdentifierValue = $("#profiles option[value='" + profileDataValue + "']").attr('data-value');
            $('#revenueHeadIdentifier').val(revenueHeadValueIdentifier);
            $('#profileIdentifier').val(profileIdentifierValue);

            $('#taxCategory').val($('input[name=TaxPayerType]:checked').val());
            stillChecking = true;
            checkIfLoginIsRequired(inputValue, revenueHeadValueIdentifier);
        }
    });


    function checkIfLoginIsRequired(revenueHeadSelected, revenueHeadValue) {

        toggleInputs(false);
        var url = "check-category";
        var taxPayerType = $('input[name=TaxPayerType]:checked').val();
        var requestData = { "staxPayerTypeId": taxPayerType, "sRevenueHeadId": revenueHeadValue };
        $.post(url, requestData, function (data) {
            if (!data.Error) {
                if (data.ResponseObject == "RequiresLogin") {
                    //show login modal
                    fromProceed = true;
                    toggleInputs(true);
                    $('#signin').modal('show');
                    stillChecking = false;
                    $("#loader").hide();
                } else {
                    $("#loader").hide();
                    $('#indexForm').get(0).submit();
                }
            } else {
                var errmsg = data.ResponseObject;
                $("#errorFlash").show();
                $("#errorMsg").html(errmsg);
                $("#loader").hide();
                toggleInputs(true);
                stillChecking = false;
            }
        });
    }

    $("#closeFlash").click(function (e) {
        $("#errorFlash").fadeOut("fast");
    });

    $("#closeModalFlash").click(function (e) {
        $("#loginErrorFlash").fadeOut("fast");
    });

    $("#signoutlnk").click(function (e) {
        $('#signoutForm').get(0).submit();
    });

    function toggleInputs(enable) {
        if (enable) {
            $("#proceedbtn").prop("disabled", false);
            $("#revenueHeadData").prop("disabled", false);
            $("input[type=radio]").attr('disabled', false);
            $("#proceedbtn").html("Proceed");
        } else {
            $("#proceedbtn").prop("disabled", true);
            $("#revenueHeadData").prop("disabled", true);
            $("input[type=radio]").attr('disabled', true);
            //   $("#proceedbtn").html("");
        }
    }

    $("#indexUserLoginForm").on('submit', function (e) {
        //data list value
        e.preventDefault();
        if (stillCheckingLogin) {
            return;
        }
        stillCheckingLogin = true;
        $("#babyloader").show();
        $("#userLoginSubmitBtn").prop("disabled", true);
        var url = "user-login";
        var requestData = { "username": $("#username").val(), "password": $("#password").val(), "__RequestVerificationToken": token };
        $.post(url, requestData, function (data) {
            if (!data.Error) {
                if (fromProceed) {
                    //close modal first
                    stillCheckingLogin = false;
                    loggedIn = true;
                    $("#babyloader").hide();
                    $("#loginErrorFlash").hide();
                    toggleInputs(false);
                    $("#signin").modal("hide");
                    $("#userLoginSubmitBtn").prop("disabled", false);
                    $("#claimToken").val(data.ResponseObject);
                    //form submit the triggering form
                    $('#indexForm').get(0).submit();
                    loggedIn = true;
                } else {
                    stillCheckingLogin = false;
                    loggedIn = true;
                    $("#claimToken").val(data.ResponseObject);
                    $("#babyloader").hide();
                    $("#loginErrorFlash").hide();
                    $("#userLoginSubmitBtn").prop("disabled", false);
                }
            } else {
                var errmsg = data.ResponseObject;
                $("#loginErrorFlash").fadeIn("fast");
                $("#loginErrorMsg").html(errmsg);
                $("#babyloader").hide();
                $("#userLoginSubmitBtn").prop("disabled", false);
                stillCheckingLogin = false;
            }
        }).fail(function () {
            $("#loginErrorFlash").fadeIn("fast");
            $("#loginErrorMsg").html("An error occurred. Please try again later.");
            $("#babyloader").hide();
            $("#userLoginSubmitBtn").prop("disabled", false);
            stillCheckingLogin = false;
        });
    });
});