using System;
using System.Collections.Generic;

using System.Text;
using System.Configuration;

namespace NFE.Classes.NFE.Objetos.Cancela
{
    public class Cancelamento : Util.Utils
    {
        string _versao;
        public string versao
        {
            get { return _versao; }
            set { _versao = value; }
        }

        string _Id;
        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        string _tpAmb;
        public string tpAmb
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

        string _nProt;
        public string nProt
        {
            get { return _nProt; }
            set { _nProt = value; }
        }

        string _xJust;
        public string xJust
        {
            get { return _xJust; }
            set { _xJust = value; }
        }

        //**************************
        //RETORNO!! - NEW 08/03/2010
        string _IdRet;
        public string IdRet
        {
            get { return _IdRet; }
            set { _IdRet = value; }
        }

        string _verAplic;
        public string verAplic
        {
            get { return _verAplic; }
            set { _verAplic = value; }
        }

        string _cStat;
        public string cStat
        {
            get { return _cStat; }
            set { _cStat = value; }
        }

        string _xMotivo;
        public string xMotivo
        {
            get { return _xMotivo; }
            set { _xMotivo = value; }
        }

        string _cUF;
        public string cUF
        {
            get { return _cUF; }
            set { _cUF = value; }
        }

        string _dhRecbto;
        public string dhRecbto
        {
            get { return _dhRecbto; }
            set { _dhRecbto = value; }
        }

        string _nProtRet;
        public string nProtRet
        {
            get { return _nProtRet; }
            set { _nProtRet = value; }
        }
        //*******************************
        //END RETORNO!! - NEW 08/03/2010

        string _Signature;
        public string Signature
        {
            get { return _Signature; }
            set { _Signature = value; }
        }

        string _CaminhoCert = ConfigurationManager.AppSettings["CaminhoCertificado"].ToString();
        public string CaminhoCert
        {
            get { return _CaminhoCert; }
            set { _CaminhoCert = value; }
        }

        string _SenhaCert = ConfigurationManager.AppSettings["SenhaCertificado"].ToString();
        public string SenhaCert
        {
            get { return _SenhaCert; }
            set { _SenhaCert = value; }
        }

        string _PastaCancelamento = ConfigurationManager.AppSettings["Ambiente"].ToString() == "2" ?
                                  ConfigurationManager.AppSettings["PastaXMLCancelamentoHomologacao"].ToString() :
                                  ConfigurationManager.AppSettings["PastaXMLCancelamentoProducao"].ToString();
        public string PastaCancelamento
        {
        get { return _PastaCancelamento; }
        set { _PastaCancelamento = value; }
        }

        string _PastaCancelamentoRetorno = ConfigurationManager.AppSettings["Ambiente"].ToString() == "2" ?
                                  ConfigurationManager.AppSettings["PastaXMLCancelamentoHomologacaoRetorno"].ToString() :
                                  ConfigurationManager.AppSettings["PastaXMLCancelamentoProducaoRetorno"].ToString();
        public string PastaCancelamentoRetorno
        {
        get { return _PastaCancelamentoRetorno; }
        set { _PastaCancelamentoRetorno = value; }
        }


        string _PastaCancelamentoCliente = ConfigurationManager.AppSettings["Ambiente"].ToString() == "2" ?
                      ConfigurationManager.AppSettings["PastaXMLCancelamentoHomologacaoCliente"].ToString() :
                      ConfigurationManager.AppSettings["PastaXMLCancelamentoProducaoCliente"].ToString();
        public string PastaCancelamentoCliente
        {
            get { return _PastaCancelamentoCliente; }
            set { _PastaCancelamentoCliente = value; }
        }

    }
}
