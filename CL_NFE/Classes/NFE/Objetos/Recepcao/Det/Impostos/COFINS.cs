using System;
using System.Collections.Generic;

using System.Text;

namespace NFE.Classes.NFE.Objetos.Recepcao.Det.Impostos
{
    public class COFINS
    {        
        string _CST;
        public string CST
        {
            get { return _CST; }
            set { _CST = value; }
        }

        decimal _vBC = 0;
        public decimal vBC
        {
            get { return _vBC; }
            set { _vBC = value; }
        }

        decimal _pCOFINS=0;
        public decimal pCOFINS
        {
            get { return _pCOFINS; }
            set { _pCOFINS = value; }
        }

        decimal _vCOFINS = 0;
        public decimal vCOFINS
        {
            get { return _vCOFINS; }
            set { _vCOFINS = value; }
        }

        decimal _qBCProd;
        public decimal qBCProd
        {
            get { return _qBCProd; }
            set { _qBCProd = value; }
        }

        decimal _vAliqProd;
        public decimal vAliqProd
        {
            get { return _vAliqProd; }
            set { _vAliqProd = value; }
        }

    }
}
