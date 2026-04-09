using System;
using System.Collections.Generic;

using System.Text;

namespace NFE.Classes.NFE.Objetos.Recepcao.Det.Impostos
{
    public class IBSCBS
    {
        string _CST;
        public string CST
        {
            get { return _CST; }
            set { _CST = value; }
        }
        string _cClassTrib;
        public string cClassTrib
        {
            get { return _cClassTrib; }
            set { _cClassTrib = value; }
        }

        decimal _vBC;
        public decimal vBC
        {
            get { return _vBC; }
            set { _vBC = value; }
        }

        decimal _pIBSUF;
        public decimal pIBSUF
        {
            get { return _pIBSUF; }
            set { _pIBSUF = value; }
        }

        decimal _vIBSUF;
        public decimal vIBSUF
        {
            get { return _vIBSUF; }
            set { _vIBSUF = value; }
        }
        decimal _pIBSMun;
        public decimal pIBSMun
        {
            get { return _pIBSMun; }
            set { _pIBSMun = value; }
        }

        decimal _vIBSMun;
        public decimal vIBSMun
        {
            get { return _vIBSMun; }
            set { _vIBSMun = value; }
        }

        decimal _vIBS;
        public decimal vIBS
        {
            get { return _vIBS; }
            set { _vIBS = value; }
        }

        decimal _pCBS;
        public decimal pCBS
        {
            get { return _pCBS; }
            set { _pCBS = value; }
        }

        decimal _vCBS;
        public decimal vCBS
        {
            get { return _vCBS; }
            set { _vCBS = value; }
        }
    }
}
