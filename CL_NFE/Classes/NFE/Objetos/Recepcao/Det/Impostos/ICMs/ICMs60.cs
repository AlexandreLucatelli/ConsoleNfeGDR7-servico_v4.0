using System;
using System.Collections.Generic;

using System.Text;

namespace NFE.Classes.NFE.Objetos.Recepcao.Det.Impostos.ICMs
{
   public class ICMs60
    {
        string _orig;
        public string orig
        {
            get { return _orig; }
            set { _orig = value; }
        }

        string _CST;
        public string CST
        {
            get { return _CST; }
            set { _CST = value; }
        }

        decimal _vBCST;
        public decimal vBCST
        {
            get { return _vBCST; }
            set { _vBCST = value; }
        }

        decimal _vICMSST;
        public decimal vICMSST
        {
            get { return _vICMSST; }
            set { _vICMSST = value; }
        }
      

     
    }
}
