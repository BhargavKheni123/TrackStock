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
using eTurns.DTO.Resources;
using eTurns.Validators;


namespace eTurnsWeb.validation
{
    /// <summary>
    /// Custom ModelValidatorProvider that returns ModelValidators based on the validation rules specified in xml files.
    /// </summary>
    public class XmlModelValidatorProvider : ModelValidatorProvider
    {
        #region Properties
        // Dictionary to temporarily store all the validation attribute types present in System.ComponentModel.DataAnnotations assembly.
        public readonly Dictionary<string, Type> _validatorTypes;

        public string MasterXmlFolderPath = HttpContext.Current.Server.MapPath("validation//MasterRules");

        public string RoomXMLFolderPath
        {
            get
            {
                string rootPath = HttpContext.Current.Server.MapPath("validation//Rules");
                return rootPath;
            }
        }

        #endregion

        #region Constructors
        public XmlModelValidatorProvider()
        {
            _validatorTypes = Assembly.LoadWithPartialName("System.ComponentModel.DataAnnotations").GetTypes()
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

            var rulesPath = String.Format("{0}\\{1}.xml", MasterXmlFolderPath,
                isPropertyValidation ? metadata.ContainerType.Name : metadata.ModelType.Name);

            XElement xElement = null;
            List<XElement> rules = null;
            if (File.Exists(rulesPath))
            {
                xElement = XElement.Load(rulesPath);
                string prop = isPropertyValidation ? metadata.PropertyName : metadata.ModelType.Name;
                rules = xElement.XPathSelectElements(String.Format("./validator[@property='{0}']", prop))
                    .ToList();
            }
            else
            {
                rules = new List<XElement>();
            }

            // Produce a validator for each validation attribute we find
            foreach (var rule in rules)
            {
                DataAnnotationsModelValidationFactory factory;

                XAttribute validationType = rule.Attribute("type");

                if (validationType == null || string.IsNullOrWhiteSpace(validationType.Value))
                {
                    continue;
                }

                var validatorType = _validatorTypes[String.Concat(validationType.Value, "Attribute")];

                if (!AttributeFactories.TryGetValue(validatorType, out factory))
                {
                    factory = DefaultAttributeFactory;
                }

                var validator = (ValidationAttribute)Activator.CreateInstance(validatorType, GetValidationArgs(rule));
                validator.ErrorMessage = ResMessage.Required; //rule.Attribute("message") != null && !String.IsNullOrEmpty(rule.Attribute("message").Value) ? rule.Attribute("message").Value : null;
                results.Add(factory(metadata, context, validator));
            }

            return results;
        }

        #endregion

        #region Private Methods
        // read the arguments passed to the validation attribute and cast it their respective type.
        private object[] GetValidationArgs(XElement rule)
        {
            var validatorArgs = rule.Attributes().Where(a => a.Name.ToString().StartsWith("arg"));
            var args = new object[validatorArgs.Count()];
            var i = 0;

            foreach (var arg in validatorArgs)
            {
                var argName = arg.Name.ToString();
                var argValue = arg.Value;

                if (!argName.Contains("-"))
                {
                    args[i] = argValue;
                }
                else
                {
                    var argType = argName.Split('-')[1];

                    switch (argType)
                    {
                        case "int":
                            args[i] = int.Parse(argValue);
                            break;

                        case "datetime":
                            args[i] = DateTime.Parse(argValue);
                            break;

                        case "char":
                            args[i] = Char.Parse(argValue);
                            break;

                        case "double":
                            args[i] = Double.Parse(argValue);
                            break;

                        case "decimal":
                            args[i] = Decimal.Parse(argValue);
                            break;

                        case "bool":
                            args[i] = Boolean.Parse(argValue);
                            break;

                        default:
                            args[i] = argValue;
                            break;
                    }
                }
            }

            return args;
        }

        //private string GetRoomPath() { 

        //}

        #endregion

    }// class
}