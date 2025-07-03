using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace NFE.Classes.NFE.Objetos.Ret_Recepcao
{
    public class RetRecepcao : Util.Utils
    {

        string _versao;
        public string versao
        {
            get { return _versao; }
            set { _versao = value; }
        }

        string _tpAmb = ConfigurationManager.AppSettings["Ambiente"].ToString();
        public string tpAmb
        {
            get { return _tpAmb; }
            set { _tpAmb = value; }
        }

        string _nRec;
        public string nRec
        {
            get { return _nRec; }
            set { _nRec = value; }
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

        string _PastaRetRecepcao = ConfigurationManager.AppSettings["Ambiente"].ToString() == "2" ?
                                        ConfigurationManager.AppSettings["PastaXMLRetRecepcaoHomologacao"].ToString() :
                                        ConfigurationManager.AppSettings["PastaXMLRetRecepcaoProducao"].ToString();
        public string PastaRetRecepcao
        {
        get { return _PastaRetRecepcao; }
        set { _PastaRetRecepcao = value; }
        }

        string _PastaRetRecepcaoCliente = ConfigurationManager.AppSettings["Ambiente"].ToString() == "2" ?
                                            ConfigurationManager.AppSettings["PastaXMLRetRecepcaoClienteHomologacao"].ToString() :
                                            ConfigurationManager.AppSettings["PastaXMLRetRecepcaoClienteProducao"].ToString();
        public string PastaRetRecepcaoCliente
        {
            get { return _PastaRetRecepcaoCliente; }
            set { _PastaRetRecepcaoCliente = value; }
        }           
    }
}
