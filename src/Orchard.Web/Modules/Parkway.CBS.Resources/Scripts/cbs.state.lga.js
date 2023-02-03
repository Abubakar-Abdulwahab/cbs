$(document).ready(function () {

    $('#state').change(function () {
        clearLgaList();
        buildLgaDropDown($('#state').val());
    });

    $('.state').change(function (e) {
        $(e.currentTarget.parentElement.nextElementSibling).children("select").empty();
        $(e.currentTarget.parentElement.nextElementSibling).children("select").append('<option value="">Select an LGA</option>');
        let options = "";
        $(stateLGAMap.get(parseInt($(this).val()))).each(function () {
            options += '<option value="' + this.Id + '">' + this.Name + '</option>';
        });
        $(e.currentTarget.parentElement.nextElementSibling).children("select").append(options);
    });

    function clearLgaList() {
        $("#lga").empty();
    }

    function buildLgaDropDown(stateId) {
        var options = "";
        if (stateId === "0") { options = '<option value="">All LGAs</option>'; }
        else
        {
            if (typeof isAdmin !== 'undefined' && isAdmin) {
                options = '<option value="">All LGAs</option>';
            } else {
                options = '<option value="">Select an LGA</option>';
            }
        }
        $(stateLGAMap.get(parseInt(stateId))).each(function () {
            options += '<option value="' + this.Id + '">' + this.Name + '</option>';
        });
        $("#lga").append(options);
    }
});