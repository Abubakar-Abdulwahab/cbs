(function (b, c) {
    var a = function (f, e) {
        var d;
        this.$element = b(f);
        this.options = b.extend({}, b.fn.wizard.defaults, e);
        this.currentStep = 1;
        this.numSteps = this.$element.find("li").length;
        this.$nextBtn = $('#' + this.$element[0].id + '-actions').find(".btn-navigate_form");
        d = this.$nextBtn.children().detach();
        this.nextText = b.trim(this.$nextBtn.text());
        this.$nextBtn.append(d);
        this.$nextBtn.on("click", b.proxy(this.next, this));
        this.$element.on("click", "li.complete", b.proxy(this.stepclicked, this));
        this.$stepContainer = this.$element.data("target") || "body";
        this.$stepContainer = b(this.$stepContainer);
        this.newCount = 1;
        this.newCount1 = 1;
        this.$wiredStep = '#wiredstep';
        this.divPtype = '.divPtype';
        this.dipValue = $('.dipValue').val();
        this.divInstr2 = '.divInstr2';
        this.divInstr3 = '.divInstr3';
        this.divInstr4 = '.divInstr4';
        this.cnt = 2;
        
    };
    
    a.prototype = {
        constructor: a,
        setState: function () {
            var n = (this.currentStep > 1);
            var o = (this.currentStep == 1);
            var d = (this.currentStep == this.numSteps);
            var dd = (this.currentStep === this.numSteps - 1);
            this.$nextBtn.attr("disabled", (d == true));

            if (d && $(this.$Attestation).prop("checked")) {
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
            var d = (this.currentStep == this.numSteps);
            var dd = (this.currentStep == this.numSteps-1);
            var nowStep = (this.currentStep == 2);
            var firstStep = (this.currentStep == 1);
            var penultimateStep = (this.currentStep == this.numSteps-1);

            if (firstStep)
            {
                var f = b.Event("change");
                
            }
            if (nowStep) {
              
            }
            if (g) {
                var f = b.Event("change");
                this.$element.trigger(f, {
                    step: this.currentStep,
                    direction: "next"
                });
              
                //if ($(_curStepId + '.dynamic-table')[0]) {

                var _curStepId = this.$wiredStep + this.currentStep;

                $(_curStepId + ' .dynamic-table').each(function () {
                    var tbl = $(this);
                    var rel = tbl.attr("rel");
                    window[rel]();
                });

                var isValid = true;
                
                $("#Employed").click(function () {
                    $('#selfEmployment').hide();
                    $('#ownership').hide();
                    $('#paidEmployment').show();
                });


                $("#SelfEmployed").click(function () {
                    $('#paidEmployment').hide();
                    $('#ownership').hide();
                    $('#selfEmployment').show();
                }); 

                $("#Owner_Partner").click(function () {
                    $('#paidEmployment').hide();
                    $('#selfEmployment').hide();
                    $('#ownership').show();
                });

                $(_curStepId + ' .Table-Data-Validation').each(function () {
                    var TableValues = $(this).val();
                    if (TableValues == "") {
                        toastr.options.positionClass = 'toast-top-right';
                        toastr.info("Kindly add the data to the tables by clicking the plus sign (+) before proceeding");
                        f.preventDefault();
                    }
                   
                });
                $(_curStepId + ' .email').each(function () {
                    var email = $(this).val();
                    isValid = isValidEmailAddress(email);
                    if (isValid == false) {
                        toastr.options.positionClass = 'toast-top-right';
                        toastr.info("Kindly enter your email in the right format");
                        f.preventDefault();
                    }
                    
                });

                $(_curStepId + ' .validation').each(function () {
                    isValid = WizardvalidityCheck(this, isValid);
                    updateReviewForm(this);
                });
                $(_curStepId + ' #RegAttestation').each(function () {
                    if ($(_curStepId + " #RegAttestation:checked").length < 1) {
                        toastr.options.positionClass = 'toast-top-right';
                        toastr.info("Kindly check the attestation box before proceeding");
                        return;
                    }
                });
               
                if (isValid == false) {
                    f.preventDefault();
                }

                if (f.isDefaultPrevented()) {
                    $('html, body').animate({ scrollTop: 0 }, 2000);
                    return
                }

                this.currentStep += 1;
                this.setState();

            }

            else {
                if (d) {
                    this.$element.trigger("finished")
                }
            }
            if (dd) {
                this.$element.trigger("finished")
                var elementt = document.getElementById("SubmitForm");
                elementt.click();
            }

            if(penultimateStep)
            {

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
                j.data("wizard", (i = new a(this, h)))
            }
            if (typeof e == "string") {
                f = i[e](g)
            }
        });
        return (f == c) ? d : f
    };
    b.fn.wizard.defaults = {};
    b.fn.wizard.Constructor = a;
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

//function getInvoice() {
//    $("#inv_container").html('<object data="http://parkway.uat.cashflow.ng/public/invoice/OAAAABBGb3JtYXQAAAAAAAJBdXRvTnVtYmVyAAsAAAAxMDAwMDYwODg2ABJJZAAAAAAAAAAAAAA=">');
//};

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





jQuery(function ($) {
    $('#WiredWizard').wizard();
});




