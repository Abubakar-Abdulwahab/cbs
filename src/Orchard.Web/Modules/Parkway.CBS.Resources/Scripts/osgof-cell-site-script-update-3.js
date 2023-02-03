$(document).ready(function () {
    var rowNumber = 0;
    var stateKey = -1;
    var lgaKey = -1;
    var cellSiteObj = null;

    $('#state').on('change', function () {
        cellSiteObj = null;
        lgaKey = -1;
        //empty LGAs and cell sites drop down
        $("#LGA").empty();
        $('<option value="" disabled selected>Select an LGA</option>').appendTo('#LGA');

        $("#cellsite").empty();
        $('<option value="" disabled selected>Select Operator Site Id</option>').appendTo('#cellsite');

        loadLGAs($('#state').val());
    });


    $('#LGA').on('change', function () {
        cellSiteObj = null;
        lgaKey = -1;
        //empty cell sites drop down
        $("#cellsite").empty();
        $('<option value="" disabled selected>Select Operator Site Id</option>').appendTo('#cellsite');

        loadCellSites($('#LGA').val());
    });


    $('#cellsite').on('change', function () {
        //empty cell sites drop down
        var cellSiteId = $('#cellsite').val();
        cellSiteObj = null;

        loadCellSiteDetails(cellSiteId);
    });


    function loadLGAs(stateId) {
        for (var key in cellSitesModel.ListOfStates) {
            if (cellSitesModel.ListOfStates[key].Id == stateId) {
                $(cellSitesModel.ListOfStates[key].ListOfLGAs).each(function () {
                    $('<option value="' + this.Id + '">' + this.Name + '</option>').appendTo('#LGA');
                });
                stateKey = key;
                break;
            }
        }
    }


    function loadCellSites(lgaId) {
        $.ajax({
            url: 'x/get-operator-cellsites',
            method: 'get',
            data: { operatorLGAId: lgaId, operatorTaxEntityId: $('#operatorTaxEntityId').val() },
            contentType: 'application/json;charset=utf-8',
            dataType: "json",
            success: function (response) {
                $.each(response.ResponseObject, function () {
                    $('<option value="' + this.Id + '">' + this.OperatorSiteId + '</option>').appendTo('#cellsite');
                });
            },
            error: function (response) {
                alert("An error occurred" + response.statusText);
            }
        });
    }

    function loadCellSiteDetails(cellSiteId) {
        $.ajax({
            url: 'x/get-cellsite/',
            method: 'get',
            data: { cellSiteId: cellSiteId },
            contentType: 'application/json;charset=utf-8',
            dataType: "json",
            success: function (response) {
                $("#categoryAmount").val(response.ResponseObject.Amount); 
                $("#latitude").val(response.ResponseObject.Lat); 
                $("#longitude").val(response.ResponseObject.Long); 
                $("#siteAddress").val(response.ResponseObject.SiteAddress); 
                $("#OSGOFID").val(response.ResponseObject.OSGOFID); 
            },
            error: function (response) {
                alert("An error occurred" + response.statusText);
            }
        });
    }


    $("#lineForm").on('submit', function (e) {
        e.preventDefault();
        var form1 = document.forms['lineForm'];
        if (!form1.checkValidity()) {
            return false;
        } else {
            insertRow();
            $('#lineForm')[0].reset();

            $("#cellsite").empty();
            $('<option value="" disabled selected>Select a cell site</option>').appendTo('#cellsite');

            $("#LGA").empty();
            $('<option value="" disabled selected>Select an LGA</option>').appendTo('#LGA');
        }
    });

    $("#dataForm").on('submit', function (e) {
        $("#dataFormSubmitBtn").prop("disabled", true);
    });

    $("#fileForm").on('submit', function (e) {
        $("#fileUploadbtn").prop("disabled", true);
    });


    $("input:file").change(function () {

        var fileName = $(this).val();
        if (fileName.length > 0) {
            $("#uploadlbl").css({ paddingRight: "0px" });
            $("#uploadlbl").removeClass('uploadlbl');
            //truncate filename
            var n = fileName.lastIndexOf('\\');
            if (n < 0) { n = fileName.lastIndexOf('/'); }
            var str = fileName.substring(n + 1, fileName.length);
            $("#fileName").html(str);
            $("#uploadInfo").html("Change employee schedule file here.");
            $("#uploadImg").hide();
            $("#fileUploadbtn").prop("disabled", false);
        }
    });

    function insertRow() {
        var tbody = $("#tbody");
        var tr = $('<tr />').appendTo(tbody);
        //CellSites[0].State | CellSites[0].LGA
        var td1 = $('<td>' + $("#state :selected").text() + " | " + $("#LGA :selected").text() + '</td>').appendTo(tr);
        //
        var tdInput1 = $('<input hidden name="CellSites[' + rowNumber + '].CellSiteId" value="' + $("#cellsite :selected").text() + '"/>').appendTo(td1);
        var tdInput2 = $('<input hidden name="CellSites[' + rowNumber + '].Year" value="' + $("#year :selected").val() + '"/>').appendTo(td1);

        //CellSites[0].Site
        var td4 = $('<td>' + $("#cellsite :selected").text() + '</td>').appendTo(tr);

        var td3 = $('<td>₦' + $("#categoryAmount").val() + '</td>').appendTo(tr);

        var td5 = $('<td>' + $("#year :selected").text() + '</td>').appendTo(tr);

        var td6 = $('<td>' + $("#longitude").val() + '</td>').appendTo(tr);
        var td8 = $('<td>' + $("#latitude").val() + '</td>').appendTo(tr);
        var td7 = $('<td>' + $("#siteAddress").val() + '</td>').appendTo(tr);

        var td2 = $('<td>' + $("#OSGOFID").val() + '</td>').appendTo(tr);

        rowNumber++;
        $("#dataFormSubmitBtn").prop("disabled", false);
    }


});