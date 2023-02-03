$(document).ready(function () {
    var token = $("input[name=__RequestVerificationToken]").val();

    $('.showsecretbtn').click(function (event) {
        var arr = $(this).attr("id").split('_');
        var clientId = $('.client_' + arr[1]).html().trim();
        $("#loader_" + arr[1]).show()
        var url = 'Settings/GetClientSecret';
        var requestData = { "clientId": clientId, "__RequestVerificationToken": token };
        $.post(url, requestData, function (data) {
            if (!data.Error) {
                $("#clientsecret_" + arr[1]).html("<b>" + data.ResponseObject + "<b>");
                $("#clientsecret_" + arr[1]).show();
                $("#showsecretbtn_" + arr[1]).hide();
                $("#loader_" + arr[1]).hide();
            } else {
            }
        });
    });

});