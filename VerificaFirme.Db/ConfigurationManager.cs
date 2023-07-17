using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.ModelBinding;

namespace VerificaFirme.Db
{
    public class ConfigurazioneManager
    {
        public static T Configurazione<T>(string cod)
        {
            using (VerificaFirmeDBContext db = new VerificaFirmeDBContext())
            {
                return Configurazione<T>(cod, db);
            }
        }

        public static T Configurazione<T>(string cod, VerificaFirmeDBContext db)
        {
            if (string.IsNullOrEmpty(cod))
                throw new ApplicationException($"Paramento obbligatorio");

            Parametri parametro = db.Parametri.Find(cod.ToUpper());
            object valore = null;
            if (parametro == null)
                //cerco new web.config
                valore = ConfigurationManager.AppSettings[cod].ToString();
            else
                valore = parametro.Valore;

            if (valore == null)
                throw new ApplicationException($"Paramento {cod} non valido");

            return (T)Convert.ChangeType(valore, typeof(T));
        }
    }
}
