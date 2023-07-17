using VerificaFirme.Db;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VerificaFirme.WebUI.Models
{
    public class RigaModuloViewModelCA : ModuloDuplicatiViewModel, IValidatableObject
    {
        public override int? NrNominativi { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            using (VerificaFirmeDBContext db = new VerificaFirmeDBContext())
            {
                if (!NrNominativi.HasValue)
                {
                    // se non esiste il modulo, il numero di nomiantivi è obbligatorio
                    if (!db.Modulo.Any(x => x.IDProcedimento == IdProcedimento && x.CodicePostazione.Equals(CodPostazione) && x.Numero == NrModulo))
                    {
                        yield return new ValidationResult("Il campo 'Numero nominativi' è obbligatorio per la creazione di un nuovo modulo", new List<string> { "NrNominativi" });
                    }
                }
            }
        }
    }
}