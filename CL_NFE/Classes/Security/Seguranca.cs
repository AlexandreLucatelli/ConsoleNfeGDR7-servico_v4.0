using System;
using System.Collections.Generic;

using System.Text;

namespace NFE.Classes.Security
{
    public class Seguranca
    {
        public string Descriptografar(string Texto)
        {
            Byte[] b = Convert.FromBase64String(Texto);
            string decryptedConnectionString = System.Text.ASCIIEncoding.ASCII.GetString(b);
            return decryptedConnectionString;
        }

        public string Criptografar(string Texto)
        {
            Byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(Texto);
            string encryptedConnectionString = Convert.ToBase64String(b);
            return encryptedConnectionString;
        }
    }
}
