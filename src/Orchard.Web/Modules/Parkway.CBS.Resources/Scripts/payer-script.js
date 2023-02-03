$(document).ready(function () {

    $("#mda_selected").on("change", function () {
        if ($("#mda_selected :selected").val() != "") {
            var url = 'RevenueHeads/Tax';
            var token = $("input[name=__RequestVerificationToken]").val();
            var requestData = { "mdaSlug": $("#mda_selected :selected").val(), "__RequestVerificationToken": token };
            $.post(url, requestData, function (data) {
                if (data.length > 0) {
                    var options = '';
                    //console.log(data);
                    for (i = 0; i < data.length; i++) {
                        options += '<option value="' + data[i].Id + '">' + data[i].Name + '</option>';
                    }
                    $("#revenuehead_selected").empty().append(options);
                } else {
                    $("#revenuehead_selected").empty().append('<option>No Revenue Heads Found</option>');
                }
            });
        } else {
            $("#revenuehead_selected").empty().append('<option>Select Revenue Head</option>');
        }
    });
});