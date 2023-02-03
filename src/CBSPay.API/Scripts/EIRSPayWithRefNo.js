jQuery(document).ready(function ($) {

    //$('#billRefDetails').hide();
    //$('#ReferenceItems').hide();
    //$('#ErrorMessage').hide();
    //$('.loader').hide();
    

    $('#FetchRefItems').click(function (e) {
        console.log(e);
        if ($("#ReferenceNumber").val() === "") {
            alert("Reference Number cannot be empty");      
                
        }
        //var phoneno = /^\+(d{3})\)(d{10})$/;
        var phoneno = /^(\+[0-9]{3}|0)[0-9]{10}$/;
        var inputtxt = $("#PhoneNumber").val();
        if (phoneno.test(inputtxt)) {
            console.log(true);
        } 
        else {
            alert("Please enter a valid phone number");
            e.preventDefault();
        }
        //else {

        //    $('.loader').show();


        //    var formData = new FormData();

        //    e.preventDefault();
        //    var model = {};
        //    model.ReferenceNumber = $("#ReferenceNumber").val();
        //    model.PaymentChannel = $("#PaymentChannel").val();
        //    model.ClientId = $("#ClientId").val();
        //    model.ClientSecret = $("#ClientSecret").val();

        //    var stringedModel = JSON.stringify(model);

        //    formData.append("formData", JSON.stringify(model));

        //    $.ajax({
        //        type: 'POST',
        //        url: "/home/getreferenceitems",
        //        contentType: "application/json; charset=utf-8",
        //        processData: false,
        //        dataType: 'json',
        //        data: stringedModel,

        //        success: function (returnModel) {
        //            console.log(returnModel);
        //            if (returnModel != null) {
        //                //var thisModel = JSON.parse(returnModel);
        //                var thisModel = returnModel;
        //                if (thisModel.Status == "Success") {
        //                    populateReferenceItemTable(thisModel);
        //                }
        //                else {
        //                    $('.loader').hide();
        //                    alert('Failed to retrieve Data.');
        //                    console.log('Failed to retrieve Data.');
        //                }
        //            }
                   
        //            else {
        //                $('.loader').hide();
        //                //diplayErrorMessage();
        //                alert('An error occurred, could not retrieve Tax Payer Data.');
        //            }
        //        },
        //        error: function (ex) {
        //            alert('Failed to retrieve Data.');
        //           // toastr.error('Failed to retrieve Data.' + ex);
        //        }
        //    });
        //}      

    });

    //function populateReferenceItemTable(returnModel) {
    //    //var thisModel = JSON.parse(returnModel);

    //    var tablebody = $('#ReferenceItemsBody');
    //    var referenceBody = $('#referenceBody');

    //    var paymentRequestItems = returnModel.ReferenceItems;
    //    var count = 0;
    //    if (paymentRequestItems.length < 1) {
    //        alert("Could not retrieve tax payer details, please try again");
    //    }

    //    paymentRequestItems.forEach(function (refItem) {
    //        var row = $('<tr/>');
    //        //var td = $('<td/>');
    //        //var newRow = "<td>" + refItem.ItemID + "</td><td>" + refItem.ItemName + "</td><td>" + refItem.ItemAmount + "</td><td>" + refItem.SettlementAmount + "</td><td>" + refItem.AmountPaid + "</td>"; AmountToPay '+ count +'
    //        var newRow = "<td>" + refItem.ItemID + "</td><td>" + refItem.ItemName + "</td><td>" + refItem.ItemAmount + "</td><td>" + refItem.SettlementAmount + "</td><td>" + '<input type="number" class="AmountToPay" name="PaymentRequestItems[' + count + '].AmountPaid" placeholder="10000" id="AmountToPay' + count + '" />' + "</td>";
    //        $('<input hidden name="PaymentRequestItems[' + count + '].ItemId" value="' + refItem.ItemID + '"/>').appendTo(referenceBody);
    //        $('<input hidden name="PaymentRequestItems[' + count + '].ItemName" value="' + refItem.ItemName + '"/>').appendTo(referenceBody);
    //        $('<input hidden name="PaymentRequestItems[' + count + '].ItemAmount" value="' + refItem.ItemAmount + '"/>').appendTo(referenceBody);
    //        $('<input hidden name="PaymentRequestItems[' + count + '].SettlementAmount" value="' + refItem.SettlementAmount + '"/>').appendTo(referenceBody);

    //        row.append(newRow);
    //        tablebody.append(row);
    //        count++;
    //    });

    //    //<label class="control-label">Total Amount</label>
    //    var billRefDetails = '<div> Bill Ref ' + returnModel.ReferenceNumber+' for ' + returnModel.TaxPayerName +' found </div>';
    //    var refAmount = '<label for="refAmount" class="control-label col-sm-3">Total Amount</label> <div class="col-sm-9"> <input class="form-control card - number" size="20" type="text" id="TotalAmount" name="TotalAmount" value="' + returnModel.Amount + '" readonly> </div>';
    //    var refID = '<input size="20" type="hidden" id="ReferenceId" name="ReferenceId" value="' + returnModel.ReferenceID + '">';
    //    //<label class="control-label">Tax Payer Name</label>
    //    var taxPayerName = '<label for="refTaxPayer" class="control-label col-sm-3">Tax Payer Name</label> <div class="col-sm-9" > <input  class="form-control card - number" size="20" type="text" id="TaxPayerName" name="TaxPayerName" value="' + returnModel.TaxPayerName + '" readonly> </div>';

    //    $('#refTaxPayer').append(taxPayerName);
    //    $('#refAmount').append(refAmount);
    //    referenceBody.append(refID);
    //    $('.loader').hide();
    //    $('#billRefDetails').append(billRefDetails)
    //    $('#FetchRefItems').hide();
    //    $('#billRefDetails').show();
    //    $('#ReferenceItems').show();
    //}

    //$('#EIRSPayWithRef').click(function (e) {
    //    //e.preventDefault();
    //    var countI = 0;
    //    //$('.AmountToPay').each(function (index, value) {
    //        $('.AmountToPay').each(function () {
    //        console.log($(this).text());
    //            $('<input hidden name="PaymentRequestItems[' + count + '].AmountPaid" value="' + $(this).text() + '"/>').appendTo(referenceBody);
    //            countI++;
    //    });

    //});

});



