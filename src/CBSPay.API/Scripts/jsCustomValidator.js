$(function () {
    jQuery.validator.addMethod('companyvalidator', function (value, element, params) {
        var vTaxPayerTypeID = $('#cboTaxPayerType').val();

        if (vTaxPayerTypeID == '2' && value == '') {
            return false;
        }
       
        return true;
    }, 'Enter Required Value');


    jQuery.validator.unobtrusive.adapters.add('companyvalidator', function (options) {
        options.rules['companyvalidator'] = {};
        options.messages['companyvalidator'] = options.message;
    });

    jQuery.validator.addMethod('individualvalidator', function (value, element, params) {
        var vTaxPayerTypeID = $('#cboTaxPayerType').val();

        if (vTaxPayerTypeID == '1' && value == '') {
            return false;
        }

        return true;
    }, 'Enter Required Value');


    jQuery.validator.unobtrusive.adapters.add('individualvalidator', function (options) {
        options.rules['individualvalidator'] = {};
        options.messages['individualvalidator'] = options.message;
    });

    jQuery.validator.addMethod('specialvalidator', function (value, element, params) {
        var vTaxPayerTypeID = $('#cboTaxPayerType').val();

        if (vTaxPayerTypeID == '2' && value == '') {
            return false;
        }

        return true;
    }, 'Enter Required Value');


    jQuery.validator.unobtrusive.adapters.add('specialvalidator', function (options) {
        options.rules['specialvalidator'] = {};
        options.messages['specialvalidator'] = options.message;
    });

    jQuery.validator.addMethod('governmentvalidator', function (value, element, params) {
        var vTaxPayerTypeID = $('#cboTaxPayerType').val();

        if (vTaxPayerTypeID == '4' && value == '') {
            return false;
        }

        return true;
    }, 'Enter Required Value');


    jQuery.validator.unobtrusive.adapters.add('governmentvalidator', function (options) {
        options.rules['governmentvalidator'] = {};
        options.messages['governmentvalidator'] = options.message;
    });

}(jQuery));