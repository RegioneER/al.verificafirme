using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Configuration;
using System;
namespace VerificaFirme.Db
{
    public class Global
    {
        
        public class TrustAllCertificatePolicy : System.Net.ICertificatePolicy
        {
            public TrustAllCertificatePolicy() { }
            public bool CheckValidationResult(ServicePoint sp, X509Certificate cert, WebRequest req, int problem)
            {
                return true;
            }
        }

    }
}