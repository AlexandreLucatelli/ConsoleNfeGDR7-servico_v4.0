using System;
using System.Collections.Generic;

using System.Text;

namespace NFE.Classes.NFE.Objetos.Recepcao.Det.Impostos.ICMs
{
    public class ICMs90
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

        decimal _vBC;
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


        decimal _vICMS;
        public decimal vICMS
        {
            get { return _vICMS; }
            set { _vICMS = value; }
        }

        string _modBCST;
        public string modBCST
        {
            get { return _modBCST; }
            set { _modBCST = value; }
        }


        decimal _pMVAST;
        public decimal pMVAST
        {
            get { return _pMVAST; }
            set { _pMVAST = value; }
        }

        decimal _pRedBCST;
        public decimal pRedBCST
        {
            get { return _pRedBCST; }
            set { _pRedBCST = value; }
        }

        decimal _vBCST;
        public decimal vBCST
        {
            get { return _vBCST; }
            set { _vBCST = value; }
        }

        decimal _pICMSST;
        public decimal pICMSST
        {
            get { return _pICMSST; }
            set { _pICMSST = value; }
        }

        decimal _vICMSST;
        public decimal vICMSST
        {
            get { return _vICMSST; }
            set { _vICMSST = value; }
        }
    }
}
