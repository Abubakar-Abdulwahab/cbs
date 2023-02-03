$(document).ready(function () {
    $('#SubmitForm').hide();
    //$('#tinNext').hide(); 
    $('#paidEmployment').hide();
    $('#selfEmployment').hide();
    $('#ownership').hide();

    //$('.tinUserTypeToggle').change(function () {
    //    var selectedValue = $(this).val();
    //    if (selectedValue == "Corporate") {
    //        $('#tinNext').hide();

    //    }
    //    else {
    //        $('#tinNext').show();
    //    }
        
    //});

    $(".Date-Field").each(function () {

        $(this).datetimepicker({
            format: "mm/dd/yyyy",
            autoclose: true,
            todayBtn: true,
            pickerPosition: "bottom-left",
            minView: "2"
        });
    });

    var newCount = 0;
    $('#add_assets').click(function (e) {
        var validCorrect = true;
        e.preventDefault();
        $('.Asset_validation').each(function () {
            validCorrect = validityCheck(this, validCorrect);
        });
        if (validCorrect == false) return;

        var AssetTable = "";
        var AssetTableRev = "";
        var TypeOfAsset = $('#AssetOwned_TypeOfAsset').val();
        var LocationOfAsset = $('#AssetOwned_LocationOfAsset').val();
        var MarketValue = $('#AssetOwned_MarketValue').val();
        var OwnershipDate = $('#AssetOwned_OwnershipDate').val();

        AssetTable = "<tr class='addRowAsset' id ='tr" + newCount + "'>" +
            "<td class='newCount'><a class='Assets_row_remove del_btn' href='#' id='" + newCount + "' ><i class='glyphicon glyphicon-trash green'></i></a></td>" +
        "<td class='TypeOfAsset'>" + TypeOfAsset + "</td><td class='LocationOfAsset'>" + LocationOfAsset +
           "</td><td class='MarketValue'>" + MarketValue + "</td><td class='OwnershipDate'>" + OwnershipDate + "</td></tr>";

        AssetTableRev = "<tr class='addRowAssetRev' id ='tr" + newCount + "'>" +
        "<td class='TypeOfAsset'>" + TypeOfAsset + "</td><td class='LocationOfAsset'>" + LocationOfAsset +
           "</td><td class='MarketValue'>" + MarketValue + "</td><td class='OwnershipDate'>" + OwnershipDate + "</td></tr>";

        $("#Assets_Table").append(AssetTable);

        $("#Rev_Assets_Table").append(AssetTableRev);

        $('.Asset_validation').each(function () {
            $(this).val('');
        });

    });

    function validityCheck(field, isValidField) {
        if ($.trim($(field).val()) == '') {
            isValidField = false;
            $(field).css({
                "border": "1px solid red",
                "background": "#FFCECE"
            });
        }
        else {
            $(field).css({
                "border": "",
                "background": ""
            });
        }
        return isValidField;
    }

    $("#Assets_Table").on('click', '.Assets_row_remove', function (e) {
        DeleteRole('Assets_row_remove', 'tr');
        DeleteRole('Rev_Assets_Table', 'tr');
        e.preventDefault();
    });

    function DeleteRole(btnProperty, prtRowId) {
        var buttonId = $('.' + btnProperty).attr("id");
        var rowId = prtRowId + buttonId;
        $("#" + rowId).remove();
    };

    $('#add_Dependants').click(function (e) {
        var validCorrect = true;
        e.preventDefault();
        $('.Dependants_validation').each(function () {
            validCorrect = validityCheck(this, validCorrect);
        });
        if (validCorrect == false) return;

        var DependantTable = "";
        var DependantTableRev = "";
        var DepChildLastName = $('#DepChildLastName').val();
        var DepChild1FirstName = $('#DepChild1FirstName').val();
        var DepChildState = $('#DepChildState').val();
        var DepChildMiddleName = $('#DepChildMiddleName').val();
        var DepChildDateofBirth = $('#DepChildDateofBirth').val();
        var DepChildTIN = $('#DepChildTIN').val();
        var DepChildRelationship = $('#DepChildRelationship').val();

        DependantTable = "<tr class='addRowDependant' id ='trr" + newCount + "'>" +
            "<td class='newCount'><a class='Dependant_row_remove del_btn' href='#' id='" + newCount + "' ><i class='glyphicon glyphicon-trash green'></i></a></td>" +
        "<td class='DepChildLastName'>" + DepChildLastName + "</td><td class='DepChild1FirstName'>" + DepChild1FirstName +
           "</td><td class='DepChildState'>" + DepChildState + "</td><td class='DepChildMiddleName'>" + DepChildMiddleName +
           "</td><td class='DepChildDateofBirth'>" + DepChildDateofBirth +
           "</td><td class='DepChildTIN'>" + DepChildTIN +
           "</td><td class='DepChildRelationship'>" + DepChildRelationship +
           "</td></tr>";

        DependantTableRev = "<tr class='addRowDependantRev' id ='tr" + newCount + "'>" +
        "<td class='DepChildLastName'>" + DepChildLastName + "</td><td class='DepChild1FirstName'>" + DepChild1FirstName +
           "</td><td class='DepChildState'>" + DepChildState + "</td><td class='DepChildMiddleName'>" + DepChildMiddleName +
           "</td><td class='DepChildDateofBirth'>" + DepChildDateofBirth +
           "</td><td class='DepChildTIN'>" + DepChildTIN +
           "</td><td class='DepChildRelationship'>" + DepChildRelationship +
           "</td></tr>";

        $("#Dependants_Table").append(DependantTable);

        $("#Rev_Dependants_Table").append(DependantTableRev);

        $('.Asset_validation').each(function () {
            $(this).val('');
        });

    });

    $("#Dependants_Table").on('click', '.Dependant_row_remove', function (e) {
        DeleteRole('Dependant_row_remove', 'trr');
        DeleteRole('Rev_Dependant_Table', 'trr');
        e.preventDefault();
    });

    //populate();
    //function populate() {
    //    var inputFields = $("input[type=text]");
    //    var emailFields = $(".email");
    //    var passwordFields = $("input[type=password]");

    //    passwordFields.each(function () {
    //        $(this).val("example");
    //    });
    //    inputFields.each(function () {
    //        $(this).val("example");
    //    });
    //    emailFields.each(function () {
    //        $(this).val("example@example.com");
    //    });
    //};


    //var Message = alertData;
    //window.onload = viewMsg();

    //function viewMsg() {
    //    $.each(Message, function (i, result) {
    //        var msg = result.Message;
    //        if (msg != "nil") {
    //            if (msg !== "") {
    //                if (msg.match("Failed")) {
    //                    toastr.options.positionClass = 'toast-top-right';
    //                    toastr.error(msg);
    //                }
    //                else {
    //                    toastr.options.positionClass = 'toast-top-right';
    //                    toastr.success(msg);
    //                }
    //            }
    //        }
    //    });
    //}

    $('.stateCascade').statePopulate({
        stateElement: '#ResidentialAddress_State',
        cityElement: '#ResidentialAddress_LGA'
    });

    $('.stateCascade2').statePopulate({
        stateElement: '#RepresentativeAddress_State',
        cityElement: '#RepresentativeAddress_LGA'
    });

   



});

function GetAssets() {
    var Assets = "";
    var rows = $('tr.addRowAsset');
    $(rows).each(function () {
        var TypeOfAsset = $(this).children('.TypeOfAsset').first().text();
        var LocationOfAsset = $(this).children('.LocationOfAsset').first().text();
        var MarketValue = $(this).children('.MarketValue').first().text();
        var OwnershipDate = $(this).children('.OwnershipDate').first().text();
        Assets = Assets + TypeOfAsset + 'β' + LocationOfAsset + 'β' + MarketValue + 'β' + OwnershipDate + 'β';
    });
    $("#AssetsTable").val(Assets);
}

function GetDependants() {
   
    var Dependants = "";
    var rows = $('tr.addRowDependant');
    $(rows).each(function () {
        var DepChildLastName = $(this).children('.DepChildLastName').first().text();
        var DepChild1FirstName = $(this).children('.DepChild1FirstName').first().text();
        var DepChildMiddleName = $(this).children('.DepChildMiddleName').first().text();
        var DepChildState = $(this).children('.DepChildState').first().text();
        var DepChildDateofBirth = $(this).children('.DepChildDateofBirth').first().text();
        var DepChildTIN = $(this).children('.DepChildTIN').first().text();
        var DepChildRelationship = $(this).children('.DepChildRelationship').first().text();

        Dependants = Dependants + DepChildLastName + 'β' + DepChild1FirstName + 'β' + DepChildMiddleName + 'β' + DepChildState + 'β' + DepChildDateofBirth + 'β' + DepChildTIN + 'β' + DepChildRelationship + 'β';
    });
    $("#DependantsTable").val(Dependants);
}