var w = window.location;
var url = w.protocol + "//" + w.host + "/";

jQuery(function ($) {

    $("div.userListBlock").on("paste", "input.numericsonly", function (e) {
        var element = this;
        var oldValue = $(element).val();
        setTimeout(function () {
            var text = $(element).val();
            if (isNaN(text)) {
                $(element).val(oldValue);
            }
        }, 50);
    });

    $("div.userListBlock").on("drop", "input.numericsonly", function (e) {
        var oldValue = this.value;

        setTimeout($.proxy(function () {
            if (this.value !== oldValue) {
                $(this).val(oldValue);
                $(this).trigger('change');
            }
        }, this), 10);
    });

    $("div.userListBlock").on("change", "input.numericsonly", function (e) {
        var text = $(this).val();
        if (isNaN(text)) {
            $(this).val("");
            $(this).focus();
        }
    });


    $.validator.unobtrusive.adapters.add('rangedate', ['minselector', 'maxselector', 'mindate', 'maxdate', 'nullable'],
    function (options) {

        var params = {
            minSelector: options.params.minselector,
            maxSelector: options.params.maxselector,
            minDate: options.params.mindate,
            maxDate: options.params.maxdate,
            nullable: options.params.nullable == "true"
        };

        options.rules['rangeDate'] = params;
        if (options.message) {
            options.messages['rangeDate'] = options.message;
        }
    });
    // The validator function
    $.validator.addMethod('rangeDate',function (value, element, param) {

    var dateVal = parseDate(value, "G"); // .Net "G" DateTime format, set as the same format used by the ModelClientValidationRule.
    var minDate;
    var maxDate;

    if (value.length < 1 && param.nullable)
        return true;

    if (param.minDate.length > 0) {
        minDate = parseDate(param.minDate, "G");
    } else {

        var minControl = $('#' + param.minSelector);
        if (minControl.exists()) {
            minDate = parseDate(minControl.val(), "G");
        } else {
            return false;
        }
    }

    if (param.maxDate.length > 0) {
        maxDate = parseDate(param.maxDate, "G");
    } else {

        var maxControl = $('#' + param.maxSelector);
        if (maxControl.exists()) {
            maxDate = parseDate(maxControl.val(), "G");
        } else {
            return false;
        }
    }

    return minDate <= dateVal && dateVal <= maxDate;
}
);


});


function ValidateEmail(email) {
    return /^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i.test(email);
}

function ucFirstAllWords(str) {
    var pieces = str.split(" ");
    for (var i = 0; i < pieces.length; i++) {
        var j = pieces[i].charAt(0).toUpperCase();
        pieces[i] = j + pieces[i].substr(1);
    } return pieces.join(" ");
}


function NotAllowSpace(event) {

    var AsciiCode;
    if (event.keyCode == 0)
        AsciiCode = event.charCode;
    else
        AsciiCode = event.keyCode;

    if (AsciiCode == 32)
        return false;
    else
        return true;
}

function chkNumeric(e, curobj) {

    var num = [e.keyCode || e.which];

    if ((num > 95 && num < 106) || (num > 36 && num < 41) || num == 9 || num == 190) {
        return;
    }
    if (e.shiftKey || e.ctrlKey || e.altKey) {
        e.preventDefault();
    } else if (num != 46 && num != 8) {
        if (isNaN(parseInt(String.fromCharCode(e.which)))) {
            e.preventDefault();
        }
    }
}

function ValidateCurrency(curval) {
    var oRegExp = /^[1-9]\d*(?:\,\d{0,2})?$/;
    return oRegExp.test(curval);
}

function capitaliseFirstLetter(str) {
    var retstr = "";
    retstr = str.charAt(0).toUpperCase() + str.slice(1);
    //    alert(retstr);
    return retstr;

}


function regIsLetters(fData) {
    var reg = new RegExp("^[a-zA-Z]+$");
    return reg.test(fData);
}

function regIsnumbersletters(fData) {
    var reg = new RegExp("^[a-zA-Z1-9]+$");
    return reg.test(fData);
}





