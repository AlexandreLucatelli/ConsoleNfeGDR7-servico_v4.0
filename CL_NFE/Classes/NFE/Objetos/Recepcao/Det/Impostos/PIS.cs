using System;
using System.Collections.Generic;

using System.Text;

namespace NFE.Classes.NFE.Objetos.Recepcao.Det.Impostos
{
    public class PIS
    {
        string _CST;
        public string CST
        {
            get { return _CST; }
            set { _CST = value; }
        }

        decimal _vBC;
        public decimal vBC
        {
            get { return _vBC; }
            set { _vBC = value; }
        }

        decimal _pPIS;
        public decimal pPIS
        {
            get { return _pPIS; }
            set { _pPIS = value; }
        }

        decimal _vPis;
        public decimal vPis
        {
            get { return _vPis; }
            set { _vPis = value; }
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
