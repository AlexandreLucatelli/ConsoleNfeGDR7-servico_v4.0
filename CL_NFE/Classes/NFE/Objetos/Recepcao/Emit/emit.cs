using System;
using System.Collections.Generic;

using System.Text;

namespace NFE.Classes.NFE.Objetos.Recepcao.Emit
{
    public class emit
    {
        string _CNPJ;
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

        string _xFant;
        public string xFant
        {
            get { return _xFant; }
            set { _xFant = value; }
        }

        string _CRT = "3";
        public string CRT
        {
            get { return _CRT; }
            set { _CRT = value; }
        }
      
         enderEmit _enderEmit = new enderEmit();

         public enderEmit EnderEmit
        {
            get { return _enderEmit; }
            set { _enderEmit = value; }
        }

        
        string _IE;
        string IE
        {
            get { return _IE; }
            set { _IE = value; }
        }

    }
}
