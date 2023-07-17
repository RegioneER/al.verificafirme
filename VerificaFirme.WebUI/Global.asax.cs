using VerificaFirme.Db;
using VerificaFirme.WebUI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;

namespace VerificaFirme.WebUI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected string CurrentAppName
        {
            get
            {
                return GetType().Namespace;
            }
        }


        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            SetCustomViewFolders();

            MvcHandler.DisableMvcResponseHeader = true;
        }

        private void SetCustomViewFolders()
        {
            // Add custom folders to the default location scheme for PARTIAL Views
            var razorEngine = ViewEngines.Engines.OfType<RazorViewEngine>().FirstOrDefault();
            razorEngine.PartialViewLocationFormats =
                razorEngine.PartialViewLocationFormats.Concat(new string[] {
                    "~/Views/{1}/Partial/Scripts/{0}.cshtml",
                    "~/Views/{1}/Partial/{0}.cshtml",
                }).ToArray();
        }

        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            if (Context.Request.ServerVariables["SCRIPT_NAME"].Contains("Errore"))
                return;
            if (Context.Request.ServerVariables["SCRIPT_NAME"].Contains("UtenteNonAutorizzato"))
                return;

            if (Context.Handler is IRequiresSessionState || Context.Handler is IReadOnlySessionState)
            {
                if (!RERIAMPrincipal.Corrente.IsAttivo)
                    throw new UnauthorizedAccessException("Utente non autorizzato");

                Context.User = RERIAMPrincipal.Corrente;
            }
        }
        protected void Application_Error()
        {
            Exception exception = Server.GetLastError();
            if (exception != null && exception.InnerException != null)
                exception = exception.InnerException;
            Server.ClearError();
#if (DEBUG)
            Session["MostraErrore"] = exception.Message;
#endif
            try
            {

#if (!DEBUG)
                //TODO chiamare il proprio log di errori
#endif               
            }
            catch (Exception ex)
            {
                // Se non sono riuscito a loggare non mi resta che mostrare l'errore anche all'utente
                //throw (ex);
                Session["EccezioneDaVisualizzare"] = Session["EccezioneDaVisualizzare"] ?? ex;
            }

            //Server.ClearError();
            if (exception is UnauthorizedAccessException)
                Response.Redirect("~/UtenteNonAutorizzato.htm", true);
            else if (exception is ApplicationException)
                Response.Redirect("~/Errore/", true);
            else if (exception is ArgumentException)
                Response.Redirect("~/Errore/", true);
            else
                Response.Redirect("~/ErroreSistema.htm", true);

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Response.AddHeader("x-frame-options", "DENY");
        }
    }
}
