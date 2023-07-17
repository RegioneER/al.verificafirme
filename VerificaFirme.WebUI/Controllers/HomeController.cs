using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using VerificaFirme.Db;
using Newtonsoft.Json;
using RER.Tools.MVC.Agid;

namespace VerificaFirme.WebUI.Controllers
{
    public class HomeController : BaseController
    {
        
        public ActionResult Index()
        {
            ViewBag.GuidaInLinea = ConfigurazioneManager.Configurazione<string>("GIL");
            return View();
        }

        public ActionResult Esci()
        {
            System.Web.Security.FormsAuthentication.SignOut();
            Session.Abandon();
            string redirectLogout = ConfigurazioneManager.Configurazione<string>("LGO");
            return Redirect(redirectLogout);
        }
    }
}


