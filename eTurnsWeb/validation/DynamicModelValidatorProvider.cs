using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Xml.XPath;
using eTurns.DTO;
using eTurns.DTO.Resources;
using eTurns.Validators;
using eTurnsMaster.DAL;
using eTurnsWeb.BAL;
using eTurnsWeb.Helper;
using Microsoft.SqlServer.Management.Smo;

namespace eTurnsWeb.validation
{
    public class DynamicModelValidatorProvider : ModelValidatorProvider
    {
        #region Properties
        // Dictionary to temporarily store all the validation attribute types present in System.ComponentModel.DataAnnotations assembly.
        public readonly Dictionary<string, Type> _validatorTypes;
        List<ValidationRulesDTO> rules = null;
        string RuleDTOType = "";
        #endregion

        #region Constructors
        public DynamicModelValidatorProvider()
        {
            _validatorTypes = Assembly.LoadWithPartialName("System.ComponentModel.DataAnnotations").GetTypes()
            //_validatorTypes = Assembly.Load("System.ComponentModel.DataAnnotations").GetTypes()
            .Where(t => t.IsSubclassOf(typeof(ValidationAttribute)))
            .ToDictionary(t => t.Name, t => t);

            // custom ValidationAttribute that validates a date for future value.
            _validatorTypes.Add("CompareDatesAttribute", typeof(CompareDatesAttribute));
            _validatorTypes.Add("CompareNumbersAttribute", typeof(CompareNumbersAttribute));
            _validatorTypes.Add("PasswordRequiredAttribute", typeof(PasswordRequiredAttribute));
        }

        #endregion

        #region Stolen from DataAnnotationsModelValidatorProvider

        // delegate that converts ValidationAttribute into DataAnnotationsModelValidator
        internal static DataAnnotationsModelValidationFactory DefaultAttributeFactory =
                (metadata, context, attribute) => new DataAnnotationsModelValidator(metadata, context, attribute);

        internal static Dictionary<Type, DataAnnotationsModelValidationFactory> AttributeFactories =
            new Dictionary<Type, DataAnnotationsModelValidationFactory>()
            {
                {
                    typeof(RangeAttribute),
                    (metadata, context, attribute) => new RangeAttributeAdapter(metadata, context, (RangeAttribute)attribute)
                },
                {
                    typeof(RegularExpressionAttribute),
                    (metadata, context, attribute) => new RegularExpressionAttributeAdapter(metadata, context, (RegularExpressionAttribute)attribute)
                },
                {
                    typeof(RequiredAttribute),
                    (metadata, context, attribute) => new RequiredAttributeAdapter(metadata, context, (RequiredAttribute)attribute)
                },
                {
                    typeof(StringLengthAttribute),
                    (metadata, context, attribute) => new StringLengthAttributeAdapter(metadata, context, (StringLengthAttribute)attribute)
                },
            };

        #endregion

        #region Public Methods

        public override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context)
        {
            var results = new List<ModelValidator>();

            // whether the validation is for a property or model
            // (remember we can apply validation attributes to a property or model and same applies here as well)
            var isPropertyValidation = metadata.ContainerType != null && !String.IsNullOrEmpty(metadata.PropertyName);

            //var rulesPath = String.Format("{0}\\{1}.xml", MasterXmlFolderPath,
            //    isPropertyValidation ? metadata.ContainerType.Name : metadata.ModelType.Name);

            var ruleDTOType = isPropertyValidation ? metadata.ContainerType.Name : metadata.ModelType.Name;
            string controller = context.RouteData.Values["controller"].ToString();
            string action = context.RouteData.Values["action"].ToString();
            List<ValidationRulesDTO> rules = GetRules(ruleDTOType,controller,action);

            var propertyRules = rules.Where(r => r.DTOProperty.ToLower() == (metadata.PropertyName ?? "").ToLower());
            // Produce a validator for each validation attribute we find
            foreach (var rule in propertyRules)
            {
                
                string ruleType = "";
                if (rule.IsRequired  == false)
                {
                    continue;
                }
                else if (rule.IsRequired)
                {
                    ruleType = "Required";                    
                    results.Add(GetModelValidator(ruleType,metadata,context));
                }

                
                break;
            }

            return results;
        }


        #endregion

        #region Private Methods

        ModelValidator GetModelValidator(string ruleType, ModelMetadata metadata, ControllerContext context)
        {
            DataAnnotationsModelValidationFactory factory;
            var validatorType = _validatorTypes[String.Concat(ruleType, "Attribute")];

            if (!AttributeFactories.TryGetValue(validatorType, out factory))
            {
                factory = DefaultAttributeFactory;
            }

            var validator = (ValidationAttribute)Activator.CreateInstance(validatorType);

            switch (ruleType)
            {
                case "Required":
                    validator.ErrorMessage = ResMessage.Required; //rule.Attribute("message") != null && !String.IsNullOrEmpty(rule.Attribute("message").Value) ? rule.Attribute("message").Value : null;
                    break;
            }

            return factory(metadata, context, validator);
        }

        List<ValidationRulesDTO> GetRules(string ruleDTOType,string controller,string action)
        {
            
            if (rules != null && RuleDTOType == ruleDTOType)
            {
                // get rules from memory per request if found for same dto
                return rules;
            }

            int moduleId = ValidationRuleModulesBAL.GetModuleIdFromEnum(controller,action,ruleDTOType);
            rules = null;
            RuleDTOType = ruleDTOType;
            string cacheKey = GetCacheKey(ruleDTOType,moduleId);
            var cachedRules = CacheHelper<IEnumerable<ValidationRulesDTO>>.GetCacheItem(cacheKey);

            if (cachedRules != null)
            {
                rules = cachedRules.ToList();
            }

            if (rules == null || rules.Count == 0)
            {
                using (ValidationRulesBAL rulesBAL = new ValidationRulesBAL())
                {
                    //EturnsDTOEnum eturnsDTO = EturnsDTOEnum.None;
                    //if (Enum.TryParse<EturnsDTOEnum>(ruleDTOType, out eturnsDTO))
                    if (moduleId > 0)
                    {
                        rules = rulesBAL.GetByModuleId(SessionHelper.EnterPriceID, SessionHelper.CompanyID, SessionHelper.RoomID, moduleId);
                    }
                }

                if (rules != null && rules.Count != 0)
                {
                    // add rules in cache
                    CacheHelper<IEnumerable<ValidationRulesDTO>>.AddCacheItem(cacheKey, rules);
                }
            }

            if (rules == null)
            {
                rules = new List<ValidationRulesDTO>();
            }

            return rules;
        }

        private static string GetCacheKey(string ruleDTOType,int moduleId)
        {
            return "Cached_ValidationRules_" + ruleDTOType + "_" + moduleId + "_" + SessionHelper.EnterPriceID + "_" + SessionHelper.CompanyID + "_" + SessionHelper.RoomID;
        }

        #endregion
    }
}