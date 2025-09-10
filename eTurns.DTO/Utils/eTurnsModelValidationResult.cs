using eTurns.DTO.Resources;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace eTurns.DTO.Utils
{
    public class eTurnsModelValidationResult : ModelValidationResult
    {
        public eTurnsModelValidationResult(string memberDisplayName, ModelValidator validator, ModelValidationResult validationResult)
        {
            this.MemberDisplayName = memberDisplayName;
            this.MemberName = validationResult.MemberName;
            this.Message = validationResult.Message;
            this.Validator = validator;
        }

        public ModelValidator Validator { get; set; }
        public string MemberDisplayName { get; set; }

    }

    public class eTurnsModelValidationResultList
    {
        #region Constructor
        public eTurnsModelValidationResultList()
        {
            this.ValidationResults = new List<eTurnsModelValidationResult>();
        }
        #endregion

        #region Public Methods
        public List<eTurnsModelValidationResult> ValidationResults { get; set; }

        public bool HasErrors()
        {
            return !(ValidationResults == null || ValidationResults.Count == 0);
        }

        /// <summary>
        /// Filter validation
        /// </summary>
        /// <param name="filterType">e.g. RequiredAttributeAdapter,RegularExpressionAttributeAdapter</param>
        /// <returns></returns>
        public List<eTurnsModelValidationResult> GetByType(Type filterType)
        {
            return this.ValidationResults.Where(r => r.Validator.GetType() == filterType).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterType">e.g. RequiredAttributeAdapter,RegularExpressionAttributeAdapter</param>
        /// <returns></returns>
        public string GetDisplayNamesCSV(Type filterType = null)
        {
            string csv = "";

            if (!HasErrors())
            {
                return csv;
            }

            if (filterType != null)
            {
                csv = string.Join(", ", this.ValidationResults.Where(r => r.Validator.GetType() == filterType)
                    .Select(r => r.MemberDisplayName).ToList());
            }

            return csv;
        }


        /// <summary>
        /// Get error messages string.
        /// </summary>
        /// <param name="filterType">e.g. RequiredAttributeAdapter,RegularExpressionAttributeAdapter</param>
        /// <returns></returns>
        public string GetErrorMessages(Type filterType = null)
        {
            string errors = "";
            if (!HasErrors())
            {
                return errors;
            }

            var filterResults = GetByType(filterType);

            foreach (var r in ValidationResults)
            {
                errors += r.Message + " \n";
            }

            return errors;
        }

        /// <summary>
        /// create csv of property in single message for validation type
        /// </summary>
        /// <param name="filterType">e.g. RequiredAttributeAdapter,RegularExpressionAttributeAdapter</param>
        /// <returns></returns>
        public string GetShortErrorMessage(Type filterType)
        {
            string msg = "";
            if (!HasErrors())
            {
                return msg;
            }

            var filterResults = GetByType(filterType);

            if (filterType == typeof(RequiredAttributeAdapter))
            {
                string nameCsv = this.GetDisplayNamesCSV(filterType);
                if (filterResults.Count == 1 )
                {
                    msg = string.Format("The field '{0}' is required.", nameCsv);
                }
                else {
                    msg = string.Format("The fields '{0}' are required.", nameCsv);
                }
            }
            //else if (filterType == typeof(RegularExpressionAttributeAdapter))
            //{

            //}

            return msg;
        }

        #endregion
    }


}
