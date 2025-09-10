using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace eTurns.Validators
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CompareDatesAttribute : ValidationAttribute, IClientValidatable
    {

        public CompareDatesAttribute(string otherPropertyName, bool allowEquality = true)
        {
            AllowEquality = allowEquality;
            OtherPropertyName = otherPropertyName;
        }

        #region Properties

        public string OtherPropertyName { get; private set; }

        public bool AllowEquality { get; private set; }

        #endregion

        protected override System.ComponentModel.DataAnnotations.ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var result = System.ComponentModel.DataAnnotations.ValidationResult.Success;
            var otherValue = validationContext.ObjectType.GetProperty(OtherPropertyName)
                .GetValue(validationContext.ObjectInstance, null);
            if (value != null)
            {
                if (value is DateTime)
                {

                    if (otherValue != null)
                    {
                        if (otherValue is DateTime)
                        {
                            if (!OtherPropertyName.ToLower().Contains("begin"))
                            {
                                if ((DateTime)value > (DateTime)otherValue)
                                {
                                    result = new System.ComponentModel.DataAnnotations.ValidationResult(ErrorMessage);
                                }
                            }
                            else
                            {
                                if ((DateTime)value < (DateTime)otherValue)
                                {
                                    result = new System.ComponentModel.DataAnnotations.ValidationResult(ErrorMessage);
                                }
                            }
                            if ((DateTime)value == (DateTime)otherValue && !AllowEquality)
                            {
                                result = new System.ComponentModel.DataAnnotations.ValidationResult(ErrorMessage);
                            }
                        }
                    }
                }
            }
            return result;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessage,
                ValidationType = "comparedates"
            };
            rule.ValidationParameters["otherpropertyname"] = OtherPropertyName;
            rule.ValidationParameters["allowequality"] = AllowEquality ? "true" : "";
            yield return rule;
        }
    }
}