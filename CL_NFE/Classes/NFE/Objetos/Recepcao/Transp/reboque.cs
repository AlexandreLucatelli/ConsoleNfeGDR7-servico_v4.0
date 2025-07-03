using System;
using System.Collections.Generic;

using System.Text;

namespace NFE.Classes.NFE.Objetos.Recepcao.Transp
{
    public class reboque
    {
        string _placa=string.Empty;
        public string placa
        {
            get { return _placa; }
            set { _placa = value; }
        }

        string _UF;
        public string UF
        {
            get { return _UF; }
            set { _UF = value; }
        }

        string _RNTC=string.Empty;
        public string RNTC
        {
            get { return _RNTC; }
            set { _RNTC = value; }
        }
    }
}
