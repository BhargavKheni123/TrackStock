using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace eTurns.DTO.Utils
{
    public class DTOCommonUtils
    {
        /// <summary>
        /// Copy property comparing name and data type
        /// </summary>
        /// <typeparam name="TCopyFrom"></typeparam>
        /// <typeparam name="TCopyTo"></typeparam>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        public static void CopyProperty<TCopyFrom, TCopyTo>(TCopyFrom parent,ref TCopyTo child,Dictionary<string,string> popertyMap) where TCopyTo : class,new()
        {
            var parentProperties = parent.GetType().GetProperties().OrderBy( p=>p.Name);
            //var childProperties = child.GetType().GetProperties().OrderBy(p => p.Name);

            popertyMap = popertyMap == null ? new Dictionary<string, string>() : popertyMap;

            foreach (var parentProperty in parentProperties)
            {
                // find mapped property name from Dictionary
                
                PropertyInfo childProperty = null;

                if (popertyMap.ContainsKey(parentProperty.Name))
                {
                    string mapPropetyName = popertyMap[parentProperty.Name];
                    childProperty = child.GetType().GetProperty(mapPropetyName);
                }
                else
                {
                    childProperty = child.GetType().GetProperty(parentProperty.Name);
                }

                //foreach (PropertyInfo childProperty in childProperties)
                {
                    //if (parentProperty.Name == childProperty.Name)
                    if(childProperty != null)
                    {
                        var childUnderlyingType = Nullable.GetUnderlyingType(childProperty.PropertyType);
                        bool isChildNullable = childUnderlyingType != null;

                        var parentUnderlyingType = Nullable.GetUnderlyingType(parentProperty.PropertyType);
                        bool isParentNullable = parentUnderlyingType != null;

                        if (parentProperty.PropertyType == childProperty.PropertyType
                            || (isChildNullable && isParentNullable && childUnderlyingType == parentUnderlyingType)
                            || (isChildNullable && childUnderlyingType == parentProperty.PropertyType)
                            || (isParentNullable && parentUnderlyingType == childProperty.PropertyType)
                            )
                        {
                            object value = parentProperty.GetValue(parent, null);
                            childProperty.SetValue(child, value, null);
                            //break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Validate against Data Annotations
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dto"></param>
        //public static string ValidateDTO<T>(T dto)
        //{
        //    string errorMsg = "";
        //    var validationContext = new ValidationContext(dto, serviceProvider: null, items: null);

        //    var validationResults = new List<ValidationResult>();

        //    var isValid = Validator.TryValidateObject(dto, validationContext, validationResults, true);

        //    // If there any exception return them in the return result
        //    if (!isValid)
        //    {
        //        foreach (ValidationResult message in validationResults)
        //        {
        //            errorMsg += message.ErrorMessage + "\n";
        //        }
        //    }

        //    return errorMsg;
        //}


        
        public static eTurnsModelValidationResultList ValidateDTO<TModuleDTO>(TModuleDTO dto, ControllerContext context, List<string> ignoreProperties = null) where TModuleDTO : class, new()
        {
            string controller = context.RouteData.Values["controller"].ToString();
            string action = context.RouteData.Values["action"].ToString();
            ignoreProperties = GetValidationIgnoreProperty<TModuleDTO>(dto,ignoreProperties);
            
            return ValidateDTO<TModuleDTO>(dto, controller, action, ignoreProperties);
        }

        /// <summary>
        /// ignore validation property if same dto used in multiple views with show \ hide property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dto"></param>
        /// <param name="ignoreProperties"></param>
        /// <returns></returns>
        public static List<string> GetValidationIgnoreProperty<T>(T dto, List<string> ignoreProperties) where T : class, new()
        {
            if (dto != null)
            {
                if (ignoreProperties == null)
                {
                    ignoreProperties = new List<string>();
                }

                // ignore validation property if same dto used in multiple views
                if (dto is OrderMasterDTO)
                {
                    PropertyInfo pInfo = dto.GetType().GetProperty("OrderType");
                    var value = pInfo.GetValue(dto, null);
                    if (Convert.ToInt32(value) == (int)OrderType.RuturnOrder && !ignoreProperties.Contains("CustomerName"))
                    {
                        ignoreProperties.Add("CustomerName");
                    }
                }
                else if (dto is TransferMasterDTO)
                {
                    PropertyInfo pInfo = dto.GetType().GetProperty("RequestType");
                    var value = pInfo.GetValue(dto, null);
                    if (Convert.ToInt32(value) == (int)RequestType.Out && !ignoreProperties.Contains("StagingName"))
                    {
                        ignoreProperties.Add("StagingName");
                    }
                }
            }

            return ignoreProperties;
        }

        /// <summary>
        /// Validate DTO with dynamic validation rules
        /// </summary>
        /// <typeparam name="TModuleDTO"></typeparam>
        /// <param name="dto"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <param name="ignoreProperties"></param>
        /// <returns></returns>
        public static eTurnsModelValidationResultList ValidateDTO<TModuleDTO>(TModuleDTO dto, string controller, string action, List<string> ignoreProperties = null) where TModuleDTO : class, new()
        {
            eTurnsModelValidationResultList validationResultList = new eTurnsModelValidationResultList();

            try
            {
                //get metadata of Model
                ModelMetadata metadataForType = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(TModuleDTO));

                foreach (var p in metadataForType.Properties)
                {
                    if (ignoreProperties != null)
                    {
                        if (ignoreProperties.Contains(p.PropertyName))
                        {
                            continue;
                        }
                    }

                    // get MetaData of property in model
                    ModelMetadata metadata = ModelMetadataProviders.Current.GetMetadataForProperty(null, typeof(TModuleDTO), p.PropertyName);

                    ControllerContext context = new ControllerContext();

                    context.HttpContext = new HttpContextWrapper(HttpContext.Current);
                    context.RequestContext = HttpContext.Current.Request.RequestContext;

                    context.RouteData.Values["controller"] = controller;
                    context.RouteData.Values["action"] = action;

                    //var validators = provider.GetValidators(metadata,context).ToList();
                    // get validation rules of property
                    var validators = metadata.GetValidators(context).ToList();

                    // validate property for each rules
                    if (validators.Count > 0)
                    {
                        foreach (var v in validators)
                        {
                            //var valResult = v.Validate(masterDTO);

                            List<ModelValidationResult> valResult = v.ValidateDynamic<TModuleDTO>(dto, metadata);
                            foreach (var r in valResult)
                            {
                                eTurnsModelValidationResult eTurnsModelValidationResult = new eTurnsModelValidationResult(
                                    metadata.DisplayName,v,r);
                                validationResultList.ValidationResults.Add(eTurnsModelValidationResult);
                            }
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                return validationResultList;
            }

            return validationResultList;

        }

    }// calss
}
