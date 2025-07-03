using System;
using System.Collections.Generic;

using System.Text;

namespace NFE.Classes.NFE.Objetos.Recepcao.Det.Impostos.ICMs
{
    public class ICMs51
    {
        decimal _orig;
        public decimal orig
        {
            get { return _orig; }
            set { _orig = value; }
        }


        decimal _CST;
        public decimal CST
        {
            get { return _CST; }
            set { _CST = value; }
        }

        decimal _modBC;
        public decimal modBC
        {
            get { return _modBC; }
            set { _modBC = value; }
        }

        decimal _pRedBC;
        public decimal pRedBC
        {
            get { return _pRedBC; }
            set { _pRedBC = value; }
        }

        decimal _vBC =0;
        public decimal vBC
        {
            get { return _vBC; }
            set { _vBC = value; }
        }


        decimal _pICMS;
        public decimal pICMS
        {
            get { return _pICMS; }
            set { _pICMS = value; }
        }


        decimal _vICMS =0;
        public decimal vICMS
        {
            get { return _vICMS; }
            set { _vICMS = value; }
        }
    }
}
