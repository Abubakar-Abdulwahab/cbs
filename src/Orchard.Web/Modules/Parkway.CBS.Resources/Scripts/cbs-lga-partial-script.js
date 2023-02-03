$(document).ready(function () {
    console.log();
    if ($("#state").val() != null) {
        console.log($("#state").val());
        changeLgas($("#state").val());
    }
    $("#state").change(function (e) {
        changeLgas(e.currentTarget.value.toString());
    });

    function changeLgas(statename) {
        let selectedStateLgas = lgaMap.get(statename);
        $("#lga").empty();
        for (let i = 0; i < selectedStateLgas.length; ++i) {
            if (selectedLga == selectedStateLgas[i].Name) { $("#lga").append("<option value='" + selectedStateLgas[i].Name + "' selected>" + selectedStateLgas[i].Name + "</option>"); }
            else { $("#lga").append("<option value='" + selectedStateLgas[i].Name + "'>" + selectedStateLgas[i].Name + "</option>"); }
            
        }
    }
});
