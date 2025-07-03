using System;
using System.Collections.Generic;

using System.Text;
using NFE.Classes.NFE.Objetos.Recepcao.Det.Impostos.ICMs;
using NFE.Classes.NFE.Objetos.Recepcao.Det.Impostos.Ipi;

namespace NFE.Classes.NFE.Objetos.Recepcao.Det.Impostos
{
    public class Imposto
    {

        ICMS _icms = new ICMS();
       public ICMS Icms
        {
            get { return _icms; }
            set { _icms = value; }
        }

     /* ICMSST _icmsst = new ICMSST();
        public ICMSST Icmsst
        {
            get { return _icmsst; }
            set { _icmsst = value; }
        }*/

        IPI _ipi = new IPI();
        public IPI Ipi
        {
            get { return _ipi; }
            set { _ipi = value; }
        }

        Ii _ii = new Ii();
        public Ii II
        {
            get { return _ii; }
            set { _ii = value; }
        }

        PIS _Pis = new PIS();
        public PIS Pis
        {
            get { return _Pis; }
            set { _Pis = value; }
        }

        COFINS _Cofins = new COFINS();
        public COFINS Cofins
        {
            get { return _Cofins; }
            set { _Cofins = value; }
        }

    }
}
