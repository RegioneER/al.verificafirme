using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace VerificaFirme.Db
{
    public partial class VerificaFirmeEntities
    {
        public VerificaFirmeEntities(string connectionString) 
            : base(connectionString)
        {
        }
    }
}