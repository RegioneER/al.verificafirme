using VerificaFirme.Db;
using VerificaFirme.WebUI.Helper;
using VerificaFirme.WebUI.Models;
using RER.Tools.MVC.Agid;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;

namespace VerificaFirme.WebUI.Controllers
{
    public class ErroreInterno : Exception
    {
        public ErroreInterno(string message) : base(message)
        {

        }
    }

    public class BaseController : Controller
    {

        private bool AbilitaSalvataggioAutomatico = ConfigurazioneManager.Configurazione<bool>("ASA");
        protected int SecondiSalvataggioAutomatico = ConfigurazioneManager.Configurazione<int>("SSA");
        protected int nrMaxRisultatiAutocomplete = ConfigurazioneManager.Configurazione<int>("NMRA");
        protected int nrMinimoCaratteriAutocomplete = ConfigurazioneManager.Configurazione<int>("NMCA");
        protected int MaxNumeroRigheModulo = ConfigurazioneManager.Configurazione<int>("MNRM");
        protected int MaxQuorum = ConfigurazioneManager.Configurazione<int>("MQP");
        protected int MaxPostazioniProcedimento = ConfigurazioneManager.Configurazione<int>("MPP");
        protected int MaxModuliProcedimento = ConfigurazioneManager.Configurazione<int>("MMP");
        protected int MinAnnoFirmatario = ConfigurazioneManager.Configurazione<int>("MAF");
        protected string NomeApplicativo = ConfigurazioneManager.Configurazione<string>("APN");



        protected VerificaFirmeDBContext db = null;
        public enum TipoAzione { Ricerca, Risultati }

        public ActionResult NavigaMenu(string c, string a)
        {
            HttpContext.Session["briciole"] = new List<BreadCrumbItem>();
            return RedirectToAction(a, c);
        }

        /// <summary>
        /// Questo metodo viene eseguito a ogni azione, ultile per verificare i permessi
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewBag.MaxQuorum = this.MaxQuorum;
            ViewBag.MaxPostazioniProcedimento = this.MaxPostazioniProcedimento;
            ViewBag.MaxModuliProcedimento = this.MaxModuliProcedimento;
            ViewBag.MinAnnoFirmatario = this.MinAnnoFirmatario;
            Regioni r = db.Regioni.Find(ConfigurazioneManager.Configurazione<string>("CR"));
            ViewBag.NomeRegione = r != null ? r.Descrizione : NomeApplicativo;
            ViewBag.NomeApplicativo = this.NomeApplicativo;

            //Guid guid = Guid.NewGuid();
            //if (string.IsNullOrEmpty(filterContext.HttpContext.Request["sessionIDBC"]))
            //    ViewBag.sessionIDBC = guid.ToString();
            //else
            //    ViewBag.sessionIDBC = filterContext.HttpContext.Request["sessionIDBC"];

            BreadcrumbExtension.AddBreadcrumb(filterContext);

            string methodName = filterContext.HttpContext.Request.HttpMethod.ToUpper();
            string actionName = filterContext.ActionDescriptor.ActionName;
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

            var utente = RERIAMPrincipal.Corrente;

            // se è amminsitratore, non ha senso fare controlli ulteriori, visto che può fare tutto
            if (!utente.IsInRole(RuoloUtente.Amministatore))
            {
                switch (controllerName)
                {
                    case "Report":
                        switch (actionName)
                        {
                            case "Index":
                                break;
                            case "AnagraficheDuplicate":

                                object tmpIdProcedimento = null;
                                filterContext.ActionParameters.TryGetValue("IdProcedimento", out tmpIdProcedimento);
                                if (tmpIdProcedimento != null)
                                {
                                    int idProc = (int)tmpIdProcedimento;
                                    if (!UtenteAbilitatoALProcedimento(idProc))
                                    {
                                        throw new UnauthorizedAccessException("Utente non autorizzato al procedimento corrente");
                                    }
                                }
                                break;
                            case "RiepilogoDataEntry":
                                tmpIdProcedimento = null;
                                filterContext.ActionParameters.TryGetValue("IdProcedimento", out tmpIdProcedimento);
                                if (tmpIdProcedimento != null)
                                {
                                    int idProc = (int)tmpIdProcedimento;
                                    if (!UtenteAbilitatoALProcedimento(idProc))
                                    {
                                        throw new UnauthorizedAccessException("Utente non autorizzato al procedimento corrente");
                                    }
                                }
                                break;
                            case "EsportaRiepilogoDataEntry":
                                tmpIdProcedimento = null;
                                filterContext.ActionParameters.TryGetValue("IdProcedimento", out tmpIdProcedimento);
                                if (tmpIdProcedimento != null)
                                {
                                    int idProc = (int)tmpIdProcedimento;
                                    if (!UtenteAbilitatoALProcedimento(idProc))
                                    {
                                        throw new UnauthorizedAccessException("Utente non autorizzato al procedimento corrente");
                                    }
                                }
                                break;
                        }
                        break;



                    case "Modulo":
                        switch (actionName)
                        {
                            case "Index":
                                break;
                            case "Dettaglio":
                                int idProc = (int)filterContext.ActionParameters["IdProcedimento"];
                                if (!UtenteAbilitatoALProcedimento(idProc))
                                {
                                    throw new UnauthorizedAccessException("Utente non autorizzato al procedimento corrente");
                                }
                                break;
                            case "Compila":
                                int idModulo = (int)filterContext.ActionParameters["IdModulo"];
                                if (!UtenteAbilitatoALModulo(idModulo))
                                {
                                    throw new UnauthorizedAccessException("Utente non autorizzato al modulo corrente");
                                }
                                break;
                            case "Modifica":
                                idProc = (int)filterContext.ActionParameters["ID"];
                                if (!UtenteAbilitatoALProcedimento(idProc))
                                {
                                    throw new UnauthorizedAccessException("Utente non autorizzato al procedimento corrente");
                                }
                                break;
                        }
                        break;
                    case "ControlloAmministrativo":
                        // Il controllo amministrativo deve essere abilitato solo ad admin e supervisori. In tutti glia ltri casi, restituisco errore
                        if (utente.IsInRole(RuoloUtente.Supervisore))
                        {
                            switch (actionName)
                            {
                                case "Index":
                                    break;
                                case "Dettaglio":
                                    int idProc = (int)filterContext.ActionParameters["IdProcedimento"];
                                    if (!UtenteAbilitatoALProcedimento(idProc))
                                    {
                                        throw new UnauthorizedAccessException("Utente non autorizzato al procedimento corrente");
                                    }
                                    break;
                                case "Modifica":
                                    idProc = (int)filterContext.ActionParameters["ID"];
                                    if (!UtenteAbilitatoALProcedimento(idProc))
                                    {
                                        throw new UnauthorizedAccessException("Utente non autorizzato al procedimento corrente");
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            throw new UnauthorizedAccessException("Utente non autorizzato all'operazione e/o risorsa richiesta");
                        }
                        break;
                    case "Procedimento":
                        switch (actionName)
                        {
                            case "ProspettoRiepilogoModulo":
                                int idProc = db.Modulo.Find((int)filterContext.ActionParameters["idModulo"]).IDProcedimento;
                                if (!UtenteAbilitatoALProcedimento(idProc))
                                {
                                    throw new UnauthorizedAccessException("Utente non autorizzato al procedimento corrente");
                                }
                                break;
                            case "ProspettoRiepilogo":
                            case "ProspettoRiepilogoDettaglioPerModulo":
                                idProc = (int)filterContext.ActionParameters["IdProcedimento"];
                                if (!UtenteAbilitatoALProcedimento(idProc))
                                {
                                    throw new UnauthorizedAccessException("Utente non autorizzato al procedimento corrente");
                                }
                                break;
                            default:
                                // solo l'admin può
                                throw new UnauthorizedAccessException("Utente non autorizzato all'operazione e/o risorsa richiesta");
                        }
                        break;
                }
            }
        }

        protected override void Initialize(RequestContext requestContext)
        {
            CultureInfo it = new CultureInfo("it");
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = it;

            if (db != null)
                db.Dispose();

            db = new VerificaFirmeDBContext();
            base.Initialize(requestContext);
        }

        public bool IsDebug()
        {
#if (!DEBUG)
            return false;
#endif
            return true;
        }

        public bool UtenteAbilitatoALProcedimento(int IdProcedimento)
        {
            RERIAMPrincipal utente = RERIAMPrincipal.Corrente;
            if (utente.IsInRole(RuoloUtente.Amministatore))
                return true;
            else
                return db.Utente.Any(x => x.Username == utente.Username && x.Procedimento.Any(y => y.ID == IdProcedimento && y.CodStato != "CON"));
        }

        public bool UtenteAbilitatoALModulo(int IdModulo)
        {

            RERIAMPrincipal utente = RERIAMPrincipal.Corrente;
            if (utente.IsInRole(RuoloUtente.Amministatore))
                return true;
            else
                return db.Utente.Any(x => x.Username == utente.Username && x.Procedimento.Any(y => y.CodStato != "CON" && y.ProcedimentoPostazione.Any(z => z.Modulo.Any(w => w.ID == IdModulo))));
        }

        public void LoggaEccezione(Exception ex, MvcApplication app)
        {
            if (!IsDebug())
            {
                if (app == null)
                    app = System.Web.HttpContext.Current.ApplicationInstance as MvcApplication;
                if (ex != null)
                {
                    //TODO chiamare il proprio log di errori
                }
            }

        }

        // (1 = A, 2 = B...27 = AA...703 = AAA...)
        protected static string GetLeteraColonnaDaIndice(int nrColonna)
        {
            int d = nrColonna;
            string letteraColonna = String.Empty;
            int mod;

            while (d > 0)
            {
                mod = (d - 1) % 26;
                letteraColonna = Convert.ToChar(65 + mod).ToString() + letteraColonna;
                d = (int)((d - mod) / 26);
            }

            return letteraColonna;
        }

        // (A = 1, B = 2...AA = 27...AAA = 703...)
        protected static int GetIndiceColonnaDaLettera(string letteraColonna)
        {
            char[] car = letteraColonna.ToUpperInvariant().ToCharArray();
            int somma = 0;
            for (int i = 0; i < car.Length; i++)
            {
                somma *= 26;
                somma += (car[i] - 'A' + 1);
            }
            return somma;
        }
    }
}