$(document).ready(function () {

    $('#state').change(function () {
        clearLgaList();
        buildLgaDropDown($('#state').val());
    });

    function clearLgaList() {
        $("#lga").empty();
    }

    function buildLgaDropDown(statename) {
        var options = "";
        if (statename.toLowerCase() === "all") { options = '<option value="all">All LGAs</option>'; }
        else {
            if (typeof isAdmin !== 'undefined' && isAdmin) {
                options = '<option value="0">All LGAs</option>';
            } else {
                options = '<option value="0">Select an LGA</option>';
            }
        }
        $(stateLGAMap.get(statename)).each(function () {
            options += '<option value="' + this.Name + '">' + this.Name + '</option>';
        });
        $("#lga").append(options);
    }


});