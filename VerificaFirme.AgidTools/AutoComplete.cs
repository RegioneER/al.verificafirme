using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.UI;

namespace RER.Tools.MVC.Agid
{
    public static partial class ExtensionMethods
    {
        public static MvcHtmlString AgidAutoCompleteListFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, SelectList items, string externalDivAdditionalClasses = null, string optionLabel = null, bool? required = null, string id = null)
        {
            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                //Func<TModel, TValue> method = expression.Compile();
                ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);

                string name = ExpressionHelper.GetExpressionText(expression);
                string displayName = helper.DisplayNameFor(expression).ToString();
                IDictionary<string, object> unobtrusiveValitaionAttributes = helper.GetUnobtrusiveValidationAttributes(name, metadata);

                writer.AddAttribute(HtmlTextWriterAttribute.Class, $"form-group");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                writer.AddAttribute(HtmlTextWriterAttribute.Class, $"select-wrapper{(!string.IsNullOrWhiteSpace(externalDivAdditionalClasses) ? " " + externalDivAdditionalClasses : "")}");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                writer.AddAttribute(HtmlTextWriterAttribute.For, id ?? helper.AgidIDFor(expression).ToString());
                writer.AddAttribute(HtmlTextWriterAttribute.Id, $"{(id ?? helper.AgidIDFor(expression).ToString())}-label");
                writer.AddAttribute(HtmlTextWriterAttribute.Class, string.Format("control-label{0}", required.HasValue && required.Value ? " required" : ""));
                writer.RenderBeginTag(HtmlTextWriterTag.Label);
                writer.Write(displayName);// new MvcHtmlString(HttpUtility.HtmlEncode(displayName)));
                writer.RenderEndTag(); // end Label


                if (required.HasValue && required.Value && !unobtrusiveValitaionAttributes.ContainsKey("data-val-required"))
                {
                    if (!unobtrusiveValitaionAttributes.ContainsKey("data-val"))
                        writer.AddAttribute("data-val", "true");
                    writer.AddAttribute("data-val-required", string.Format("Il campo {0} è obbligatorio", displayName));
                }
                writer.AddAttribute(HtmlTextWriterAttribute.Name, name);
                writer.AddAttribute(HtmlTextWriterAttribute.Id, id ?? helper.AgidIDFor(expression).ToString());
                writer.AddAttribute("aria-labelledby", $"{(id ?? helper.AgidIDFor(expression).ToString())}-label");

                foreach (var attr in unobtrusiveValitaionAttributes)
                {
                    writer.AddAttribute(attr.Key, attr.Value.ToString());
                }
                writer.AddAttribute("aria-required", ((required.HasValue && required.Value) || unobtrusiveValitaionAttributes.ContainsKey("data-val-required")).ToString().ToLower());
                writer.RenderBeginTag(HtmlTextWriterTag.Select);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, $"form-control");

                if (optionLabel != null)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Value, "");
                    writer.RenderBeginTag(HtmlTextWriterTag.Option);
                    writer.Write(optionLabel);
                    writer.RenderEndTag(); // end Option
                }

                foreach (var item in items)
                {
                    //if (item.Selected)
                    //    writer.AddAttribute(HtmlTextWriterAttribute.Selected, "selected");
                    //writer.AddAttribute(HtmlTextWriterAttribute.Value, item.Value);
                    //writer.RenderBeginTag(HtmlTextWriterTag.Option);
                    //writer.Write(item.Text.Trim().Replace("\t", "").Replace("\n", "").Replace("\r", ""));
                    //writer.RenderEndTag(); // end Option

                    // Faccio così, altrimenti il htmltextwriter durante la formattazione aggiunge dei tab che sfasano tutto
                    writer.WriteLine($"<option value='{item.Value}' {(item.Selected ? "selected" : string.Empty)}>{item.Text}</option>");
                }

                writer.RenderEndTag(); // end Select
                writer.Write(helper.ValidationMessageFor(expression, "", new { @class = "text-danger" }).ToHtmlString());

                writer.RenderEndTag(); // end Div
                writer.RenderEndTag(); // end Div
                writer.WriteLine
                    (
                    @"<script>
                        document.addEventListener('DOMContentLoaded', function() 
                            {
                                accessibleAutocomplete.enhanceSelectElement({
                                    selectElement: document.querySelector('#" + (id ?? helper.AgidIDFor(expression).ToString()) + @"'),
                                    showAllValues: true,
                                    defaultValue: '',
                                    autoselect: false,
                                    showNoOptionsFound: false,
                                    dropdownArrow: () => '',})
                            })
                    </script>"
                    );

            }

            return new MvcHtmlString(stringWriter.ToString());
        }

        public static MvcHtmlString AgidAutoCompleteList<TModel>(this HtmlHelper<TModel> helper, string name, string label, SelectList items, object htmlAttributes, string optionLabel = null, string titleLabel = null, bool required = false, string id = null, int labelWidth = 12, int controlWidth = 12, bool disabled = false)
        {
            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "form-group");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                writer.AddAttribute(HtmlTextWriterAttribute.Class, $"select-wrapper{(disabled ? " disabled" : "")}");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                if (!string.IsNullOrEmpty(label))
                    writer.Write(helper.Label(name, label, htmlAttributes: new { @class = $"col-{labelWidth} control-label{(required ? " required" : string.Empty)}", @title = titleLabel, @for = id ?? name }).ToHtmlString());

                writer.AddAttribute(HtmlTextWriterAttribute.Name, name);
                writer.AddAttribute(HtmlTextWriterAttribute.Id, id ?? name);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, $"col-{controlWidth}");
                if (required)
                {
                    writer.AddAttribute("data-val", $"true");
                    writer.AddAttribute("data-val-required", $"Il campo '{label}' è obbligatorio");
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "required");
                }

                if (disabled)
                    writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "");
                writer.AddAttribute("aria-required", required.ToString().ToLower());
                writer.RenderBeginTag(HtmlTextWriterTag.Select);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, $"form-control");

                if (optionLabel != null)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Value, "");
                    writer.RenderBeginTag(HtmlTextWriterTag.Option);
                    writer.Write(optionLabel);
                    writer.RenderEndTag(); // end Option
                }

                foreach (var item in items)
                {
                    //if (item.Selected)
                    //    writer.AddAttribute(HtmlTextWriterAttribute.Selected, "selected");
                    //writer.AddAttribute(HtmlTextWriterAttribute.Value, item.Value);
                    //writer.RenderBeginTag(HtmlTextWriterTag.Option);
                    //writer.Write(item.Text);
                    //writer.RenderEndTag(); // end Option

                    // Faccio così, altrimenti il htmltextwriter durante la formattazione aggiunge dei tab che sfasano tutto
                    writer.WriteLine($"<option value='{item.Value}' {(item.Selected ? "selected" : string.Empty)}>{item.Text}</option>");
                }

                writer.RenderEndTag(); // end Select



                writer.Write(helper.ValidationMessage(name, "", new { @class = "text-danger" }).ToHtmlString());

                writer.RenderEndTag(); // Div

                writer.RenderEndTag(); // Div

                writer.WriteLine
                   (
                   @"<script>
                        document.addEventListener('DOMContentLoaded', function() 
                            {
                                accessibleAutocomplete.enhanceSelectElement({
                                selectElement: document.querySelector('#" + (id ?? name) + @"'),
                                showAllValues: true,
                                defaultValue: '',
                                autoselect: false,
                                showNoOptionsFound: false,
                                dropdownArrow: () => '',})
                            })
                    </script>"
                   );

            }

            return new MvcHtmlString(stringWriter.ToString());
        }
    }
}
