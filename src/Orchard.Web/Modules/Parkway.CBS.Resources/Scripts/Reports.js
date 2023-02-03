$(document).ready(function () {
    $("#StartDate").datetimepicker({
        format: "dd MM yyyy",
        autoclose: true,
        todayBtn: true,
        pickerPosition: "bottom-left",
        minView: "2"
    });
    $("#EndDate").datetimepicker({
        format: "dd MM yyyy",
        autoclose: true,
        todayBtn: true,
        pickerPosition: "bottom-left",
        minView: "2"
    });
});