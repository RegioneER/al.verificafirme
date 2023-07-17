using VerificaFirme.Db;
using VerificaFirme.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace VerificaFirme.WebUI.Controllers
{
    public class ProcedimentoController : BaseController
    {
        // GET: Procedimento
        public ActionResult Index()
        {
            return RedirectToAction("Cerca");
        }

        public ActionResult Cerca(int? IdProcedimento, string CodStato)
        {
            var query = db.Procedimento.AsQueryable();

            if (IdProcedimento.HasValue)
            {
                query = query.Where(x => x.ID == IdProcedimento.Value);
            }
            if (!string.IsNullOrWhiteSpace(CodStato))
            {
                query = query.Where(x => x.CodStato == CodStato);
            }

            var model = new CercaProcedimentoViewModel
            {
                CodStato = CodStato,
                IdProcedimento = IdProcedimento,
                RisultatiRicerca = query.OrderByDescending(x => x.DataOraModifica).ThenBy(x => x.Descrizione).ToList()
            };

            if (TempData["Alerts"] != null)
            {
                model.Alerts = (List<Alert>)TempData["Alerts"];
            }

            return View(model);
        }

        public async Task<ActionResult> Modifica(int? ID)
        {
            if (ID.HasValue)
            {
                var procedimento = await db.Procedimento.Include(x=>x.Utente).FirstOrDefaultAsync(x=>x.ID == ID.Value);
                if (procedimento == null)
                    return RedirectToAction("Cerca");

                procedimento.NumeroModuliOriginale = procedimento.NumeroModuli;
                procedimento.NumeroPostazioniOriginale = procedimento.NumeroPostazioni;

                if (TempData["Alerts"] != null)
                {
                    procedimento.Alerts = (List<Alert>)TempData["Alerts"];
                }

                return View(procedimento);
            }
            else
            {
                return RedirectToAction("Cerca");
            }
        }

        public ActionResult Crea()
        {
            Procedimento p = new Procedimento
            {
                ID = 0,
                CodStato = "CRE"
            };

            return View("Modifica", p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Modifica(Procedimento model)
        {
            if (model.ID == 0)
            {
                ModelState.Remove(nameof(model.CodStato));
                ModelState.Remove(nameof(model.ID));
            }

            if (ModelState.IsValid)
            {
                Procedimento procedimento;
                RERIAMPrincipal utente = RERIAMPrincipal.Corrente;

                if (model.ID == 0)
                {
                    if (model.NumeroModuli < model.NumeroPostazioni)
                    {
                        model.Alerts.Add(new Alert { Title = "Operazione NON eseguita!", AlertType = Alert.AlertTypeEnum.Warning, Messages = new List<string> { "Il numero delle postazioni non deve essere maggiore del numero dei moduli" } });
                        return View(model);
                    }

                    procedimento = new Procedimento
                    {
                        AnnoFirmatario = model.AnnoFirmatario,
                        CodStato = "CRE",
                        Descrizione = model.Descrizione,
                        NumeroModuli = model.NumeroModuli,
                        NumeroPostazioni = model.NumeroPostazioni,
                        QuorumFirme = model.QuorumFirme,
                        DataOraCreazione = DateTime.Now,
                        DataOraModifica = DateTime.Now,
                        UsernameCreazione = utente.Username,
                        UsernameModifica = utente.Username
                    };

                    RidistribuisciModulo(model, procedimento);
                    db.Procedimento.Add(procedimento);
                }
                else
                {
                    procedimento = await db.Procedimento.FindAsync(model.ID);

                    if (!procedimento.IsModificabile)
                    {
                        procedimento.NumeroModuli = model.NumeroModuli;
                        procedimento.Descrizione = model.Descrizione;
                        procedimento.QuorumFirme = model.QuorumFirme;
                        procedimento.AnnoFirmatario = model.AnnoFirmatario;
                        procedimento.ProcedimentoPostazione.OrderBy(x => x.ModuloA).Last().ModuloA = model.NumeroModuli;
                        db.Entry(procedimento).State = System.Data.Entity.EntityState.Modified;
                    }
                    else if (model.NumeroModuli < model.NumeroPostazioni)
                    {
                        model.Alerts.Add(new Alert { Title = "Operazione NON eseguita!", AlertType = Alert.AlertTypeEnum.Warning, Messages = new List<string> { "Il numero delle postazioni non deve essere maggiore del numero dei moduli" } });
                        return View(model);
                    }
                    else
                    {

                        if (procedimento.NumeroModuli != model.NumeroModuli || procedimento.NumeroPostazioni != model.NumeroPostazioni || !procedimento.ProcedimentoPostazione.Any())
                        {
                            RidistribuisciModulo(model, procedimento, true);
                        }
                        else
                        {
                            procedimento.ProcedimentoPostazione = model.ProcedimentoPostazione;
                        }

                        procedimento.AnnoFirmatario = model.AnnoFirmatario;
                        procedimento.Descrizione = model.Descrizione;
                        procedimento.NumeroModuli = model.NumeroModuli;
                        procedimento.NumeroPostazioni = model.NumeroPostazioni;
                        procedimento.QuorumFirme = model.QuorumFirme;
                        procedimento.DataOraModifica = DateTime.Now;
                        procedimento.UsernameModifica = utente.Username;


                        db.Entry(procedimento).State = System.Data.Entity.EntityState.Modified;
                    }
                }

                await db.SaveChangesAsync();

                TempData["Alerts"] = new List<Alert> { new Alert { Title = "Operazione eseguita", AlertType = Alert.AlertTypeEnum.Success, Messages = new List<string> { "Salvataggio eseguito!" } } };
                return RedirectToAction("Modifica", new { procedimento.ID });
            }

            model.SetAlerts(ModelState);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RidistribuisciPostazioni(int ID)
        {
            Procedimento procedimento;
            procedimento = await db.Procedimento.FindAsync(ID);

            Procedimento model = new Procedimento
            {
                NumeroModuli = procedimento.NumeroModuli,
                NumeroPostazioni = procedimento.NumeroPostazioni
            };
            RidistribuisciModulo(model, procedimento, true);

            procedimento.DataOraModifica = DateTime.Now;
            procedimento.UsernameModifica = RERIAMPrincipal.Corrente.Username;

            db.Entry(procedimento).State = System.Data.Entity.EntityState.Modified;

            await db.SaveChangesAsync();

            TempData["Alerts"] = new List<Alert> { new Alert { Title = "Operazione eseguita", AlertType = Alert.AlertTypeEnum.Success, Messages = new List<string> { "Ridistribuzione moduli eseguita correttamente!" } } };
            return RedirectToAction("Modifica", new { ID });
        }

        private static void RidistribuisciModulo(Procedimento model, Procedimento procedimento, bool cancellaPrecedenti = false)
        {
            int tmpDa = 1;
            List<int> nrModuliDivisiPerPostazione = new List<int>();
            int nrModuliPerPostazione = model.NumeroModuli / model.NumeroPostazioni;
            int restoModuliPerPostazione = model.NumeroModuli % model.NumeroPostazioni;

            if (cancellaPrecedenti)
                procedimento.ProcedimentoPostazione.Clear();

            for (int i = 1; i <= model.NumeroPostazioni; i++)
            {
                int tmp = nrModuliPerPostazione;
                if (restoModuliPerPostazione > 0)
                {
                    tmp++;
                    restoModuliPerPostazione--;
                }
                nrModuliDivisiPerPostazione.Add(tmp);
            }
            for (int i = 1; i <= model.NumeroPostazioni; i++)
            {
                procedimento.ProcedimentoPostazione.Add(new ProcedimentoPostazione
                {
                    CodicePostazione = GetLeteraColonnaDaIndice(i),
                    ModuloDa = tmpDa,
                    ModuloA = (tmpDa) + (nrModuliDivisiPerPostazione[i - 1] - 1)
                });
                tmpDa += nrModuliDivisiPerPostazione[i - 1];
            }
        }

        public ActionResult AbilitazioneUtenti(int? IdProcedimento, int? IdProfilo)
        {
            //AbilitazioneUtentiProcedimentoViewModel model = new AbilitazioneUtentiProcedimentoViewModel();

            AbilitazioneUtentiProcedimentoViewModel model = new AbilitazioneUtentiProcedimentoViewModel();
            if (!IdProcedimento.HasValue && !IdProfilo.HasValue)
            {
                model.ElencoAbilitazioni = null;
                model.IdProcedimento = IdProcedimento;

                return View(model);
            }
            else
            {
                var procedimento = db.Procedimento.Find(IdProcedimento.Value);
                model.ElencoAbilitazioni = new List<UtenteProcedimento>();
                model.IdProcedimento = IdProcedimento;
                model.IdProfilo = IdProfilo;

                model.ElencoAbilitazioni = (from u in db.Utente
                                            where ((u.UtenteProfilo.Any(x => ((IdProfilo.HasValue && x.IDProfilo == IdProfilo.Value) || (!IdProfilo.HasValue))
                                                                             && x.ValidoDal <= DateTime.Now
                                                                             && (!x.ValidoAl.HasValue || (x.ValidoAl.HasValue && x.ValidoAl.Value >= DateTime.Now)))))
                                            select new UtenteProcedimento
                                            {
                                                Abilitato = u.Procedimento.Any(x => x.ID == IdProcedimento),
                                                DescrizioneProcedimento = procedimento.Descrizione,
                                                IdProcediento = procedimento.ID,
                                                DescrizioneProfilo = u.UtenteProfilo.FirstOrDefault().Profilo.Descrizione,
                                                IdProfilo = u.UtenteProfilo.FirstOrDefault().Profilo.ID,
                                                Nominativo = u.Cognome + " " + u.Nome,
                                                Username = u.Username
                                            })
                                            .OrderBy(x=>x.IdProfilo).ThenBy(x=>x.Nominativo)
                                            .ToList();

                model.ElencoAbilitazioni.RemoveAll(x => x.DescrizioneProfilo == RuoloUtente.Amministatore.ToString());

                if (TempData["Alerts"] != null)
                {
                    model.Alerts = (List<Alert>)TempData["Alerts"];
                }

                return View("AbilitazioneUtenti", model);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Concludi(int IDProcedimentoCorrente, int? IDProcedimentoRicerca, string CodStatoRicerca)
        {
            var procedimento = await db.Procedimento.FindAsync(IDProcedimentoCorrente);
            procedimento.CodStato = "CON";
            procedimento.DataOraModifica = DateTime.Now;
            procedimento.UsernameModifica = RERIAMPrincipal.Corrente.Username;

            await db.SaveChangesAsync();
            TempData["Alerts"] = new List<Alert> { new Alert { Title = "Operazione eseguita", AlertType = Alert.AlertTypeEnum.Success, Messages = new List<string> { $"Procedimento '{procedimento.Descrizione}' concluso correttamente!" } } };
            return RedirectToAction("Cerca", new { IdProcedimento = IDProcedimentoRicerca, CodStato = CodStatoRicerca });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Elimina(Procedimento model)
        {
            if (!model.IsEliminabile)
            {
                model.Alerts.Add(new Alert { Title = "Operazione NON eseguita!", AlertType = Alert.AlertTypeEnum.Warning, Messages = new List<string> { "Il procedimento ha già dei moduli inseriti." } });
                return RedirectToAction("Modifica", new {model.ID });
            }

            db.Procedimento_Elimina(model.ID);
            
            TempData["Alerts"] = new List<Alert> { new Alert { Title = "Operazione eseguita", AlertType = Alert.AlertTypeEnum.Success, Messages = new List<string> { $"Procedimento eliminato correttamente!" } } };
            return RedirectToAction("Cerca");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Riattiva(int IDProcedimentoCorrente, int? IDProcedimentoRicerca, string CodStatoRicerca)
        {
            var procedimento = await db.Procedimento.FindAsync(IDProcedimentoCorrente);
            procedimento.CodStato = "ATT";
            procedimento.DataOraModifica = DateTime.Now;
            procedimento.UsernameModifica = RERIAMPrincipal.Corrente.Username;

            await db.SaveChangesAsync();
            
            TempData["Alerts"] = new List<Alert> { new Alert { Title = "Operazione eseguita", AlertType = Alert.AlertTypeEnum.Success, Messages = new List<string> { $"Procedimento '{procedimento.Descrizione}' riattivato correttamente!" } } };
            
            return RedirectToAction("Cerca", new { IdProcedimento = IDProcedimentoRicerca, CodStato = CodStatoRicerca });
        }

        //public async Task<ActionResult> CercaAbilitazioneUtenti(int IdProcedimento, int? IdProfilo)
        //{

        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SalvaAbilitazioneUtenti(AbilitazioneUtentiProcedimentoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var procedimento = await db.Procedimento.FindAsync(model.IdProcedimento);
                if (procedimento.Utente == null)
                {
                    procedimento.Utente = new HashSet<Utente>();
                }
                foreach (var item in model.ElencoAbilitazioni)
                {
                    var utente = await db.Utente.FindAsync(item.Username);

                    if (item?.Abilitato ?? false)
                    {
                        if (!procedimento.Utente.Any(x => x.Username == item.Username))
                            procedimento.Utente.Add(utente);
                    }
                    else
                    {
                        if (procedimento.Utente.Any(x => x.Username == item.Username))
                            procedimento.Utente.Remove(utente);
                    }
                }

                db.Entry(procedimento).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
                
                TempData["Alerts"] = new List<Alert> { new Alert { Title = "Operazione eseguita", AlertType = Alert.AlertTypeEnum.Success, Messages = new List<string> { $"Utenti salvati con successo!" } } };
                
                return RedirectToAction("AbilitazioneUtenti", new { model.IdProcedimento, model.IdProfilo });
            }
            return View("AbilitazioneUtenti", model);
        }

        public ActionResult ProspettoRiepilogo(int idProcedimento)
        {
            ViewBag.IdModulo = (int?)null;
            ViewBag.IdProcedimento = idProcedimento;
            ProspettoViewModel pvm = new ProspettoViewModel(idProcedimento);
            return View(pvm);
        }

        //    public ActionResult ProspettoRiepilogoDettaglioPerModulo(int idProcedimento)
        //    {
        //        ViewBag.IdModulo = (int?)null;
        //        ViewBag.IdProcedimento = idProcedimento;

        //        ProspettoViewModel pvm = new ProspettoViewModel(idProcedimento);
        //        return View("ProspettoRiepilogo", pvm);
        //    }

        //    public ActionResult ProspettoRiepilogoModulo(int idModulo)
        //    {
        //        var idProcedimento = db.Modulo.First(x => x.ID == idModulo).IDProcedimento;
        //        ViewBag.IdModulo = idModulo;
        //        ViewBag.IdProcedimento = idProcedimento;

        //        ProspettoViewModel pvm = new ProspettoViewModel(idProcedimento);
        //        return View("ProspettoRiepilogo", pvm);
        //    }

        //    public ActionResult ProspettoRiepilogoDettaglioCA(int idProcedimento)
        //    {
        //        ViewBag.IdModulo = (int?)null;
        //        ViewBag.IdProcedimento = idProcedimento;

        //        ProspettoViewModel pvm = new ProspettoViewModel(idProcedimento);
        //        return View("ProspettoRiepilogo", pvm);
        //    }

        //    public ActionResult ProspettoRiepilogoCA(int idProcedimento)
        //    {
        //        ViewBag.IdModulo = (int?)null;
        //        ViewBag.IdProcedimento = idProcedimento;
        //        ProspettoViewModel pvm = new ProspettoViewModel(idProcedimento);
        //        return View("ProspettoRiepilogo", pvm);
        //    }
    }
}