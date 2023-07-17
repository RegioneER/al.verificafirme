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
using System.Xml;

namespace RER.Tools.MVC.Agid
{
    public static partial class ExtensionMethods
    {
        //public static void CaricaDaXmlNewsManager<NewsCruscotto>(this IEnumerable<NewsCruscotto> list, string xml)
        //{
        //    XmlDocument xDoc = new XmlDocument();
        //    xDoc.LoadXml(xml);

        //    var notizie = xDoc.SelectNodes("/ArrayOfNotizia/Notizia");

        //    list = new List<NewsCruscotto>();
        //    foreach (XmlNode n in notizie)
        //    {
        //        DateTime tmpData, tmpVisibileDal, tmpVisibileAl;
        //        bool tmpIsInEvidenza, tmpIsVisibile, tmpIsNew;

        //        NewsCruscotto tmpNews = new NewsCruscotto();

        //        tmpNews.Data = DateTime.TryParse(n.SelectSingleNode("Data").InnerText, out tmpData) ? tmpData : (DateTime?)null;
        //        tmpNews.DataLibera = n.SelectSingleNode("DataLibera").InnerText;
        //        tmpNews.Descrizione = n.SelectSingleNode("Descrizione").InnerText;
        //        tmpNews.IsInEvidenza = bool.TryParse(n.SelectSingleNode("IsInEvidenza").InnerText, out tmpIsInEvidenza) ? tmpIsInEvidenza : (bool?)null;
        //        tmpNews.IsNew = bool.TryParse(n.SelectSingleNode("IsNew").InnerText, out tmpIsNew) ? tmpIsNew : (bool?)null;
        //        tmpNews.IsVisibile = bool.TryParse(n.SelectSingleNode("IsVisibile").InnerText, out tmpIsVisibile) ? tmpIsVisibile : (bool?)null;
        //        tmpNews.VisibileDal = DateTime.TryParse(n.SelectSingleNode("VisibileDal").InnerText, out tmpVisibileDal) ? tmpVisibileDal : (DateTime?)null;
        //        tmpNews.VisibileAl = DateTime.TryParse(n.SelectSingleNode("VisibileAl").InnerText, out tmpVisibileAl) ? tmpVisibileAl : (DateTime?)null;
        //        tmpNews.ID = int.Parse(n.SelectSingleNode("ID").InnerText);
        //        tmpNews.Titolo = n.SelectSingleNode("Titolo").InnerText;

        //        tmpNews.Allegati = new List<AllegatoNews>();
        //        XmlNodeList allegati = n.SelectNodes("Allegati/Allegato");
        //        foreach (XmlNode a in allegati)
        //        {
        //            int tmpDimensione, tmpIDNotizia;
        //            DateTime tmpDataOra;
        //            Guid tmpGuid;

        //            AllegatoNews tmpAllegato = new AllegatoNews
        //            {
        //                GUID = Guid.TryParse(a.SelectSingleNode("GUID").InnerText, out tmpGuid) ? tmpGuid : (Guid?)null,
        //                DataOra = DateTime.TryParse(a.SelectSingleNode("DataOra").InnerText, out tmpDataOra) ? tmpDataOra : (DateTime?)null,
        //                Descrizione = a.SelectSingleNode("Descrizione").InnerText,
        //                Dimensione = int.TryParse(a.SelectSingleNode("GUID").InnerText, out tmpDimensione) ? tmpDimensione : (int?)null,
        //                Formato = a.SelectSingleNode("Formato").InnerText,
        //                ID = int.Parse(a.SelectSingleNode("ID").InnerText),
        //                IDNotizia = int.TryParse(a.SelectSingleNode("IDNotizia").InnerText, out tmpIDNotizia) ? tmpIDNotizia : (int?)null,
        //                NomeFile = a.SelectSingleNode("NomeFile").InnerText
        //            };
        //            tmpNews.Allegati.Add(tmpAllegato);
        //        }
        //        list.Add(tmpNews);
        //    }
        //}

        public static MvcHtmlString AgidNewsCarousel<TModel>(this HtmlHelper<TModel> helper, IEnumerable<NewsCruscotto> news, int maxLunghezzaTesto = 150)
        {
            #region html
            //        < div class="it-carousel-wrapper it-carousel-landscape-abstract-three-cols">
            //    <div class="it-carousel-all owl-carousel it-card-bg">
            //        @foreach(var item in Model.NewsCruscotto.OrderByDescending(x => x.Data).Where(x => x.IsNew.HasValue && x.IsNew.Value))
            //        {
            //            bool oltre200 = item.Descrizione.Length > 200;
            //            <div class="it-single-slide-wrapper">
            //                <div class="card-wrapper">
            //                    <div class="card card-bg">
            //                        <div class="flag-icon"></div>
            //                        <div class="card-body">
            //                            <div class="category-top">
            //                                @*<a class="category" href="#">Category</a>*@
            //                                <span class="category">@(item.Data?.ToString("dd/MM/yyyy"))</span>
            //                            </div>
            //                            <h5 class="card-title big-heading">@item.Titolo</h5>

            //                            @if(oltre200)
            //        {
            //                                < p class="card-text">@Html.Raw(HttpUtility.HtmlDecode($"{item.Descrizione.Substring(0, 200)}<span id='spanPuntini{item.ID}'>...</span><span style='display: none;' id='spanTesto{item.ID}'>{item.Descrizione.Substring(200)}</span>"))</p>
            //                                <br />
            //                                <a class="read-more" href="" onclick="return mostraTesto(@item.ID);">
            //                                    <span class="text" id="spanLinkMostraTesto@(item.ID)">Mostra articolo completo</span>
            //                                    <svg class="icon">
            //                                        <use xlink:href="/bootstrap-italia/dist/svg/sprite.svg#it-arrow-right"></use>
            //                                    </svg>
            //                                </a>
            //                            }
            //                            else
            //                            {
            //                                <p class="card-text">@Html.Raw(HttpUtility.HtmlDecode(item.Descrizione))</p>
            //                            }

            //@if(item.Allegati.Any())
            //                            {
            //                                < i >
            //                                    < p class= "card-text" >
            //                                         Allegati:
            //                                    </ p >
            //                                </ i >
            //                                < ul >
            //                                    @foreach(var doc in item.Allegati)
            //                                    {
            //    var url = Url.Action("ScaricaFileNewsManager", "Home", new { IDDocumento = doc.ID });
            //                                        < li >
            //                                            < a class= "card-text" href = "@url" > @doc.Descrizione(@doc.NomeFile) </ a >


            //                                          </ li >
            //                                    }
            //                                </ ul >
            //                            }
            //                        </ div >
            //                    </ div >
            //                </ div >
            //            </ div >
            //        }
            //    </ div >
            //</ div >
            #endregion

            StringWriter stringWriter = new StringWriter();
            using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
            {
                #region style
                /*
                <style>
                    .card-text {
                        width: 100%;
                        margin-bottom: 5px;
                        padding-bottom: 5px;
                        max-height: 200px;
                        overflow: hidden;
                        text-overflow: ellipsis;
                        content: "";
                        position: relative;
                    }

                        .card-text:before {
                            content: '';
                            width: 100%;
                            height: 100%;
                            position: absolute;
                            left: 0;
                            top: 0;
                            background: linear-gradient(rgba(255,255,255,0) 150px, white);
                        }
                </style>
                */

                //writer.RenderBeginTag(HtmlTextWriterTag.Style);
                //writer.Write(@" .card-text {
                //                    width: 100%;
                //                    margin-bottom: 5px;
                //                    padding-bottom: 5px;
                //                    max-height: 200px;
                //                    overflow: hidden;
                //                    text-overflow: ellipsis;
                //                    content: "";
                //                    position: relative;
                //                }

                //                    .card-text:before {
                //                        content: '';
                //                        width: 100%;
                //                        height: 100%;
                //                        max-height: 200px;
                //                        position: absolute;
                //                        left: 0;
                //                        top: 0;
                //                        background: linear-gradient(rgba(255,255,255,0) 180px, white);
                //                    }");                
                //writer.RenderEndTag(); // end Style
                #endregion

                #region scripts
                /*
                <script>
                   function mostraTesto(itemId) {
                       var spanPuntiniID = "#spanPuntini" + itemId;
                       var spanTestoID = "#spanTesto" + itemId;
                       var spanLinkMostraTestoID = "#spanLinkMostraTesto" + itemId;
                       $(spanPuntiniID).toggle();
                       $(spanTestoID).toggle();
                       if ($(spanTestoID).is(":hidden")) {
                           $(spanLinkMostraTestoID).html("Mostra articolo completo");
                       }
                       else {
                           $(spanLinkMostraTestoID).html("Nascondi testo articolo");
                       }
                       return false;
                   }
               </script>
                */

                writer.RenderBeginTag(HtmlTextWriterTag.Script);
                writer.WriteLine("function mostraTesto(itemId) {");
                writer.WriteLine("    var spanPuntiniID = '#spanPuntini' + itemId;");
                writer.WriteLine("    var spanTestoID = '#spanTesto' + itemId;");
                writer.WriteLine("    var spanLinkMostraTestoID = '#spanLinkMostraTesto' + itemId;");
                writer.WriteLine("    $(spanPuntiniID).toggle();");
                writer.WriteLine("    $(spanTestoID).toggle();");
                writer.WriteLine("    if ($(spanTestoID).is(':hidden')) {");
                writer.WriteLine("       $(spanLinkMostraTestoID).html('Mostra articolo completo');");
                writer.WriteLine("    }");
                writer.WriteLine("    else {");
                writer.WriteLine("       $(spanLinkMostraTestoID).html('Nascondi testo articolo');");
                writer.WriteLine("    }");
                writer.WriteLine("    return false;");
                writer.Write("}");
                writer.RenderEndTag(); // end Script
                #endregion

                writer.AddAttribute(HtmlTextWriterAttribute.Class, $"it-carousel-wrapper it-carousel-landscape-abstract-three-cols");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, $"it-carousel-all owl-carousel it-card-bg");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                foreach (var item in news)
                {
                    bool oltreLunghezzaLimite = item.Descrizione.Length > maxLunghezzaTesto;
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"it-single-slide-wrapper");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"card-wrapper");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"card card-bg");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);

                    // flag per le news nuove
                    if (item.IsNew.HasValue && item.IsNew.Value)
                    {
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, $"flag-icon");
                        writer.RenderBeginTag(HtmlTextWriterTag.Div);
                        writer.RenderEndTag(); // end Div
                    }

                    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"card-body");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);

                    // category
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"category-top");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"category");
                    writer.RenderBeginTag(HtmlTextWriterTag.Span);
                    writer.Write(item.Data?.ToString("dd/MM/yyyy"));
                    writer.RenderEndTag(); // end Span class = 'cetegory'
                    writer.RenderEndTag(); // end Div class = 'category-top'

                    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"card-title big-heading");
                    writer.RenderBeginTag(HtmlTextWriterTag.H5);
                    writer.Write(item.Titolo);
                    writer.RenderEndTag(); // end H5

                    // TODO - tagliare ilt esto causa problemi, visto che il testoc ontiene dell'html. Bisogna capire come gestirlo altrimenti si sfasa tutto quando si taglia.
                    // L'ideale sarebbe trovare un modo per farlo con css / html evitando quindi di dover intervenire sul testo
                    //if (oltreLunghezzaLimite)
                    //{
                    //    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"card-text");
                    //    writer.RenderBeginTag(HtmlTextWriterTag.P);

                    //    writer.Write(HttpUtility.HtmlDecode(item.Descrizione.Substring(0, maxLunghezzaTesto)));

                    //    writer.AddAttribute(HtmlTextWriterAttribute.Id, $"spanPuntini{item.ID}");
                    //    writer.RenderBeginTag(HtmlTextWriterTag.Span);
                    //    writer.Write("...");
                    //    writer.RenderEndTag(); // end Span

                    //    writer.AddAttribute(HtmlTextWriterAttribute.Style, $"display: none;");
                    //    writer.AddAttribute(HtmlTextWriterAttribute.Id, $"spanTesto{item.ID}");
                    //    writer.RenderBeginTag(HtmlTextWriterTag.Span);
                    //    writer.Write(HttpUtility.HtmlDecode(item.Descrizione.Substring(maxLunghezzaTesto)));
                    //    writer.RenderEndTag(); // end Span

                    //    writer.RenderEndTag(); // end P

                    //    writer.WriteBreak();

                    //    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"read-more");
                    //    writer.AddAttribute(HtmlTextWriterAttribute.Href, $"");
                    //    writer.AddAttribute(HtmlTextWriterAttribute.Onclick, $"return mostraTesto({item.ID})");
                    //    writer.RenderBeginTag(HtmlTextWriterTag.A);

                    //    writer.AddAttribute(HtmlTextWriterAttribute.Id, $"spanLinkMostraTesto{item.ID}");
                    //    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"text");
                    //    writer.RenderBeginTag(HtmlTextWriterTag.Span);
                    //    writer.Write("Mostra articolo completo");
                    //    writer.RenderEndTag(); // end Span

                    //    writer.AddAttribute(HtmlTextWriterAttribute.Class, $"icon");
                    //    writer.RenderBeginTag("svg");

                    //    writer.AddAttribute($"xlink:href", "/bootstrap-italia/dist/svg/sprite.svg#it-arrow-right");
                    //    writer.RenderBeginTag("use");
                    //    writer.RenderEndTag(); // end Div

                    //    writer.RenderEndTag(); // end svg

                    //    writer.RenderEndTag(); // end A
                    //}
                    //else
                    //{
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, $"card-text");
                        writer.RenderBeginTag(HtmlTextWriterTag.Div);
                        writer.Write(HttpUtility.HtmlDecode(item.Descrizione));
                        writer.RenderEndTag(); // end Div
                    //}

                    if (item.Allegati.Any())
                    {
                        writer.WriteBreak();
                        writer.AddAttribute(HtmlTextWriterAttribute.Class, $"card-text");
                        writer.RenderBeginTag(HtmlTextWriterTag.P);
                        writer.RenderBeginTag(HtmlTextWriterTag.I);
                        writer.Write("Allegati: ");
                        writer.RenderEndTag(); // end I
                        writer.RenderEndTag(); // end P

                        writer.RenderBeginTag(HtmlTextWriterTag.Ul);
                        foreach (var allegato in item.Allegati)
                        {
                            UrlHelper uHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);

                            writer.RenderBeginTag(HtmlTextWriterTag.Li);
                            writer.AddAttribute(HtmlTextWriterAttribute.Class, $"card-text");
                            writer.AddAttribute(HtmlTextWriterAttribute.Href, uHelper.Action("ScaricaFileNewsManager", "Home", new { IDDocumento = allegato.ID }));
                            writer.RenderBeginTag(HtmlTextWriterTag.A);
                            writer.Write($"{allegato.Descrizione}({allegato.NomeFile})");
                            writer.RenderEndTag(); // end A
                            writer.RenderEndTag(); // end Li

                        }
                        writer.RenderEndTag(); // end Ul
                    }

                    writer.RenderEndTag(); // end Div class = 'card-body'

                    writer.RenderEndTag(); // end Div class = 'card card-bg'
                    writer.RenderEndTag(); // end Div class = 'card-wrapper'
                    writer.RenderEndTag(); // end Div class = 'it-single-slide-wrapper'
                }

                writer.RenderEndTag(); // end Div class = 'it-carousel-all owl-carousel it-card-bg'
                writer.RenderEndTag(); // end Div class = 'it-carousel-wrapper it-carousel-landscape-abstract-three-cols'


                writer.WriteLineNoTabs("");
                writer.WriteLineNoTabs("");
            }

            return new MvcHtmlString(stringWriter.ToString());

        }
    }
}
