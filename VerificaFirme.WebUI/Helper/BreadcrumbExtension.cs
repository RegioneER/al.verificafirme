using RER.Tools.MVC.Agid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.UI;
using System.Linq;
using VerificaFirme.Db;
using System.Web;
using System.Web.SessionState;

namespace VerificaFirme.WebUI.Helper
{
    [Serializable]
    public class MyRoute
    {
        public int? IdProcedimento { get; set; }
        public int? idModulo { get; set; }

        public MyRoute() { }
    }

    public static class BreadcrumbExtension
    {
        private static string GetActionDesc(string controller, string action)
        {

            switch (controller)
            {
                case "Home":
                    switch (action)
                    {
                        case "":
                        case "Index":
                            return "Home";
                        default:
                            return "";
                    }
                case "Modulo":
                    switch (action)
                    {
                        case "":
                        case "Index":
                            return "Data entry";
                        case "Compila":
                            return "Compilazione modulo";
                        case "Dettaglio":
                            return "Dettaglio procedimento";
                        default:
                            return "";
                    }
                case "Procedimento":
                    switch (action)
                    {
                        case "":
                        case "Index":
                            return "Configurazione procedimenti";
                        case "Cerca":
                            return "Configurazione procedimenti";
                        case "Crea":
                            return "Crea procedimento";
                        case "ProspettoRiepilogo":
                            return "Prospetto riepilogo";
                        case "Modifica":
                            return "Configura procedimento";
                        case "AbilitazioneUtenti":
                        case "CercaAbilitazioneUtenti":
                            return "Abilitazione utenti";
                        default:
                            return "";
                    }
                case "ControlloAmministrativo":
                    switch (action)
                    {
                        case "":
                        case "Index":
                            return "Controllo amm. - Ricerca procedimenti";
                        case "Dettaglio":
                            return "Controllo amm. - Data Entry";
                        case "Compila":
                            return "Controllo amm. - Compilazione modulo";
                        default:
                            return "";
                    }
                case "Report":
                    switch (action)
                    {
                        case "":
                        case "Index":
                            return "Verifiche";
                        case "AnagraficheDuplicate":
                            return "Gestione anagrafiche";
                        case "RiepilogoDataEntry":
                            return "Riepilogo data entry";
                        default:
                            return "";
                    }
                default:
                    return "";
            }
        }

        static public void AddBreadcrumb(ActionExecutingContext filterContext)
        {
            HttpSessionState httpSession = HttpContext.Current.Session;
            var controller = (Controller)filterContext.Controller;

            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string[] controllerNoBC = new string[] { "Base", "Home" };

            if (!controllerNoBC.Contains(controllerName))
            {
                string methodName = filterContext.HttpContext.Request.HttpMethod.ToUpper();
                string actionName = filterContext.ActionDescriptor.ActionName;
                string urlCorrente = HttpContext.Current.Request.RawUrl;

                if ((controllerName == "Modulo" || controllerName == "ControlloAmministrativo") && (actionName == "CreaModulo" || actionName == "SalvaOperazioniSulModulo"))
                {
                    // questa action non va aggiunta alle briciole. Da un redirect to action che causerebbe una doppia breadcrumb
                }
                else
                {

                    List<BreadCrumbItem> briciole = new List<BreadCrumbItem>();
                    if (httpSession["briciole"] != null)
                    {
                        briciole = (List<BreadCrumbItem>)httpSession["briciole"];
                    }
                    else
                    {
                        briciole = RecreateBreadcrumbs(filterContext);
                    }

                    if (controllerName == "Procedimento" && actionName == "Modifica")
                    {
                        briciole.RemoveAll(x => x.ControllerName == "Procedimento" && x.ActionName == "Crea");
                    }

                    if (briciole.Any() && briciole.Last().ControllerName == controllerName && briciole.Last().ActionName == actionName)
                    {
                        briciole.RemoveAt(briciole.Count - 1);
                    }

                    //sono tornato a una pagina precedente, cancello le BC da quella pagina in poi
                    var briciola = briciole.FirstOrDefault(x => x.ActionLink == urlCorrente || (x.ControllerName == controllerName && x.ActionName == actionName) || (actionName == "Compila" && controllerName == "Modulo" && x.ControllerName == "ControlloAmministrativo" && x.ActionName == "Compila"));
                    if (briciola != null)
                    {
                        int indice = briciole.IndexOf(briciola);
                        briciole.RemoveRange(indice, (briciole.Count() - (indice)));
                    }

                    string link = GetActionDesc(controllerName, actionName);
                    if (!string.IsNullOrEmpty(link))
                        briciole.Add(new BreadCrumbItem { LinkText = link, ActionName = actionName, ControllerName = controllerName, ActionLink = urlCorrente });

                    httpSession["briciole"] = briciole;
                }
            }
        }

        static private List<BreadCrumbItem> RecreateBreadcrumbs(ActionExecutingContext filterContext)
        {
            List<BreadCrumbItem> bcItems = new List<BreadCrumbItem>();
            string controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string action = filterContext.ActionDescriptor.ActionName;

            int idProcedimento;
            object tmpIdProcedimento = null;
            filterContext.ActionParameters.TryGetValue("IdProcedimento", out tmpIdProcedimento);
            if (tmpIdProcedimento != null)
            {
                idProcedimento = (int)tmpIdProcedimento;
            }
            else
            {
                filterContext.ActionParameters.TryGetValue("IDProcedimento", out tmpIdProcedimento);
                if (tmpIdProcedimento != null)
                {
                    idProcedimento = (int)tmpIdProcedimento;
                }
                else
                {
                    idProcedimento = 0;
                }
            }

            int IDModulo;
            object tmpIdModulo = null;
            if (filterContext.ActionParameters.TryGetValue("IDModulo", out tmpIdModulo))
            {
                IDModulo = (int)tmpIdModulo;
            }
            else
            {
                if (filterContext.ActionParameters.TryGetValue("IdModulo", out tmpIdModulo))
                {
                    IDModulo = (int)tmpIdModulo;
                }
                else
                {
                    IDModulo = 0;
                }
            }

            switch (controller)
            {
                case "Procedimento":
                    switch (action)
                    {
                        case "AbilitazioneUtenti":
                            break;
                        case "Index":
                            break;
                        case "Dettagli":
                            bcItems.Add(new BreadCrumbItem { ControllerName = "Procedimento", ActionName = "Cerca", LinkText = "Configurazione procedimenti" });
                            break;
                        case "Modifica":
                            bcItems.Add(new BreadCrumbItem { ControllerName = "Procedimento", ActionName = "Cerca", LinkText = "Configurazione procedimenti" });
                            break;
                        case "Crea":
                            bcItems.Add(new BreadCrumbItem { ControllerName = "Procedimento", ActionName = "Cerca", LinkText = "Configurazione procedimenti" });
                            break;
                        case "ProspettoRiepilogo":
                            bcItems.Add(new BreadCrumbItem { ControllerName = "Modulo", ActionName = "Index", LinkText = "Cerca procedimento" });
                            break;
                        case "ProspettoRiepilogoCA":
                            bcItems.Add(new BreadCrumbItem { ControllerName = "ControlloAmministrativo", ActionName = "Index", LinkText = "Cerca procedimento" });
                            break;
                        case "ProspettoRiepilogoDettaglioCA":
                            bcItems.Add(new BreadCrumbItem { ControllerName = "ControlloAmministrativo", ActionName = "Index", LinkText = "Cerca procedimento" });
                            bcItems.Add(new BreadCrumbItem { ControllerName = "ControlloAmministrativo", ActionName = $"Dettaglio", RouteValues = new MyRoute { IdProcedimento = idProcedimento }, LinkText = "Dettaglio procedimento" });
                            break;
                        case "ProspettoRiepilogoDettaglioPerModulo":
                            bcItems.Add(new BreadCrumbItem { ControllerName = "Modulo", ActionName = "Index", LinkText = "Cerca procedimento" });
                            bcItems.Add(new BreadCrumbItem { ControllerName = "Modulo", ActionName = $"Dettaglio", RouteValues = new MyRoute { IdProcedimento = idProcedimento }, LinkText = "Dettaglio procedimento" });
                            break;
                        case "ProspettoRiepilogoModulo":
                            bcItems.Add(new BreadCrumbItem { ControllerName = "Modulo", ActionName = "Index", LinkText = "Cerca procedimento" });
                            using (VerificaFirmeDBContext db = new VerificaFirmeDBContext())
                            {
                                idProcedimento = db.Modulo.First(c => c.ID == IDModulo).IDProcedimento;
                                bcItems.Add(new BreadCrumbItem { ControllerName = "Modulo", ActionName = $"Dettaglio", RouteValues = new MyRoute { IdProcedimento = idProcedimento }, LinkText = "Dettaglio procedimento" });
                            }

                            bcItems.Add(new BreadCrumbItem { ControllerName = "Modulo", ActionName = $"Compila", RouteValues = new MyRoute { idModulo = IDModulo }, LinkText = "Compilazione modulo" });
                            break;
                    }
                    break;
                case "Home":
                    switch (action)
                    {
                        case "Cerca":
                        case "RicercaProcedimento":
                            bcItems.Add(new BreadCrumbItem { ControllerName = controller, ActionName = "RicercaProcedimenti", LinkText = "Ricerca procedimento" });
                            break;
                    }
                    break;
                case "Modulo":
                    switch (action)
                    {
                        case "Index":
                            break;
                        case "Dettaglio":
                            bcItems.Add(new BreadCrumbItem { ControllerName = "Modulo", ActionName = "Index", LinkText = "Data entry" });
                            break;
                        case "Compila":
                            using (VerificaFirmeDBContext db = new VerificaFirmeDBContext())
                            {
                                idProcedimento = db.Modulo.First(c => c.ID == IDModulo).IDProcedimento;
                                bcItems.Add(new BreadCrumbItem { ControllerName = "Modulo", ActionName = "Index", LinkText = "Data entry" });
                                bcItems.Add(new BreadCrumbItem { ControllerName = "Modulo", ActionName = $"Dettaglio", RouteValues = new MyRoute { IdProcedimento = idProcedimento }, LinkText = "Dettaglio procedimento" });
                            }
                            break;
                    }
                    break;
                case "Report":
                    switch (action)
                    {
                        case "AnagraficheDuplicate":
                            bcItems.Add(new BreadCrumbItem { ControllerName = "Report", ActionName = "Index", LinkText = "Verifiche" });
                            break;
                        case "RiepilogoDataEntry":
                            bcItems.Add(new BreadCrumbItem { ControllerName = "Report", ActionName = "Index", LinkText = "Verifiche" });
                            break;
                    }
                    break;
                case "ControlloAmministrativo":
                    switch (action)
                    {

                        case "Index":
                            break;
                        case "Dettaglio":
                            bcItems.Add(new BreadCrumbItem { ControllerName = "ControlloAmministrativo", ActionName = "Index", LinkText = "Cerca procedimento" });
                            break;
                        case "Compila":
                            using (VerificaFirmeDBContext db = new VerificaFirmeDBContext())
                            {
                                idProcedimento = db.Modulo.First(c => c.ID == IDModulo).IDProcedimento;
                                bcItems.Add(new BreadCrumbItem { ControllerName = "ControlloAmministrativo", ActionName = "Index", LinkText = "Cerca procedimento" });
                                bcItems.Add(new BreadCrumbItem { ControllerName = "ControlloAmministrativo", ActionName = $"Dettaglio", RouteValues = new MyRoute { IdProcedimento = idProcedimento }, LinkText = "Dettaglio procedimento" });
                            }
                            break;
                        default:
                            break;
                    }
                    break;
            }

            return bcItems;

        }

        public static string BuildAgidBreadcrumbNavigationFirmeSessione(this HtmlHelper<dynamic> helper, string actionHome, int? IDModulo = null, int? idProcedimento = null, bool creaUltimoElemento = false)
        {
            try
            {
                HttpSessionState httpSession = HttpContext.Current.Session;
                List<BreadCrumbItem> bcItems = new List<BreadCrumbItem>();

                if (httpSession["briciole"] != null)
                {
                    bcItems = (List<BreadCrumbItem>)httpSession["briciole"];
                }

                StringWriter stringWriter = new StringWriter();
                using (HtmlTextWriter writer = new HtmlTextWriter(stringWriter))
                {
                    writer.Write(helper.BuildAgidBreadcrumbNavigation(actionHome, bcItems, creaUltimoElemento));
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