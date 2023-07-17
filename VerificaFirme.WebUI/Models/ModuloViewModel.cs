using VerificaFirme.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VerificaFirme.WebUI.Models
{
    public class ModuloViewModel : VerificaFirme.Db.ModelAlert
    {
        private VerificaFirmeDBContext db = new VerificaFirmeDBContext();

        public Procedimento Procedimento { get; set; }
        public Modulo Modulo { get; set; }

        public string PostazioneFiltro { get; set; }
        public int? ModuloFiltro { get; set; }

        public bool FromCompila { get; set; }

        public bool MostraAncheCompilati { get; set; }

        public List<Modulo> MieiModuliInLavorazione
        {
            get
            {
                return db.Modulo.Where(x => x.IDProcedimento == Procedimento.ID &&
                                            (
                                                (!string.IsNullOrEmpty(x.UsernameModifica) && x.UsernameModifica.Equals(RERIAMPrincipal.Corrente.Username, StringComparison.InvariantCultureIgnoreCase)) ||
                                                (string.IsNullOrEmpty(x.UsernameModifica) && x.UsernameCreazione.Equals(RERIAMPrincipal.Corrente.Username, StringComparison.InvariantCultureIgnoreCase))
                                            )
                                      ).OrderByDescending(x => x.DataOraModifica.HasValue ? x.DataOraModifica.Value : x.DataOraCreazione).ToList();
            }
        }

        public List<Modulo> ModuliInLavorazioneAltriUtenti
        {
            get
            {
                return db.Modulo.Where(x => x.IDProcedimento == Procedimento.ID &&
                                        (
                                            (!string.IsNullOrEmpty(x.UsernameModifica) && !x.UsernameModifica.Equals(RERIAMPrincipal.Corrente.Username, StringComparison.InvariantCultureIgnoreCase)) ||
                                            (string.IsNullOrEmpty(x.UsernameModifica) && !x.UsernameCreazione.Equals(RERIAMPrincipal.Corrente.Username, StringComparison.InvariantCultureIgnoreCase))
                                        )
                                  ).OrderByDescending(x => x.DataOraModifica.HasValue ? x.DataOraModifica.Value : x.DataOraCreazione).ToList();
            }
        }

        public List<Modulo> TuttiModuliInLavorazione
        {
            get
            {
                List<Modulo> lista = new List<Modulo>();
                lista.AddRange(MieiModuliInLavorazione);
                lista.AddRange(ModuliInLavorazioneAltriUtenti);

                if (!string.IsNullOrEmpty(PostazioneFiltro))
                    lista = lista.Where(x => x.CodicePostazione.Equals(PostazioneFiltro)).ToList();

                if (ModuloFiltro.HasValue)
                    lista = lista.Where(x => x.Numero == ModuloFiltro.Value).ToList();

                return lista.OrderBy(x => x.CodicePostazione).ThenBy(x => x.Numero).ToList();
            }
        }


        public SelectList ElencoPostazioniTotali
        {
            get
            {
                List<SelectListItem> postazioni = new List<SelectListItem>();
                //postazioni.Add(new SelectListItem() { Text = "...", Value = "" });

                foreach (ProcedimentoPostazione pp in Procedimento.ProcedimentoPostazione.OrderBy(x => x.CodicePostazione))
                    postazioni.Add(new SelectListItem() { Text = pp.CodicePostazione, Value = pp.CodicePostazione, Selected = pp.CodicePostazione.Equals(PostazioneFiltro) });

                return new SelectList(postazioni, "Value", "Text");

            }
        }

        public SelectList ElencoModuliTotali
        {
            get
            {
                List<SelectListItem> moduli = new List<SelectListItem>();
                //moduli.Add(new SelectListItem() { Text = "...", Value = "" });

                for (int i = 1; i <= Procedimento.NumeroModuli; i++)
                    moduli.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });

                return new SelectList(moduli, "Value", "Text");
            }
        }


        private SelectList _ElencoPostazioni;
        public SelectList ElencoPostazioni
        {
            get
            {
                if (_ElencoPostazioni == null)
                {
                    List<SelectListItem> postazioni = new List<SelectListItem>();

                    foreach (ProcedimentoPostazione pp in Procedimento.ProcedimentoPostazione.OrderBy(x => x.CodicePostazione))
                    {
                        if (db.Modulo.Count(x => x.IDProcedimento == Procedimento.ID && x.CodicePostazione.Equals(pp.CodicePostazione)) < pp.NumeroModuliPostazione)
                        {
                            string postazione = pp.CodicePostazione;
                            postazioni.Add(new SelectListItem() { Text = postazione, Value = postazione });
                        }
                    }
                    _ElencoPostazioni = new SelectList(postazioni, "Value", "Text");
                }
                return _ElencoPostazioni;
            }
        }


        public SelectList ElencoModuli(string postazione)
        {
            List<SelectListItem> moduli = new List<SelectListItem>();
            ProcedimentoPostazione pp = Procedimento.ProcedimentoPostazione.FirstOrDefault(x => x.CodicePostazione.Equals(postazione));

            for (int i = pp.ModuloDa; i <= pp.ModuloA; i++)
            {
                if (MostraAncheCompilati)
                {
                    moduli.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
                }
                else
                {
                    // se devo far vedere solo i moduli non compilati, verifico prima che il modulo non esista già
                    if (!db.Modulo.Any(x => x.IDProcedimento == pp.IDProcedimento && x.CodicePostazione.Equals(postazione) && x.Numero == i))
                    {
                        moduli.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
                    }
                }
            }

            return new SelectList(moduli, "Value", "Text");
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

        private SelectList _CategorieNullita;
        public SelectList CategorieNullita
        {
            get
            {
                if (_CategorieNullita == null)
                {
                    _CategorieNullita = new SelectList(db.CategorieEsclusione.ToList().OrderBy(x => x.DescrizioneBreve), nameof(Db.CategorieEsclusione.Cod), nameof(Db.CategorieEsclusione.DescrizioneBreve), null);
                }
                return _CategorieNullita;
            }
        }

        private SelectList _CategorieSanabilita;
        public SelectList CategorieSanabilita
        {
            get
            {
                if (_CategorieSanabilita == null)
                {
                    _CategorieSanabilita = new SelectList(db.CategorieSanabilita.ToList().OrderBy(x => x.DescrizioneBreve), nameof(Db.CategorieSanabilita.Cod), nameof(Db.CategorieSanabilita.DescrizioneBreve), null);
                }
                return _CategorieSanabilita;
            }
        }
        public bool AbilitaSalvataggioAutomatico
        {
            get
            {
                return ConfigurazioneManager.Configurazione<bool>("ASA");
            }
        }
        public int MilliSecondiSalvataggioAutomatico
        {
            get
            {
                return ConfigurazioneManager.Configurazione<int>("SSA") * 1000;
            }
        }

        public int NumeroMaxRigheModulo
        {
            get
            {
                return ConfigurazioneManager.Configurazione<int>("MNRM");
            }
        }
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

        //// (1 = A, 2 = B...27 = AA...703 = AAA...)
        //private static string GetLeteraColonnaDaIndice(int nrColonna)
        //{
        //    int d = nrColonna;
        //    string letteraColonna = String.Empty;
        //    int mod;

        //    while (d > 0)
        //    {
        //        mod = (d - 1) % 26;
        //        letteraColonna = Convert.ToChar(65 + mod).ToString() + letteraColonna;
        //        d = (int)((d - mod) / 26);
        //    }

        //    return letteraColonna;
        //}

        // (A = 1, B = 2...AA = 27...AAA = 703...)
        //private static int GetIndiceColonnaDaLettera(string letteraColonna)
        //{
        //    char[] car = letteraColonna.ToUpperInvariant().ToCharArray();
        //    int somma = 0;
        //    for (int i = 0; i < car.Length; i++)
        //    {
        //        somma *= 26;
        //        somma += (car[i] - 'A' + 1);
        //    }
        //    return somma;
        //}
    }
}