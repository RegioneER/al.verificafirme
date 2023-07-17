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
    public class FirmatariDuplicatiViewModel : VerificaFirme.Db.ModelAlert
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
        public string CodComuneListaElettorale { get; set; }
        public bool ProcedimentoConcluso { get; set; }

        public int? IdProcedimento { get; set; }

        public int? IdModalita { get; set; }

        [Display(Name = "Solo nominativi da correggere")]
        public bool SoloDaCorreggere { get; set; }

        public bool ISControlloAmministrativo { get; set; }

        public RigaFirmatarioDuplicato[] RigheVerificate { get; set; }

        public List<Report_NominativiDuplicatiPerProcedimento_Result> RisultatoDuplicati { get; set; }
        public List<Report_VerificaMinori_Result> RisultatoMinori { get; set; }

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

        private SelectList _ElencoModalita;
        public SelectList ElencoModalita
        {
            get
            {
                if (_ElencoModalita == null)
                {
                    SelectListItem item = null;
                    List<SelectListItem> lista = new List<SelectListItem>();

                    item = new SelectListItem();
                    item.Value = "1";
                    item.Text = "Verifica duplicati";
                    lista.Add(item);

                    item = new SelectListItem();
                    item.Value = "2";
                    item.Text = "Verifica minori";
                    lista.Add(item);

                    _ElencoModalita = new SelectList(lista, "Value", "Text", IdModalita);

                }
                return _ElencoModalita;
            }
        }

        private SelectList _CategorieNullita;
        public SelectList CategorieNullita
        {
            get
            {
                if (_CategorieNullita == null)
                {
                    _CategorieNullita = new SelectList(db.CategorieEsclusione.OrderBy(x => x.DescrizioneBreve).ToList(), nameof(Db.CategorieEsclusione.Cod), nameof(Db.CategorieEsclusione.DescrizioneBreve), null);
                }
                return _CategorieNullita;
            }
        }

        private SelectList _ElencoComuniRestoItalia;
        public SelectList ElencoComuniRestoItalia
        {
            get
            {
                if (_ElencoComuniRestoItalia == null)
                {
                    var elencoComuni = db.vvComune.ToList();
                    string codRegioneApp = ConfigurazioneManager.Configurazione<string>("CR");
                    _ElencoComuniRestoItalia = new SelectList(elencoComuni.Where(x => x.cod_reg != codRegioneApp).OrderBy(x => x.DescrizioneCompleta), nameof(Db.vvComune.cod_istat), nameof(Db.vvComune.DescrizioneCompleta));
                }
                return _ElencoComuniRestoItalia;
            }
        }

        private SelectList _ElencoComuniPropri;
        public SelectList ElencoComuniPropri
        {
            get
            {
                if (_ElencoComuniPropri == null)
                {
                    var elencoComuni = db.vvComune.ToList();
                    vvComune cNontrovato = new vvComune();
                    string codRegioneApp = ConfigurazioneManager.Configurazione<string>("CR");
                    cNontrovato.cod_reg = codRegioneApp;
                    cNontrovato.cod_istat = "NT" + codRegioneApp;
                    cNontrovato.sigla_prov = "";
                    cNontrovato.des_com = "NON TROVATO";
                    cNontrovato.DescrizioneCompleta = "NON TROVATO";
                    elencoComuni.Add(cNontrovato);
                    _ElencoComuniPropri = new SelectList(elencoComuni.Where(x => x.cod_reg == codRegioneApp).OrderBy(x => x.DescrizioneCompleta), nameof(Db.vvComune.cod_istat), nameof(Db.vvComune.DescrizioneCompleta));
                }
                return _ElencoComuniPropri;
            }
        }

    }
}