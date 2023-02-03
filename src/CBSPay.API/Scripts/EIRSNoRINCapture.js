jQuery(document).ready(function ($) {

    $("#TaxPayerTypeSelect").change(function () {
        
        var taxPayerType = $("#TaxPayerTypeSelect option:selected").text();
        if (taxPayerType == "") {
            alert("Please select appropriate tax payer type");
        }

        $.ajax({
            type: 'GET',
            url: "/home/FetchEconomicActivities/?taxPayerTypeName="+taxPayerType,
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            //data: {
            //    taxPayerTypeName: taxPayerType
            //},
            success: function (returnModel) {
                console.log(returnModel);
                if (returnModel != null) {
                    //var thisModel = JSON.parse(returnModel);
                    var thisModel = returnModel;
                    if (thisModel.Status == "Success") {
                        populateDropDown(thisModel);
                    }
                    else {
                        alert('Failed to fetch economic activities for this taxpayer type.');
                        console.log('Failed to retrieve Data.');
                    }
                }

                else {
                    alert('An error occurred, Failed to fetch economic activities for this taxpayer type.');

                }
            },
            error: function (ex) {
                alert('Failed to fetch economic activities for this taxpayer type.');
            }
        });
    });

    function populateDropDown(returnModel) {

        var items = returnModel.EconomicActivities;

        $('#ecoActivity').children('option:not(:first)').remove();
        items.forEach(function (refItem)
        {
            $('<option value="' + refItem.EconomicActivitiesName + '">' + refItem.EconomicActivitiesName + '</option>').appendTo('#ecoActivity');
        });
        
        
    };
});