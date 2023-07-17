using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
    [Serializable]
    public class BreadCrumbItem
    {
        public string LinkText { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public object RouteValues { get; set; }
        /// <summary>
        /// Campo alternativo alla tupla Action-controller-routevalues. Se presente, viene usato questo
        /// </summary>
        public string ActionLink { get; set; }
        public BreadCrumbItem()
        {
            RouteValues = null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static partial class ExtensionMethods
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="actionHome"></param>
        /// <param name="bcItems"></param>
        /// <param name="creaUltimoElemento">Indica se l'ultimo elemento deve essere visualizzato. Nel caso sia true, l'ultimo elemento sarà creato, ma non sarà cliccabile</param>
        /// <returns></returns>
        public static string BuildAgidBreadcrumbNavigation(this HtmlHelper<dynamic> helper, string actionHome, List<BreadCrumbItem> bcItems, bool creaUltimoElemento = false)
        {
            try
            {
                StringWriter stringWriter = new StringWriter();
                using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
                {
                    int counter = 1;
                    foreach (var item in bcItems)
                    {
                        if (counter < bcItems.Count())
                        {
                            writer.RenderBeginTag(HtmlTextWriterTag.Li);
                            writer.Write($"&nbsp;/&nbsp;");
                            if (!string.IsNullOrWhiteSpace(item.ActionLink))
                            {
                                writer.AddAttribute(HtmlTextWriterAttribute.Href, item.ActionLink);
                                writer.RenderBeginTag(HtmlTextWriterTag.A);
                                writer.Write(item.LinkText);
                                writer.RenderEndTag(); // end a
                            }
                            else
                            {
                                writer.Write(helper.ActionLink(item.LinkText, item.ActionName, item.ControllerName, item.RouteValues, null).ToHtmlString());
                            }
                            writer.RenderEndTag(); // end I
                        }
                        else if(creaUltimoElemento)
                        {
                            writer.RenderBeginTag(HtmlTextWriterTag.Li);
                            writer.Write($"&nbsp;/&nbsp;");
                            writer.Write(item.LinkText);
                            writer.RenderEndTag(); // end I
                        }
                        counter++;
                    }
                }

                return stringWriter.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

    }
}
