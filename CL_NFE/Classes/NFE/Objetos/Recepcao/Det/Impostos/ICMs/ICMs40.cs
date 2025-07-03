using System;
using System.Collections.Generic;

using System.Text;

namespace NFE.Classes.NFE.Objetos.Recepcao.Det.Impostos.ICMs
{
    public class ICMs40
    {
        decimal _orig;
        public decimal orig
        {
            get { return _orig; }
            set { _orig = value; }
        }


        decimal _CST;
        public decimal CST
        {
            get { return _CST; }
            set { _CST = value; }
        }


        float _CST41;
        public float CST41
        {
            get { return _CST41; }
            set { _CST41 = value; }
        }

        float _CST50;
        public float CST50
        {
            get { return _CST50; }
            set { _CST50 = value; }
        }

    }
}
