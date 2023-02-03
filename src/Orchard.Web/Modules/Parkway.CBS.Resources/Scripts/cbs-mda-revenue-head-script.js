$(document).ready(function () {
    var i = 0;
    $('#addAnotherRevHead').click(function () {
        giveMeNewRevenueHeadForm(i, false);
    });

    $('a[name="addsub"]').click(function (event) {
        //get the id of the element that clicked it
        var aid = event.target.id;
        var coords = aid.split('_');
        var coords1 = parseInt(coords[1], 10);
        var coords2 = parseInt(coords[2], 10);
        
        myfunction(coords, true);
    });

    $('#addsub[id*=addSubRevHead_]').click(function () {
        alert("ds");
    });

    function myfunction(c, isSubRevenue) {
        alert(c[0]);
    }

    function giveMeNewRevenueHeadForm(indexNumber, isSubRevenue) {
        var maindiv = $("#maindiv");

        //if (isSubRevenue) {
        //    var commondiv = $('<div id=wantToDuplicate' + indexNumber + ' />').prependTo(maindiv);
        //} else {
        //    var commondiv = $('<div id=wantToDuplicate margin' + indexNumber + ' />').prependTo(maindiv);

        //}
        var commondiv = $('<div id=wantToDuplicate' + indexNumber + ' />').prependTo(maindiv);

        var fieldset = $('<fieldset id=fieldset' + indexNumber + ' />').appendTo(commondiv);
        var table = $('<table class=tab id=tab' + indexNumber + ' />').appendTo(fieldset);
        //table head
        var thead =  $('<thead />').appendTo(table);
        var theadtr =  $('<tr />').appendTo(thead);
        var theadtrth =  $('<th scope=col id=tabtitleheader_' + indexNumber + ' />').appendTo(theadtr);
        var theadtrth1 =  $('<th scope=col />').appendTo(theadtr);
        theadtrth.html("Revenue Head");
        //table body
        var tbody =  $('<tbody />').appendTo(table);
        var trbody = $('<tr />').appendTo(tbody);
        var trbodytd = $('<td />').appendTo(trbody);
        var trbodytd1 = $('<td />').appendTo(trbody);

        inputlblname = $('<label />').appendTo(trbodytd);
        var nameinput = $('<input class="text medium" type=text required id=Name_' + indexNumber + ' />').appendTo(trbodytd);
        var namespan = $('<span class=hint />').appendTo(trbodytd);
        namespan.html("Add the name of your MDA here. For example Ministry of Land.");

        var inputlblcode = $('<label />').appendTo(trbodytd1);
        var codeinput = $('<input class="text medium" type=text required id=Code_' + indexNumber + ' />').appendTo(trbodytd1);
        var namespan = $('<span class=hint />').appendTo(trbodytd1);
        namespan.html("Add an MDA identification code. For example MDA/FF/LG-SL/049.");

        inputlblname.html("Revenue Head Name");
        inputlblcode.html("Revenue Head Code");

        //table footer
        var footer = $('<tfoot />').appendTo(table);
        var trfooter = $('<tr />').appendTo(footer);
        var tdtrfooter = $('<td />').appendTo(trfooter);
        var tdtrfooter = $('<td style=text-align:right />').appendTo(trfooter);
        var atdtrfooter = $('<a class="text smaller" href=# id=addSubRevHead_' + indexNumber + ' />').appendTo(tdtrfooter);
        atdtrfooter.html("Add Sub-Revenue Head")
    }

    function setName($level) {
        console.log($level);
        var newValue = "Revenue Head";
        for (var i = 1; i < $level; i++) {
            newValue = "Sub-" + newValue
        }
        return newValue;
    }
});