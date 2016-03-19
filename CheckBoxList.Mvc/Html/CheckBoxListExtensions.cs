using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace CheckBoxList.Mvc.Html
{
    public static class CheckBoxListExtensions
    {
        public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, string name, IEnumerable<CheckBoxListItem> checkboxList)
        {
            return CheckBoxListHelper(htmlHelper, name, checkboxList, null);
        }

        public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, string name, IEnumerable<CheckBoxListItem> checkboxList, object htmlAttributes)
        {
            return CheckBoxListHelper(htmlHelper, name, checkboxList, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, string name, IEnumerable<CheckBoxListItem> checkboxList, IDictionary<string, object> htmlAttributes)
        {
            return CheckBoxListHelper(htmlHelper, name, checkboxList, htmlAttributes);
        }

        public static MvcHtmlString CheckBoxListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
            where TProperty : IEnumerable<CheckBoxListItem>
        {
            return CheckBoxListFor(htmlHelper, expression, null);
        }

        public static MvcHtmlString CheckBoxListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
            where TProperty : IEnumerable<CheckBoxListItem>
        {
            return CheckBoxListFor(htmlHelper, expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcHtmlString CheckBoxListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
            where TProperty : IEnumerable<CheckBoxListItem>
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            var name = ExpressionHelper.GetExpressionText(expression);

            var func = expression.Compile();
            var checkboxList = func(htmlHelper.ViewData.Model) as IEnumerable<CheckBoxListItem>;

            return CheckBoxListHelper(htmlHelper, name, checkboxList, htmlAttributes);
        }

        private static MvcHtmlString CheckBoxListHelper(HtmlHelper htmlHelper, string expression, IEnumerable<CheckBoxListItem> checkboxList, IDictionary<string, object> htmlAttributes)
        {
            return SelectInternal(htmlHelper, expression, checkboxList, htmlAttributes);
        }

        private static MvcHtmlString SelectInternal(this HtmlHelper htmlHelper, string name, IEnumerable<CheckBoxListItem> checkboxList, IDictionary<string, object> htmlAttributes)
        {
            var fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            if (string.IsNullOrEmpty(fullName))
            {
                throw new ArgumentException("name");
            }
            
            var listItemBuilder = BuildItems(htmlHelper, name, checkboxList.ToList());

            var tagBuilder = new TagBuilder("div")
            {
                InnerHtml = listItemBuilder.ToString()
            };
            tagBuilder.MergeAttributes(htmlAttributes);
            tagBuilder.MergeAttribute("name", fullName, true /* replaceExisting */);
            tagBuilder.GenerateId(fullName);

            return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.Normal));
        }
        
        private static StringBuilder BuildItems(this HtmlHelper htmlHelper, string name, IList<CheckBoxListItem> checkboxList)
        {
            var listItemBuilder = new StringBuilder();

            for (var i = 0; i < checkboxList.Count(); i++)
            {
                listItemBuilder.AppendLine(htmlHelper.GetCheckBoxListItemHtml(name, checkboxList[i], i));
            }

            return listItemBuilder;
        }

        private static string GetCheckBoxListItemHtml(this HtmlHelper htmlHelper, string name, CheckBoxListItem item, int index)
        {
            var checkbox = htmlHelper.CheckBox(GetChildControlName(name, index, "IsChecked"), item.IsChecked);
            var text = htmlHelper.Hidden(GetChildControlName(name, index, "Text"), item.Text);
            var value = htmlHelper.Hidden(GetChildControlName(name, index, "Value"), item.Value);

            var sb = new StringBuilder();
            sb.AppendLine("<div>");
            sb.AppendLine(checkbox.ToHtmlString());
            sb.AppendLine(text.ToHtmlString());
            sb.AppendLine(value.ToHtmlString());
            sb.AppendLine(HttpUtility.HtmlEncode(item.Text));
            sb.AppendLine("</div>");

            return sb.ToString();
        }

        private static string GetChildControlName(string parentName, int index, string childName)
        {
            return string.Format("{0}[{1}].{2}", parentName, index, childName);
        }
    }
}
