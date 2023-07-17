using VerificaFirme.Db;
using RER.Tools.MVC.Agid.MetadataAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace VerificaFirme.WebUI.Models
{
    public class RiepilogoDataEntryViewModel : ExtendedModel
    {
        private readonly VerificaFirmeDBContext db = new VerificaFirmeDBContext();

        public enum TipoRaggruppamentoDataEntry
        {
            Modulo = 1,
            Postazione = 2,
            Provincia = 3,
            Comune = 4
        }

        [Display(Name = "Procedimento")]
        public int? IdProcedimento { get; set; }

        [Display(Name = "Raggruppamento")]
        public int? IdRaggruppamento { get; set; }

        public List<Report_RiepilogoDataEntry_Result> RisultatoRicerca { get; set; }

        private SelectList _ElencoProcedimenti;
        public SelectList ElencoProcedimenti
        {
            get
            {
                if (_ElencoProcedimenti == null)
                {
                    RERIAMPrincipal utente = RERIAMPrincipal.Corrente;
                    List<Procedimento> lista = null;
                    if (utente.IsInRole(RuoloUtente.Amministatore))
                        lista = db.Procedimento.ToList();
                    else
                        lista = db.Procedimento.Where(x => x.Utente.Any(z => z.Username == RERIAMPrincipal.Corrente.Username) && x.CodStato != "CON").ToList();

                    _ElencoProcedimenti = new SelectList(lista.OrderByDescending(x => x.DataOraModifica).ThenBy(x => x.Descrizione).ToList(), nameof(Db.Procedimento.ID), nameof(Db.Procedimento.Descrizione), IdProcedimento);
                }
                return _ElencoProcedimenti;
            }
        }

        private SelectList _ElencoRaggruppamenti;
        public SelectList ElencoRaggruppamenti
        {
            get
            {
                if (_ElencoRaggruppamenti == null)
                {
                    SelectListItem item = null;
                    List<SelectListItem> lista = new List<SelectListItem>();

                    item = new SelectListItem();
                    item.Value = "1";
                    item.Text = "Modulo";
                    lista.Add(item);

                    item = new SelectListItem();
                    item.Value = "2";
                    item.Text = "Postazione";
                    lista.Add(item);

                    item = new SelectListItem();
                    item.Value = "3";
                    item.Text = "Provincia";
                    lista.Add(item);

                    item = new SelectListItem();
                    item.Value = "4";
                    item.Text = "Comune";
                    lista.Add(item);


                    _ElencoRaggruppamenti = new SelectList(lista, "Value", "Text", IdRaggruppamento);

                }
                return _ElencoRaggruppamenti;
            }
        }
    }
}