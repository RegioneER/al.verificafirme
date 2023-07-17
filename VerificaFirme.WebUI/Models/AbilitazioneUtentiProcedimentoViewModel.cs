using VerificaFirme.Db;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VerificaFirme.WebUI.Models
{
    public class AbilitazioneUtentiProcedimentoViewModel: ModelAlert
    {
        private VerificaFirmeDBContext db = new VerificaFirmeDBContext();

        [Display(Name = "Procedimento")]
        public int? IdProcedimento { get; set; }
        [Display(Name = "Profilo")]
        public int? IdProfilo { get; set; }

        public List<UtenteProcedimento> ElencoAbilitazioni { get; set; }

        private SelectList _ElencoProcedimenti { get; set; }
        public SelectList ElencoProcedimenti
        {
            get
            {
                if (_ElencoProcedimenti == null)
                {
                    _ElencoProcedimenti = new SelectList(db.Procedimento.Where(x=>x.CodStato != "CON").OrderByDescending(x => x.DataOraModifica).ThenBy(x => x.Descrizione).ToList(), nameof(Procedimento.ID), nameof(Procedimento.Descrizione), IdProcedimento);
                }
                return _ElencoProcedimenti;

            }
        }

        private SelectList _ElencoProfili { get; set; }
        public SelectList ElencoProfili
        {
            get
            {
                if (_ElencoProfili == null)
                {
                    _ElencoProfili = new SelectList(db.Profilo.Where(x => x.Descrizione != "Amministatore").ToList().OrderBy(x => x.Descrizione), nameof(Profilo.ID), nameof(Profilo.Descrizione), IdProfilo);
                }
                return _ElencoProfili;

            }
        }
    }
}