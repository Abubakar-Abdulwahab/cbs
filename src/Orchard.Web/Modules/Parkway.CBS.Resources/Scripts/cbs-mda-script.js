$(document).ready(function () {
    //None = 0, Fixed = 1, Variable = 2, OneOff = 3, DirectAssessment = 4, SelfAssessment = 5, FileUpload = 6, FixedAmountAssessment = 7

    $(".billing_type_control").prop("disabled", true);
    toggleDemandNoticeChildren();
    //all types of hackey
    //if billing type is direct assessment
    if (billingType === 4 || billingType === 5) {
        console.log("HERER");
        $("#billing_type_payee").show();
        toggleDirectAssessmentChildren();
    } else if (billingType === 6) {
        //for file upload
        $("#billing_type_file").show();
        $("#SAmount").prop("required", false);
        $("#TemplateValue").prop("required", true);
        if (templatesModel.SelectedImplementation !== null){$('<option value="" disabled selected>Select an adapter implementation</option>').appendTo('.file_upload_adapter');}        
        loadFileUploadImpls(templatesModel.SelectedTemplate, templatesModel.SelectedImplementation);
    }
   

    //if billing is recuring, set up recurring view
    if (hasFrequencyValue) {
        $("#billing_type_panel").show();
        $("#DueDateModel_DueOnNextBillingDate").prop("disabled", false);
        $(".frequency_control").show();
        $(".billing_type_control").prop("disabled", false);
        //fixed billing
        if (billingType === 1) {
            $(".variable_billing").hide();
            $(".fixed_billing").show();
            handleFixedPart(fixedType);
            //if fixed type is monthly
            if (fixedType === 3) {
                switch (Number(frequencyModel.FixedBill.MonthlyBill.WeekDay)) {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        $('#f_day_dropdown').hide();
                        break;
                }
            }
        } else if (billingType === 2) {
            $(".fixed_billing").hide();
            $(".variable_billing").show();
            handleVariablePart(fixedType);
        } else {
            $(".fixed_billing").hide();
            $(".variable_billing").hide();
            $("#DueDateModel_DueOnNextBillingDate").prop('checked', false);
        }
    } else {
        $("#billing_type_panel").hide();
        $(".billing_type_control").prop("disabled", true);
        $(".frequency_control").hide();
        $(".variable_billing").hide();
        $(".fixed_billing").hide();
        $("#DueDateModel_DueOnNextBillingDate").prop('checked', false);
    }


    $('.billing_type_selector').click(function () {

        //uncheck the other radio boxes and set the required attr to false
        $("#TemplateValue").prop("required", false);
        $("#FileUploadAdapterValue").prop("required", false);

        $(".billing_type_selector_nosch").prop('checked', false);
        $("#billing_type_payee").hide();
        $("#SAmount").prop("required", true);

        if ($('.billing_type_selector').prop('checked')) {
            $("#billing_type_panel").show();
            $(".billing_type_control").prop("disabled", false);
            $(".frequency_control").prop("disabled", false);
        } else {
            $(".billing_type_control").prop("disabled", true);
            $('.billing_type_control').attr('checked', false);
            $(".frequency_control").hide();
            checkDueDate(false);
        }
    });

    $('#BillingDemandNotice_IsChecked').change(function () {
        toggleDemandNoticeChildren();
    });

    $('#DirectAssessment_AllowFileUpload').change(function () {
        toggleDirectAssessmentChildren();
    });

    function toggleDemandNoticeChildren() {
        if ($("#BillingDemandNotice_IsChecked").prop('checked')) {
            $("#BillingDemandNotice_EffectiveFrom").prop("required", true);
            $("#BillingDemandNotice_EffectiveFrom").prop("disabled", false);
            $("#demandnoticedropdown").prop("required", true);
            $("#demandnoticedropdown").prop("disabled", false);
        } else {
            $("#BillingDemandNotice_EffectiveFrom").prop("required", false);
            $("#BillingDemandNotice_EffectiveFrom").val(0);
            $("#BillingDemandNotice_EffectiveFrom").prop("disabled", true);
            $("#demandnoticedropdown").prop("required", false);
            $('#demandnoticedropdown').prop('selectedIndex', 0);
            $("#demandnoticedropdown").prop("disabled", true);
        }
    }

    function toggleDirectAssessmentChildren() {
        if ($("#DirectAssessment_AllowFileUpload").prop('checked')) {
            $("#AdapterValue").prop("disabled", false);
            $("#AdapterValue").prop("required", true);
        } else {
            $("#AdapterValue").prop("disabled", true);
            $("#AdapterValue").prop("required", false);
        }
    }

    $('.billing_type_selector_nosch').click(function (event) {
        //uncheck the other radio boxes
        $(".billing_type_selector").prop('checked', false);
        $("#billing_type_panel").hide();
        $(".billing_type_control").prop("disabled", true);
        $('.billing_type_control').attr('checked', false);
        $(".frequency_control").hide();
        $("#billing_type_file").hide();
        $("#billing_type_payee").hide();
        //set the require attr to false
        $("#TemplateValue").prop("required", false);
        $("#FileUploadAdapterValue").prop("required", false);
        $("#TemplateValue").val("");


        checkDueDate(false);
        if ($(this).val() == "DirectAssessment") {
            $("#billing_type_payee").show();
            $("#SAmount").prop("required", false);
        } else if ($(this).val() == "FileUpload") {
            $("#billing_type_file").show();
            $("#SAmount").prop("required", false);
            $("#TemplateValue").prop("required", true);
        } else {
            $("#billing_type_payee").hide();
            $("#SAmount").prop("required", true);
        }
    });


    $('input:radio[name="BillingType"]').change(function () {
        $("#FrequencyType").val(1);

        if ($(this).val() === 'variable') {
            $(".frequency_control").show();

            $('#v_year').hide();
            $('#v_month').hide();
            $('#v_week').hide();
            $('#v_day').show();
            $(".recurring_control").prop("disabled", false);
            $(".recurring_variable_control").prop("disabled", false);
            $(".variable_billing").show();
            $(".fixed_billing").hide();
            checkDueDate(true);
        } else if ($(this).val() === 'fixed') {
            $(".frequency_control").show();

            $('#f_year').hide();
            $('#f_month').hide();
            $('#f_week').hide();
            $('#f_week_number').hide();
            $('#f_day_dropdown').hide();
            $('#f_day_number').show();
            $('#f_day').hide();
            $(".recurring_variable_control").prop("disabled", true);
            $(".recurring_control").prop("disabled", false);
            $(".variable_billing").hide();
            $(".fixed_billing").show();
            checkDueDate(true);
        }
    });


    $('#TemplateValue').on('change', function () {
        var selectedTemplate = $('#TemplateValue').val();
        //find selected template in templates
        $('#FileUploadAdapterValue').empty();
        $('<option value="" disabled selected>Select an adapter implementation</option>').appendTo('.file_upload_adapter');
        loadFileUploadImpls(selectedTemplate, "");
    });


    function loadFileUploadImpls(selectedTemplate, selectedImpl) {
        for (var key in templatesModel.ListOfTemplates) {
            if (templatesModel.ListOfTemplates[key].Name === selectedTemplate) {
                $("#FileUploadAdapterValue").prop("required", true);
                
                $(templatesModel.ListOfTemplates[key].ListOfUploadImplementations).each(function () {
                    //console.log(selectedImpl);
                    //console.log("this value " + this.Value);
                    if (selectedImpl != null && selectedImpl === this.Value) {
                        $('<option value="' + this.Value + '">' + this.Name + '</option>').appendTo('.file_upload_adapter');
                    } else {
                        $('<option value="' + this.Value + '">' + this.Name + '</option>').appendTo('.file_upload_adapter');
                    }
                });

                if (selectedImpl != null && selectedImpl.length > 0){ $("#FileUploadAdapterValue").val(selectedImpl); }
                break;
            }
        }
    }


    $('#FrequencyType').on('change', function () {
        var checkedRadio = $('input[name=BillingType]:checked').val();
        var selectedValue = $(this).val();
        if (checkedRadio === 'variable') {
            handleVariablePart(selectedValue);
        } else if (checkedRadio === 'fixed') {
            handleFixedPart(selectedValue);
        }
    });


    function checkDueDate(isRecurring) {
        //alert("check");
        if (isRecurring) {
            $('#DueDateModel_DueOnNextBillingDate').prop('disabled', false);
            $('#DueDateModel_DueOnNextBillingDate').prop('checked', true);
            //$('#DueDateModel_DueDateInterval').attr('disabled', true);
            //$('#filterResults').attr('disabled', true);
        } else {
            $('#DueDateModel_DueOnNextBillingDate').prop('checked', false);
            $('#DueDateModel_DueOnNextBillingDate').prop('disabled', true);
            //$('#DueDateModel_DueDateInterval').attr('disabled', false);
            //$('#filterResults').attr('disabled', false);
        }

    }


    function handleVariablePart(selectedValue) {
        switch (Number(selectedValue)) {
            case 1:
                $('#v_year').hide();
                $('#v_month').hide();
                $('#v_week').hide();
                $('#v_day').show();
                break;
            case 2:
                $('#v_year').hide();
                $('#v_month').hide();
                $('#v_week').show();
                $('#v_day').hide();
                break;
            case 3:
                $('#v_year').hide();
                $('#v_month').show();
                $('#v_week').hide();
                $('#v_day').hide();
                break;
            case 4:
                $('#v_year').show();
                $('#v_month').hide();
                $('#v_week').hide();
                $('#v_day').hide();
                break;
            default:
                $('#v_year').hide();
                $('#v_month').hide();
                $('#v_week').hide();
                $('#v_day').show();
                break;
        }
    }

    $('#f_week_select').on('change', function () {
        var selectedValue = $(this).val();
        if (selectedValue === 'FirstDayOfTheMonth' || selectedValue === 'FirstWeekDayOfTheMonth' || selectedValue === 'LastDayOfWeekDayOfTheMonth' || selectedValue === 'LastDayOfTheMonth') {
            $('#f_day_dropdown').hide();
        } else {
            $('#f_day_dropdown').show();
        }
    });

    function handleFixedPart(selectedValue) {
        switch (Number(selectedValue)) {
            case 1:
                $('#f_year').hide();
                $('#f_month').hide();
                $('#f_week').hide();
                $('#f_week_number').hide();
                $('#f_day_dropdown').hide();
                $('#f_day').hide();
                $('#f_day_number').show();
                break;
            case 2:
                $('#f_year').hide();
                $('#f_month').hide();
                $('#f_week').hide();
                $('#f_week_number').hide();
                $('#f_day_dropdown').hide();
                $('#f_day').show();
                $('#f_day_number').hide();
                break;
            case 3:
                $('#f_year').hide();
                $('#f_month').show();
                $('#f_week').show();
                $('#f_week_number').hide();
                $('#f_day_dropdown').show();
                $('#f_day').hide();
                $('#f_day_number').hide();
                break;
            case 4:
                $('#f_year').show();
                $('#f_month').show();
                $('#f_week').show();
                $('#f_week_number').hide();
                $('#f_day_dropdown').show();
                $('#f_day').hide();
                $('#f_day_number').hide();
                break;
            default:
                $('#f_year').hide();
                $('#f_month').hide();
                $('#f_week').hide();
                $('#f_week_number').hide();
                $('#f_day').hide();
                $('#f_day_number').hide();
                break;
        }
    }
});