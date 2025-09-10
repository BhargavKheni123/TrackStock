using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.DTO
{
    class CustomDataAnnotations
    {
    }

    public class EmailAttribute : RegularExpressionAttribute
    {
        public EmailAttribute()
            : base("^a-z0-9_\\+-]+(\\.[a-z0-9_\\-]+)*@[a-z0-9-]+(\\.[a-z0-9-]+)*\\.([a-z]{2,4})$}")
        {

        }
    }

    public class CheckCriticleWithMin : ValidationAttribute, IClientValidatable
    {
        private string _compareProperty = string.Empty;
        private string _errorMessage = "Criticle quantity must be lessthan minimum quantity";

        private double? _criticleValue = null;
        private double? _minimumValue = null;

        public CheckCriticleWithMin(string compareProperty)
        {
            _compareProperty = compareProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {


            _criticleValue = (double?)value;

            if (_criticleValue == null || _criticleValue.GetValueOrDefault(0) == 0)
            {
                return ValidationResult.Success;
            }
            else
            {

                var curProperty = validationContext.ObjectInstance.GetType().GetProperty(_compareProperty);

                _minimumValue = (double?)curProperty.GetValue(validationContext.ObjectInstance, null);

                if (_minimumValue == null)
                {
                    return new ValidationResult(_errorMessage);
                }
                else if (_minimumValue.GetValueOrDefault(0) <= _criticleValue.GetValueOrDefault(0))
                {
                    return new ValidationResult(_errorMessage);
                }

            }

            return ValidationResult.Success;
        }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule() { ErrorMessage = ErrorMessage, ValidationType = "criticlequantitycheck" };
            rule.ValidationParameters["criticleqty"] = _criticleValue;
            rule.ValidationParameters["minimumqty"] = _minimumValue;

            yield return rule;
        }
    }

    public class CheckMinimumWithMaximum : ValidationAttribute, IClientValidatable
    {
        private string _compareProperty = string.Empty;
        private string _errorMessage = "Minimum quantity must be less then maximum quantity";

        private double? _minimumValue = null;
        private double? _maximumValue = null;

        public CheckMinimumWithMaximum(string compareProperty)
        {
            _compareProperty = compareProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {


            _minimumValue = (double?)value;
            if (_minimumValue == null || (_minimumValue.HasValue && _minimumValue.GetValueOrDefault(0) == 0))
            {
                return ValidationResult.Success;
            }
            else
            {

                var curProperty = validationContext.ObjectInstance.GetType().GetProperty(_compareProperty);

                _maximumValue = (double?)curProperty.GetValue(validationContext.ObjectInstance, null);

                if (_maximumValue == null)
                {
                    return new ValidationResult(_errorMessage);
                }
                else if (_maximumValue.GetValueOrDefault(0) <= _minimumValue.GetValueOrDefault(0))
                {
                    return new ValidationResult(_errorMessage);
                }

            }

            return ValidationResult.Success;
        }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {

            var rule = new ModelClientValidationRule() { ErrorMessage = ErrorMessage, ValidationType = "minimumquantitycheck" };
            rule.ValidationParameters["minimumqty"] = _minimumValue;
            rule.ValidationParameters["maximumqty"] = _maximumValue;

            yield return rule;
        }
    }


}
