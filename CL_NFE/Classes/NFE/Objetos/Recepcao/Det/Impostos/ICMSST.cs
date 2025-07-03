using System;
using System.Collections.Generic;

using System.Text;

namespace NFE.Classes.NFE.Objetos.Recepcao.Det.Impostos
{
    public class ICMSST
    {
        int _modBC;
        public int modBC
        {
            get { return _modBC; }
            set { _modBC = value; }
        }

        int _pMVA;
        public int pMVA
        {
            get { return _pMVA; }
            set { _pMVA = value; }
        }


        int _pRedBC;
        public int pRedBC
        {
            get { return _pRedBC; }
            set { _pRedBC = value; }
        }

        int _vBC;
        public int vBC
        {
            get { return _vBC; }
            set { _vBC = value; }
        }

        int _pICMS;
        public int pICMS
        {
            get { return _pICMS; }
            set { _pICMS = value; }
        }


        int _vICMS;
        public int vICMS
        {
            get { return _vICMS; }
            set { _vICMS = value; }
        }

    }
}
