using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VerificaFirme.WebUI.Models
{
    public class OperazioniModuloViewModel
    {
        public int IdProcedimento { get; set; }
        public int IdModulo { get; set; }
        public string ComuneLista { get; set; }
        public bool IsNullo { get; set; }
        public string MotivoNullita { get; set; }
        public int NrNominativi { get; set; }
    }
}