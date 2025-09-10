using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.Validators
{
    /// <summary>
    /// Compares two dates to each other, ensuring that one is larger than the other
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CompareNumbersAttribute : ValidationAttribute, IClientValidatable
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CompareNumbersAttribute"/> class.
        /// </summary>
        /// <param name="otherPropertyName">Name of the compare to date property.</param>
        /// <param name="allowEquality">if set to <c>true</c> equal dates are allowed.</param>
        public CompareNumbersAttribute(string otherPropertyName, bool allowEquality = true)
        {
            AllowEquality = allowEquality;
            OtherPropertyName = otherPropertyName;
        }

        #region Properties

        /// <summary>
        /// Gets the name of the  property to compare to
        /// </summary>
        public string OtherPropertyName { get; private set; }

        /// <summary>
        /// Gets a value indicating whether dates could be the same
        /// </summary>
        public bool AllowEquality { get; private set; }


        #endregion

        /// <summary>
        /// Validates the specified value with respect to the current validation attribute.
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <param name="validationContext">The context information about the validation operation.</param>
        /// <returns>
        /// An instance of the <see cref="T:System.ComponentModel.DataAnnotations.ValidationResult"/> class.
        /// </returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var result = ValidationResult.Success;
            var otherValue = validationContext.ObjectType.GetProperty(OtherPropertyName).GetValue(validationContext.ObjectInstance, null);
            if (value != null)
            {
                decimal currentDecimalValue;
                if (decimal.TryParse(value.ToString(), out currentDecimalValue))
                {

                    if (otherValue != null)
                    {
                        decimal otherDecimalValue;
                        if (decimal.TryParse(otherValue.ToString(), out otherDecimalValue))
                        {
                            if (!OtherPropertyName.ToLower().Contains("begin"))
                            {
                                if (currentDecimalValue > otherDecimalValue)
                                {
                                    result = new ValidationResult(ErrorMessage);
                                }
                            }
                            else
                            {
                                if (currentDecimalValue < otherDecimalValue)
                                {
                                    result = new ValidationResult(ErrorMessage);
                                }
                            }
                            if (currentDecimalValue == otherDecimalValue && !AllowEquality)
                            {
                                result = new ValidationResult(ErrorMessage);
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// When implemented in a class, returns client validation rules for that class.
        /// </summary>
        /// <param name="metadata">The model metadata.</param>
        /// <param name="context">The controller context.</param>
        /// <returns>
        /// The client validation rules for this validator.
        /// </returns>
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessage,
                ValidationType = "comparenumbers"
            };
            rule.ValidationParameters["otherpropertyname"] = OtherPropertyName;
            rule.ValidationParameters["allowequality"] = AllowEquality ? "true" : "";
            yield return rule;
        }
    }
}
