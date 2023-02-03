$(document).ready(function () {
    var secret = $("#clientsecret");
    var button = $("#showsecret");
    var token = $("input[name=__RequestVerificationToken]").val();

    $('#showsecret').click(function () {
        if (secret.is(':hidden')) {
            button.html("Hide Secret");
            secret.show();
        } else {
            button.html("Show Client Secret");
            secret.hide();
        }
    });

    $('.showsecretbtn').click(function (event) {
        var arr = $(this).attr("id").split('_');
        var clientId = $('.client_' + arr[1]).html().trim();
        $("#loader_"+arr[1]).show()
        var url = 'Settings/GetClientSecret';
        var requestData = { "clientId": clientId, "__RequestVerificationToken": token };
        $.post(url, requestData, function (data) {
            if (data.length > 0) {
                $("#clientsecret_" + arr[1]).html("<b>"+data+"<b>");
                $("#clientsecret_" + arr[1]).show();
                $("#showsecretbtn_" + arr[1]).hide();
                $("#loader_" + arr[1]).hide();
            } else {
            }
        });
    });

    $('#stateName').change(function () {
        var selectedValue = $("#stateName option:selected").text();
        //$('.refData')[0].options.length = 0;
        $('.refData').html('');
        var selectElement = $('.refData');
        $('#loader').show();
        $('<option value="" disabled="" selected="">Select Ref Data <div name="loader" id="loader" class="loader" style="margin:0 auto;"></div></option>').appendTo(selectElement);
        //modelSelect.html("<option> loading ... </option>");
        var url = 'GetRegsiteredRefDataItems';
        var requestData = { "stateName": selectedValue, "__RequestVerificationToken": token };
        $.post(url, requestData, function (data) {
            
            if (data.length > 0) {
                for (var val in data) {
                    $('<option />', { value: data[val], text: data[val] }).appendTo(selectElement);
                }
            } else {
            }
            $('#loader').hide();
        });
    });
});