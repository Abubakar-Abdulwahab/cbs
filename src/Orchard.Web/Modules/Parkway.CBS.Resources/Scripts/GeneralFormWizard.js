
toastr.options.positionClass = 'toast-top-right';

//initialize jQuery on the page; the letter 'b' could be replaced with '$'
(function (b, c) {
    var formWizard = function (f, e) {
        var nextBtnChildren;
        this.$element = b(f);
        this.options = b.extend({}, b.fn.wizard.defaults, e);
        this.currentStep = 1;
        this.numSteps = this.$element.find("li").length;
        /*Customized to Enable Out Of Wizard Buttons*/
        this.$nextBtn = $('#' + this.$element[0].id + '-actions').find(".btn-navigate_form");
        /*End Customized to Enable Out Of Wizard Buttons*/
        nextBtnChildren = this.$nextBtn.children().detach();
        this.nextText = b.trim(this.$nextBtn.text());
        this.$nextBtn.append(nextBtnChildren);
        this.$nextBtn.on("click", b.proxy(this.next, this));
        this.$element.on("click", "li.complete", b.proxy(this.stepclicked, this));
        this.$stepContainer = this.$element.data("target") || "body";
        this.$stepContainer = b(this.$stepContainer);
        this.$wiredStep = '#wiredstep';
        //this.newCount = 1;
        //this.newCount1 = 1;
        //this.divPtype = '.divPtype';
        //this.dipValue = $('.dipValue').val();
        //this.divInstr2 = '.divInstr2';
        //this.divInstr3 = '.divInstr3';
        //this.divInstr4 = '.divInstr4';
        //this.cnt = 2;
    };
    
    formWizard.prototype = {
        constructor: formWizard,
        setState: function () {

            //checks for the wizard location
            var n = (this.currentStep > 1);
            var o = (this.currentStep == 1);
            var lastStep = (this.currentStep == this.numSteps);
            var dd = (this.currentStep === this.numSteps - 1);
            this.$nextBtn.attr("disabled", (lastStep == true));

            if (lastStep && $(this.$Attestation).prop("checked")) {
                this.$nextBtn.prop("disabled", false);
            }

            var h = this.$nextBtn.data();
            if (h && h.last) {
                this.lastText = h.last;
                if (typeof this.lastText != "undefined") {
                    var l = (dd != true) ? this.nextText : this.lastText;
                    var f = this.$nextBtn.children().detach();
                    this.$nextBtn.text(l).append(f)
                }
            }
            var j = this.$element.find("li");
            j.removeClass("active").removeClass("complete");
            var m = "li:lt(" + (this.currentStep - 1) + ")";
            var g = this.$element.find(m);
            g.addClass("complete");
            var e = "li:eq(" + (this.currentStep - 1) + ")";
            var k = this.$element.find(e);
            k.addClass("active");
            var i = k.data().target;
            this.$stepContainer.find(".step-pane").removeClass("active");
            b(i).addClass("active");
            this.$element.trigger("changed")
        },
        Attestation: function () {
            if ($(this.$Attestation).prop("checked")) {
                this.$nextBtn.prop("disabled", false);
            }
            else {
                this.$nextBtn.prop("disabled", true);
            }
        },
        stepclicked: function (h) {
            var d = b(h.currentTarget);
            var g = this.$element.find("li").index(d);
            var f = b.Event("stepclick");
            this.$element.trigger(f, {
                step: g + 1
            });
            if (f.isDefaultPrevented()) {
                return
            }
            this.currentStep = (g + 1);
            this.setState()
        },
        previous: function () {
            var d = (this.currentStep > 1);
            if (d) {
                var f = b.Event("change");
                this.$element.trigger(f, {
                    step: this.currentStep,
                    direction: "previous"
                });
                if (f.isDefaultPrevented()) {
                    return
                }
                this.currentStep -= 1;
                this.setState()
            }
        },
        next: function () {
            var g = (this.currentStep + 1 <= this.numSteps);
            var lastStep = (this.currentStep == this.numSteps);
            var dd = (this.currentStep == this.numSteps-1);
            var nowStep = (this.currentStep == 2);
            var firstStep = (this.currentStep == 1);
            var penultimateStep = (this.currentStep == this.numSteps-1);

            if (firstStep)
            {
                var innerMarkup = '<h5 class="row-title row-title-right"> All fields with &nbsp;<i class="red fa  fa-circle"></i>&nbsp;are required</h5>';

                document.getElementById("page-title-header").innerHTML = innerMarkup;

                var f = b.Event("change");              
            }

            if (nowStep) {
                var _curStepId = this.$wiredStep + this.currentStep;

                var ExecutivesDetails = "";
                var rows = $('tr.addRow');
                $(rows).each(function () {
                    var FirstName = $(this).children('.FirstName').first().text();
                    var LastName = $(this).children('.LastName').first().text();
                    var EMail = $(this).children('.EMail').first().text();
                    var Mobile = $(this).children('.ExecMobile').first().text();
                    ExecutivesDetails = ExecutivesDetails + FirstName + 'β' + LastName + 'β' + EMail + 'β' + Mobile + 'β';
                });
                $("#ExecutivesDetails").val(ExecutivesDetails);
                var paswrd = $("#PasswordInput").val();
                $("#ContactPassword").val(paswrd);

                var execvaldat = $("#ExecutivesDetails").val();
                if (execvaldat == "") {
                    toastr.options.positionClass = 'toast-top-right';
                    toastr.info("Kindly add the executive details to the table by clicking the plus sign (+) before proceeding");
                    toastr.info("Kindly add the executive details to the table by clicking the plus sign (+) before proceeding");
                }

                //do validation on second step depending on the form type
                    //$(_curStepId + ' .mobile').each(function () {
                    //    var phoneNumber = $(this).val();
                    //    isValid = isValidPhoneNumber(phoneNumber);
                    //    if (isValid == false) {
                    //        toastr.options.positionClass = 'toast-top-right';
                    //        toastr.info("Kindly enter your phone number correctly");
                    //        //f.preventDefault();
                    //    }

                    //});

                //do validation independent
                    var regPattern = new RegExp('^(\\+234|0)\d{10}$');
                    var phoneNumber = document.getElementsByClassName("mobile").value;


                    //var contactPhoneNumber = $("#ContactPhoneNumber").val();
                    //if (contactPhoneNumber == "" || regPattern.test(contactPhoneNumber) == false) {
                    //    toastr.options.positionClass = 'toast-top-right';
                    //    toastr.info("Kindly enter the correct contact phone number before proceeding");
                    //}

                    //var companyPhoneNumber = $("#CompanyMobile").val();
                    //if (companyPhoneNumber == "" || regPattern.test(companyPhoneNumber) == false) {
                    //    toastr.options.positionClass = 'toast-top-right';
                    //    toastr.info("Kindly enter the correct company phone number before proceeding");
                    //}

                    
                    //var res = regPattern.test(phoneNumber)
                    //if (res == false) {
                    //    toastr.options.positionClass = 'toast-top-right';
                    //    toastr.info("Kindly enter the correct phone number before proceeding");
                    //    event.preventDefault();
                    //}
                       



                    if ($("#RegAttestation:checked").length < 1) {
                        toastr.options.positionClass = 'toast-top-right';
                        toastr.info("Kindly check the attestation box before proceeding");
                        return;
                    }
                    else {
                        $("#RegAttestationrev").prop("checked", true);
                        $("#RegAttestationrev").prop("enabled", false);
                    }
            }

            if (g) {
                var f = b.Event("change");
                this.$element.trigger(f, {
                    step: this.currentStep,
                    direction: "next"
                });

                var _curStepId = this.$wiredStep + this.currentStep;
                var isValid = true;

                $(_curStepId + ' .validUpload').each(function () {
                    var valUp = this.files.length;
                    if (valUp == 0) {
                        toastr.options.positionClass = 'toast-top-right';
                        toastr.info("Kindly add the required documents before proceeding.");
                        isValid = false;
                    }
                });

                //Section for TIN Registration Form 
                $(_curStepId + ' .dynamic-table').each(function () {
                    var tbl = $(this);
                    var rel = tbl.attr("rel");
                    window[rel]();
                });


                $(_curStepId + ' .Table-Data-Validation').each(function () {
                    var TableValues = $(this).val();
                    if (TableValues == "") {
                        toastr.options.positionClass = 'toast-top-right';
                        toastr.info("Kindly add the data to the tables by clicking the plus sign (+) before proceeding");
                        f.preventDefault();
                    }
                });
                //Section ends here

                //validate email address
                $(_curStepId + ' .email').each(function () {
                    var email = $(this).val();
                    isValid = isValidEmailAddress(email);
                    if (isValid == false) {
                        toastr.options.positionClass = 'toast-top-right';
                        toastr.info("Kindly enter your email in the right format");
                        f.preventDefault();
                    }

                });

                ////validate phone number
                //var regPattern = new RegExp('^(\\+234|0)\d{10}$');
                //var phoneNumber = document.getElementsByClassName("mobile").value;

                //var res = regPattern.test(phoneNumber)
                //if (res == false) {
                //    toastr.options.positionClass = 'toast-top-right';
                //    toastr.info("Kindly enter the correct phone number before proceeding");
                //    event.preventDefault();
                //}


                $(_curStepId + ' .corporateValidation').each(function () {
                    isValid = WizardvalidityCheck(this, isValid);
                    updateReviewForm(this);
                });

                $(_curStepId + ' .validation').each(function () {
                    isValid = WizardvalidityCheck(this, isValid);
                    updateReviewForm(this);
                });
                

            //validate TIN Registration Form
                if (isValid == true) {
                    $(_curStepId + ' .landForm').each(function () {
                        updateReviewForm(this);
                    });
                }
                //in cases where attestion box has to be checked
                //$(_curStepId + ' #RegAttestation').each(function () {
                //    if ($(_curStepId + " #RegAttestation:checked").length < 1) {
                //        toastr.options.positionClass = 'toast-top-right';
                //        toastr.info("Kindly check the attestation box before proceeding");
                //        return;
                //    }
                //});

                var email = $(_curStepId + " #ContactEmail").val();
                if (email != undefined) {
                    if (isValidEmailAddress(email)) {
                        $(_curStepId + " #ContactEmail").css({
                            "border": "",
                            "background": ""
                        });
                    } else {
                        isValid = false;
                        $(_curStepId + " #ContactEmail").css({
                            "border": "1px solid red",
                            "background": "#FFCECE"
                        });
                    }
                }
                
                if (isValid == false) {
                    f.preventDefault();
                }

                if (f.isDefaultPrevented()) {
                    $('html, body').animate({ scrollTop: 0 }, 2000);
                    return
                }

                this.currentStep += 1;
                this.setState();

            } else {
                if (d) {
                    this.$element.trigger("finished")
                }
            }
            if (dd) {
                if (!$("#LandAttestation:checked")) {
                    toastr.options.positionClass = 'toast-top-right';
                    toastr.info("Kindly check the attestation box before proceeding");
                    var landElement = document.getElementById("SubmitLandForm");
                    landElement.disabled();
                    return;
                }

                else {
                    $("#LandAttestation").prop("checked", true);
                    $("#LandAttestation").prop("enabled", false);

                    this.$element.trigger("finished")
                    var landElement = document.getElementById("SubmitLandForm");
                    var element = document.getElementById("SubmitForm");
                    $('#SubmitLandForm').trigger('click');
                    element.click();
                    landElement.click();
                }
                
            }
            scrollTo(0, 0);
        },
        selectedItem: function (d) {
            return {
                step: this.currentStep
            }
        },
    
    };
    b.fn.wizard = function (e, g) {
        var f;
        var d = this.each(function () {
            var j = b(this);
            var i = j.data("wizard");
            var h = typeof e == "object" && e;
            if (!i) {
                j.data("wizard", (i = new formWizard(this, h)))
            }
            if (typeof e == "string") {
                f = i[e](g)
            }
        });
        return (f == c) ? d : f
    };
    b.fn.wizard.defaults = {};
    b.fn.wizard.Constructor = formWizard;
    b(function () {
        b("body").on("mousedown.wizard.data-api", ".wizard", function () {
            var d = b(this);
            if (d.data("wizard")) {
                return
            }
            d.wizard(d.data())
        })
    })
})(window.jQuery);

function WizardvalidityCheck(field, isValidField) {
    if ($.trim($(field).val()) == '') {
        isValidField = false;
        $(field).css({
            "border": "1px solid red",
            "background": "#FFCECE"
        });
    }
    else {
        $(field).css({
            "border": "",
            "background": ""
        });
    }
    return isValidField;
}

function updateReviewForm(field) {
    var fieldId = field.getAttribute("Id");
    var revElement = "#" + fieldId + "rev";
    $(revElement).text($(field).val());
    var frmElement = document.getElementById(fieldId);
    var revElementDrop = document.getElementById(fieldId + "rev");
   
    if ( frmElement.tagName === 'SELECT') {
        var fieldValue = frmElement.options[frmElement.selectedIndex].text;
        $(revElement).text(fieldValue);
    }
}

function isValidEmailAddress(emailAddress) {
    var pattern = /^([a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+(\.[a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+)*|"((([ \t]*\r\n)?[ \t]+)?([\x01-\x08\x0b\x0c\x0e-\x1f\x7f\x21\x23-\x5b\x5d-\x7e\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|\\[\x01-\x09\x0b\x0c\x0d-\x7f\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))*(([ \t]*\r\n)?[ \t]+)?")@(([a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.)+([a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.?$/i;
    return pattern.test(emailAddress);
};

function isValidPhoneNumber(phoneNumber) {
    var regPattern = new RegExp('^(\\+234|0)\d{10}$');
    return regPattern.test(phoneNumber);
};


jQuery(function ($) {
    $('#WiredWizard').wizard();
});




