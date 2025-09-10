using eTurns.DTO.Resources;
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
    public class PasswordRequiredAttribute : ValidationAttribute, IClientValidatable
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CompareNumbersAttribute"/> class.
        /// </summary>
        /// <param name="otherPropertyName">Name of the compare to date property.</param>
        /// <param name="allowEquality">if set to <c>true</c> equal dates are allowed.</param>
        public PasswordRequiredAttribute(string otherPropertyName, string ModelIdPropertyName)
        {
            _OtherPropertyName = otherPropertyName;
            _ModelIdPropertyName = ModelIdPropertyName;
        }

        #region Properties

        /// <summary>
        /// Gets the name of the  property to compare to
        /// </summary>
        public string _OtherPropertyName { get; private set; }
        public string _ModelIdPropertyName { get; private set; }

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
            string currentPropertyValue = Convert.ToString(value);
            string OtherPassworvalue = Convert.ToString(validationContext.ObjectType.GetProperty(_OtherPropertyName).GetValue(validationContext.ObjectInstance, null));
            string ModelIdValue = Convert.ToString(validationContext.ObjectType.GetProperty(_ModelIdPropertyName).GetValue(validationContext.ObjectInstance, null));
            long ModelIdLongValue = 0;
            long.TryParse(ModelIdValue, out ModelIdLongValue);
            if (ModelIdLongValue < 1)
            {
                if (string.IsNullOrWhiteSpace(currentPropertyValue))
                {

                    result = new ValidationResult(ErrorMessage);
                }
            }
            return result;
        }
        public override string FormatErrorMessage(string name)
        {

            return string.Format(ResMessage.Required, name);
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
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "passwordrequired"
            };
            rule.ValidationParameters["otherpropertyname"] = _OtherPropertyName;
            rule.ValidationParameters["modelidpropertyname"] = _ModelIdPropertyName;
            yield return rule;
        }

    }

}