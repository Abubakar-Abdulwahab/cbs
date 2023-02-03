$(document).ready(function () {
    var prefix =  $("#CodePrefix").val();
    var counter = $("#Indexer").val();
    $('#anotherOne').click(function (event) {
        var name = event.target.name;
        giveMeaNewForm(name);
        event.defaultPrevented;
    });

    $('.remove').click(function (event) {
        var arr = $(this).attr("id").split('_');
        //console.log(arr[1]);
        var tabNumber = arr[1];
        var tab = $("#tab_" + tabNumber);
        //var prev_tabs = tab.prevAll();
        //remove tab
        tab.remove();
        //rearrageNumbering
        //rearrangeNumbering(tabNumber, prev_tabs);
        event.defaultPrevented;
    });

    function giveMeaNewForm(name) {
        var container = $("#container");
        
        var table = $('<table class=items id=tab_' + counter + ' />').prependTo(container);
        //table head
        var thead = $('<thead />').appendTo(table);
        var theadtr = $('<tr />').appendTo(thead);
        var theadtrth = $('<th scope=col />').appendTo(theadtr);
        var theadtrth1 = $('<th scope=col />').appendTo(theadtr);

        var threadtrthp = $('<p align=right />').appendTo(theadtrth1);
        var threadtrthpa = $('<a id=remove_' + counter + ' href=# name=remove />').appendTo(threadtrthp);
        threadtrthpa.html("x");
        
        theadtrth.html(name);
        threadtrthpa.on("click", { name: counter }, removetab);
        //table body
        var tbody = $('<tbody />').appendTo(table);
        var trbody = $('<tr />').appendTo(tbody);
        var trbodytd = $('<td />').appendTo(trbody);
        var trbodytd1 = $('<td />').appendTo(trbody);

        inputlblname = $('<label />').appendTo(trbodytd);
        var nameinput = $('<input class="text large" type=text required name=RevenueHeadsCollection[' + counter + '].Name id="nameinput" />').appendTo(trbodytd);
        //var nameinput = $('<input class="text large" type=text required id=Name />').appendTo(trbodytd);
        var namespan = $('<span class=hint />').appendTo(trbodytd);

        var inputlblcode = $('<label />').appendTo(trbodytd1);
        var codeinput = $('<input id="codeinput" class="text large" type=text required name=RevenueHeadsCollection[' + counter + '].Code id="codeinput" value='+prefix+' />').appendTo(trbodytd1);
        var hiddenField = $("<input type='hidden' name='RevenueHeadsCollection.Index' value='"+ counter +"' />").appendTo(trbodytd1);
        var namespan1 = $('<span class=hint />').appendTo(trbodytd1);

        inputlblname.html(name + " Name");
        inputlblcode.html(name + " Code");
        if (name == "MDA") {
            namespan.html("Add the name of your MDA here. For example Ministry of Land.");
            namespan1.html("Add an MDA identification code. For example MDA/FF/LG-SL/049.");
        } else {
            namespan.html("Add the name of your Revenue Head here. For example Land Use Charge");
            namespan1.html("Add a Revenue Head identification code. For example MDA/FF/RH-SL/049");
        }
        ++counter;
    }

    function removetab(event) {
        var tabNumber = event.data.name;
        var tab = $("#tab_" + tabNumber);
        //var prev_tabs = tab.prevAll();
        //remove tab
        tab.remove();
        //counter--;
        //rearrageNumbering
        //rearrangeNumbering(tabNumber, prev_tabs);
    }

    /*
    function rearrangeNumbering(tabNumber, all_tabs) {
        counter--;
        var rowValue = parseInt(tabNumber, 10);
        all_tabs.each(function () {
            $(this).find('input')[0].name = '[' + rowValue + '].Name';
            $(this).find('input')[1].name = '[' + rowValue + '].Code';
            $(this).attr('id','tab_' + rowValue);
            counter = ++rowValue;
        });
    }*/

});