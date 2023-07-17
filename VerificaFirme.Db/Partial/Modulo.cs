using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerificaFirme.Db
{
    public class ModuloCompletatoException : Exception
    {
        public ModuloCompletatoException(string errore) : base(errore) { }
    }

    [MetadataType(typeof(ModuloMetadata))]
    public partial class Modulo : ExtendedModel
    {

        public string Stato
        {
            get
            {
                int righeInserite = ModuloNominativo.Count(x =>
                                            !string.IsNullOrEmpty(x.Nome) ||
                                            !string.IsNullOrEmpty(x.Cognome) ||
                                            x.DataNascita.HasValue ||
                                            !string.IsNullOrEmpty(x.CodComuneListaElettorale) ||
                                            !string.IsNullOrEmpty(x.CodComuneNascita) ||
                                            !string.IsNullOrEmpty(x.CodCategorieEsclusione));

                string ret = null;

                if (IsCompleto)
                    ret = $"Completato {(IsNullo ? " (modulo NULLO) ": "")}- {NumeroRighe}/{NumeroRighe}";
                else
                {
                    if (righeInserite <= NumeroRighe)
                        ret = $"In lavorazione - {righeInserite}/{NumeroRighe}";

                    if (righeInserite == 0)
                        ret = $"Creato - {righeInserite}/{NumeroRighe}";
                }

                return ret;
            }
        }

        public int UltimaRigaCompletata
        {
            get
            {
                ModuloNominativo mn = ModuloNominativo.LastOrDefault(x =>
                                           (!string.IsNullOrEmpty(x.Nome) &&
                                           !string.IsNullOrEmpty(x.Cognome) &&
                                           x.DataNascita.HasValue &&
                                           !string.IsNullOrEmpty(x.CodComuneNascita)) ||
                                           !string.IsNullOrEmpty(x.CodCategorieEsclusione));

                return mn != null ? mn.NumeroRiga : 0;
            }
        }

        public string UtenteModifica(bool perConferma)
        {
            string ret = null;
            using (VerificaFirmeDBContext db = new VerificaFirmeDBContext())
            {
                if (!string.IsNullOrEmpty(UsernameModifica))
                {
                    Utente utente = db.Utente.Find(UsernameModifica);
                    if (utente != null)
                        ret = $"{utente.Cognome} {utente.Nome} -{(perConferma ? " alle ore " : "")} {DataOraModifica.Value:dd/MM/yyyy H:mm}";
                    else
                        ret = $"{UsernameModifica} -{(perConferma ? " alle ore " : "")} {DataOraModifica.Value:dd/MM/yyyy H:mm}";
                }
                else
                {
                    Utente utente = db.Utente.Find(UsernameCreazione);
                    if (utente != null)
                        ret = $"{utente.Cognome} {utente.Nome} -{(perConferma ? " alle ore " : "")} {DataOraCreazione:dd/MM/yyyy H:mm}";
                    else
                        ret = $"{UsernameModifica} -{(perConferma ? " alle ore " : "")} {DataOraCreazione:dd/MM/yyyy H:mm}";
                }

            }
            return ret;
        }
    }

    public partial class ModuloMetadata
    {
        public int ID { get; set; }
        public int IDProcedimento { get; set; }
        public string CodicePostazione { get; set; }
        public int Numero { get; set; }
        public bool IsNullo { get; set; }
        public int NumeroRighe { get; set; }
        public string CodComuneListaElettorale { get; set; }
        public string CodCategoriaEsclusione { get; set; }
        public bool IsCompleto { get; set; }
        public string UsernameCreazione { get; set; }
        public System.DateTime DataOraCreazione { get; set; }
        public string UsernameModifica { get; set; }
        public Nullable<System.DateTime> DataOraModifica { get; set; }
    }
}
