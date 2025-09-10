window.customValidation = window.customValidation ||
    {
        relatedControlValidationCalled: function (event) {
            if (!customValidation.activeValidator) {
                customValidation.formValidator = $(event.data.source).closest('form').data('validator');
            }
            customValidation.formValidator.element($(event.data.target));
        },
        relatedControlCollection: [],
        formValidator: undefined,
        addDependatControlValidaitonHandler: function (element, dependentPropertyName) {
            var id = $(element).attr('id');
            if ($.inArray(id, customValidation.relatedControlCollection) < 0) {
                customValidation.relatedControlCollection.push(id);
                $(element).on(
                    'blur',
                    { source: $(element), target: $('#' + dependentPropertyName) },
                    customValidation.relatedControlValidationCalled);
            }
        }
    };

$.validator.unobtrusive.adapters.add('comparedates', ['otherpropertyname', 'allowequality'],
        function (options) {
            options.rules['comparedates'] = options.params;
            if (options.message) {
                options.messages['comparedates'] = options.message;
            }
        }
    );

$.validator.unobtrusive.adapters.add('comparenumbers', ['otherpropertyname', 'allowequality'],
        function (options) {
            options.rules['comparenumbers'] = options.params;
            if (options.message) {
                options.messages['comparenumbers'] = options.message;
            }
        }
    );
$.validator.unobtrusive.adapters.add('passwordrequired', ['otherpropertyname', 'modelidpropertyname'],
        function (options) {
            options.rules['passwordrequired'] = options.params;
            if (options.message) {
                options.messages['passwordrequired'] = options.message;
            }
        }
    );

        $.validator.addMethod('passwordrequired', function (value, element, params) {
            
            var modelId = $('input[name="' + params.modelidpropertyname + '"]').val();
            if (modelId < 1) {
                if ($.trim(value) == "") {
                    return false;
                }
            }
            customValidation.addDependatControlValidaitonHandler(element, params.otherpropertyname);
            return true;
        }, '');

$.validator.addMethod('comparedates', function (value, element, params) {
    var otherFieldValue = $('input[name="' + params.otherpropertyname + '"]').val();
    if (otherFieldValue && value) {
        var currentValue = Date.parse(value);
        var otherValue = Date.parse(otherFieldValue);
        if ($(element).attr('name').toLowerCase().indexOf('begin') >= 0) {
            if (params.allowequality) {
                if (currentValue > otherValue) {
                    return false;
                }
            } else {
                if (currentValue >= otherValue) {
                    return false;
                }
            }
        } else {
            if (params.allowequality) {
                if (currentValue < otherValue) {
                    return false;
                }
            } else {
                if (currentValue <= otherValue) {
                    return false;
                }
            }
        }
    }
    customValidation.addDependatControlValidaitonHandler(element, params.otherpropertyname);
    return true;
}, '');



$.validator.addMethod('comparenumbers', function (value, element, params) {
    var otherFieldValue = $('input[name="' + params.otherpropertyname + '"]').val();
    if (otherFieldValue && value) {
        var currentValue = parseFloat(value);
        var otherValue = parseFloat(otherFieldValue);
        if ($(element).attr('name').toLowerCase().indexOf('begin') >= 0) {
            if (params.allowequality) {
                if (currentValue > otherValue) {
                    return false;
                }
            } else {
                if (currentValue >= otherValue) {
                    return false;
                }
            }
        } else {
            if (params.allowequality) {
                if (currentValue < otherValue) {
                    return false;
                };
            } else {
                if (currentValue <= otherValue) {
                    return false;
                };
            }
        }
    }
    customValidation.addDependatControlValidaitonHandler(element, params.otherpropertyname);
    return true;
}, '');

