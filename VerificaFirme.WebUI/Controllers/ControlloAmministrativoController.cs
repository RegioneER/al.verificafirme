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
    public class ControlloAmministrativoController : ModuloController
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public override async Task<ActionResult> CreaModulo(ModuloDuplicatiViewModel model)
        {
            // Da analisi, se il modulo esiste già si va direttamente al modulo
            if (DBHelper.ModuloGiaInserito(db, model.IdProcedimento, model.CodPostazione, model.NrModulo))
            {
                var idModulo = db.Modulo.First(x => x.IDProcedimento == model.IdProcedimento && x.CodicePostazione.Equals(model.CodPostazione) && x.Numero == model.NrModulo).ID;
                return RedirectToAction("Compila", new { IdModulo = idModulo });
            }
            else if (!model.NrNominativi.HasValue)
            {
                TempData["Alerts"] = new List<Alert> { new Alert { AlertType = Alert.AlertTypeEnum.Warning, IsDismissible = true, Title = "Avvia procedura", Messages = new List<string> { "Impossibile creare un nuovo modulo. Indicare il numero dei nominativi!" } } };
                return RedirectToAction("Dettaglio", new { IdProcedimento = model.IdProcedimento });
            }

            if (DBHelper.ModuloNonDellaPostazione(db, model.IdProcedimento, model.CodPostazione, model.NrModulo))
            {
                TempData["Alerts"] = new List<Alert> { new Alert { AlertType = Alert.AlertTypeEnum.Warning, IsDismissible = true, Title = "Avvia procedura", Messages = new List<string> { "Impossibile creare un nuovo modulo. Il modulo richiesto non fa parte della postazione!" } } };
                return RedirectToAction("Dettaglio", new { IdProcedimento = model.IdProcedimento });
            }

            if (model.NrNominativi > this.MaxNumeroRigheModulo)
            {
                TempData["Alerts"] = new List<Alert> { new Alert { AlertType = Alert.AlertTypeEnum.Warning, IsDismissible = true, Title = "Avvia procedura", Messages = new List<string> { $"Impossibile creare un nuovo modulo. Superato il numero massimo del numero dei nominativi ({MaxNumeroRigheModulo})!" } } };
                return RedirectToAction("Dettaglio", new { IdProcedimento = model.IdProcedimento });
            }

            if (model.NrNominativi <= 0)
            {
                TempData["Alerts"] = new List<Alert> { new Alert { AlertType = Alert.AlertTypeEnum.Warning, IsDismissible = true, Title = "Avvia procedura", Messages = new List<string> { $"Impossibile creare un nuovo modulo. Inserire un numero maggiore di zero!" } } };
                return RedirectToAction("Dettaglio", new { IdProcedimento = model.IdProcedimento });
            }

            if (ModelState.IsValid)
            {

                Modulo modulo = SalvaNuovoModulo(model);
                await db.SaveChangesAsync();

                return RedirectToAction("Compila", new { IdModulo = modulo.ID });
            }
            else
            {
                model.SetAlerts(ModelState);
                TempData["Alerts"] = model.Alerts;
                return RedirectToAction("Dettaglio", new { IdProcedimento = model.IdProcedimento });
            }
        }

        public override ActionResult Dettaglio(int IdProcedimento, string CodPostazioneFiltro, int? NrModuloFiltro)
        {
            Procedimento procedimento = db.Procedimento.Find(IdProcedimento);
            ModuloViewModel model = new ModuloViewModel { Procedimento = procedimento, ModuloFiltro = NrModuloFiltro, PostazioneFiltro = CodPostazioneFiltro };

            //model.MostraAncheCompilati = true;

            if (TempData["Alerts"] != null)
                model.Alerts.AddRange((List<Alert>)TempData["Alerts"]);

            ViewBag.IdModulo = (int?)null;
            ViewBag.IdProcedimento = IdProcedimento;

            return View(model);
        }
    }
}