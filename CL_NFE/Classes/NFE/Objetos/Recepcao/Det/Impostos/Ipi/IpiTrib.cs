using System;
using System.Collections.Generic;

using System.Text;

namespace NFE.Classes.NFE.Objetos.Recepcao.Det.Impostos.Ipi
{
   public class IpiTrib
    {

        string _CST =string.Empty;
        public string CST
        {
            get { return _CST; }
            set { _CST = value; }
        }

        decimal _vBC = 0;
        public decimal vBC
        {
            get { return _vBC; }
            set { _vBC = value; }
        }

        decimal _qUnid = 0;
        public decimal qUnid
        {
            get { return _qUnid; }
            set { _qUnid = value; }
        }


        decimal _vUnid =0;
        public decimal vUnid
        {
            get { return _vUnid; }
            set { _vUnid = value; }
        }

        decimal _pIPI = 0;
        public decimal pIPI
        {
            get { return _pIPI; }
            set { _pIPI = value; }
        }



        decimal _vIPI = 0 ;
        public decimal vIPI
        {
            get { return _vIPI; }
            set { _vIPI = value; }
        }

        public IPInt _IPINT = new IPInt();
        public IPInt IPINT
        {
            get { return _IPINT; }
            set { _IPINT = value; }
        }
      
    }
}
