using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VerificaFirme.WebUI.Models
{
    public class Riga
    {
        public int NrRiga { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string LuogoNascita { get; set; }
        public DateTime? DataNascita { get; set; }
        public string DataNascitaString { get; set; }
        public string ComuneElettorale { get; set; }
        public string NumeroLista { get; set; }
        public string CategoriaSanabilita { get; set; }
        public string CategoriaNullita { get; set; }
        public string Note { get; set; }

        public bool RigaModificata(Db.ModuloNominativo rigaDB, Db.VerificaFirmeDBContext db)
        {
            if (!Uguali(Nome, rigaDB.Nome))
                return true;
            
            if (!Uguali(Cognome, rigaDB.Cognome))
                return true;
            
            string codNascita = db.vvComune.FirstOrDefault(x => x.DescrizioneCompleta == LuogoNascita.ToUpper())?.cod_istat;
            if (!Uguali(codNascita, rigaDB.CodComuneNascita))
                return true;

            if (!Uguali(NumeroLista, rigaDB.NListaElettorale))
                return true;

            string codComuneElet = db.vvComune.FirstOrDefault(x => x.DescrizioneCompleta == ComuneElettorale.ToUpper())?.cod_istat;
            if (!Uguali(codComuneElet, rigaDB.CodComuneListaElettorale))
                return true;
            
            if (!Uguali(CategoriaNullita, rigaDB.CodCategorieEsclusione))
                return true;
            
            if (!Uguali(CategoriaSanabilita, rigaDB.CodCategorieSanabilita))
                return true;
            
            if (!Uguali(DataNascitaString, rigaDB.DataNascita.HasValue ? rigaDB.DataNascita.Value.ToString("dd/MM/yyyy") : null))
                return true;

            if (!Uguali(Note, rigaDB.Note))
                return true;

            return false;
        }

        private bool Uguali(string a, string b)
        {
            return string.Compare(a, b, StringComparison.InvariantCultureIgnoreCase) == 0;
        }
    }
}