$(document).ready(function () {
    var commandLGAMap = new Map();

    $("#esform1").on('submit', function (e) {
    clearStateList();
    preBuildStateDropDown($('#state').val());

    clearLgaList();
    preBuildLgaDropDown($('#state').val());
    
    clearModalCommandList();
    getModalCommandList($('#lga').val());

    $('#calEstStartDate').attr('value',$('#startDate').val());
    $('#calEstEndDate').attr('value',$('#endDate').val());
    });


    $("#estModalForm").on('submit', function (e) {
        e.preventDefault();
        $("#calculateEstSubBtn").attr("disabled", true);
        getEstimate($('#calEstOfficerQty').val(), $('#calEstStartDate').val().toString(), $('#calEstEndDate').val().toString(), $('#state').val(), $('#lga').val(), $('#subSubCategoryId').val());
    });

    $('#calEstModalCloseBtn').click(function(){
        $('#calEstModal-content-container').hide("fast",function(){
            $('#calEstModal').hide();
        });
    });

    $('#calEstModalToggle').click(function(){
        $('#calEstModal').show("fast",function(){
            $('#calEstModal-content-container').show("fast");
            setTimeout(function(){$('#calEstModal').css('background-color','rgba(9,16,39,0.71)');},400);
            $(window).scrollTop(0);
        });
    });

    $('#calEstState').change(function () {
        clearLgaList();
        buildLgaDropDown($('#calEstState').val());
    });

     $('#calEstLga').change(function () {
        clearModalCommandList();
        getModalCommandList($('#calEstLga').val());
    });

    function clearModalCommandList() {
        $("#modalCommands").empty();
    }

    function clearLgaList() {
        $("#calEstLga").empty();
    }

    function clearStateList(){
        $("#calEstState").empty();
    }

    function buildLgaDropDown(stateId) {
        var options = '<option value="0">Select an LGA</option>';
        $(stateLGAMap.get(parseInt(stateId))).each(function () {
            options += '<option value="' + this.Id + '">' + this.Name + '</option>';
        });
        $("#calEstLga").append(options);
    }

    function preBuildLgaDropDown(stateId) {
        var options = '<option value="0">Select an LGA</option>';
        $(stateLGAMap.get(parseInt(stateId))).each(function () {
            if($('#lga').val() == this.Id){
            options += '<option value="' + this.Id + '" selected>' + this.Name + '</option>';
            }else{
            options += '<option value="' + this.Id + '">' + this.Name + '</option>';
            }
        });
        $("#calEstLga").append(options);
    }

    function preBuildStateDropDown(stateNum){
        var options = '<option value="0">Select a State</option>';
        states.forEach((state)=>{
            if(stateNum == state.Id){
            options += '<option value="' + state.Id + '" selected>' + state.Name + '</option>';
            }else{
            options += '<option value="' + state.Id + '">' + state.Name + '</option>';
            }
        });
        $("#calEstState").append(options);
    }


    function getModalCommandList(lgaId) {
        //event.preventDefault();
        var commandList = commandLGAMap.get(lgaId);
        $('#searchError').empty();
        if (commandList === undefined) {
            //do ajax call
            var url = 'x/get-commands-in-lga';
            var requestData = { "lgaId": lgaId, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
            $.post(url, requestData, function (data) {
                if (!data.Error) {
                    commandLGAMap.set(lgaId, data.ResponseObject);
                    buildModalCommandDropDown(lgaId);
                } else {
                    $('#searchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000" >' + data.ResponseObject+'</span > ');
                }
               
            }).fail(function () {
                $('#searchError').append('<span class="field-validation-error tiny-caption" style = "color:#ff0000">An error occurred try refreshing the page.</span > ');
            }).always(function () {
            });

        } else {
            if(commandLGAMap !== undefined){
                buildModalCommandDropDown(lgaId);
            }
        }
    }


    function buildModalCommandDropDown(lgaId) {
        var options = "";
        $(commandLGAMap.get(lgaId)).each(function () {
            options += '<option data-value="' + this.Id + '" value="' + this.Name + '(' + this.Code + ')" ></option>';
        });
        $("#modalCommands").append(options);
    }

    function getEstimate(officerCount,start,end, stateId, lgaId, subSubCategoryId){
        $('#calculateEstError').empty();
        $('#profileloader').show();
        $('#estimateNoteText').empty();
        if(officerCount == 0){
            $('#profileloader').hide();
            $("#calculateEstSubBtn").attr("disabled", false);
            $('#calculateEstError').html('<span class="field-validation-error tiny-caption" style = "color:#ff0000">Number of officers has to be greater than 0.</span > ');
            return;
        }
        $('#totalCostVal').empty();
        if(officerCount !== undefined){
        //do ajax call
            var url = 'get-estimate';
            var requestData = { "officerQty": officerCount, "startDate": start, "endDate": end, "stateId":stateId, "lgaId": lgaId, "subSubTaxCategoryId":subSubCategoryId, "__RequestVerificationToken": $("input[name=__RequestVerificationToken]").val() };
            $.post(url, requestData, function (data) {
                if (!data.Error) {
                    $('#profileloader').hide();
                    if (data.StatusCode == 204) {
                        $('#calculateEstError').html('<span class="field-validation-error tiny-caption" style = "color:#ff0000" > ' + data.ResponseObject + ' </span > ');
                        return;
                    }
                    $('#totalCostVal').html("&#8358;"+formatAmount(data.ResponseObject.computedAmount));
                    $('#estimateNoteText').html(data.ResponseObject.estimateNote);
                } else {
                    $('#profileloader').hide();
                    $('#calculateEstError').html('<span class="field-validation-error tiny-caption" style = "color:#ff0000" > An error occured when trying to fetch cost estimate, please try again after a few seconds</span > ');
                }
               
            }).fail(function () {
                $('#calculateEstError').html('<span class="field-validation-error tiny-caption" style = "color:#ff0000">An error occurred try refreshing the page.</span > ');
            }).always(function () {
                $('#profileloader').hide();
                $("#calculateEstSubBtn").attr("disabled", false);
            });
        }
    }

    function formatAmount(x) {
        return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + ".00";
    }


});