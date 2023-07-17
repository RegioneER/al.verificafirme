using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using VerificaFirme.Db;
using System.Data.Entity;
using System.Web.SessionState;

namespace VerificaFirme.Db
{
    public enum RuoloUtente
    {
        Amministatore = 1,
        Supervisore = 2,
        Dataentry = 3
    }

    [Serializable]
    public class ProfiloPrincipal
    {
        public int ID { get; set; }
        public string Descrizione { get; set; }
    }

    [Serializable]
    public class UtenteProfiloPrincipal
    {
        public string Username { get; set; }
        public int IDProfilo { get; set; }
        public DateTime ValidoDal { get; set; }
        public DateTime? ValidoAl { get; set; }

        public ProfiloPrincipal Profilo { get; set; }
    }

    [Serializable]
    public class RERIAMPrincipal : IPrincipal
    {
        #region Identity Properties  

        public string Username { get; set; }
        public string Cognome { get; private set; }
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public string Telefono { get; private set; }
        public bool IsAttivo { get; private set; }
        public bool IsProfilato { get; private set; }
        public bool IsSuperUser { get; private set; }
        #endregion
        public List<UtenteProfiloPrincipal> Abilitazioni { get; private set; }
        public UtenteProfiloPrincipal AbilitazioniCorrente
        {
            get
            {
                if (Abilitazioni == null || Abilitazioni.Count == 0)
                    return null;

                return Abilitazioni.FirstOrDefault(x => x.ValidoDal <= DateTime.Now && (!x.ValidoAl.HasValue || x.ValidoAl.Value >= DateTime.Now));
            }
        }
        public virtual IIdentity Identity
        {
            get; private set;
        }

        /// <summary>
        /// Verifica se l'utente è abilitato al ruolo specificato
        /// </summary>
        /// <param name="ruolo">Il ruolo da verificare</param>
        /// <returns>true se l'utente è abilitato al ruolo specificato, altrimenti false</returns>
        public virtual bool IsInRole(string ruolo)
        {
            RuoloUtente ruoloEnum = (RuoloUtente)Enum.Parse(typeof(RuoloUtente), ruolo);
            return IsInRole(ruoloEnum);
        }

        /// <summary>
        /// Verifica se l'utente è abilitato al ruolo specificato
        /// </summary>
        /// <param name="ruolo">Il ruolo da verificare</param>
        /// <returns>true se l'utente è abilitato al ruolo specificato, altrimenti false</returns>
        public virtual bool IsInRole(RuoloUtente ruolo)
        {
            // true se:
            // - l'utente è associato a quel ruolo
            // - il ruolo è attivo alla data odierna: data dal minore o uguale a oggi e data al nulla oppure maggiore o uguale a oggi
            return Abilitazioni.Any(x => x.Profilo.ID == (int)ruolo && x.ValidoDal <= DateTime.Now && (!x.ValidoAl.HasValue || (x.ValidoAl.HasValue && x.ValidoAl.Value >= DateTime.Now)));
        }

        /// <summary>
        /// Verifica se l'utente è abilitato ad almeno uno dei ruoli spedificati
        /// </summary>
        /// <param name="ruoli">Uno o più ruoli</param>
        /// <returns>true se l'utente è abilitato ad almeno uno dei ruoli, altrimenti false</returns>
        public virtual bool IsInRole(params RuoloUtente[] ruoli)
        {
            // true se:
            // - l'utente è associato a quel ruolo
            // - il ruolo è attivo alla data odierna: data dal minore o uguale a oggi e data al nulla oppure maggiore o uguale a oggi
            var result = false;
            foreach (RuoloUtente ruolo in ruoli)
            {
                result = result || Abilitazioni.Any(x => x.Profilo.ID == (int)ruolo && x.ValidoDal <= DateTime.Now && (!x.ValidoAl.HasValue || (x.ValidoAl.HasValue && x.ValidoAl.Value >= DateTime.Now)));
            }
            return result;
        }

        public RERIAMPrincipal()
        {
            Identity = new GenericIdentity("Test");
        }

        public RERIAMPrincipal(string username)
        {
            Identity = new GenericIdentity(username);
            Username = username;

            Carica();
        }

        public static RERIAMPrincipal Corrente
        {
            get
            {
                HttpContext httpContext = HttpContext.Current;
                if (httpContext == null)
                    return null;

                HttpRequest httpRequest = httpContext.Request;
                HttpSessionState httpSession = httpContext.Session;

                string username = "";

#if (DEBUG)
                username = @"UtenzaTest";
#else
                username = string.Format(@"{0}\{1}", httpRequest.Headers["Domain"], httpRequest.Headers["Username"]);
#endif

                RERIAMPrincipal utenteSessione = (RERIAMPrincipal)httpSession["RERIAMPrincipalSessione"];
                if (utenteSessione == null || utenteSessione != null && !utenteSessione.Identity.Name.Equals(username, StringComparison.InvariantCultureIgnoreCase))
                {
                    utenteSessione = new RERIAMPrincipal(username);

                    if (utenteSessione == null || !utenteSessione.IsAttivo)
                        throw new UnauthorizedAccessException("Utente non autorizzato ad accedere all'applicazione");

                    httpSession["RERIAMPrincipalSessione"] = utenteSessione;
                }

                return utenteSessione;
            }
        }

        private void Carica()
        {
            using (VerificaFirmeDBContext db = new VerificaFirmeDBContext())
            {
                var utente = db.Utente.Find(Username) ?? throw new UnauthorizedAccessException("Utente non autorizzato");
                Abilitazioni = ToAbilitazioniPrincipal(db.UtenteProfilo.Include(z => z.Profilo).Where(x => x.Username == Username).ToList());
                IsAttivo = utente.IsAttivo;
                Nome = utente.Nome;
                Cognome = utente.Cognome;
                Email = utente.Email;
            }
        }

        private List<UtenteProfiloPrincipal> ToAbilitazioniPrincipal(List<UtenteProfilo> profili)
        {
            List<UtenteProfiloPrincipal> lista = new List<UtenteProfiloPrincipal>();
            foreach (UtenteProfilo up in profili)
                lista.Add(new UtenteProfiloPrincipal
                {
                    IDProfilo = up.IDProfilo,
                    Username = up.Username,
                    ValidoAl = up.ValidoAl,
                    ValidoDal = up.ValidoDal,
                    Profilo = new ProfiloPrincipal { Descrizione = up.Profilo.Descrizione, ID = up.Profilo.ID }
                });

            if (lista.Count(x => x.ValidoDal <= DateTime.Now && (!x.ValidoAl.HasValue || x.ValidoAl.Value >= DateTime.Now)) != 1)
            {
                throw new UnauthorizedAccessException("Utente senza una profilatura valida");
            }
            return lista;
        }
    }
}