$(document).ready(function () {
    if (showIncidentReportForm) {
        doIncidentReportForm(true);
    }else {
        doIncidentReportForm(false);
    }

    $('#isIncidentReportedTrue').change(function () {
        doIncidentReportForm(true);
    });

    $('#isIncidentReportedFalse').change(function () {
        doIncidentReportForm(false);
    });

    function doFreeForm() {
        $("#reasonDiv").show();
        toggleRequiredPropertyForCategory(false);
    }

    function doIncidentReportForm(boolVal) {
        if(boolVal){
            $("#incidentReportDiv").show();
            toggleRequiredPropertyForIncidentReportStatus(boolVal);
        }else{
            $("#incidentReportDiv").hide();
            toggleRequiredPropertyForIncidentReportStatus(boolVal);
        }
    }

    function toggleRequiredPropertyForCategory(boolVal) {
        $("#subcategory").attr("required", boolVal);
        $("#subcategory").attr("disabled", !boolVal);
        $("#reason").attr("required", !boolVal);
        $("#reason").attr("disabled", boolVal);
        $("#showFreeForm").attr("checked", !boolVal);
    }

    function toggleRequiredPropertyForIncidentReportStatus(boolVal) {
        //$("#incidentReportedDate").attr("required", boolVal);
        $("#courtaffidavitfile").attr("required", !boolVal);
        $("#affidavitDateOfIssuance").attr("required", !boolVal);
        displayRequiredAsteriskForIncidentReport(boolVal);
    }

    function displayRequiredAsteriskForIncidentReport(boolVal) {
        if (boolVal) {
            $('label[for="uploadCourtAffidavit"] .required-sym').hide();
            $('label[for="AffidavitDateOfIssuance"] .required-sym').hide();
        } else {
            $('label[for="uploadCourtAffidavit"] .required-sym').show();
            $('label[for="AffidavitDateOfIssuance"] .required-sym').show();
        }
    }
});