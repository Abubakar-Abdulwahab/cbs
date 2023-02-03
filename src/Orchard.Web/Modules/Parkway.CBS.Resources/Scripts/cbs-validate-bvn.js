$(function(){
var bvn = "";

if($("#fname").val() != ""){ $(".bvn-input-field-container").hide(); $(".complete-reg-form-container").show(); return; }

$("#submitBtn").click(function(){
    disableElements();
    bvn = $("#bvnField").val();
    if(bvn.length == 11){
        checkIfBvnExists(bvn);
    }else{ $(".bvn-input-field-container .error-text").show(); 
           $(".bvn-input-field-container .error-text").html("Bvn must be 11 digits"); 
            enableElements();
            return; }

});

function disableElements(){
    $("#bvnField").prop("disabled",true);
    $("#submitBtn").prop("disabled",true);
    $(".bvn-input-field-container .error-text").hide();
    $(".profileloader").show();
    $('#infoFlashMsg').hide();
    $('#ErrorFlash').hide();
}

function enableElements(){
    $("#bvnField").prop("disabled",false);
    $("#submitBtn").prop("disabled",false);
    $(".profileloader").hide();
}

function validateBvn(bvn,token){
    //var token = $("input[name=__RequestVerificationToken]").val();
    let url = "https://app.bank3d.ng/Vaps.API/api/v2/utility/verifyBvn";
    var requestData = {"__RequestVerificationToken": token, "bvn":bvn };
    const req = $.post(url,requestData, function(response){
        //let response = xhr.responseJSON;
            $(".bvn-input-field-container").fadeOut("fast","linear",function(){
                $(".complete-reg-form-container").fadeIn("fast","linear");
                $("#fname").val(response.responseObject.firstName+" "+response.responseObject.middleName+" "+response.responseObject.lastName);
                $("#phoneNumber").val(response.responseObject.phoneNumber);
                $("#bvn").val(bvn);
                });
    });

    $(document).ajaxError(function(event,xhr,options){
        enableElements();
            $(".bvn-input-field-container .error-text").show(); 
            $(".bvn-input-field-container .error-text").html(xhr.responseJSON.message); 
    });
}

function checkIfBvnExists(bvn){
    var token = $("input[name=__RequestVerificationToken]").val();
    var bvnReqData = {"__RequestVerificationToken": token, "bvn":bvn };
    let url = "/c/x/validate-bvn";
    const bvnReq = $.post(url,bvnReqData,function(response){
        if(response.ResponseObject.Registered){
             enableElements();
            $(".bvn-input-field-container .error-text").show(); 
            $(".bvn-input-field-container .error-text").html(response.ResponseObject.Message);
        }else{
            validateBvn(bvn,token);
        }
    });
    
}
});