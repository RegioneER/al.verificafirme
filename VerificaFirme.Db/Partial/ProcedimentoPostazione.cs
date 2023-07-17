using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerificaFirme.Db
{
    [MetadataType(typeof(ProcedimentoPostazioneMetadata))]
    public partial class ProcedimentoPostazione : ExtendedModel
    {
        public int NumeroModuliPostazione
        {
            get { return ModuloA - ModuloDa + 1; }
        }
    }

    public partial class ProcedimentoPostazioneMetadata
    {
        public int IDProcedimento { get; set; }
        public string CodicePostazione { get; set; }
        public int ModuloDa { get; set; }
        public int ModuloA { get; set; }
    }
}
