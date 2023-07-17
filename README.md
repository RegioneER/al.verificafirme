# Verifica firme

L'applicativo Verifica firme ha l'obiettivo di fornire gli strumenti per la verifica firme relative a una richiesta di Referendum o a una Proposta di Legge di Iniziativa Popolare.

La verifica delle firme è necessaria sia in fase di presentazione della richiesta per attestarne l'ammissibilità sia in fase di verifica del raggiungimento del quorum per la presentazione del Referendum o della Proposta di Legge di Iniziativa Popolare.


## Indice

- [Come iniziare](#comeiniziare)
- [Contenuto del pacchetto](#contenutopacchetto)
- [Licenze software dei componenti di terze parti](#licenzesoftware)

## <a name="comeiniziare"/>Come iniziare
### Prerequisiti
L'applicazione VerificaFirme necessita di:
- Microsoft SQLServer 2016 o versioni superiori
- Microsoft IIS 8.0 Windows Server 2012 o versioni superiori
- Microsoft Visual Studio 2019 o versioni superiori
- Microsoft .NET Framework 4.8

## <a name="contenutopacchetto"/>Contenuto del pacchetto
La soluzione VerificaFirme include 3 progetti e due Script SQL per la creazione del database utilizzato.

### Progetti applicazione
- VerificaFirme.Db: applicazione di logica
- VerificaFirme.WebUI: applicazione per la verifica delle raccolta firme
- RER.Tools.MVC.Agid: libreria di classi per la gestione dei controlli web AGID

### Script di configurazione
Nel progetto sono stati inseriti alcuni script per la creazione delle tabelle, viste, stored procedure e i relativi dati base:
- Script_DB_Schema.sql: creazione tabelle per il database VerificaFirme
- Script_DB_Dati.sql: script di inserimento dati per le tabelle necessarie


### Configurazione
Per l'installazione e configurazione del sistema procedere come di seguito:

1. creare l'utenza **usrVerificaFirme** (utenza e password) a livello master per il login nell'istanza SQLServer
2. creare un nuovo database denominato **VerificaFirme**
3. eseguire lo script *Script_DB_Schema.sql*. Lo script crea le tabelle, viste, stored procedure e l'utenza usrVerificaFirme con ruolo db_datareader e db_datawriter di accesso
4. eseguire lo script *Script_DB_Dati.sql* per popolare le tabelle necessario
5. estratti i progetti in una cartella, eseguire Microsoft Visual Studio e aprire il file *VerificaFirme.sln*
9. eseguire il ripristino dei packages tramite NuGet
10.	modificare i riferimenti della connessione a SQL Server nei web.config (istanza e password)
11.	compilare la soluzione

### Parametri
Nella tabella Parametri presente sul database è possibile configurare alcune opzioni

1. **APN**: titolo dell'applicazione
2. **ASA**: abilitazione del salvataggio automatico dei moduli
3. **CR**: codice della regione (tabella Regioni) - in base alla regione impostata, i comuni della lista elettorali verranno filtrati sulla regione scelta
4. **GIL**: nome del pdf contenente la guida in linea
5. **LGO**: url di logout
6. **MAF**: anno minino per i firmatari
7. **MMP**: numero massimo di moduli per singolo procedimento
8. **MNRM**: numero massimo di righe per singolo modulo
9. **MPP**: massimo numero di postazioni per procedimento
10. **MQP**: massimo quorum per procedimento
11. **NMCA**: numero minimo di caratteri da imputare per l'autocompletamento di ricerca per i comuni
12. **NMRA**: massimo numero di risultato per i risultati dell'autocompletamento di ricerca per i comuni
13. **SSA**: numero di secondi di attesa per il salvataggio automatico



## <a name="licenzesoftware"/>Licenze software dei componenti di terze parti

### Componenti distribuiti con VerificaFirme
Vengono di seguito elencati i componenti distribuiti o richiesti con VerificaFirme che hanno una propria licenza diversa da CC0.

- [bootstrap-italia 1.6.4](https://italia.github.io/bootstrap-italia/) ©
Agenzia per l'Italia Digitale e Team per la Trasformazione Digitale, licenza BSD-3-Clause
- [fontawesome 5.5.0](https://fontawesome.com/) © Fonticons, Inc., licenza GPL

### Principali dipendenze per la fase di compilazione e sviluppo
- [Antlr 3.5.0.2](https://github.com/antlr/antlrcs) © Sam Harwell, Terence Parr, licenza BSD
- [EntityFramework 6.4.4](https://github.com/dotnet/ef6/wiki) © Microsoft, licenza Microsoft .NET Library
- [EntityFramework.it 6.2.0](https://github.com/dotnet/ef6/wiki) © Microsoft, licenza Microsoft .NET Library
- [jQuery 3.6.0](https://jquery.com/) © jQuery Foundation, licenza MIT
- [jQuery.Validation 1.19.3](https://jqueryvalidation.org/) © Jörn Zaefferer, licenza MIT
- [Microsoft.AspNet.Mvc 5.2.7](https://www.asp.net/web-pages) © Microsoft, licenza Microsoft .NET Library
- [Microsoft.AspNet.Mvc.it 5.2.7](https://www.asp.net/web-pages) © Microsoft, licenza Microsoft .NET Library
- [Microsoft.AspNet.Razor 3.2.7](https://www.asp.net/web-pages) © Microsoft, licenza Microsoft .NET Library
- [Microsoft.AspNet.Razor.it 3.2.7](https://www.asp.net/web-pages) © Microsoft, licenza Microsoft .NET Library
- [Microsoft.AspNet.Web.Optimization 1.1.3] © Microsoft, licenza Microsoft .NET Library
- [Microsoft.AspNet.WebPages 3.2.7](https://www.asp.net/web-pages) © Microsoft, licenza Microsoft .NET Library
- [Microsoft.AspNet.WebPages.it 3.2.7](https://www.asp.net/web-pages) © Microsoft, licenza Microsoft .NET Library
- [Microsoft.CodeDom.Providers.DotNetCompilerPlatform 2.0.1](https://www.asp.net) © Microsoft, licenza Microsoft .NET Library
- [Microsoft.jQuery.Unobtrusive.Validation 3.2.12](https://www.asp.net) © Microsoft, licenza Microsoft .NET Library
- [Microsoft.Web.Infrastructure 1.0.0](https://www.asp.net) © Microsoft, licenza Microsoft .NET Library
- [Newtonsoft.Json 12.0.3](https://www.newtonsoft.com/json) © James Newton-King, licenza MIT
- [WebGrease 1.6.0](http://webgrease.codeplex.com/) © webgrease@microsoft.com, licenza Microsoft .NET Library


### Componenti utilizzati per la documentazione

Di seguito è elencato il componente utilizzato per il sito della documentazione, ma non ridistribuito nel software VerificaFirme

- [ghostwriter](https://ghostwriter.kde.org/it/) © K Desktop Environment e.V., licenza GPLv3

La licenza di VerificaFirme è **GNU Affero General Public License (AGPL) versione 3 e successive (codice SPDX: AGPL-3.0-or-later)** ed è visibile sul sito [GNU Affero General Public License](https://www.gnu.org/licenses/agpl-3.0.html)

