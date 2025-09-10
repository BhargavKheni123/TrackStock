using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace eTurnsWeb
{
    public static class LabelHelpers
    {

        public static MvcHtmlString RequiredLabelFor<TModel, TValue>(
         this HtmlHelper<TModel> html,
         Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            var label = html.LabelFor(expression, htmlAttributes);
            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            if (metadata.IsRequired)
            {
                var htmlText = label.ToString();
                htmlText = htmlText.Replace("</label>", @" <em>*</em></label>");
                label = new MvcHtmlString(htmlText);
            }
            return label;
        }

        public static MvcHtmlString RequiredLabelFor<TModel, TValue>(
            this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression)
        {
            return RequiredLabelFor(html, expression, null);
        }

        //public static MvcHtmlString Label(this HtmlHelper html, string expression, string id = "", bool generatedId = false)
        //{
        //    return LabelHelper(html, ModelMetadata.FromStringExpression(expression, html.ViewData), expression, id, generatedId);
        //}

        //[SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
        //public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string id = "", bool generatedId = false)
        //{
        //    return LabelHelper(html, ModelMetadata.FromLambdaExpression(expression, html.ViewData), ExpressionHelper.GetExpressionText(expression), id, generatedId);
        //}

        //internal static MvcHtmlString LabelHelper(HtmlHelper html, ModelMetadata metadata, string htmlFieldName, string id, bool generatedId)
        //{
        //    string labelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
        //    if (String.IsNullOrEmpty(labelText))
        //    {
        //        return MvcHtmlString.Empty;
        //    }
        //    var sb = new StringBuilder();
        //    sb.Append(labelText);
        //    if (metadata.IsRequired)
        //        sb.Append("*");

        //    var tag = new TagBuilder("label");
        //    if (!string.IsNullOrWhiteSpace(id))
        //    {
        //        tag.Attributes.Add("id", id);
        //    }
        //    else if (generatedId)
        //    {
        //        tag.Attributes.Add("id", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName) + "_Label");
        //    }

        //    tag.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));
        //    tag.SetInnerText(sb.ToString());

        //    return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        //}
    }

}

