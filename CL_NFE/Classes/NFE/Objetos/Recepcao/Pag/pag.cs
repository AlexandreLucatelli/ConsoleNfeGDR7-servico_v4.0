using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NFE.Classes.NFE.Objetos.Recepcao.Pag
{
    public class pag
    {
        int _indPag;
        public int indPag
        {
            get { return _indPag; }
            set { _indPag = value; }
        }

        string _tPag;
        public string tPag
        {
            get { return _tPag; }
            set { _tPag = value; }
        }

        decimal _vPag;
        public decimal vPag
        {
            get { return _vPag; }
            set { _vPag = value; }
        }

        string _CNPJ;
        public String CNPJ
        {
            get { return _CNPJ; }
            set { _CNPJ = value; }
        }

        string _tBand;
        public string tBand
        {
            get { return _tBand; }
            set { _tBand = value; }
        }

        decimal _vTroco;
        public decimal vTroco
        {
            get { return _vTroco; }
            set { _vTroco = value; }
        }
    }
}
