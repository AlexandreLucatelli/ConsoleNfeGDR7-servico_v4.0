using System;
using System.Collections.Generic;

using System.Text;

namespace NFE.Classes.NFE.Objetos.Recepcao.Transp.Cobranca
{
    public class cobr 
    {
        public cobr()
        {

        }

        fat _Fat = new fat();
        public fat Fat
        {
            get { return _Fat; }
            set { _Fat = value; }
        }

        dup _DUP = new dup();
        public dup DUP
        {
            get { return _DUP; }
            set { _DUP = value; }
        }
    }
   
}
