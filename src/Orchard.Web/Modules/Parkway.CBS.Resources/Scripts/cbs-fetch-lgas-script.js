$(document).ready(function () {

    $('#selectedState').change(function () {
            var stateId = $('#selectedState').val();
            clearLgaList();
            loadLga(stateId);
    });  

    function clearLgaList() {
        $("#lgaslist").empty();        
    }

    function buildDataList(lga) {
        for (var val in lga) {
            if(lgaSelectedPrev !== 0 && lgaSelectedPrev === lga[val].Id){
                $('<option value="' + lga[val].Id + '" selected>' + lga[val].Name + '</option>').appendTo($("#lgaslist"));
            }else{
                $('<option value="' + lga[val].Id + '">' + lga[val].Name + '</option>').appendTo($("#lgaslist"));
            }
        }
    }

    function showProfileLoader(lgaLoaded) {
        if (lgaLoaded) {
            //show profile loader
            $("#profileloader").hide();
        } else {
            //show profile loader
            $("#profileloader").show();
        }
    }

//Loads the LGAs
    function loadLga(stateId){
        if (lgaObj[stateId] !== undefined) {
                buildDataList(lgaObj[stateId].Lgas);
            } else {
                var url = "x/get-lgas-bystates";
                var requestData = { "stateId": stateId };
                //show spinner
                showProfileLoader(false);
                
                $.post(url, requestData, function (data) {
                    if (!data.Error) {
                        //show the data list and bind the recods
                        lgaObj[stateId] = {};
                        lgaObj[stateId].Lgas = data.ResponseObject;
                        buildDataList(lgaObj[stateId].Lgas);
                    } else {
                        var errmsg = data.ResponseObject;
                        $("#errorFlash").show();
                        $("#errorMsg").html(errmsg);
                        $("#loader").hide();
                        
                    }
                }).always(function () {
                    showProfileLoader(true);
                });
            }

    }
    
    //check if the page is being reloaded with a pre-selected state
    if(loadedBefore){
        var stateId = $('#selectedState').val();
        clearLgaList();
        loadLga(stateId);
    }
    
});