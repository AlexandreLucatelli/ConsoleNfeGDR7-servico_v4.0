using System;
using System.Collections.Generic;

using System.Text;

namespace NFE.Classes.NFE.Objetos.Recepcao.Dest
{
   public class dest
    {
       

       string _CNPJ=string.Empty;
      public string CNPJ
       {
           get { return _CNPJ; }
           set { _CNPJ = value; }
       }

       string _xNome;
       public string xNome
       {
           get { return _xNome; }
           set { _xNome = value; }
       }

       string _email;
       public string email
       {
           get { return _email; }
           set { _email = value; }
       }

       enderDest _enderDest = new enderDest();
       public enderDest EnderDest
       {
           get { return _enderDest; }
           set { _enderDest = value; }
       }

       string _indIEDest;
       public string indIEDest
       {
           get { return _indIEDest; }
           set { _indIEDest = value; }
       }
    }
}
