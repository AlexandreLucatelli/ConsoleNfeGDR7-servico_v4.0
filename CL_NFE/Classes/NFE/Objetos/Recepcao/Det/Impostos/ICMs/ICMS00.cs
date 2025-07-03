using System;
using System.Collections.Generic;

using System.Text;

namespace NFE.Classes.NFE.Objetos.Recepcao.Det.Impostos.ICMs
{
    public class ICMS00
    {

        string _orig;
        public string orig
        {
            get { return _orig; }
            set { _orig = value; }
        }

        string _modBC;
        public string modBC
        {
            get { return _modBC; }
            set { _modBC = value; }
        }

        decimal _vBC =0;
        public decimal vBC
        {
            get { return _vBC; }
            set { _vBC = value; }
        }


        string _CST;
        public string CST
        {
            get { return _CST; }
            set { _CST = value; }
        }

     

        decimal _pICMS;
        public decimal pICMS
        {
            get { return _pICMS; }
            set { _pICMS = value; }
        }


        decimal _vICMS=0;
        public decimal vICMS
        {
            get { return _vICMS; }
            set { _vICMS = value; }
        }

    }
}
