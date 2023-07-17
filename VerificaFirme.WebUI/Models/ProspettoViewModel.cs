using VerificaFirme.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VerificaFirme.WebUI.Models
{
    public class ProspettoViewModel
    {
        public enum TipoCompletamentoModulo
        {
            NonCompilato, Compilato, ParzialmenteCompilato
        }

        public class ProspettoModuloDettaglio
        {
            public int Modulo { get; set; }
            public TipoCompletamentoModulo Completamento { get; set; }
            public string CompletamentoTesto
            {
                get
                {
                    if (Completamento == TipoCompletamentoModulo.Compilato)
                        return "Compilato";
                    if (Completamento == TipoCompletamentoModulo.NonCompilato)
                        return "Non compilato";
                    if (Completamento == TipoCompletamentoModulo.ParzialmenteCompilato)
                        return "Parzialmente compilato";

                    return "";
                }
            }
        }
        public class ProspettoDettaglio
        {
            public string Postazione { get; set; }
            public List<ProspettoModuloDettaglio> Dettaglio { get; set; }
        }

        public int IDProcedimento { get; set; }
        public string TitoloProcedimento { get; set; }
        public List<ProspettoDettaglio> Postazioni { get; set; }

        public int NumeroFirme { get; set; }
        public int QuorumFirme { get; set; }

        public ProspettoViewModel(int idProcedimento)
        {
            using (VerificaFirmeDBContext db = new VerificaFirmeDBContext())
            {
                IDProcedimento = idProcedimento;

                Procedimento p = db.Procedimento.Find(idProcedimento);
                QuorumFirme = p.QuorumFirme;
                NumeroFirme = p.NrFirme;
                TitoloProcedimento = p.Descrizione;
                Postazioni = new List<ProspettoDettaglio>();
                foreach (ProcedimentoPostazione pp in p.ProcedimentoPostazione)
                {
                    ProspettoDettaglio pd = new ProspettoDettaglio();
                    pd.Postazione = pp.CodicePostazione;
                    pd.Dettaglio = new List<ProspettoModuloDettaglio>();

                    List<Modulo> modulidb = db.Modulo.Where(x => x.IDProcedimento == IDProcedimento && x.CodicePostazione.Equals(pp.CodicePostazione)).ToList();
                    for (int i = pp.ModuloDa; i <= pp.ModuloA; i++)
                    {
                        ProspettoModuloDettaglio pmd = new ProspettoModuloDettaglio();
                        pmd.Modulo = i;
                        pmd.Completamento = TipoCompletamentoModulo.NonCompilato;

                        Modulo modulo = modulidb.FirstOrDefault(x => x.Numero == i);
                        if (modulo != null)
                        {
                            pmd.Completamento = modulo.IsCompleto ? TipoCompletamentoModulo.Compilato : TipoCompletamentoModulo.ParzialmenteCompilato;
                        }
                        pd.Dettaglio.Add(pmd);
                    }

                    Postazioni.Add(pd);
                }
            }
        }
    }
}