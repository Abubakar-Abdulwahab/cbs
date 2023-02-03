
$(document).ready(function () {
    //
    // $("#chkMenuSwitch").click(function () {
    //     if (this.checked == true) {
    //         window.location.href = "/Login/Dashboard";
    //     } else {
    //         window.location.href = "/Login/Index";
    //     }
    // });

    //$(document).on("shown.bs.dropdown", ".btn-group", function () {
    //    var table = $(this).closest("table");

    //    if (table.length > 0) {
    //        var $ul = $(this).children(".dropdown-menu");
    //        var $button = $(this).children(".dropdown-toggle");

    //        var ulOffset = $ul.offset();
    //        var tableOffset = table.offset();

    //        var ulTableTopSpace = ulOffset.top - tableOffset.top;
    //        var spaceUpTable = ulTableTopSpace - $ul.outerHeight() - $button.outerHeight();
    //        var spaceDownTable = table.height() - ulTableTopSpace - $ul.outerHeight();

    //        var spaceUpWindow = (ulOffset.top - $button.outerHeight() - $ul.outerHeight()) - $(window).scrollTop();
    //        var spaceDownWindow = $(window).scrollTop() + $(window).height() - (ulOffset.top + $ul.height());

    //        if (spaceDownWindow < 0 && (spaceUpWindow >= 0 || spaceUpWindow > spaceDownWindow)) {
    //            $(this).addClass("dropup");
    //        }
    //        if (spaceDownTable < 0 && (spaceUpTable >= 0 || spaceUpTable > spaceDownTable)) {
    //            $(this).addClass("dropup");
    //        }
    //    }
    //}).on("hidden.bs.dropdown", ".btn-group", function () {
    //    $(this).removeClass("dropup");
    //});


    $('.bs-select').selectpicker();

    $('.dropdown-menu li').on('click', function (event) {
        var $checkbox = $(this).find('.checkbox');
        if (!$checkbox.length) {
            return;
        }
        var $input = $checkbox.find('input');
        var $icon = $checkbox.find('span.fa');
        if ($input.is(':checked')) {
            $input.prop('checked', false);
            $icon.removeClass('fa-check-square-o').addClass('fa-square-o')
        } else {
            $input.prop('checked', true);
            $icon.removeClass('fa-square-o').addClass('fa-check-square-o')
        }
        return false;
    });
})


function jsfn_EmailCheck(strEmail) {
    if ((/^[A-Z0-9._%+-]+@(?:[A-Z0-9-]+\.)+[A-Z]{2,6}$/i).test(strEmail)) { return true; } else { return false; }
}

function jsfn_MobileCheck(strMobile) {
    var mob = /^[1-9]{1}[0-9]{9}$/;
    return mob.test(strMobile);
}

function jsfn_Trim(str) {
    while (str.charAt(0) == (" ")) {
        str = str.substring(1);
    }
    while (str.charAt(str.length - 1) == " ") {
        str = str.substring(0, str.length - 1);
    }

    return str;
}

function jsfn_ajaxPost(p_url, p_urlParameter, p_ResponseFunction) {
    $.ajax({
        url: p_url,
        type: 'POST',
        dataType: 'json',
        data: p_urlParameter,
        success: p_ResponseFunction,
        error: function (jqXHR, exception) {
            if (jqXHR.status == 401) {
                window.location.href = '/Login.aspx';
            }
        }
    });
}

function jsfn_ajaxPost_Html(p_url, p_urlParameter, p_ResponseFunction) {
    $.ajax({
        url: p_url,
        type: 'POST',
        dataType: 'html',
        data: p_urlParameter,
        success: p_ResponseFunction,
        error: function (jqXHR, exception) {
            if (jqXHR.status == 401) {
                window.location.href = '/Login.aspx';
            }
        }
    });
}

function jsfn_ConvertToDatable(p_Table, p_ColumnsList, p_EmptyTableMsg) {

    //$.extend(true, $.fn.DataTable.TableTools.classes, {
    //    "container": "btn-group tabletools-btn-group pull-right",
    //    "buttons": {
    //        "normal": "btn btn-sm default",
    //        "disabled": "btn btn-sm default disabled"
    //    }
    //});

    p_Table.dataTable({

        // Internationalisation. For more info refer to http://datatables.net/manual/i18n
        "language": {
            "aria": {
                "sortAscending": ": activate to sort column ascending",
                "sortDescending": ": activate to sort column descending"
            },
            "emptyTable": "No data available in table",
            "info": "Showing _START_ to _END_ of _TOTAL_ entries",
            "infoEmpty": "No entries found",
            "infoFiltered": "(filtered1 from _MAX_ total entries)",
            "lengthMenu": "Show _MENU_ entries",
            "search": "Search:",
            "zeroRecords": "No matching records found"
        },

        "dom": "<'row' <'col-md-12'T>><'row'<'col-md-6 col-sm-12'l><'col-md-6 col-sm-12'f>r><'table-scrollable't><'row'<'col-md-5 col-sm-12'i><'col-md-7 col-sm-12'p>>", // horizobtal scrollable datatable

        "bStateSave": false, // save datatable state(pagination, sort, etc) in cookie.

        "columns": p_ColumnsList,
        "lengthMenu": [
            [10, 20, 30, 40, 50, -1],
            [10, 20, 30, 40, 50, "All"] // change per page values here
        ],
        // set the initial value
        "pageLength": 10,
        "pagingType": "bootstrap_full_number",
        "language": {
            "search": "Search : ",
            "emptyTable": p_EmptyTableMsg,
            "lengthMenu": "  _MENU_ records",
            "paginate": {
                "previous": "Prev",
                "next": "Next",
                "last": "Last",
                "first": "First"
            }
        },
        "columnDefs": [{  // set default column settings
            'orderable': false,
            'targets': [0]
        }, {
            "searchable": false,
            "targets": [0]
        }],
        "order": [
            [1, "asc"]
        ], // set first column as a default sort by asc
        "tableTools": {
            "sSwfPath": "../../Content/Styles/plugins/datatables/extensions/TableTools/swf/copy_csv_xls_pdf.swf",
            "aButtons": [{
                "sExtends": "pdf",
                "sButtonText": "PDF"
            }, {
                "sExtends": "csv",
                "sButtonText": "CSV"
            }, {
                "sExtends": "xls",
                "sButtonText": "Excel"
            }, {
                "sExtends": "print",
                "sButtonText": "Print",
                "sInfo": 'Please press "CTR+P" to print or "ESC" to quit',
                "sMessage": "Generated by DataTables"
            }]
        }
    });
}

function jsfn_ConvertToDatableWithCustomSort(p_Table, p_ColumnsList, p_EmptyTableMsg, p_SortOrder, p_columnDefs) {

    //$.extend(true, $.fn.DataTable.TableTools.classes, {
    //    "container": "btn-group tabletools-btn-group pull-right",
    //    "buttons": {
    //        "normal": "btn btn-sm default",
    //        "disabled": "btn btn-sm default disabled"
    //    }
    //});

    p_Table.dataTable({

        // Internationalisation. For more info refer to http://datatables.net/manual/i18n
        "language": {
            "aria": {
                "sortAscending": ": activate to sort column ascending",
                "sortDescending": ": activate to sort column descending"
            },
            "emptyTable": "No data available in table",
            "info": "Showing _START_ to _END_ of _TOTAL_ entries",
            "infoEmpty": "No entries found",
            "infoFiltered": "(filtered1 from _MAX_ total entries)",
            "lengthMenu": "Show _MENU_ entries",
            "search": "Search:",
            "zeroRecords": "No matching records found"
        },

        "dom": "<'row' <'col-md-12'T>><'row'<'col-md-6 col-sm-12'l><'col-md-6 col-sm-12'f>r><'table-scrollable't><'row'<'col-md-5 col-sm-12'i><'col-md-7 col-sm-12'p>>", // horizobtal scrollable datatable

        "bStateSave": false, // save datatable state(pagination, sort, etc) in cookie.

        "columns": p_ColumnsList,
        "lengthMenu": [
            [10, 20, 30, 40, 50, -1],
            [10, 20, 30, 40, 50, "All"] // change per page values here
        ],
        // set the initial value
        "pageLength": 10,
        "pagingType": "bootstrap_full_number",
        "language": {
            "search": "Search : ",
            "emptyTable": p_EmptyTableMsg,
            "lengthMenu": "  _MENU_ records",
            "paginate": {
                "previous": "Prev",
                "next": "Next",
                "last": "Last",
                "first": "First"
            }
        },
        "responsive": true,
        "columnDefs": p_columnDefs,
        "order": p_SortOrder // set first column as a default sort by asc
    });
}

function jsfn_ShowLoading() {
    $.blockUI({
        message: $('#domMessage'),
        baseZ: 10099,
        css: {
            border: 'none',
            padding: '15px',
            backgroundColor: '#000',
            '-webkit-border-radius': '10px',
            '-moz-border-radius': '10px',
            opacity: .5,
            color: '#fff'
        }
    });
}

function jsfn_HideLoading() {
    $.unblockUI();
}

function jsfnAjaxFailure(ajaxerror) {
    if (ajaxerror.status == 401) {
        window.location.href = '/Login';
    }
}

function jsfn_SetSlider(p_Slider) {
    var jsFolder = "http://cdn.justcheck.co.in/scripts/carouselengine/";
    if (typeof html5Lightbox === "undefined") {
        html5Lightbox = jQuery(".html5lightbox").html5lightbox({
            skinsfoldername: "",
            jsfolder: jsFolder,
            barheight: 64,
            showtitle: false,
            showdescription: false,
            shownavigation: false,
            thumbwidth: 80,
            thumbheight: 60,
            thumbtopmargin: 12,
            thumbbottommargin: 8,
            titlebottomcss: '{color:#333; font-size:14px; font-family:Armata,sans-serif,Arial; overflow:hidden; text-align:left;}',
            descriptionbottomcss: '{color:#333; font-size:12px; font-family:Arial,Helvetica,sans-serif; overflow:hidden; text-align:left; margin:4px 0px 0px; padding: 0px;}'
        });
    }
    jQuery('#' + p_Slider).amazingcarousel({
        jsfolder: jsFolder,
        skinsfoldername: "",
        interval: 3000,
        itembottomshadowimagetop: 100,
        donotcrop: false,
        random: false,
        showhoveroverlay: false,
        rownumber: 1,
        height: 422,
        arrowheight: 60,
        showbottomshadow: false,
        itembackgroundimagewidth: 100,
        imageheight: 422,
        skin: "Fashion",
        responsive: true,
        lightboxtitlebottomcss: "{color:#333; font-size:14px; font-family:Armata,sans-serif,Arial; overflow:hidden; text-align:left;}",
        enabletouchswipe: true,
        navstyle: "none",
        backgroundimagetop: -40,
        arrowstyle: "mouseover",
        bottomshadowimagetop: 95,
        hidehoveroverlayontouch: false,
        continuous: false,
        itembackgroundimagetop: 0,
        hoveroverlayimage: "hoveroverlay-64-64-4.png",
        itembottomshadowimage: "itembottomshadow-100-100-5.png",
        lightboxshowdescription: false,
        width: 1200,
        navswitchonmouseover: false,
        showhoveroverlayalways: false,
        transitioneasing: "easeOutExpo",
        lightboxshownavigation: false,
        showitembackgroundimage: false,
        itembackgroundimage: "",
        playvideoimagepos: "center",
        circular: true,
        arrowimage: "arrows-42-60-0.png",
        scrollitems: 1,
        direction: "horizontal",
        lightboxdescriptionbottomcss: "{color:#333; font-size:12px; font-family:Arial,Helvetica,sans-serif; overflow:hidden; text-align:left; margin:4px 0px 0px; padding: 0px;}",
        supportiframe: false,
        navimage: "bullet-16-16-1.png",
        backgroundimagewidth: 110,
        showbackgroundimage: false,
        lightboxbarheight: 64,
        showplayvideo: true,
        spacing: 0,
        lightboxthumbwidth: 80,
        navdirection: "horizontal",
        itembottomshadowimagewidth: 100,
        backgroundimage: "",
        lightboxthumbtopmargin: 12,
        autoplay: true,
        lightboxnogroup: false,
        arrowwidth: 42,
        transparent: false,
        continuousduration: 2500,
        bottomshadowimage: "bottomshadow-110-95-0.png",
        scrollmode: "page",
        navmode: "page",
        lightboxshowtitle: false,
        lightboxthumbbottommargin: 8,
        arrowhideonmouseleave: 1000,
        showitembottomshadow: false,
        lightboxthumbheight: 60,
        navspacing: 8,
        pauseonmouseover: true,
        imagefillcolor: "FFFFFF",
        playvideoimage: "playvideo-64-64-0.png",
        transitionduration: 1000,
        visibleitems: 3,
        imagewidth: 1200,
        usescreenquery: false,
        bottomshadowimagewidth: 110,
        screenquery: {
            tablet: {
                screenwidth: 900,
                visibleitems: 2
            },
            mobile: {
                screenwidth: 600,
                visibleitems: 1
            }
        },
        responsive: [
            {
                breakpoint: 600,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1
                }
            }],
        navwidth: 16,
        loop: 0,
        navheight: 16
    });
}

// Extend the default Number object with a formatMoney() method:
// usage: someVar.formatMoney(decimalPlaces, symbol, thousandsSeparator, decimalSeparator)
// defaults: (2, "$", ",", ".")
Number.prototype.formatMoney = function (places, symbol, thousand, decimal) {
    places = !isNaN(places = Math.abs(places)) ? places : 2;
    symbol = symbol !== undefined ? symbol : "$";
    thousand = thousand || ",";
    decimal = decimal || ".";
    var number = this,
        negative = number < 0 ? "-" : "",
        i = parseInt(number = Math.abs(+number || 0).toFixed(places), 10) + "",
        j = (j = i.length) > 3 ? j % 3 : 0;
    return symbol + negative + (j ? i.substr(0, j) + thousand : "") + i.substr(j).replace(/(\d{3})(?=\d)/g, "$1" + thousand) + (places ? decimal + Math.abs(number - i).toFixed(places).slice(2) : "");
};