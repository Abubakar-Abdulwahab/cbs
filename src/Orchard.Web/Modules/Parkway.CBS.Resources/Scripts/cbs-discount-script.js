$(document).ready(function () {
    var index = $("#Indexer").val();
    var indexPenalty = $("#IndexerPenalty").val();

    $(".adddiscount").click(function(){
        var formGroup = $('<div class="form-inline" id="discount_'+ index +'"></div> ').appendTo($("#discountDiv"));
        //create discount drop down
        var discountTypeSelectOption = $('<select required="True" name="DiscountModel[' + index + '].BillingDiscountType" class="form-control"><option value="">Select Discount Type</option><option value="Flat">Flat Rate</option><option value="Percent">% of assessment fee</option></select>').appendTo(formGroup);
        //create discount free form
        var discount = $('<input required="True" type="number" name="DiscountModel[' + index + '].Discount" placeholder="Discount" min="1" class="form-control"/> ').appendTo(formGroup);
        //effective for
        var numberOf = $('<small> Effective Till: </small><input required="True" name="DiscountModel[' + index + '].EffectiveFrom" type="number" min="1" placeholder="Days" class="form-control"/> ').appendTo(formGroup);
        //effective for type
        var effectiveForTypeSelectOption = $('<select required="True" name="DiscountModel[' + index + '].EffectiveFromType" class="form-control"><option value="">Select Range Type</option><option value="Days">Days</option><option value="Weeks">Weeks</option><option value="Months">Months</option><option value="Years">Years</option></select>').appendTo(formGroup);

        var removeLink = $('<a id=remove_' + index + ' href=# name="remove" > &nbsp;&nbsp;Remove</a>').appendTo(formGroup);
        removeLink.on("click", { name: index }, removeDiscount);
        //create unordered index hidden field
        var hiddenField = "<input type='hidden' name='DiscountModel.Index' value='"+ index++ +"' />";
        formGroup.after(hiddenField);
        return false;
    });

    function removeDiscount(event) {
        var divNumber = event.data.name;
        var discountRow = $("#discount_" + divNumber);
        discountRow.remove();
        event.preventDefault();
    }

    $('.remove').click(function (event) {
        var arr = $(this).attr("id").split('_');
        var rowNumber = arr[1];
        var row = $("#discount_" + rowNumber);
        //remove row
        row.remove();
        event.preventDefault();
    });
    
    $(".addpenalty").click(function(){
        var penaltyFormGroup = $('<div class="form-inline" id="penalty_'+ indexPenalty +'"></div> ').appendTo($("#penaltyDiv"));
        //create discount drop down
        var penaltyValueTypeSelectOption = $('<div class="form-group"><select required="True" name="PenaltyModel[' + indexPenalty + '].PenaltyValueType" class="form-control"><option value="">Select Penalty Type</option><option value="FlatRate">Flat Rate</option><option value="Percentage">% of assessment fee</option></select></div>').appendTo(penaltyFormGroup);
        //create discount free form
        var value = $('<div class="form-group"><input required="True" type="number" name="PenaltyModel[' + indexPenalty + '].Value" placeholder="Value" min="1" class="form-control"/></div>').appendTo(penaltyFormGroup);
        //effective for
        var numberOf = $('<small> Effective From: </small> <div class="form-group"> <input required="True" name="PenaltyModel[' + indexPenalty + '].EffectiveFrom" type="number" min="1" placeholder="Days" class="form-control"/> </div>').appendTo(penaltyFormGroup);
        //effective for type
        var effectiveForTypeSelectOption = $('<div class="form-group"> <select required="True" name="PenaltyModel[' + indexPenalty + '].EffectiveFromType" class="form-control"><option value="">Select Range Type</option><option value="Days">Days</option><option value="Weeks">Weeks</option><option value="Months">Months</option><option value="Years">Years</option></select> </div>').appendTo(penaltyFormGroup);

        var removeLink = $('<a id=removepen_' + indexPenalty + ' href=# name="removepen"> &nbsp;&nbsp;Remove</a>').appendTo(penaltyFormGroup);
        removeLink.on("click", { name: indexPenalty }, removePenalty);
        //create unordered index hidden field
        var hiddenField = "<input type='hidden' name='PenaltyModel.Index' value='"+ indexPenalty++ +"' />";
        penaltyFormGroup.after(hiddenField);
        return false;
    });

    function removePenalty(event) {
        var divNumber = event.data.name;
        var discountRow = $("#penalty_" + divNumber);
        discountRow.remove();
        event.preventDefault();
    }
    
    $('.removepen').click(function (event) {
        var arr = $(this).attr("id").split('_');
        var rowNumber = arr[1];
        var row = $("#penalty_" + rowNumber);
        //remove row
        row.remove();
        event.preventDefault();
    });

});
