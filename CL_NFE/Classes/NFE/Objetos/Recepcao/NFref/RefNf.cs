using System;
using System.Collections.Generic;

using System.Text;

namespace NFE.Classes.NFE.Objetos.NFref
{
    public class refNF
    {
        string _cUF;
        public string cUF
        {
            get { return _cUF; }
            set { _cUF = value; }
        }

        string _AAMM;
        public string AAMM
        {
            get { return _AAMM; }
            set { _AAMM = value; }
        }

        string _CNPJ;
        public string CNPJ
        {
            get { return _CNPJ; }
            set { _CNPJ = value; }
        }

        string _mod;
        public string mod
        {
            get { return _mod; }
            set { _mod = value; }
        }

        string _serie;
        public string serie
        {
            get { return _serie; }
            set { _serie = value; }
        }

        string _chNfeReferenciada;
        public string chNfeReferenciada
        {
            get { return _chNfeReferenciada; }
            set { _chNfeReferenciada = value; }
        }

        string _nNF;
        public string nNF
        {
            get { return _nNF; }
            set { _nNF = value; }
        }

        string _tpImp;
        public string tpImp
        {
            get { return _tpImp; }
            set { _tpImp = value; }
        }

        string _tpEmis;
        public string tpEmis
        {
            get { return _tpEmis; }
            set { _tpEmis = value; }
        }

        string _cDV;
        public string cDV
        {
            get { return _cDV; }
            set { _cDV = value; }
        }

        string _tpAmb;
        public string tpAmb
        {
            get { return _tpAmb; }
            set { _tpAmb = value; }
        }

        string _finNFe;
        public string finNFe
        {
            get { return _finNFe; }
            set { _finNFe = value; }
        }

        string _procEmi;
        public string procEmi
        {
            get { return _procEmi; }
            set { _procEmi = value; }
        }

        string _verProc;
        public string verProc
        {
            get { return _verProc; }
            set { _verProc = value; }
        }

        private string __NfeReferenciada;
        public string NfeReferenciada
        {
            get { return __NfeReferenciada; }
            set { __NfeReferenciada = value; }
        }
    }
}
