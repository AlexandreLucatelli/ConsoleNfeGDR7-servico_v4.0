using System;
using System.Collections.Generic;

using System.Text;

namespace NFE.Classes.NFE.Objetos.Recepcao.Transp
{
    public class Transportadora
    {
        string _CNPJ=string.Empty;
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

        string _IE;
        public string IE
        {
            get { return _IE; }
            set { _IE = value; }
        }

        string _xEnder;
        public string xEnder
        {
            get { return _xEnder; }
            set { _xEnder = value; }
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
