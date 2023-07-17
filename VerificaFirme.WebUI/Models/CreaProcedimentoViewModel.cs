//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web;
//using VerificaFirme.Db;

//namespace VerificaFirme.WebUI.Models
//{
//    [MetadataType(typeof(ProcedimentoMetadata))]
//    public class ModificaProcedimentoViewModel : Procedimento, IValidatableObject
//    {
//        public int? NumeroPostazioniOriginale { get; set; }
//        public int? NumeroModuliOriginale { get; set; }


//        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
//        {
//            if (NumeroModuli < NumeroPostazioni)
//            {
//                yield return new ValidationResult("Il numero delle postazioni non deve essere maggiore del numero dei moduli", new string[] { "NumeroPostazioni", "NumeroModuli" });
//            }

//            if (AnnoFirmatario > DateTime.Now.Year)
//            {
//                yield return new ValidationResult("'Anno di validità dei firmatari' non può essere maggiore dell'anno in corso.", new string[] { "AnnoFirmatario" });
//            }

//            if (AnnoFirmatario < 2000)
//            {
//                yield return new ValidationResult("'Anno di validità dei firmatari' non può essere minore del 2000.", new string[] { "AnnoFirmatario" });
//            }

//            int maxPostazioniProcedimento = ConfigurazioneManager.Configurazione<int>("MPP");
//            int maxModuliProcedimento = ConfigurazioneManager.Configurazione<int>("MMP");

//            if (NumeroPostazioni > maxPostazioniProcedimento)
//            {
//                yield return new ValidationResult($"'Numero postazioni' non può essere maggiore di {maxPostazioniProcedimento}.", new string[] { "NumeroPostazioni" });
//            }

//            if (NumeroModuli > maxModuliProcedimento)
//            {
//                yield return new ValidationResult($"'Numero moduli' non può essere maggiore di {maxModuliProcedimento}.", new string[] { "NumeroModuli" });
//            }


//            if (!IsModificabile)
//            {
//                // se non è modificabile, l'unica operazione concessa è l'eumento del numero di moduli
//                if (NumeroModuli < NumeroModuliOriginale)
//                {
//                    yield return new ValidationResult("Il numero dei moduli non può essere ridotto per i procedimenti in essere o conlusi", new string[] { "NumeroModuli" });
//                }
//            }
//            else
//            {
//                if (NumeroPostazioni == NumeroPostazioniOriginale && NumeroModuli == NumeroModuliOriginale)
//                {
//                    using (VerificaFirmeDBContext db = new VerificaFirmeDBContext())
//                    {
//                        if (ProcedimentoPostazione != null && ProcedimentoPostazione.Any())
//                        {
//                            int index = 0;
//                            var conteggio = NumeroModuli;
//                            foreach (var pp in ProcedimentoPostazione)
//                            {
//                                int tmpDiff = ((pp.ModuloA - pp.ModuloDa) + 1);
//                                conteggio -= tmpDiff;
//                                if (ProcedimentoPostazione.Any(x => x.CodicePostazione != pp.CodicePostazione && pp.ModuloDa >= x.ModuloDa && pp.ModuloDa <= x.ModuloA))
//                                {
//                                    yield return new ValidationResult($"I moduli della postazione {pp.CodicePostazione} si sovrappongono ai moduli delle altre postazioni", new string[] { $"ProcedimentoPostazione[{index}].ModuloDa" });
//                                }
//                                if (ProcedimentoPostazione.Any(x => x.CodicePostazione != pp.CodicePostazione && pp.ModuloA >= x.ModuloDa && pp.ModuloA <= x.ModuloA))
//                                {
//                                    yield return new ValidationResult($"I moduli della postazione {pp.CodicePostazione} si sovrappongono ai moduli delle altre postazioni", new string[] { $"ProcedimentoPostazione[{index}].ModuloA" });
//                                }
//                                if (pp.ModuloA < pp.ModuloDa)
//                                {
//                                    yield return new ValidationResult("Il campo 'Modulo a' non può essere inferiore al valore del campo 'Modulo da'", new string[] { $"ProcedimentoPostazione[{index}].ModuloDa", $"ProcedimentoPostazione[{index}].ModuloA" });
//                                }
//                                if (pp.ModuloDa < 1)
//                                {
//                                    yield return new ValidationResult("Il campo 'Modulo da' non può essere inferiore a 1", new string[] { $"ProcedimentoPostazione[{index}].ModuloDa" });
//                                }
//                                if (pp.ModuloDa < 1 || pp.ModuloA > NumeroModuli)
//                                {
//                                    yield return new ValidationResult("Il campo 'Modulo a' non può essere superiore al numero totale dei moduli", new string[] { $"ProcedimentoPostazione[{index}].ModuloA" });
//                                }
//                                index++;
//                            }
//                            if (conteggio > 0)
//                            {
//                                yield return new ValidationResult("Uno o più moduli non sono compresi negli intervalli specificati. Verificare nuovamente la distribuzione dei moduli alle postazioni.", new string[] { $"ProcedimentoPostazione[{index}].ModuloA" });
//                            }
//                        }
//                    }
//                }
//            }
//        }
//    }
//}