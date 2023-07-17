using VerificaFirme.Db;
using VerificaFirme.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace VerificaFirme.WebUI.Controllers
{
    public class ReportController : BaseController
    {
        // GET: Report
        public ActionResult Index(int? IdProcedimento)
        {
            ReportViewModel model = new ReportViewModel();
            model.IdProcedimento = IdProcedimento;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MostraReport(int IdReport, int? IdProcedimento)
        {
            if (IdReport == 1)
            {
                if (IdProcedimento.HasValue)
                    return RedirectToAction("AnagraficheDuplicate", new { IdProcedimento = IdProcedimento.Value });
                else
                    return RedirectToAction("AnagraficheDuplicate");
            }
            else if (IdReport == 2)
            {
                if (IdProcedimento.HasValue)
                    return RedirectToAction("RiepilogoDataEntry", new { IdProcedimento = IdProcedimento.Value });
                else
                    return RedirectToAction("RiepilogoDataEntry");
            }
            else
                throw new ApplicationException("Report non disponibile");
        }
        public ActionResult AnagraficheDuplicate(int? IdProcedimento, string CodComuneListaElettorale, bool? SoloDaCorreggere, int? IdModalita)
        {
            FirmatariDuplicatiViewModel model = new FirmatariDuplicatiViewModel();
            model.IdProcedimento = IdProcedimento;
            model.IdModalita = IdModalita;

            model.CodComuneListaElettorale = db.vvComune.FirstOrDefault(x => x.DescrizioneCompleta == CodComuneListaElettorale.ToUpper())?.cod_istat;
            model.SoloDaCorreggere = SoloDaCorreggere ?? false;

            if (IdProcedimento.HasValue)
                model.ProcedimentoConcluso = db.Procedimento.Find(IdProcedimento.Value).IsConcluso;

            if (IdProcedimento.HasValue && IdModalita.HasValue)
            {
                if (IdModalita == 1)
                    model.RisultatoDuplicati = db.Report_NominativiDuplicatiPerProcedimento(IdProcedimento, null, model.CodComuneListaElettorale, !(SoloDaCorreggere ?? false)).ToList();
                else
                    model.RisultatoMinori = db.Report_VerificaMinori(IdProcedimento, null).ToList();
            }

            return View(model);
        }

        public ActionResult RiepilogoDataEntry(int? IdProcedimento, int? IdRaggruppamento)
        {
            RiepilogoDataEntryViewModel model = new RiepilogoDataEntryViewModel();
            model.IdProcedimento = IdProcedimento;
            model.IdRaggruppamento = IdRaggruppamento;

            if (IdProcedimento.HasValue)
            {
                model.RisultatoRicerca = db.Report_RiepilogoDataEntry(IdProcedimento, IdRaggruppamento).ToList();
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SalvaModifiche(FirmatariDuplicatiViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.IdProcedimento.HasValue)
                {
                    if (db.Procedimento.Find(model.IdProcedimento.Value).IsConcluso)
                    {
                        throw new ApplicationException("Procedimento concluso!");
                    }
                }

                string userName = RERIAMPrincipal.Corrente.Username;

                foreach (var item in model.RigheVerificate)
                {
                    var riga = db.ModuloNominativo.First(x => x.IDModulo == item.IDModulo && x.NumeroRiga == item.NrRiga);
                    if (item.RigaModificata(riga, db))
                    {
                        riga.CodCategorieEsclusione = item.CategoriaNullita;
                        riga.Note = item.Note;

                        riga.ModuloNominativoLog.Add(new ModuloNominativoLog { DataOraModifica = DateTime.Now, IDModuloNominativo = riga.IDModulo, NumeroRiga = riga.NumeroRiga, Username = userName });

                        db.Entry(riga).State = System.Data.Entity.EntityState.Modified;
                    }
                }

                await db.SaveChangesAsync();
            }
            else
            {

            }

            string comune = db.vvComune.FirstOrDefault(x => x.cod_istat == model.CodComuneListaElettorale)?.DescrizioneCompleta;

            return RedirectToAction(model.ISControlloAmministrativo ? "AnagraficheDuplicateCA" : "AnagraficheDuplicate", new { IdProcedimento = model.IdProcedimento, CodComuneListaElettorale = comune, SoloDaCorreggere = model.SoloDaCorreggere });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SalvaModificheMinori(FirmatariDuplicatiViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.IdProcedimento.HasValue)
                {
                    if (db.Procedimento.Find(model.IdProcedimento.Value).IsConcluso)
                    {
                        throw new ApplicationException("Procedimento concluso!");
                    }
                }

                string userName = RERIAMPrincipal.Corrente.Username;

                foreach (var item in model.RigheVerificate)
                {
                    var riga = db.ModuloNominativo.First(x => x.IDModulo == item.IDModulo && x.NumeroRiga == item.NrRiga);
                    if (item.RigaModificata(riga, db))
                    {
                        riga.CodCategorieEsclusione = item.CategoriaNullita;
                        riga.Note = item.Note;

                        riga.ModuloNominativoLog.Add(new ModuloNominativoLog { DataOraModifica = DateTime.Now, IDModuloNominativo = riga.IDModulo, NumeroRiga = riga.NumeroRiga, Username = userName });

                        db.Entry(riga).State = System.Data.Entity.EntityState.Modified;
                    }
                }

                await db.SaveChangesAsync();
            }
            else
            {

            }

            string comune = db.vvComune.FirstOrDefault(x => x.cod_istat == model.CodComuneListaElettorale)?.DescrizioneCompleta;

            return RedirectToAction(model.ISControlloAmministrativo ? "AnagraficheDuplicateCA" : "AnagraficheDuplicate", new { IdProcedimento = model.IdProcedimento, CodComuneListaElettorale = comune, SoloDaCorreggere = model.SoloDaCorreggere, IdModalita = model.IdModalita });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SalvaMassivo(FirmatariDuplicatiViewModel model, string MotivoNullita)
        {
            if (ModelState.IsValid)
            {
                if (model.IdProcedimento.HasValue)
                {
                    if (db.Procedimento.Find(model.IdProcedimento.Value).IsConcluso)
                    {
                        throw new ApplicationException("Procedimento concluso!");
                    }
                }

                string userName = RERIAMPrincipal.Corrente.Username;

                List<Report_VerificaMinori_Result> lista = db.Report_VerificaMinori(model.IdProcedimento, null).ToList();

                foreach (var item in lista)
                {
                    var riga = db.ModuloNominativo.First(x => x.IDModulo == item.IDModulo && x.NumeroRiga == item.NumeroRiga);

                    riga.CodCategorieEsclusione = MotivoNullita;

                    riga.ModuloNominativoLog.Add(new ModuloNominativoLog { DataOraModifica = DateTime.Now, IDModuloNominativo = riga.IDModulo, NumeroRiga = riga.NumeroRiga, Username = userName });

                    db.Entry(riga).State = System.Data.Entity.EntityState.Modified;

                }

                await db.SaveChangesAsync();
            }
            else
            {

            }

            string comune = db.vvComune.FirstOrDefault(x => x.cod_istat == model.CodComuneListaElettorale)?.DescrizioneCompleta;

            return RedirectToAction(model.ISControlloAmministrativo ? "AnagraficheDuplicateCA" : "AnagraficheDuplicate", new { IdProcedimento = model.IdProcedimento, CodComuneListaElettorale = comune, SoloDaCorreggere = model.SoloDaCorreggere, IdModalita = model.IdModalita });

        }
    }
}