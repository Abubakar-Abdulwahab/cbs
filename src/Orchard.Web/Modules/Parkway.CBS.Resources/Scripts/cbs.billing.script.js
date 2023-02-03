$(document).ready(function () {
    var dayValue;
    var endsValue;
    var constant;
    var weekValue;
    var monthValue;
    var yearMonthValue;
    var yearDateValue;
    //Tooltip Functions
    $(".amount").tooltip({ 'trigger': 'hover', 'title': 'Amount Payable' });

    
    //Focusin Event
    $(".amount").focusin(function () {
        $(this).val("");
    })
    $(".amount").keyup(function (event) {

        if (event.which >= 37 && event.which <= 40) return;
        //format number
        $(this).val(function (index, value) {
            return value.replace(/[^\.0-9]/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ",");
        })
    });
    $(".is_recurring").tooltip({ 'trigger': 'hover', 'title': 'You need to check this to enable the billing frequency controls' });

    $('#Billing_Frequency').on('change', function () {
        var selectedValue = $(this).val();
        //console.log("Selected value is ", selectedValue);
        if (selectedValue != "") {
            switch (Number(selectedValue)) {
                case 0:
                    $('.days').show();
                    $('.weeks').hide();
                    $('.months').hide();
                    $('.years').hide();
                    /*$('.billing-start .monthly').hide();
					$('.billing-start .weekly').hide();
					$('.billing-start .yearly').hide();*/
                    //$('.interval').text('Day');
                    $(".day").change(function () { console.log($(this).val()); dayValue = $(this).val(); });
                    $("#summary").text("Daily").css("display", "block");

                    break;
                case 1:
                    $('.weeks').show();
                    $('.days').hide();
                    $('.months').hide();
                    $('.years').hide();
                    $('p.first').text('');
                    $('p.yeardate').text('');
                    $('p.second').text('');
                    /*$('.billing-start .weekly').show();
					$('.billing-start .monthly').hide();
					$('.billing-start .yearly').hide();
					$('.interval').text('Week');*/

                    break;
                case 2:
                    $('.months').show();
                    $('.weeks').hide();
                    $('.days').hide();
                    $('.years').hide();
                    $('p.first').text('');
                    $('p.yeardate').text('');
                    $('p.second').text('');
                    /*$('.billing-start .monthly').show();
					$('.billing-start .weekly').hide();
					$('.billing-start .yearly').hide();
					$('.interval').text('Month');*/
                    break;
                case 3:
                    $('.years').show();
                    $('.months').hide();
                    $('.weeks').hide();
                    $('.days').hide();
                    $('p.first').text('');
                    $('p.yeardate').text('');
                    $('p.second').text('');
                    /* $('.billing-start .yearly').show();
                     $('.interval').text('Year');*/

                    break;
                default:
                    $('.billing-start .monthly').hide();
                    $('.billing-start .weekly').hide();
                    $('.billing-start .yearly').hide();
                    break;
            }
        } else {
            $('.billing-start .monthly').hide();
            $('.billing-start .weekly').hide();
            $('.billing-start .yearly').hide();
        }

    });

    $('.shouldEnd').change(function () {
        var selectedValue = $(this).val();
        if (selectedValue != "") {
            switch (Number(selectedValue)) {
                case 0:
                    $('.billing-end .after').attr('diabled', 'diabled');
                    $('.billing-end .on').attr('diabled', 'diabled');
                    break;
                case 1:
                    $('.billing-end .after').attr('diabled', '');;
                    $('.billing-end .on').attr('diabled', 'diabled');;
                    break;
                case 2:
                    $('.billing-end .on').attr('diabled', '');;
                    $('.billing-end .after').attr('diabled', 'diabled');
                    break;
                default:

            }
        }
    });

    $('#after').focusout(function () {
        if ($(this).val() < 0) {
            $(this).val('0');
            $("p.second").text("");
        } else {
            constant = ", " + $(this).val() + " times";
            $("p.second").text(constant);
        }
    });

    $("#datepicker3").datepicker().on('changeDate', function (ev) {
        $(".after").text("");
        $("p.second").text(", until " + $(this).val());

    });
    
    $("#datepicker4").datepicker().on('changeDate', function (ev) {
        $(".after").text("");
        $("p.second").text(", until " + $(this).val());

    });

    $(".day").change(function () {
        $("p.yeardate").text("");

        dayValue = $(this).val();
        if (dayValue > 1) {
            $("p.first").text("Every " + dayValue + " days ");

        } else {

            $("p.first").text("Daily ");
        }
    });

    $("input:radio[id=duration]").change(function () {
        if (this.value == "EndsAfter") {
            $("#after").prop("disabled", false);
            $(".ends-on").prop("disabled", true);
        } else if (this.value == "NeverEnds") {
            $(".ends-on").text("");
            $("#after").val("");
            $(".ends-on").prop("disabled", true);
            $("#after").prop("disabled", true);
            $("#summary").append(endsValue);
        } else if (this.value == "EndsOn") {
            $("#after").val("");
            $("#after").prop("disabled", true);
            $(".ends-on").prop("disabled", false);
        }
    });

    $(".week").change(function () {
        $("p.yeardate").text("");
        weekValue = $(this).val();


        $("p.first").text("Weekly on " + weekValue);
        //debugger;
    });
    $(".yearweek").change(function () {

        $("p.yeardate").text($(this).val());
    });
    var dayzValue;
    //$("#sundayWeek").change(function(){
    $("#sundayWeek,#sundayMonth,#sundayYear").change(function () {
        if ($(this).is(':checked')) {
            $("p.first").append($(this).val());

        } else {
            //console.log($("p.first").text().replace(new RegExp('Sunday'),""));
            var answer = $("p.first").text().replace(new RegExp(' Sunday'), "");
            $("p.first").text(answer);
            //console.log($("p.first").text());
            debugger;
        }
    });
    $("#mondayWeek,#mondayMonth,#mondayYear").change(function () {
        if ($(this).is(':checked')) {
            $("p.first").append($(this).val());
            debugger;

        } else {

            var answer = $("p.first").text().replace(new RegExp(' Monday'), "");
            $("p.first").text(answer);
        }
    });
    $("#tuesdayWeek,#tuesdayMonth,#tuesdayYear").change(function () {
        if ($(this).is(':checked')) {
            $("p.first").append($(this).val());
            debugger;

        } else {

            var answer = $("p.first").text().replace(new RegExp(' Tuesday'), "");
            $("p.first").text(answer);
        }
    });
    $("#wednesdayWeek,#wednesdayMonth,#wednesdayYear").change(function () {
        if ($(this).is(':checked')) {
            $("p.first").append($(this).val());
            debugger;

        } else {

            var answer = $("p.first").text().replace(new RegExp(' Wednesday'), "");
            $("p.first").text(answer);
        }
    });
    $("#thursdayWeek,#thursdayMonth,#thursdayYear").change(function () {
        if ($(this).is(':checked')) {
            $("p.first").append($(this).val());
            debugger;

        } else {

            var answer = $("p.first").text().replace(new RegExp(' Thursday'), "");
            $("p.first").text(answer);
        }
    });
    $("#fridayWeek,#fridayMonth,#fridayYear").change(function () {
        if ($(this).is(':checked')) {
            $("p.first").append($(this).val());
            debugger;

        } else {

            var answer = $("p.first").text().replace(new RegExp(' Friday'), "");
            $("p.first").text(answer);
        }
    });
    $("#saturdayWeek,#saturdayMonth,#saturdayYear").change(function () {
        if ($(this).is(':checked')) {
            $("p.first").append($(this).val());
            debugger;

        } else {

            var answer = $("p.first").text().replace(new RegExp(' Saturday'), "");
            $("p.first").text(answer);
        }
    });


    $(".month").change(function () {
        $("p.yeardate").text("");
        monthValue = $(this).val();
        $("p.first").text("Every " + monthValue + "month(s) ,");
    });

    $(".yearmonth").change(function () {
        yearMonthValue = $(this).val();
        $("p.first").text("Anually on ," + yearMonthValue + " ");
    });
    $(".yearday").change(function () {
        $("p.first").text("");
        yearDateValue = $(this).val();
        $("p.yeardate").text(yearDateValue);
    });
    $(".monthWeek").change(function () {
        $("p.first").append($(this).val() + " of the month");
        /*console.log('Trigered ',$(this).val());
        debugger;*/
    });

});