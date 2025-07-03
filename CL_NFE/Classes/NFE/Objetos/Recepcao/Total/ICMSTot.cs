using System;
using System.Collections.Generic;

using System.Text;

namespace NFE.Classes.NFE.Objetos.Recepcao.Total
{
    public class ICMSTot
    {

        decimal _vBC;
        public  decimal vBC
        {
            get { return _vBC; }
            set { _vBC = value; }
        }

        decimal _vICMS;
        public decimal vICMS
        {
            get { return _vICMS; }
            set { _vICMS = value; }
        }

        decimal _vBCST;
        public decimal vBCST
        {
            get { return _vBCST; }
            set { _vBCST = value; }
        }

        decimal _vST;
        public decimal vST
        {
            get { return _vST; }
            set { _vST = value; }
        }


        decimal _vProd;
        public decimal vProd
        {
            get { return _vProd; }
            set { _vProd = value; }
        }


        decimal _vFrete;
        public decimal vFrete
        {
            get { return _vFrete; }
            set { _vFrete = value; }
        }


        decimal _vSeg;
        public decimal vSeg
        {
            get { return _vSeg; }
            set { _vSeg = value; }
        }


        decimal _vDesc;
        public decimal vDesc
        {
            get { return _vDesc; }
            set { _vDesc = value; }
        }

        decimal _vII;
        public decimal vII
        {
            get { return _vII; }
            set { _vII = value; }
        }

        decimal _vIPI;
        public decimal vIPI
        {
            get { return _vIPI; }
            set { _vIPI = value; }
        }

        decimal _vPIS;
        public decimal vPIS
        {
            get { return _vPIS; }
            set { _vPIS = value; }
        }

        decimal _vCOFINS;
        public decimal vCOFINS
        {
            get { return _vCOFINS; }
            set { _vCOFINS = value; }
        }

        decimal _vOutro;
        public decimal vOutro
        {
            get { return _vOutro; }
            set { _vOutro = value; }
        }

        decimal _vNF;
        public decimal vNF
        {
            get { return _vNF; }
            set { _vNF = value; }
        }

        private IssQNtot _ISSQNtot = new IssQNtot();
        public IssQNtot ISSQNtot
        {
            get { return _ISSQNtot; }
            set { _ISSQNtot = value; }
        }

        private retrib _Retrib = new retrib();
        public retrib Retrib
        {
            get { return _Retrib; }
            set { _Retrib = value; }
        }
    }
}
