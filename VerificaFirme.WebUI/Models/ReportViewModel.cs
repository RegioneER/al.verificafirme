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
    public class ReportViewModel : VerificaFirme.Db.ModelAlert
    {
        private VerificaFirmeDBContext db = new VerificaFirmeDBContext();

        public int NrMaxRisultatiAutocomplete
        {
            get
            {
                return ConfigurazioneManager.Configurazione<int>("NMRA");
            }
        }
        public int NrMinimoCaratteriAutocomplete
        {
            get
            {
                return ConfigurazioneManager.Configurazione<int>("NMCA");
            }
        }

        public int? IdProcedimento { get; set; }

        private SelectList _ElencoReport;
        public SelectList ElencoReport
        {
            get
            {
                if (_ElencoReport == null)
                {
                    SelectListItem item = null;
                    List<SelectListItem> lista = new List<SelectListItem>();

                    item = new SelectListItem();
                    item.Value = "1";
                    item.Text = "Gestione anagrafiche";
                    lista.Add(item);

                    item = new SelectListItem();
                    item.Value = "2";
                    item.Text = "Riepilogo Data Entry";
                    lista.Add(item);


                    _ElencoReport = new SelectList(lista, "Value", "Text");

                }
                return _ElencoReport;
            }
        }

    }
}