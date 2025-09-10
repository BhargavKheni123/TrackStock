using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;



namespace eTurnsWeb.Helper
{
    public class ModelValidationFilterAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.ModelState.IsValid == false)
            {
                // Return the validation errors in the response body.
                var errors = new Dictionary<string, IEnumerable<string>>();
                foreach (KeyValuePair<string, System.Web.Http.ModelBinding.ModelState> keyValue in actionContext.ModelState)
                {
                    errors[keyValue.Key] = keyValue.Value.Errors.Select(e => e.ErrorMessage);
                }

                actionContext.Response =
                    actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, errors);
            }
        }

    }

 
}