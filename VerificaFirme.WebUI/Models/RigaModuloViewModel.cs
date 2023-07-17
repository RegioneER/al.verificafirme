using RER.Tools.MVC.Agid.MetadataAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VerificaFirme.WebUI.Models
{
    public class ModuloDuplicatiViewModel: VerificaFirme.Db.ModelAlert
    {
        public string CodPostazione { get; set; }

        public int NrModulo { get; set; }

        [RequiredIf("IdModulo", 0, ErrorMessage = "Il campo 'Numero nominativi' è obbligatorio")]
        public virtual int? NrNominativi { get; set; }

        public int IdProcedimento { get; set; }

        public int IdModulo { get; set; }

        public Riga[] Righe { get; set; }

        public virtual List<ErroreValidazioneForm> Valida(Db.VerificaFirmeDBContext db)
        {
            List<ErroreValidazioneForm> errori = new List<ErroreValidazioneForm>();

            // valori che sono considerati validi per il comune, anche se non presenti nella vista
            // ES - Estero
            // NT - Comune non trovato
            string[] eccezioniCodiceIstat = new string[] { "NON TROVATO", "ESTERO" };

            // Verifico tutti i dati. Solo se non ci sono errori, procedo con  
            foreach (var item in Righe)
            {
                if (string.IsNullOrWhiteSpace(item.CategoriaNullita))
                {
                    // Nome
                    if (string.IsNullOrWhiteSpace(item.Nome))
                    {
                        string nomeCampo = $"Righe_{item.NrRiga - 1}_{nameof(Riga.Nome)}";
                        errori.Add(new ErroreValidazioneForm(nomeCampo, "Il campo non deve essere vuoto", nameof(Riga.Nome), item.NrRiga));
                    }

                    // Cognome
                    if (string.IsNullOrWhiteSpace(item.Cognome))
                    {
                        string nomeCampo = $"Righe_{item.NrRiga - 1}_{nameof(Riga.Cognome)}";
                        errori.Add(new ErroreValidazioneForm(nomeCampo, "Il campo non deve essere vuoto", nameof(Riga.Cognome), item.NrRiga));
                    }

                    // Comune di nascita
                    if (string.IsNullOrWhiteSpace(item.LuogoNascita))
                    {
                        string nomeCampo = $"Righe_{item.NrRiga - 1}_{nameof(Riga.LuogoNascita)}";
                        errori.Add(new ErroreValidazioneForm(nomeCampo, "Il campo non deve essere vuoto", "Luogo di nascita", item.NrRiga));
                    }
                    else if (!db.vvComune.Any(x => x.DescrizioneCompleta == item.LuogoNascita))
                    {
                        string nomeCampo = $"Righe_{item.NrRiga - 1}_{nameof(Riga.LuogoNascita)}";
                        errori.Add(new ErroreValidazioneForm(nomeCampo, "Valore non valido per il campo", "Luogo di nascita", item.NrRiga));
                    }

                    // Comune elettorale
                    if (string.IsNullOrWhiteSpace(item.ComuneElettorale))
                    {
                        string nomeCampo = $"Righe_{item.NrRiga - 1}_{nameof(Riga.ComuneElettorale)}";
                        errori.Add(new ErroreValidazioneForm(nomeCampo, "Il campo non deve essere vuoto", "Comune elettorale", item.NrRiga));
                    }
                    else if (!db.vvComune.Any(x => x.DescrizioneCompleta == item.ComuneElettorale) && !eccezioniCodiceIstat.Contains(item.ComuneElettorale))
                    {
                        string nomeCampo = $"Righe_{item.NrRiga - 1}_{nameof(Riga.ComuneElettorale)}";
                        errori.Add(new ErroreValidazioneForm(nomeCampo, "Valore non valido per il campo", "Comune elettorale", item.NrRiga));
                    }

                    // Data di nascita
                    if (!item.DataNascita.HasValue && string.IsNullOrEmpty(item.DataNascitaString))
                    {
                        //data a null e dataString nulla
                        string nomeCampo = $"Righe_{item.NrRiga - 1}_{nameof(Riga.DataNascita)}String";
                        errori.Add(new ErroreValidazioneForm(nomeCampo, "Il campo non deve essere vuoto", "Data di nascita", item.NrRiga));
                    }
                    else if (!item.DataNascita.HasValue && !string.IsNullOrEmpty(item.DataNascitaString))
                    {
                        // dataString valorizzata. Controllo che non sia un errore di deserializzazione
                        string nomeCampo = $"Righe_{item.NrRiga - 1}_{nameof(Riga.DataNascita)}String";
                        DateTime dataTemp = new DateTime();
                        if (DateTime.TryParse(item.DataNascitaString, out dataTemp))
                        {
                            item.DataNascita = dataTemp;
                        }
                        else
                        {
                            errori.Add(new ErroreValidazioneForm(nomeCampo, $"Il valore '{item.DataNascitaString}' non rappresenta una data valida", "Data di nascita", item.NrRiga));
                            item.DataNascita = null;
                        }
                    }
                }
                else
                {
                    // validazioni per riga segnata come null
                }
            }

            return errori;
        }
    }
}