$(document).ready(function(){

var formPage = 1;
$("#backBtn").prop("disabled",true);
makeBasicInfoRequired(true);
makeContactInfoRequired(false);
makeLoginInfoRequired(false);


$("#regBzform").submit(function(e){
    e.preventDefault();
    if(formPage === 1){
        $("#basicInfoContainer").fadeOut("fast","linear",function(){
            $("#backBtn").prop("disabled",false);
            $(window).scrollTop(0);
            $("#contactInfoContainer").fadeIn("fast", "linear", function(){
                formPage = 2;
                makeBasicInfoRequired(false);
                makeContactInfoRequired(true);
                makeLoginInfoRequired(false);
            });
        });
    }else if(formPage === 2){
        $("#contactInfoContainer").fadeOut("fast","linear",function(){
            $("#submitBtn").html("Submit");
            $(window).scrollTop(0);
            $("#loginInfoContainer").fadeIn("fast", "linear", function(){
                formPage = 3;
                makeBasicInfoRequired(false);
                makeContactInfoRequired(false);
                makeLoginInfoRequired(true);
            });
        });
    }else if(formPage === 3){
        $("#regBzform").off("submit");
        $("#regBzform").submit();
    }
});


$("#backBtn").click(function(){
    if(formPage === 3){
        $("#loginInfoContainer").fadeOut("fast","linear",function(){
            $("#submitBtn").html("Next");
            $(window).scrollTop(0);
            $("#contactInfoContainer").fadeIn("fast","linear",function(){
                formPage = 2;
                makeBasicInfoRequired(false);
                makeContactInfoRequired(true);
                makeLoginInfoRequired(false);
            });
        });
    }else if(formPage === 2){
        $("#contactInfoContainer").fadeOut("fast","linear",function(){
            $("#backBtn").prop("disabled",true);
            $(window).scrollTop(0);
            $("#basicInfoContainer").fadeIn("fast","linear",function(){
                formPage = 1;
                makeBasicInfoRequired(true);
                makeContactInfoRequired(false);
                makeLoginInfoRequired(false);
            });
        });
    }
});



function makeBasicInfoRequired(val){
    $("#bzName").attr("required",val);
    $("#compNumber").attr("required",val);
    $("#bzType").attr("required",val);
    $("#rcNumber").attr("required",val);
    $("#state").attr("required",val);
    $("#lga").attr("required",val);
    $("#offAddress").attr("required",val);
}

function makeContactInfoRequired(val){
    $("#contactName").attr("required",val);
    $("#contactPhoneN").attr("required",val);
}

function makeLoginInfoRequired(val){
    $("#UserName").attr("required",val);
    $("#Password").attr("required",val);
    $("#ConfirmPassword").attr("required",val);
}    



});