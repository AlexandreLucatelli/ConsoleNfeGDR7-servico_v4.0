using System;
using System.Collections.Generic;

using System.Text;

namespace NFE.Classes.NFE.Objetos.Recepcao.Det.Impostos.Ipi
{
    public class IPI
    {
        string _clEnq =string.Empty;
        public string clEnq
        {
            get { return _clEnq; }
            set { _clEnq = value; }
        }

        string _CNPJProd = string.Empty;
        public string CNPJProd
        {
            get { return _CNPJProd; }
            set { _CNPJProd = value; }
        }

        string _cSelo = string.Empty;
        public string cSelo
        {
            get { return _cSelo; }
            set { _cSelo = value; }
        }


        float _qSelo = 0;
        public float qSelo
        {
            get { return _qSelo; }
            set { _qSelo = value; }
        }

        string _cEnq = string.Empty;
        public string cEnq
        {
            get { return _cEnq; }
            set { _cEnq = value; }
        }


        IpiTrib _IPITrib = new IpiTrib();
        public IpiTrib IPITrib
        {
            get { return _IPITrib; }
            set { _IPITrib = value; }
        }

       

    }
}
