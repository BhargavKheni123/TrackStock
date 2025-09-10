using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace eTurns.DTO.Utils
{
    public static class DTOExtentionMethods
    {
        #region Validate Dynamic
        public static List<ModelValidationResult> ValidateDynamic<T>(this ModelValidator validator, T dto, ModelMetadata metadata)
        {
            List<ModelValidationResult> results = new List<ModelValidationResult>();

            Type vType = validator.GetType();
            PropertyInfo pInfo = dto.GetType().GetProperty(metadata.PropertyName);

            if (validator is RequiredAttributeAdapter)
            {
                results = validateRequire<T>((RequiredAttributeAdapter)validator, dto, pInfo);
            }
            else if (validator is RegularExpressionAttributeAdapter)
            {
                results = validateRgex<T>((RegularExpressionAttributeAdapter)validator, dto, pInfo);
            }
            else if (validator is StringLengthAttributeAdapter)
            {

            }


            return results;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pInfo"></param>
        /// <param name="attribType">apply filter with type e.g. RegularExpressionAttribute</param>
        /// <returns></returns>
        public static List<CustomAttributeData> GetCustomAttrbutes(this PropertyInfo pInfo, Type attribType = null)
        {
            List<CustomAttributeData> attributeData = pInfo.GetCustomAttributesData().ToList();

            if (attribType != null)
            {
                   attributeData = attributeData.Where(t => t.Constructor.DeclaringType == attribType).ToList();
            }

            return attributeData;
        }

        static List<ModelValidationResult> validateRgex<T>(RegularExpressionAttributeAdapter r, T dto, PropertyInfo pInfo)
        {
            List<ModelValidationResult> results = new List<ModelValidationResult>();

            var value = pInfo.GetValue(dto, null);
            
            if (!string.IsNullOrWhiteSpace(Convert.ToString(value)))
            {
                List<CustomAttributeData> customAttrib = pInfo.GetCustomAttrbutes(typeof(RegularExpressionAttribute));
                foreach (var cData in customAttrib)
                {
                    var typedArg = cData.ConstructorArguments;
                    string regex = typedArg[0].Value.ToString();
                    Regex regex1 = new System.Text.RegularExpressions.Regex(regex);
                    if (!regex1.IsMatch(value.ToString()))
                    {
                        var tmpRes = r.Validate(dto).ToList();
                        string msg = "";

                        var resData = cData.NamedArguments.ToList().Where(re => re.MemberInfo.Name == "ErrorMessageResourceName").FirstOrDefault();
                        if (resData != null)
                        {
                            switch (resData.TypedValue.Value.ToString())
                            {
                                case "InvalidEmail":
                                    msg = String.Format(eTurns.DTO.Resources.ResMessage.InvalidEmail, pInfo.Name);
                                    break;
                                case "NumberOnly":
                                    msg = String.Format(eTurns.DTO.Resources.ResMessage.NumberOnly, pInfo.Name);
                                    break;
                                case "InvalidFilename":
                                    msg = eTurns.DTO.Resources.ResMessage.InvalidValue;
                                    break;
                                case "InvalidFileType":
                                    msg = eTurns.DTO.Resources.ResMessage.InvalidFileType;
                                    break;
                                case "InvalidURL":
                                    msg = eTurns.DTO.Resources.ResMessage.InvalidURL;
                                    break;
                                case "MultiEmail":
                                    msg = eTurns.DTO.Resources.ResMessage.MultiEmail;
                                    break;
                                case "InvalidValue":
                                default:
                                    msg = eTurns.DTO.Resources.ResMessage.InvalidValue;
                                    break;
                            }

                        }

                        results.Add(new ModelValidationResult() { MemberName = pInfo.Name, Message = msg });
                    }
                }
            }

            return results;
        }

        static List<ModelValidationResult> validateRequire<T>(RequiredAttributeAdapter r,T dto ,PropertyInfo pInfo)
        {
            List<ModelValidationResult> results = new List<ModelValidationResult>();
            
            var value = pInfo.GetValue(dto, null);

            if ( string.IsNullOrWhiteSpace(Convert.ToString(value)))
            {
                var tmpRes = r.Validate(dto).ToList();
                string msg = "";
                if (tmpRes.Count > 0)
                {
                    msg = tmpRes[0].Message;
                }
                else
                {
                    msg = String.Format(eTurns.DTO.Resources.ResMessage.Required, pInfo.Name);
                }
                results.Add(new ModelValidationResult() { MemberName = pInfo.Name, Message = msg });
            }

            return results;
        }

        #endregion
    }
}
