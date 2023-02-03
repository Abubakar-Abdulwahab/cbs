$(document).ready(function () {
    var token = $("input[name=__RequestVerificationToken]").val();
    var officeArray = new Array();
    var mdasArray = new Array();
    var exDropDown = new Object();

    $("#officeId").change(function () {
        //disable button
        $('#submitBtn').prop("disabled", true);

        //when an office is selected, clean up display
        //lets check if we already have this key store
        var selected = $('#officeId').val();
        if (exDropDown["expt" + selected] == undefined) {
            //do work
            var url = 'c/active-mdas';
            var requestData = { "sId": selected, "__RequestVerificationToken": token };
            $("#x-babyloader").show();
            doAJAXCall(url, requestData, selected, 1, 0);
        } else {
            buildSelectForExpertSystem(exDropDown["expt" + selected], selected, false, true);
        }
    });

    $("#mda-select").change(function () {
        //disable button
        $('#submitBtn').prop("disabled", true);

        //when an office is selected, clean up display
        //lets check if we already have this key store
        var selectedExSys = $('#officeId').val();
        var selectedMDA = $('#mda-select').val();
        if (exDropDown["expt" + selectedExSys]["mda" + selectedMDA].RevenueHeads == undefined) {
            //do work
            var url = 'c/active-revenue-heads';
            var requestData = { "sId": selectedExSys, "mId": selectedMDA, "__RequestVerificationToken": token };
            $("#m-babyloader").show();
            doAJAXCall(url, requestData, selectedExSys, 2, selectedMDA);
        } else {
            buildSelectForMDA(exDropDown["expt" + selectedExSys]["mda" + selectedMDA].RevenueHeads, selectedExSys, selectedMDA, false, true)
        }
    });

    function doAJAXCall(url, requestData, selected, level, selectedMda) {
        $.post(url, requestData, function (data) {
            //if (!data.Error) {

            //    //$("#totalamt").html("&#x20A6;" + data.ResponseObject.Amount);
            //    //Page = 1;
            //    //PageSize = data.ResponseObject.PageSize;
            //    //$("#paginator").show();
            //    //$("#pageSize").html(PageSize);
            //    //$("#page").html(Page);
            //    //handleReport(data.ResponseObject.PayeeExcelReport);
            //    //buildTableFromProcess(data.ResponseObject.Payees, false);
            //    //canProgress = true;
            //    //$("#proceedbtn").attr("disabled", false);
            //} else {
            //    //$("#proceedbtn").attr("disabled", true);
            //    //$("#level").html("Error reading the file:" + data.ResponseObject).css("color", "red");
            //}
        }).fail(function () {
            //$("#proceedbtn").attr("disabled", true);
            //$("#level").html("Error reading the file: Please try again later or contact Admin").css("color", "red");
        }).always(function () {
            //$('.payeeloader').hide();
        }).done(function (data) {
            if (!data.Error) {
                if (level == 1) {
                    buildSelectForExpertSystem(data.ResponseObject, selected, true, false);
                } else if (level == 2) {
                    buildSelectForMDA(data.ResponseObject, selected, selectedMda, true, false)
                }
            }
        });
    }


    function buildSelectForExpertSystem(list, selected, addToObj, fromInStore) {
        //lets see what level we are dealing with
            //populate the mda drop down list
        //get mda select
            var mdaSelectopt = $("#mda-select");
            //empty it
            mdaSelectopt.empty();
            $("#revenue-select").empty();
            var optString = "";
            //if the list has any values, lets populate the drop down
            if (fromInStore) {
                var anyValue = false;
                optString = '<option value="" selected disabled>Select an MDA</option>';
                for (var key in list) {
                    optString += '<option value="' + list[key].Id + '">' + list[key].Name + '</option>';
                    anyValue = true;
                }
                if (!anyValue) {
                    optString = '<option value="" selected disabled>No MDA found try again later</option>';
                }
            }
            else {
                exDropDown["expt" + selected] = {};
                if (list.length > 0) {
                    optString = '<option value="" selected disabled>Select an MDA</option>';
                    for (var val in list) {
                        optString += '<option value="' + list[val].Id + '">' + list[val].Name + '</option>';
                        //add the list item to the list of local mdas in the sys object dictionary
                        if (addToObj) {
                            exDropDown["expt" + selected]["mda" + list[val].Id] = { Name: list[val].Name, Id: list[val].Id };//list[val].Name;
                        }
                    }
                } else {
                    optString = '<option value="" selected disabled>No MDA found try again later</option>';
                }
            }
            $("#mdaDiv").show();
            $("#revDiv").hide();
            $("#x-babyloader").hide();
            $(optString).appendTo(mdaSelectopt);
    }


    function buildSelectForMDA(list, selectedSys, selectedMda, addToObj, fromInStore) {
        //lets see what level we are dealing with
        //populate the mda drop down list
        //get mda select
        var revenueSelectopt = $("#revenue-select");
        //empty it
        revenueSelectopt.empty();
        var optString = "";
        //if the list has any values, lets populate the drop down
        if (fromInStore) {
            var anyValue = false;
            optString = '<option value="" selected disabled>Select an Revenue Head</option>';
            for (var key in list) {
                optString += '<option value="' + list[key].Id + '">' + list[key].Name + '</option>';
                anyValue = true;
            }
            if (!anyValue) {
                optString = '<option value="" selected disabled>No Revenue Head found try again later</option>';
            }
        }
        else {
            if (list.length > 0) {
                optString = '<option value="" selected disabled>Select an Revenue Head</option>';
                for (var val in list) {
                    optString += '<option value="' + list[val].Id + '">' + list[val].Name + '</option>';
                }
                if (addToObj) {
                    exDropDown["expt" + selectedSys]["mda" + selectedMda].RevenueHeads = list;
                }
            } else {
                optString = '<option value="" selected disabled>No Revenue Head found try again later</option>';
            }
        }

        //$("#mdaDiv").show();
        $("#revDiv").show();
        $("#x-babyloader").hide();
        $("#m-babyloader").hide();
        
        $('#submitBtn').prop("disabled", false);
        revenueSelectopt.show();
        $(optString).appendTo(revenueSelectopt);
    }

    function cleanUpDisplay(level) {
        if (level == 1) {
            $("#mda").remove();
            //$("#mda").hide();
            $("#revenue").remove();
            //$("#revenue").hide();
        } else if (level == 2) {
            //$('select').children('option:not(:first)').remove();
        } else if (level == 3) {

        }
    }
    //$("#mda_selected").on("change", function () {
    //    if ($("#mda_selected :selected").val() != "") {
    //        var url = 'RevenueHeads/Tax';
    //        var token = $("input[name=__RequestVerificationToken]").val();
    //        var requestData = { "mdaSlug": $("#mda_selected :selected").val(), "__RequestVerificationToken": token };
    //        $.post(url, requestData, function (data) {
    //            if (data.length > 0) {
    //                var options = '';
    //                //console.log(data);
    //                for (i = 0; i < data.length; i++) {
    //                    options += '<option value="' + data[i].Id + '">' + data[i].Name + '</option>';
    //                }
    //                $("#revenuehead_selected").empty().append(options);
    //            } else {
    //                $("#revenuehead_selected").empty().append('<option>No Revenue Heads Found</option>');
    //            }
    //        });
    //    } else {
    //        $("#revenuehead_selected").empty().append('<option>Select Revenue Head</option>');
    //    }
    //});    


    //$("#officeSelectForm").on('submit', function (e) {
    //    //e.preventDefault();
    //    ////data list value
    //    //var inputValue = $("#officeId :selected").val();
    //    //console.log(inputValue);
    //    //var officeId = $("#offices option[value='" + inputValue + "']").attr('data-value');
    //    //var officePath = $("#offices option[value='" + inputValue + "']").attr('data-Path');
    //    //if (officeId == undefined || officePath == undefined) {
    //    //    $("#errorFlash").show();
    //    //    $("#errorMsg").html("Please select a valid item");
    //    //}
    //    //$('#officeId').val(officeId);
    //    //$('#officePath').val(officePath);
    //    //$('#officeSelectForm').get(0).submit();
    //});

    $("#closeModalFlash").click(function (e) {
        $('#errorFlash').hide();
    });
});