using System;
using System.Collections.Generic;

using System.Text;

namespace NFE.Classes.NFE.Objetos.Recepcao.Ide
{
   public class ide
    {
       string _cUF;
      public  string cUF
        {
            get { return _cUF; }
            set { _cUF = value; }
        }

      string _cNF;
      public string cNF
      {
          get { return _cNF; }
          set { _cNF = value; }
      }


        string _natOp;
        public string natOp
        {
            get { return _natOp; }
            set { _natOp = value; }
        }

        int _indPag;
        public int indPag
        {
            get { return _indPag; }
            set { _indPag = value; }
        }

        string _dSaiEnt;
        public string dSaiEnt
        {
            get { return _dSaiEnt; }
            set { _dSaiEnt = value; }
        }

        string _Mod;
        public string Mod
        {
            get { return _Mod; }
            set { _Mod = value; }
        }

        string _Serie;
        public string Serie
        {
            get { return _Serie; }
            set { _Serie = value; }
        }

        string _nNF;
        public string nNF
        {
            get { return _nNF; }
            set { _nNF = value; }
        }


        /// <summary>
        /// Data e hora do evento no formato AAAA-MM-DDThh: mm:ssTZD 
        /// (UTC - Universal Coordinated Time, onde TZD pode ser -02:00 
        /// (Fernando de Noronha), -03:00 
        /// (Brasília) ou -04:00 
        /// (Manaus), no horário de verão serão - 01:00, -02:00 e -03:00. 
        /// Ex.: 2010-08-19T13:00:15-03:00.
        /// </summary>
        string _dEmi = string.Format("{0:yyyy-MM-ddTHH:mm:ss}{1}", DateTime.Now.AddMinutes(-3), TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).ToString().Substring(0, 6)); 
        public string dEmi
        {
            get { return _dEmi; }
            set { _dEmi = value; }
        }

        int _tpNF;
        public int tpNF
        {
            get { return _tpNF; }
            set { _tpNF = value; }
        }

        int _cMunFG;
        public int cMunFG
        {
            get { return _cMunFG; }
            set { _cMunFG = value; }
        }

        int _tpImp;
        public int tpImp
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

        string _dhCont;
        public string dhCont
        {
            get { return _dhCont; }
            set { _dhCont = value; }
        }

        string _xJust;
        public string xJust
        {
            get { return _xJust; }
            set { _xJust = value; }
        }

        string _indFinal = "0";
        public string indFinal
        {
            get { return _indFinal; }
            set { _indFinal = value; }
        }

        string _indPres;
        public string indPres
        {
            get { return _indPres; }
            set { _indPres = value; }
        }

        string _indIEDest;
        public string indIEDest
        {
            get { return _indIEDest; }
            set { _indIEDest = value; }
        }

        string _idDest;
        public string idDest
        {
            get { return _idDest; }
            set { _idDest = value; }
        }
    }
}
