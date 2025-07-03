using System;
using System.Collections.Generic;

using System.Text;

namespace NFE.Classes.NFE.Objetos.Recepcao.Entregas
{
   public  class Entrega
    {

        string _CNPJ=string.Empty;
        public string CNPJ
        {
            get { return _CNPJ; }
            set { _CNPJ = value; }
        } 

        string _xLgr;
        public string xLgr
        {
            get { return _xLgr; }
            set { _xLgr = value; }
        }

        string _nro;
        public string nro
        {
            get { return _nro; }
            set { _nro = value; }
        }

        string _xCpl;
        public string xCpl
        {
            get { return _xCpl; }
            set { _xCpl = value; }
        }

        string _xBairro;
        public string xBairro
        {
            get { return _xBairro; }
            set { _xBairro = value; }
        }

        string _cMun;
        public string cMun
        {
            get { return _cMun; }
            set { _cMun = value; }
        }

        string _xMun;
        public string xMun
        {
            get { return _xMun; }
            set { _xMun = value; }
        }

        string _UF;
        public string UF
        {
            get { return _UF; }
            set { _UF = value; }
        }
    }
}
