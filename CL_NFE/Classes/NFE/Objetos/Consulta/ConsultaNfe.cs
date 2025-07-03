using System;
using System.Collections.Generic;

using System.Text;

namespace NFE.Classes.NFE.Objetos.Consulta
{
    public class ConsultaNfe
    {
        string _versao;
        public string versao
        {
            get { return _versao; }
            set { _versao = value; }
        }

        int _tpAmb;
        public int tpAmb
        {
            get { return _tpAmb; }
            set { _tpAmb = value; }
        }

        string _xServ;
        public string xServ
        {
            get { return _xServ; }
            set { _xServ = value; }
        }

        string _chNFe;
        public string chNFe
        {
            get { return _chNFe; }
            set { _chNFe = value; }
        }

    }
}
