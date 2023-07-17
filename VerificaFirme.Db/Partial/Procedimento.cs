using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerificaFirme.Db
{
    [MetadataType(typeof(ProcedimentoMetadata))]
    public partial class Procedimento : ExtendedModel, IValidatableObject
    {
        private int? _nrFirme;
        public int NrFirme
        {
            get
            {
                if (!_nrFirme.HasValue)
                {
                    using (VerificaFirmeDBContext db = new VerificaFirmeDBContext())
                    {
                        _nrFirme = (from mn in db.ModuloNominativo
                                    join m in db.Modulo on mn.IDModulo equals m.ID
                                    where m.IsCompleto && m.IDProcedimento == ID && !m.IsNullo &&
                                    string.IsNullOrEmpty(mn.CodCategorieEsclusione) &&
                                    !string.IsNullOrEmpty(mn.Nome) &&
                                    !string.IsNullOrEmpty(mn.Cognome) &&
                                    !string.IsNullOrEmpty(mn.CodComuneListaElettorale) &&
                                    !string.IsNullOrEmpty(mn.CodComuneNascita) &&
                                    mn.DataNascita.HasValue
                                    select mn).Count();
                    }
                }
                return _nrFirme.Value;
            }
        }

        public int? NumeroPostazioniOriginale { get; set; }
        public int? NumeroModuliOriginale { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (NumeroModuli < NumeroPostazioni)
            {
                yield return new ValidationResult("Il numero delle postazioni non deve essere maggiore del numero dei moduli", new string[] { "NumeroPostazioni", "NumeroModuli" });
            }

            if (AnnoFirmatario > DateTime.Now.Year)
            {
                yield return new ValidationResult("'Anno di validità dei firmatari' non può essere maggiore dell'anno in corso.", new string[] { "AnnoFirmatario" });
            }

            int maxPostazioniProcedimento = ConfigurazioneManager.Configurazione<int>("MPP");
            int maxModuliProcedimento = ConfigurazioneManager.Configurazione<int>("MMP");
            int minimoAnnoFirmatario = ConfigurazioneManager.Configurazione<int>("MAF");
            int maxQuorum = ConfigurazioneManager.Configurazione<int>("MQP");

            if (AnnoFirmatario < minimoAnnoFirmatario)
            {
                yield return new ValidationResult($"'Anno di validità dei firmatari' non può essere minore del {minimoAnnoFirmatario}.", new string[] { "AnnoFirmatario" });
            }

            if (QuorumFirme > maxQuorum)
            {
                yield return new ValidationResult($"'Quorum firme' non può essere maggiore di del {maxQuorum}.", new string[] { "AnnoFirmatario" });
            }

           

            if (NumeroPostazioni > maxPostazioniProcedimento)
            {
                yield return new ValidationResult($"'Numero postazioni' non può essere maggiore di {maxPostazioniProcedimento}.", new string[] { "NumeroPostazioni" });
            }

            if (NumeroModuli > maxModuliProcedimento)
            {
                yield return new ValidationResult($"'Numero moduli' non può essere maggiore di {maxModuliProcedimento}.", new string[] { "NumeroModuli" });
            }

            if (QuorumFirme <= 0)
            {
                yield return new ValidationResult($"'Quorum' non può essere minore o uguale a zero", new string[] { "QuorumFirme" });
            }


            if (!IsModificabile)
            {
                // se non è modificabile, l'unica operazione concessa è l'eumento del numero di moduli
                if (NumeroModuli < NumeroModuliOriginale)
                {
                    yield return new ValidationResult("Il numero dei moduli non può essere ridotto per i procedimenti in essere o conlusi", new string[] { "NumeroModuli" });
                }
            }
            else
            {
                if (NumeroPostazioni == NumeroPostazioniOriginale && NumeroModuli == NumeroModuliOriginale)
                {
                    using (VerificaFirmeDBContext db = new VerificaFirmeDBContext())
                    {
                        if (ProcedimentoPostazione != null && ProcedimentoPostazione.Any())
                        {
                            int index = 0;
                            var conteggio = NumeroModuli;
                            foreach (var pp in ProcedimentoPostazione)
                            {
                                int tmpDiff = ((pp.ModuloA - pp.ModuloDa) + 1);
                                conteggio -= tmpDiff;
                                if (ProcedimentoPostazione.Any(x => x.CodicePostazione != pp.CodicePostazione && pp.ModuloDa >= x.ModuloDa && pp.ModuloDa <= x.ModuloA))
                                {
                                    yield return new ValidationResult($"I moduli della postazione {pp.CodicePostazione} si sovrappongono ai moduli delle altre postazioni", new string[] { $"ProcedimentoPostazione[{index}].ModuloDa" });
                                }
                                if (ProcedimentoPostazione.Any(x => x.CodicePostazione != pp.CodicePostazione && pp.ModuloA >= x.ModuloDa && pp.ModuloA <= x.ModuloA))
                                {
                                    yield return new ValidationResult($"I moduli della postazione {pp.CodicePostazione} si sovrappongono ai moduli delle altre postazioni", new string[] { $"ProcedimentoPostazione[{index}].ModuloA" });
                                }
                                if (pp.ModuloA < pp.ModuloDa)
                                {
                                    yield return new ValidationResult("Il campo 'Modulo a' non può essere inferiore al valore del campo 'Modulo da'", new string[] { $"ProcedimentoPostazione[{index}].ModuloDa", $"ProcedimentoPostazione[{index}].ModuloA" });
                                }
                                if (pp.ModuloDa < 1)
                                {
                                    yield return new ValidationResult("Il campo 'Modulo da' non può essere inferiore a 1", new string[] { $"ProcedimentoPostazione[{index}].ModuloDa" });
                                }
                                if (pp.ModuloDa < 1 || pp.ModuloA > NumeroModuli)
                                {
                                    yield return new ValidationResult("Il campo 'Modulo a' non può essere superiore al numero totale dei moduli", new string[] { $"ProcedimentoPostazione[{index}].ModuloA" });
                                }
                                index++;
                            }
                            if (conteggio > 0)
                            {
                                yield return new ValidationResult("Uno o più moduli non sono compresi negli intervalli specificati. Verificare nuovamente la distribuzione dei moduli alle postazioni.", new string[] { $"ProcedimentoPostazione[{index}].ModuloA" });
                            }
                        }
                    }
                }
            }
        }

        public bool IsModificabile
        {
            get
            {
                using (VerificaFirmeDBContext db = new VerificaFirmeDBContext())
                {
                    return !db.Modulo.Any(x => x.IDProcedimento == ID);
                }
            }
        }

        public bool IsEliminabile
        {
            get
            {
                using (VerificaFirmeDBContext db = new VerificaFirmeDBContext())
                {
                    return !db.Modulo.Any(x => x.IDProcedimento == ID);
                }
            }
        }

        public bool IsConcluso
        {
            get { return !string.IsNullOrEmpty(CodStato) && CodStato.Equals("CON"); }
        }

        public bool IsVisualizzabile(RERIAMPrincipal utente)
        {
            bool ret = false;

            switch (CodStato)
            {
                case "ATT":
                    ret = true;
                    break;
                case "CON":
                    ret = utente.IsInRole(RuoloUtente.Amministatore);
                    break;
                case "CRE":
                    ret = true;
                    break;
                default:
                    throw new ApplicationException("Stato non valido");
            }

            return ret;
        }

        public bool IsCompletabile
        {
            get
            {
                // TODO - da implementare
                return true;
            }
        }

        public string UtenteModifica()
        {
            string ret = null;
            using (VerificaFirmeDBContext db = new VerificaFirmeDBContext())
            {
                Utente utente = db.Utente.Find(UsernameModifica);
                if (utente != null)
                    ret = $"{utente.Cognome} {utente.Nome} - {DataOraModifica:dd/MM/yyyy H:mm}";
                else
                    ret = $"{UsernameModifica} - {DataOraModifica:dd/MM/yyyy H:mm}";
            }

            return ret;
        }
    }

    public partial class ProcedimentoMetadata
    {
        public int ID { get; set; }
        public string CodStato { get; set; }
        [Display(Name = "Descrizione"), MaxLength(250, ErrorMessage = "Superata la lunghezza massima del campo")]
        public string Descrizione { get; set; }
        [Display(Name = "Quorum")]
        public int QuorumFirme { get; set; }
        [Display(Name = "Numero postazioni")]
        public int NumeroPostazioni { get; set; }
        [Display(Name = "Numero moduli")]
        public int NumeroModuli { get; set; }
        [Display(Name = "Anno di validità dei firmatari")]
        public int AnnoFirmatario { get; set; }
    }
}
