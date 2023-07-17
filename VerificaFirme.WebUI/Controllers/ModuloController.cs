using VerificaFirme.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VerificaFirme.Db;
using System.Data.Entity.Core;
using System.Data.Entity.Validation;
using System.Data.Entity;
using Newtonsoft.Json;

namespace VerificaFirme.WebUI.Controllers
{
    public class ModuloController : BaseController
    {

        // GET: Scheda

        public ActionResult Index(CercaProcedimentoViewModel model)
        {
            var query = db.Procedimento.AsQueryable();

            if (!RERIAMPrincipal.Corrente.IsInRole(RuoloUtente.Amministatore))
            {
                query = query.Where(x => x.Utente.Any(z => z.Username == RERIAMPrincipal.Corrente.Username) && x.CodStato != "CON");
            }

            if (model != null)
            {
                if (model.IdProcedimento.HasValue)
                {
                    query = query.Where(x => x.ID == model.IdProcedimento.Value);
                }
                if (!string.IsNullOrWhiteSpace(model.CodStato))
                {
                    query = query.Where(x => x.CodStato == model.CodStato);
                }
            }

            if (TempData["Alerts"] != null)
                model.Alerts.AddRange((List<Alert>)TempData["Alerts"]);

            model.RisultatiRicerca = query.OrderByDescending(x => x.DataOraModifica).ThenBy(x => x.Descrizione).ToList();
            return View(model);
        }

        public virtual ActionResult Dettaglio(int IdProcedimento, string CodPostazione, int? NrModulo)
        {
            Procedimento procedimento = db.Procedimento.Find(IdProcedimento) ?? throw new ApplicationException("Procedimento non valido");
            ModuloViewModel model = new ModuloViewModel { Procedimento = procedimento };

            if (TempData["Alerts"] != null)
                model.Alerts.AddRange((List<Alert>)TempData["Alerts"]);

            ViewBag.IdModulo = (int?)null;
            ViewBag.IdProcedimento = IdProcedimento;


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> CreaModulo(ModuloDuplicatiViewModel model)
        {
            if (DBHelper.ModuloGiaInserito(db, model.IdProcedimento, model.CodPostazione, model.NrModulo))
            {
                TempData["Alerts"] = new List<Alert> { new Alert { AlertType = Alert.AlertTypeEnum.Warning, IsDismissible = true, Title = "Avvia procedura", Messages = new List<string> { "Impossibile creare un nuovo modulo. Il modulo è già presente!" } } };
                return RedirectToAction("Dettaglio", new { model.IdProcedimento });
            }

            if (DBHelper.ModuloNonDellaPostazione(db, model.IdProcedimento, model.CodPostazione, model.NrModulo))
            {
                TempData["Alerts"] = new List<Alert> { new Alert { AlertType = Alert.AlertTypeEnum.Warning, IsDismissible = true, Title = "Avvia procedura", Messages = new List<string> { "Impossibile creare un nuovo modulo. Il modulo richiesto non fa parte della postazione!" } } };
                return RedirectToAction("Dettaglio", new { model.IdProcedimento });
            }

            if (DBHelper.ProcedimentoChiuso(db, model.IdProcedimento))
            {
                TempData["Alerts"] = new List<Alert> { new Alert { AlertType = Alert.AlertTypeEnum.Warning, IsDismissible = true, Title = "Avvia procedura", Messages = new List<string> { "Impossibile creare un nuovo modulo. Il procedimento è già concluso!" } } };
                return RedirectToAction("Dettaglio", new { model.IdProcedimento });
            }

            if (model.NrNominativi > this.MaxNumeroRigheModulo)
            {
                TempData["Alerts"] = new List<Alert> { new Alert { AlertType = Alert.AlertTypeEnum.Warning, IsDismissible = true, Title = "Avvia procedura", Messages = new List<string> { $"Impossibile creare un nuovo modulo. Superato il numero massimo del numero dei nominativi ({MaxNumeroRigheModulo})!" } } };
                return RedirectToAction("Dettaglio", new { model.IdProcedimento });
            }

            if (model.NrNominativi <= 0)
            {
                TempData["Alerts"] = new List<Alert> { new Alert { AlertType = Alert.AlertTypeEnum.Warning, IsDismissible = true, Title = "Avvia procedura", Messages = new List<string> { $"Impossibile creare un nuovo modulo. Inserire un numero maggiore di zero!" } } };
                return RedirectToAction("Dettaglio", new { model.IdProcedimento });
            }


            Modulo modulo = SalvaNuovoModulo(model);
            await db.SaveChangesAsync();

            return RedirectToAction("Compila", new { IdModulo = modulo.ID });
        }

        protected Modulo SalvaNuovoModulo(ModuloDuplicatiViewModel model)
        {
            Procedimento p = db.Procedimento.Include(x => x.ProcedimentoPostazione).FirstOrDefault(x => x.ID == model.IdProcedimento);
            p.CodStato = "ATT";
            db.Entry(p).State = System.Data.Entity.EntityState.Modified;

            var modulo = new Db.Modulo
            {
                CodicePostazione = model.CodPostazione,
                Numero = model.NrModulo,
                NumeroRighe = model.NrNominativi.Value,
                IsCompleto = false,
                IsNullo = false,
                CodCategoriaEsclusione = null,
                CodComuneListaElettorale = null,
                IDProcedimento = model.IdProcedimento,
                UsernameCreazione = RERIAMPrincipal.Corrente.Username,
                DataOraCreazione = DateTime.Now
            };
            db.Modulo.Add(modulo);

            for (int i = 0; i < model.NrNominativi; i++)
            {
                ModuloNominativo mn = new ModuloNominativo()
                {
                    CodCategorieEsclusione = null,
                    CodCategorieSanabilita = null,
                    CodComuneListaElettorale = null,
                    CodComuneNascita = null,
                    Cognome = null,
                    DataNascita = null,
                    IDModulo = modulo.ID,
                    NListaElettorale = null,
                    Nome = null,
                    Note = null,
                    NumeroRiga = (i + 1)
                };

                db.ModuloNominativo.Add(mn);

                mn.ModuloNominativoLog.Add(
                        new ModuloNominativoLog()
                        {
                            NumeroRiga = mn.NumeroRiga,
                            DataOraModifica = DateTime.Now,
                            Username = RERIAMPrincipal.Corrente.Username
                        });

            }

            return modulo;
        }

        public virtual async Task<ActionResult> Compila(int IdModulo)
        {
            var modulo = await db.Modulo.FindAsync(IdModulo) ?? throw new ApplicationException("Modulo non valido");
            ModuloViewModel model = new ModuloViewModel
            {
                Modulo = modulo,
                Procedimento = modulo.ProcedimentoPostazione.Procedimento,
                FromCompila = true
            };

            if (TempData["Errori"] != null)
            {
                ViewBag.Errori = TempData["Errori"];
            }

            if (TempData["Alerts"] != null)
                model.Alerts.AddRange((List<Alert>)TempData["Alerts"]);

            SetViewBag(modulo);
            return View(model);
        }

        private void SetViewBag(Modulo modulo)
        {
            string titolo = $"Procedimento {modulo.ProcedimentoPostazione.Procedimento.Descrizione} - Modulo {modulo.CodicePostazione}.{modulo.Numero}";
            ViewBag.SottoTitolo = titolo;

            ViewBag.IdModulo = modulo.ID;
            ViewBag.IdProcedimento = modulo.IDProcedimento;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> SalvaOperazioniSulModulo(OperazioniModuloViewModel model)
        {
            var procedimento = await db.Procedimento.FindAsync(model.IdProcedimento);
            var modulo = await db.Modulo.FindAsync(model.IdModulo);
            bool moduloNullo = modulo.IsNullo;

            if (modulo.IsCompleto)
            {
                if (model.NrNominativi != modulo.NumeroRighe)
                    modulo.IsCompleto = false;

                if (model.IsNullo != modulo.IsNullo)
                    modulo.IsCompleto = false;

                if (string.IsNullOrEmpty(model.MotivoNullita) && !string.IsNullOrEmpty(modulo.CodCategoriaEsclusione))
                    modulo.IsCompleto = false;

                if (!string.IsNullOrEmpty(model.MotivoNullita) && string.IsNullOrEmpty(modulo.CodCategoriaEsclusione))
                    modulo.IsCompleto = false;

                if (!string.IsNullOrEmpty(model.MotivoNullita) && !string.IsNullOrEmpty(modulo.CodCategoriaEsclusione) && model.MotivoNullita != modulo.CodCategoriaEsclusione)
                    modulo.IsCompleto = false;
            }

            if (model.IsNullo)
            {
                modulo.IsNullo = model.IsNullo;
                modulo.CodCategoriaEsclusione = model.MotivoNullita;
            }
            else
            {
                modulo.IsNullo = false;
                modulo.CodCategoriaEsclusione = null;
            }

            if (!string.IsNullOrWhiteSpace(model.ComuneLista))
                modulo.CodComuneListaElettorale = db.vvComune.FirstOrDefault(x => x.DescrizioneCompleta == model.ComuneLista.ToUpper())?.cod_istat;
            else
                modulo.CodComuneListaElettorale = null;

            modulo.NumeroRighe = model.NrNominativi;
            modulo.UsernameModifica = RERIAMPrincipal.Corrente.Username;
            modulo.DataOraModifica = DateTime.Now;


            db.Entry(modulo).State = System.Data.Entity.EntityState.Modified;

            var righe = db.ModuloNominativo.Where(x => x.IDModulo == model.IdModulo);
            bool logga = false;

            foreach (var item in righe)
            {
                if (!string.IsNullOrWhiteSpace(model.ComuneLista))
                {
                    item.CodComuneListaElettorale = db.vvComune.FirstOrDefault(x => x.DescrizioneCompleta == model.ComuneLista.ToUpper())?.cod_istat;
                    logga = true;
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }

                if (modulo.IsNullo)
                {
                    item.CodCategorieEsclusione = modulo.CodCategoriaEsclusione;
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    if (moduloNullo)
                    {
                        //il modulo su db era nullo ma l'utente lo ha tolto. 
                        item.CodCategorieEsclusione = null;
                        logga = true;
                        db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    }
                }

                if (logga)
                    item.ModuloNominativoLog.Add(
                            new ModuloNominativoLog()
                            {
                                NumeroRiga = item.NumeroRiga,
                                DataOraModifica = DateTime.Now,
                                Username = RERIAMPrincipal.Corrente.Username,
                                IDModuloNominativo = model.IdModulo
                            });
            }

            if (righe.Count() < model.NrNominativi)
            {
                // aggiungo le righe che mancano
                int differenza = Math.Abs(model.NrNominativi - righe.Count());
                for (int i = 1; i <= differenza; i++)
                {
                    var riga = new ModuloNominativo()
                    {
                        CodCategorieEsclusione = modulo.CodCategoriaEsclusione,
                        CodCategorieSanabilita = null,
                        Nome = null,
                        Cognome = null,
                        CodComuneNascita = null,
                        CodComuneListaElettorale = db.vvComune.FirstOrDefault(x => x.DescrizioneCompleta == model.ComuneLista.ToUpper())?.cod_istat,
                        DataNascita = null,
                        IDModulo = model.IdModulo,
                        NumeroRiga = (righe.Count() + i),
                        NListaElettorale = null,
                        Note = null
                    };
                    riga.ModuloNominativoLog.Add(
                        new ModuloNominativoLog()
                        {
                            NumeroRiga = riga.NumeroRiga,
                            DataOraModifica = DateTime.Now,
                            Username = RERIAMPrincipal.Corrente.Username,
                            IDModuloNominativo = model.IdModulo
                        });
                    db.ModuloNominativo.Add(riga);
                }
            }
            //await db.SaveChangesAsync();
            if (righe.Count() > model.NrNominativi)
            {
                // elimino le righe in eccesso
                int differenza = Math.Abs(model.NrNominativi - righe.Count());
                var daCancellare = righe.OrderByDescending(x => x.NumeroRiga).Take(differenza);
                db.ModuloNominativo.RemoveRange(daCancellare);
            }

            await db.SaveChangesAsync();

            TempData["Alerts"] = new List<Alert> { new Alert { AlertType = Alert.AlertTypeEnum.Success, IsDismissible = true, Title = "Operazioni sul modulo", Messages = new List<string> { "Operazione eseguita correttamente!" } } };

            ViewBag.IdModulo = model.IdModulo;
            ViewBag.IdProcedimento = model.IdProcedimento;

            return RedirectToAction("Compila", new { model.IdModulo });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> SalvaNota(int IdProcedimentoNota, int IdModulo, int IdRigaModuloNota, string NotaRigaModulo)
        {
            var modulo = await db.Modulo.FindAsync(IdModulo);
            modulo.UsernameModifica = RERIAMPrincipal.Corrente.Username;
            modulo.DataOraModifica = DateTime.Now;
            db.Entry(modulo).State = System.Data.Entity.EntityState.Modified;

            var riga = db.ModuloNominativo.FirstOrDefault(x => x.IDModulo == IdModulo && x.NumeroRiga == IdRigaModuloNota);
            riga.Note = NotaRigaModulo?.ToUpper();
            riga.ModuloNominativoLog.Add(new ModuloNominativoLog { DataOraModifica = DateTime.Now, IDModuloNominativo = riga.IDModulo, NumeroRiga = riga.NumeroRiga, Username = RERIAMPrincipal.Corrente.Username });

            await db.SaveChangesAsync();

            TempData["Alerts"] = new List<Alert> { new Alert { AlertType = Alert.AlertTypeEnum.Success, IsDismissible = true, Title = "Nota", Messages = new List<string> { "Nota salvata con successo!" } } };

            return RedirectToAction("Compila", new { IdModulo });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Salva(ModuloDuplicatiViewModel model)
        {
            try
            {
                SalvaTemporaneo(model, false, true);
            }
            catch (ModuloCompletatoException)
            {
                TempData["Alerts"] = new List<Alert> { new Alert { AlertType = Alert.AlertTypeEnum.Error, IsDismissible = true, Title = "Salvataggio NON riuscito!", Messages = new List<string> { "Il modulo risulta già essere completato!" } } };

                return RedirectToAction("Compila", new { model.IdModulo });
            }
            catch (Exception ex)
            {
                throw ex;
            }


            TempData["Alerts"] = new List<Alert> { new Alert { AlertType = Alert.AlertTypeEnum.Success, IsDismissible = true, Title = "Salvataggio", Messages = new List<string> { "Modulo salvato con successo!" } } };

            return RedirectToAction("Compila", new { model.IdModulo });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> SalvaECompleta(ModuloDuplicatiViewModel model)
        {
            List<ErroreValidazioneForm> errori = new List<ErroreValidazioneForm>();

            // mi assicuro che i dati vengano salvati su DB, per evitare perdite di dati
            try
            {
                SalvaTemporaneo(model, true, false);
            }
            catch (ModuloCompletatoException)
            {
                TempData["Alerts"] = new List<Alert> { new Alert { AlertType = Alert.AlertTypeEnum.Error, IsDismissible = true, Title = "Salvataggio NON riuscito!", Messages = new List<string> { "Il modulo risulta già essere completato!" } } };

                return RedirectToAction("Compila", new { model.IdModulo });
            }
            catch (Exception ex)
            {
                throw ex;
            }

            errori = model.Valida(db);

            if (errori.Any())
            {
                // ci sono degli errori, qundi l'utente deve sistemarli

                TempData["Errori"] = errori;
                Modulo m = await db.Modulo.FirstOrDefaultAsync(x => x.ID == model.IdModulo);
                m.IsCompleto = false;
                await db.SaveChangesAsync();

            }
            else
            {
                //tutto ok. salvo.
                await db.SaveChangesAsync();

                return RedirectToAction("Dettaglio", new { model.IdProcedimento });
            }

            return RedirectToAction("Compila", new { model.IdModulo });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult SalvaEsci(ModuloDuplicatiViewModel model)
        {
            try
            {
                SalvaTemporaneo(model, false, true);
            }
            catch (ModuloCompletatoException)
            {
                TempData["Alerts"] = new List<Alert> { new Alert { AlertType = Alert.AlertTypeEnum.Error, IsDismissible = true, Title = "Salvataggio NON riuscito!", Messages = new List<string> { "Il modulo risulta già essere completato!" } } };

                return RedirectToAction("Compila", new { model.IdModulo });
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return RedirectToAction("Dettaglio", new { model.IdProcedimento });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<ActionResult> SalvaCompletaEProsegui(ModuloDuplicatiViewModel model)
        {
            List<ErroreValidazioneForm> errori = new List<ErroreValidazioneForm>();

            // mi assucuro che i dati vengano salvati su DB, per evitare perdite di dati
            try
            {
                SalvaTemporaneo(model, true, false);
            }
            catch (ModuloCompletatoException)
            {
                TempData["Alerts"] = new List<Alert> { new Alert { AlertType = Alert.AlertTypeEnum.Error, IsDismissible = true, Title = "Salvataggio NON riuscito!", Messages = new List<string> { "Il modulo risulta già essere completato!" } } };

                return RedirectToAction("Compila", new { model.IdModulo });
            }
            catch (Exception ex)
            {
                throw ex;
            }


            errori = model.Valida(db);

            if (errori.Any())
            {
                // ci sono degli errori
                TempData["Errori"] = errori;
                Modulo m = await db.Modulo.FirstOrDefaultAsync(x => x.ID == model.IdModulo);
                m.IsCompleto = false;
                await db.SaveChangesAsync();
            }
            else
            {
                //non ci sono errori. posso procedere all'avvio del nuovo modulo.
                if (DBHelper.ModuloGiaInserito(db, model.IdProcedimento, model.CodPostazione, model.NrModulo))
                {
                    TempData["Alerts"] = new List<Alert> { new Alert { AlertType = Alert.AlertTypeEnum.Warning, IsDismissible = true, Title = "Avvia procedura", Messages = new List<string> { "Impossibile creare un nuovo modulo. Il modulo è già presente!" } } };
                    return RedirectToAction("Compila", new { model.IdModulo });
                }

                if (DBHelper.ModuloNonDellaPostazione(db, model.IdProcedimento, model.CodPostazione, model.NrModulo))
                {
                    TempData["Alerts"] = new List<Alert> { new Alert { AlertType = Alert.AlertTypeEnum.Warning, IsDismissible = true, Title = "Avvia procedura", Messages = new List<string> { "Impossibile creare un nuovo modulo. Il modulo richiesto non fa parte della postazione!" } } };
                    return RedirectToAction("Compila", new { model.IdModulo });
                }

                if (DBHelper.ProcedimentoChiuso(db, model.IdProcedimento))
                {
                    TempData["Alerts"] = new List<Alert> { new Alert { AlertType = Alert.AlertTypeEnum.Warning, IsDismissible = true, Title = "Avvia procedura", Messages = new List<string> { "Impossibile creare un nuovo modulo. Il procedimento è già concluso!" } } };
                    return RedirectToAction("Dettaglio", new { model.IdProcedimento });
                }

                Modulo modulo = SalvaNuovoModulo(model);

                await db.SaveChangesAsync();

                return RedirectToAction("Compila", new { IdModulo = modulo.ID });
            }

            return RedirectToAction("Compila", new { model.IdModulo });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public object SalvaAjax(ModuloDuplicatiViewModel model)
        {
            int righeComplete = -1;
            bool isCompleto = false;

            try
            {
                SalvaTemporaneo(model, false, true);

                try
                {
                    Modulo modulo = db.Modulo.Include(x => x.ModuloNominativo).FirstOrDefault(x => x.ID == model.IdModulo);
                    righeComplete = modulo.UltimaRigaCompletata;
                    isCompleto = modulo.IsCompleto;
                }
                catch { }

            }
            catch (DbEntityValidationException evex)
            {
                LoggaEccezione(evex, null);
                return "ERR-EF-VAL";
            }
            catch (ModuloCompletatoException mcex)
            {
                LoggaEccezione(mcex, null);
                return "ERR-MCEX";
            }
            catch (EntityException eex)
            {
                LoggaEccezione(eex, null);
                return "ERR-EF";
            }
            catch (Exception ex)
            {
                LoggaEccezione(ex, null);
                return "ERR-ND";
            }

            return $"OK|{righeComplete}|{(isCompleto ? "1" : "0")}";
        }


        protected void SalvaTemporaneo(ModuloDuplicatiViewModel model, bool moduloIsCompleto = false, bool save = true)
        {
            // Uno dei salvataggi successivi. Si va in modifica su tutte le righe
            List<ModuloNominativo> lista = db.ModuloNominativo.Where(x => x.IDModulo == model.IdModulo).ToList();
            RERIAMPrincipal utente = RERIAMPrincipal.Corrente;

            Modulo m = db.Modulo.FirstOrDefault(x => x.ID == model.IdModulo);
            Procedimento p = db.Procedimento.FirstOrDefault(x => x.ID == m.IDProcedimento);
            if (utente.IsInRole(RuoloUtente.Dataentry) && (m.IsCompleto || p.CodStato == "CON"))
            {
                throw new ModuloCompletatoException("Modulo non modificabile");
            }

            bool salvoDaCA = false;

            var currType = this.GetType();
            if (currType != typeof(ModuloController))
            {
                //sto salvando dal controllo amministrativo
                salvoDaCA = true;
            }


            bool rimettiModuloNonCompleto = false;
            foreach (var item in model.Righe)
            {
                var riga = lista.FirstOrDefault(x => x.NumeroRiga == item.NrRiga);
                if (item.RigaModificata(riga, db))
                {
                    rimettiModuloNonCompleto = true;
                    riga.CodCategorieEsclusione = item.CategoriaNullita;
                    if (utente.IsInRole(RuoloUtente.Amministatore) || utente.IsInRole(RuoloUtente.Supervisore))
                    {
                        // solo supervisore e admin possono compilare la cat. sanabilita
                        riga.CodCategorieSanabilita = item.CategoriaSanabilita;
                    }

                    riga.Nome = item.Nome?.ToUpper();
                    riga.Cognome = item.Cognome?.ToUpper();
                    riga.CodComuneNascita = db.vvComune.FirstOrDefault(x => x.DescrizioneCompleta == item.LuogoNascita.ToUpper())?.cod_istat;
                    riga.CodComuneListaElettorale = db.vvComune.FirstOrDefault(x => x.DescrizioneCompleta == item.ComuneElettorale.ToUpper())?.cod_istat;

                    if (item.DataNascita.HasValue)
                    {
                        riga.DataNascita = item.DataNascita;
                    }
                    else
                    {
                        DateTime dataTemp = new DateTime();
                        if (DateTime.TryParse(item.DataNascitaString, out dataTemp))
                        {
                            riga.DataNascita = dataTemp;
                        }
                        else
                        {
                            riga.DataNascita = null;
                        }
                    }

                    riga.IDModulo = model.IdModulo;
                    riga.NumeroRiga = item.NrRiga;
                    riga.NListaElettorale = item.NumeroLista?.ToUpper();
                    riga.Note = item.Note?.ToUpper();

                    riga.ModuloNominativoLog.Add(new ModuloNominativoLog { DataOraModifica = DateTime.Now, IDModuloNominativo = riga.IDModulo, NumeroRiga = riga.NumeroRiga, Username = utente.Username });

                    db.Entry(riga).State = System.Data.Entity.EntityState.Modified;
                }
            }

            m.UsernameModifica = utente.Username;
            m.DataOraModifica = DateTime.Now;

            if (!m.IsCompleto && moduloIsCompleto)
                m.IsCompleto = true;

            if (rimettiModuloNonCompleto && salvoDaCA)
                m.IsCompleto = false;

            db.Entry(m).State = System.Data.Entity.EntityState.Modified;

            if (save)
                db.SaveChanges();

        }

        public virtual JsonResult GetModuliPostazione(int idProcedimento, string postazione)
        {
            ProcedimentoPostazione pp = db.ProcedimentoPostazione.FirstOrDefault(x => x.IDProcedimento == idProcedimento && x.CodicePostazione.Equals(postazione));
            if (pp != null)
            {
                List<SelectListItem> items = new List<SelectListItem>();
                for (int i = pp.ModuloDa; i <= pp.ModuloA; i++)
                {
                    if (!db.Modulo.Any(x => x.IDProcedimento == idProcedimento && x.CodicePostazione.Equals(postazione) && x.Numero == i))
                    {
                        items.Add(new SelectListItem
                        {
                            Value = i.ToString(),
                            Text = i.ToString()
                        });
                    }
                }

                return Json(items, JsonRequestBehavior.AllowGet);
            }
            else
                return null;
        }

        public virtual JsonResult GetPostazioni(int idProcedimento)
        {
            List<ProcedimentoPostazione> postazioni = db.ProcedimentoPostazione.Where(x => x.IDProcedimento == idProcedimento).OrderBy(x => x.CodicePostazione).ToList();
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (ProcedimentoPostazione pp in postazioni)
            {
                if (db.Modulo.Count(x => x.IDProcedimento == pp.IDProcedimento && x.CodicePostazione.Equals(pp.CodicePostazione)) < pp.NumeroModuliPostazione)
                {
                    string postazione = pp.CodicePostazione;
                    items.Add(new SelectListItem
                    {
                        Value = pp.CodicePostazione,
                        Text = pp.CodicePostazione
                    });
                }
            }

            return Json(items, JsonRequestBehavior.AllowGet);
        }
    }
}