using VerificaFirme.Db;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VerificaFirme.WebUI.Models
{
    public class CercaProcedimentoViewModel : ModelAlert
    {
        private VerificaFirmeDBContext db = new VerificaFirmeDBContext();

        [Display(Name = "Procedimento")]
        public int? IdProcedimento { get; set; }
        [Display(Name = "Stato")]
        public string CodStato { get; set; }

        private SelectList _ElencoStati { get; set; }
        public SelectList ElencoStati
        {
            get
            {
                if (_ElencoStati == null)
                {
                    List<Stato> lista = null;
                    if (RERIAMPrincipal.Corrente.IsInRole(RuoloUtente.Amministatore))
                    {
                        lista = db.Stato.OrderBy(x => x.Descrizione).ToList();
                    }
                    else
                    {
                        lista = db.Stato.Where(x => x.Cod != "CON").OrderBy(x => x.Descrizione).ToList();
                    }

                    _ElencoStati = new SelectList(lista, nameof(Stato.Cod), nameof(Stato.Descrizione), CodStato);
                }
                return _ElencoStati;

            }
        }

        private SelectList _ElencoProcedimenti { get; set; }
        public SelectList ElencoProcedimenti
        {
            get
            {
                if (_ElencoProcedimenti == null)
                {
                    List<Procedimento> procedimenti = null;
                    if (RERIAMPrincipal.Corrente.IsInRole(RuoloUtente.Amministatore))
                    {
                        procedimenti = db.Procedimento.ToList();
                    }
                    else
                    {
                        procedimenti = db.Procedimento.Where(x => x.Utente.Any(z => z.Username == RERIAMPrincipal.Corrente.Username) && x.CodStato != "CON").ToList();
                    }

                    _ElencoProcedimenti = new SelectList(procedimenti.OrderByDescending(x => x.DataOraModifica).ThenBy(x => x.Descrizione), nameof(Procedimento.ID), nameof(Procedimento.Descrizione), IdProcedimento);
                }
                return _ElencoProcedimenti;

            }
        }

        public List<Procedimento> RisultatiRicerca { get; set; }
    }
}
