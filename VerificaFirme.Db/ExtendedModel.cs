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
    [Serializable]
    public class ModelAlert
    {
        public ModelAlert() { Alerts = new List<Alert>(); }

        [JsonIgnore]
        public List<Alert> Alerts { get; set; }

        public void SetAlerts(System.Web.Mvc.ModelStateDictionary ModelState)
        {
            Alert alert = new Alert();
            alert.AlertType = Alert.AlertTypeEnum.Error;
            alert.Title = "ATTENZIONE! Sono presenti degli errori! Nessun dato è stato salvato";

            foreach (var val in ModelState.Values)
            {
                foreach (var err in val.Errors)
                {
                    alert.Messages.Add(err.ErrorMessage);
                }
            }

            this.Alerts.Add(alert);
        }
    }

    [Serializable]
    public class ExtendedModel : ModelAlert
    {
        public ExtendedModel() { }

        #region public methods
        public static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            return (propertyExpression.Body as MemberExpression).Member.Name;
        }
        #endregion
    }

    public class DBHelper
    {
        public static bool ModuloGiaInserito(VerificaFirmeDBContext db, int idProcedimento, string codPostazione, int numeroModulo)
        {
            return db.Modulo.Any(x => x.IDProcedimento == idProcedimento && x.CodicePostazione.Equals(codPostazione) && x.Numero == numeroModulo);

        }

        public static bool ModuloNonDellaPostazione(VerificaFirmeDBContext db, int idProcedimento, string codPostazione, int numeroModulo)
        {

            ProcedimentoPostazione pp = db.ProcedimentoPostazione.FirstOrDefault(x => x.IDProcedimento == idProcedimento && x.CodicePostazione.Equals(codPostazione));
            if (pp == null)
                throw new ApplicationException($"Postazione non presente. Procedimento {idProcedimento} e postazione {codPostazione}");

            return numeroModulo < pp.ModuloDa || numeroModulo > pp.ModuloA;
        }

        public static bool ProcedimentoChiuso(VerificaFirmeDBContext db, int idProcedimento)
        {
            return db.Procedimento.Any(x => x.ID == idProcedimento && x.CodStato == "CON");
        }
    }
}
