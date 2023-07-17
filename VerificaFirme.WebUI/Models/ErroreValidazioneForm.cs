using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VerificaFirme.WebUI.Models
{
    [Serializable]
    public class ErroreValidazioneForm
    {
        public string id { get; set; }
        public string DescrizioneErrore { get; set; }
        public string NomeCampo { get; set; }
        public int Riga { get; set; }
        public ErroreValidazioneForm(string Id, string DescrizioneErrore, string NomeCampo, int Riga)
        {
            this.id = Id;
            this.DescrizioneErrore = DescrizioneErrore;
            this.NomeCampo = NomeCampo;
            this.Riga = Riga;
        }
    }
}