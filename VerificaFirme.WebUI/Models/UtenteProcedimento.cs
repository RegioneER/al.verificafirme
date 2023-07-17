using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VerificaFirme.WebUI.Models
{
    public class UtenteProcedimento
    {
        public string Nominativo { get; set; }
        public int IdProcediento { get; set; }
        public string DescrizioneProcedimento { get; set; }
        public int IdProfilo { get; set; }
        public string DescrizioneProfilo { get; set; }
        public bool? Abilitato { get; set; }
        public string Username { get; set; }

    }
}