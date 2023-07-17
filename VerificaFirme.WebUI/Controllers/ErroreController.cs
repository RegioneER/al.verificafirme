using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VerificaFirme.WebUI.Controllers
{
    public class ErroreController : BaseController
    {
        // GET: Errore
        public ActionResult Index()
        {
            Exception ex = (Exception)Session["EccezioneDaVisualizzare"];
            if (ex != null && IsDebug())
                return View("Errore", ex);
            else
                return View("ErroreGenerico");
        }

        public ActionResult Errore()
        {
            Exception ex = (Exception)Session["EccezioneDaVisualizzare"];
            return View(ex);
        }
        public ActionResult ErroreGenerico()
        {
                return View();
        }
    }
}