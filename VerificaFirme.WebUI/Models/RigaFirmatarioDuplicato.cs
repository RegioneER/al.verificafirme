using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VerificaFirme.WebUI.Models
{
    public class RigaFirmatarioDuplicato
    {
        public int NrRiga { get; set; }
        public int IDModulo { get; set; }
        public string CategoriaNullita { get; set; }
        public string Note { get; set; }

        public bool RigaModificata(Db.ModuloNominativo rigaDB, Db.VerificaFirmeDBContext db)
        {

            if (!Uguali(CategoriaNullita, rigaDB.CodCategorieEsclusione))
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