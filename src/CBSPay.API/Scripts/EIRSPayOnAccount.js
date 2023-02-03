jQuery(document).ready(function ($) {
    $('.step2').hide(); 
    $('#PayOnAccountItems').hide();


    $('#FetchTaxPayerData').click(function (e) {
        if ($("#POASearchValue").val() == "") {
            alert(" The Search Value cannot be empty");
        }
        else {

            $('.loader').show();


            var formData = new FormData();

            e.preventDefault();
            var model = {}; 
            model.TaxPayerPOASearchOption = $("#POASearchOptions").val();
            model.POASearchOptionValue = $("#POASearchValue").val();
            model.PaymentChannel = $("#PaymentChannel").val();
            model.ClientId = $("#ClientId").val();
            model.ClientSecret = $("#ClientSecret").val();

            var stringedModel = JSON.stringify(model);

            formData.append("formData", JSON.stringify(model));

            $.ajax({
                type: 'POST',
                url: "/home/VerifyTaxPayer/",
                contentType: "application/json; charset=utf-8",
                processData: false,
                dataType: 'json',
                data: stringedModel,

                success: function (returnModel) {
                    console.log(returnModel);
                    if (returnModel != null) {
                        //var thisModel = JSON.parse(returnModel);
                        var thisModel = returnModel;
                        if (thisModel.Status == "Success") {
                            populatePOATable(thisModel);
                        }
                        else {
                            $('.loader').hide();
                            alert('Failed to retrieve Data.');
                            console.log('Failed to retrieve Data.');
                        }
                    }

                    else {
                        $('.loader').hide();
                        alert('An error occurred, could not retrieve Tax Payer Data.');

                    }
                },
                error: function (ex) {
                    $('.loader').hide();
                    alert('Failed to retrieve Data.');

                }
            });
        }




    });

    function populatePOATable(returnModel) {
        //var thisModel = JSON.parse(returnModel);

        var tablebody = $('#POAItemList');
        var referenceBody = $('#POAItemsBody');

        var paymentRequestItems = returnModel.TaxPayerResponses;
        var count = 0;
        if (paymentRequestItems.length < 1)
        {
            alert("Could not retrieve tax payer details, please try again");
        }

        paymentRequestItems.forEach(function (refItem) {
            var row = $('<tr id="row' +refItem.TaxPayerID+'"/>');
            //var td = $('<td/>');
            var newRow = "<td>" + refItem.TaxPayerRIN + "</td><td>" + refItem.TaxPayerName + "</td><td>" + refItem.TaxPayerTypeName + "</td><td>" + '<input type="radio" name="SelectedTaxPayer" value="' + refItem.TaxPayerID +'">' + "</td>";
            $('<input hidden name="PaymentRequestItems[' + count + '].ItemId" value="' + refItem.TaxPayerRIN + '"/>').appendTo(referenceBody);
            $('<input hidden name="PaymentRequestItems[' + count + '].ItemName" value="' + refItem.TaxPayerName + '"/>').appendTo(referenceBody);
            $('<input hidden name="PaymentRequestItems[' + count + '].ItemAmount" value="' + refItem.TaxPayerTypeName + '"/>').appendTo(referenceBody);
            $('<input hidden name="PaymentRequestItems[' + count + '].SettlementAmount" value="' + refItem.SettlementAmount + '"/>').appendTo(referenceBody);

            row.append(newRow);
            tablebody.append(row);
            count++; 
            
        });

        $('.loader').hide();
        $('#FetchTaxPayerData').hide();
        $('#PayOnAccountItems').show();
        $('#POAItemList').show();
        $('#POAItemsBody').show();

    }

    $('#ProceedToStep2').click(function (e) {
        var countI = 0;
        var id = "";
        var selected = $("input[type='radio'][name='SelectedTaxPayer']:checked");
        if (selected.length > 0) {
            id = selected.val();
        }
        var rowID = "row" + id;
        var selectedRow = $('#'+rowID+ 'td:first-child');

        //var tableData = selectedRow.cells[];
        var selectedRIN = $('#' + rowID + ' td:first-child').text();
        var selectedTaxPayerName = $('#' + rowID + ' td:nth-child(2)').text();
        

        DisplayPayOnAccountStep2Body(selectedRIN, selectedTaxPayerName, id);
        $('.loader').hide();
        $('.step1').hide();
        $('#PayOnAccountItems').hide();
        $('.step2').show();
    });

    function DisplayPayOnAccountStep2Body(selectedRIN, selectedTaxPayerName, id) {

        var payOnAccountStep2Body = $('#step2_form');

        var taxPayerID = '<input size="20" type="hidden" id="TaxPayerID"  name="TaxPayerID" value="' + id + '">';
        //var taxPayerTypeID = '<input size="20" type="hidden" id="TaxPayerTypeID" name="TaxPayerTypeID" value="' + returnModel.TaxPayerTypeID + '">';
        var RINValue = '<input size="20" type="text" class="form-control"  id="" name="TaxPayerRIN" value="' + selectedRIN + '" readonly>';
        var taxPayerName = '<input class="form-control" size="20" type="text" id="" name="TaxPayerName" value="' + selectedTaxPayerName + '" readonly>';

        $('#TaxPayerName').append(taxPayerName);
        $('#RINValue').append(RINValue);
        payOnAccountStep2Body.append(taxPayerID);
       
    }


});



