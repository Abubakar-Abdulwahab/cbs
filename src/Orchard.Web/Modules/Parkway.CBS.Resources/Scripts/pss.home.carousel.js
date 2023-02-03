$(document).ready(function () {
    var prevIndex = 0;
    const carouselContentList = [
        ["Welcome to the Police Specialized Automation Services (POSSAP) Portal", "You can pay for services such as Special protection services, Guards service, Police extracts etc."],
        ["SPECIAL PROTECTION SERVICE", "This service is primarily detailed for the protection of PEPs (Politically Exposed Person’s) as well as private citizens. The service is rendered by personnel of the Force’s Specialized Units, PMF, SPU and CTU."],
        ["GUARDS SERVICE", "This service offers the services of Police officers to a wider range of the population as a whole. Guards Services allows the public and corporations such as Banks to request for Police protection of residential property, commercial property, events, and escort."],
        ["POLICE EXTRACT", "A Police Extract is a document usually done for reports of lost or missing items or documents. This service allows the public as well as corporate bodies to request for a Police Extract Document."],
        ["POLICE CHARACTER CERTIFICATE", "A Police Character Clearance is done to check whether an applicant has a criminal record. This service allows for the public to request for this document."],
        ["POLICE INVESTIGATION REPORT", "An investigation report is a document issued at the end of an investigation into a criminal case. This service allows for the public to request for this document."],
    ];

    $("#requestToggle").click(function () { changeContent(1); });
    $("#approvalToggle").click(function () { changeContent(2); });
    $("#fulfilmentToggle").click(function () { changeContent(3); });
    setIndicator(1);
    $('.carousel').carousel({
        interval: 8000
    });

    $('#pssCarousel').on('slide.bs.carousel', function (e) {
        setIndicator(e.to + 1);
    });

    $(".carousel-nav-toggle").on("click", function () {
        $('.carousel').carousel(parseInt($(this).find("p").html()) - 1);
    });

    function changeContent(carouselIndex) {
        if (carouselIndex == (prevIndex + 1)) { return; }
        switch (carouselIndex) {
            case 1:
                toggleCrumbs(carouselIndex);
                $("#serviceContent > h4:nth-child(1)").empty();
                $("#serviceContent > h4:nth-child(1)").html("Request Service");
                $("#serviceContent > p:nth-child(2)").empty();
                $("#serviceContent > p:nth-child(2)").html("Click on the “Request Service” or “Get Started” button to access a page to request a service. You might be required to create an account if you don’t already have one.");
                $("#serviceContentImg").attr("src", "/Themes/PoliceTheme/Styles/images/RequestService.png");
                break;
            case 2:
                toggleCrumbs(carouselIndex);
                $("#serviceContent > h4:nth-child(1)").empty();
                $("#serviceContent > h4:nth-child(1)").html("Wait For Approval");
                $("#serviceContent > p:nth-child(2)").empty();
                $("#serviceContent > p:nth-child(2)").html("Upon applying for a service, your request will be reviewed by the appropriate authorities based on the approval guideline.");
                $("#serviceContentImg").attr("src", "/Themes/PoliceTheme/Styles/images/Approval.png");
                break;
            case 3:
                toggleCrumbs(carouselIndex);
                $("#serviceContent > h4:nth-child(1)").empty();
                $("#serviceContent > h4:nth-child(1)").html("Receive Your Service");
                $("#serviceContent > p:nth-child(2)").empty();
                $("#serviceContent > p:nth-child(2)").html("Congrats! Your request has been approved; you are to kindly follow the laid down instructions to receive your request.");
                $("#serviceContentImg").attr("src", "/Themes/PoliceTheme/Styles/images/Fulfillment.png");
                break;

        }
        prevIndex = carouselIndex - 1;
    }


    function toggleCrumbs(crumbIndex) {
        let crumbs = $("#crumbs .col-2");
        for (let x = 0; x < crumbs.length; ++x) {
            if (x == (crumbIndex - 1)) {
                $("#crumbs .col-2")[x].classList.remove("numbered-crumb");
                $("#crumbs .col-2")[x].classList.add("active-numbered-crumb");
                $("#crumbs .col-2")[prevIndex].classList.remove("active-numbered-crumb");
                $("#crumbs .col-2")[prevIndex].classList.add("numbered-crumb");
            }
        }
    }

    function setIndicator(activeImage) {
            $(".carousel-nav-toggle").removeClass("active");
        $("#img" + activeImage + "Toggle").addClass("active");
        if (activeImage > 1) {
            $("#contentSupportText").show();
        } else { $("#contentSupportText").hide(); }
        $("#contentHeader").html(carouselContentList[activeImage - 1][0]);
        $("#contentText").html(carouselContentList[activeImage - 1][1]);
    }
});