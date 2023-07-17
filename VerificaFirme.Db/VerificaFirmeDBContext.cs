using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace VerificaFirme.Db
{
    public class VerificaFirmeDBContext : VerificaFirmeEntities
    {
        public VerificaFirmeDBContext()
            : base(GetConnetionString())
        {
        }

        private static string GetConnetionString()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["VerificaFirmeEntities"].ConnectionString;

            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["passwordIsCrypted"]) || !bool.Parse(ConfigurationManager.AppSettings["passwordIsCrypted"]))
            {
                return connectionString;
            }

            string[] elem = connectionString.Split(';');

            for (int i = 0; i < elem.Count(); i++)
            {
                string[] pwdElem = elem[i].Split('=');
                if (pwdElem[0].Equals("password", StringComparison.CurrentCultureIgnoreCase))
                {
                    elem[i] = $"{pwdElem[0]}={elem[i].Replace("password=", "")}";
                }
                if (pwdElem[0].Equals("user id", StringComparison.CurrentCultureIgnoreCase))
                {
                    elem[i] = $"{pwdElem[0]}={elem[i].Replace("user id=", "")}";
                }
            }
            return string.Join(";", elem);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}