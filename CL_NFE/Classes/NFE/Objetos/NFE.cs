using System;
using System.Collections.Generic;

using System.Text;
using NFE.Classes.NFE.Objetos.Recepcao.Dest;
using NFE.Classes.NFE.Objetos.Recepcao.Emit;
using NFE.Classes.NFE.Objetos.Recepcao.Det.Impostos;
using NFE.Classes.NFE.Objetos.Recepcao.Det;
using NFE.Classes.NFE.Objetos.Recepcao.Total;
using NFE.Classes.NFE.Objetos.Recepcao.Transp;
using NFE.Classes.NFE.Objetos.Recepcao.Ide;
using NFE.Classes.NFE.Objetos.Cancela;
using NFE.Classes.NFE.Objetos.Consulta;
using NFE.Classes.NFE.Objetos.Ret_Recepcao;
using NFE.Classes.NFE.Objetos.NFref;
using NFE.Classes.NFE.Objetos.Recepcao.Entregas;
using NFE.Classes.Util;
using System.Configuration;
using NFE.Classes.NFE.Objetos.Recepcao.Pag;

namespace NFE.Classes.NFE.Objetos
{
    public class NotaFiscalEletronica
    {
        

        string _PastaXMLRecepcao = ConfigurationManager.AppSettings["Ambiente"].ToString() == "2" ?
                                   ConfigurationManager.AppSettings["PastaXMLRecepcaoHomologacao"].ToString() :
                                   ConfigurationManager.AppSettings["PastaXMLRecepcaoProducao"].ToString();

        public string PastaXMLRecepcao
        {
            get { return _PastaXMLRecepcao; }
            set { _PastaXMLRecepcao = value; }
        }

        string _PastaXMLRecepcaoRetorno = ConfigurationManager.AppSettings["Ambiente"].ToString() == "2" ?
            ConfigurationManager.AppSettings["PastaXMLRecepcaoRetornoHomologacao"].ToString() :
            ConfigurationManager.AppSettings["PastaXMLRecepcaoRetornoProducao"].ToString();
        public string PastaXMLRecepcaoRetorno
        {
            get { return _PastaXMLRecepcaoRetorno; }
            set { _PastaXMLRecepcaoRetorno = value; }
        }

        string _PastaXMLRecepcaoCliente = ConfigurationManager.AppSettings["Ambiente"].ToString() == "2" ?
           ConfigurationManager.AppSettings["PastaXMLRecepcaoClienteHomologacao"].ToString() :
           ConfigurationManager.AppSettings["PastaXMLRecepcaoClienteProducao"].ToString();
        public string PastaXMLRecepcaoCliente
        {
            get { return _PastaXMLRecepcaoCliente; }
            set { _PastaXMLRecepcaoCliente = value; }
        }

        public NotaFiscalEletronica()
        {
            IdLote = "0";
            infNfe = "";
        }

      string _IdLote;
      public string IdLote
        {
            get { return _IdLote; }
            set {_IdLote=value; }
        }

        string _infNfe;
       public string infNfe
        {
            get { return _infNfe; }
            set { _infNfe = value; }
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

       string _AAMM;
       public string AAMM
       {
           get { return _AAMM; }
           set { _AAMM = value; }
       }

       string _nNF;
       public string nNF
       {
           get { return _nNF; }
           set { _nNF = value; }
       }

       string _cNF;
       public string cNF
       {
           get { return _cNF; }
           set { _cNF = value; }
       }

       string _cDV;
       public string cDV
       {
           get { return _cDV; }
           set { _cDV = value; }
       }


        string _CNPJ;
       public string CNPJ
       {
           get { return _CNPJ; }
           set { _CNPJ = value; }
       }

        string _cUf;
        public string cUf
       {
           get { return _cUf; }
           set { _cUf = value; }
       }

        string _LocalEmbarque;
        public string LocalEmbarque
        {
            get { return _LocalEmbarque; }
            set { _LocalEmbarque = value; }
        }

        string _UFEmbarque;
        public string UFEmbarque
        {
            get { return _UFEmbarque; }
            set { _UFEmbarque = value; }
        }

        string _NumDocDI;
        public string NumDocDI
        {
            get { return _NumDocDI; }
            set { _NumDocDI = value; }
        }

        string _DataRegistroDI;
        public string DataRegistroDI
        {
            get { return _DataRegistroDI; }
            set { _DataRegistroDI = value; }
        }

        string _LocalDesembDI;
        public string LocalDesembDI
        {
            get { return _LocalDesembDI; }
            set { _LocalDesembDI = value; }
        }

        string _UFDesembDI;
        public string UFDesembDI
        {
            get { return _UFDesembDI; }
            set { _UFDesembDI = value; }
        }

        string _IDFabEstrangeiro;
        public string IDFabEstrangeiro
        {
            get { return _IDFabEstrangeiro; }
            set { _IDFabEstrangeiro = value; }
        }

        pag _pag = new pag();
        public pag Pag
        {
            get { return _pag; }
            set { _pag = value; }
        }

        ide _ide = new ide();
        public ide Ide
        {
            get { return _ide; }
            set { _ide = value; }
        }

        dest _dest = new dest();
        public dest Dest
        {
            get { return _dest; }
            set { _dest = value; }
        }

        det _det = new det();
        public det Det
        {
            get { return _det; }
            set { _det = value; }
        }


        emit _emit = new emit();
        public emit Emit
        {
            get { return _emit; }
            set { _emit = value; }
        }

        Imposto _Imposto = new Imposto();
        public Imposto Imposto
        {
            get { return _Imposto; }
            set { _Imposto = value; }
        }

        ICMSTot _IcmsTot = new ICMSTot();
        public ICMSTot IcmsTot
        {
            get { return _IcmsTot; }
            set { _IcmsTot = value; }
        }

        Transporte _Tranp = new Transporte();
        public Transporte TRansp
        {
            get { return _Tranp; }
            set { _Tranp = value; }
        }

        private Cancelamento _Cancela = new Cancelamento();
        public Cancelamento Cancela
        {
            get { return _Cancela; }
            set { _Cancela = value; }
        }

        private ConsultaNfe _ConsultaNfe = new ConsultaNfe();
        public ConsultaNfe ConsultaNfe
        {
            get { return _ConsultaNfe; }
            set { _ConsultaNfe = value; }
        }

        private RetRecepcao _RetRecepcao = new RetRecepcao();
        public RetRecepcao RetRecepcao
        {
            get { return _RetRecepcao; }
            set { _RetRecepcao = value; }
        }


        private refNF _RefNf = new refNF();
        public refNF RefNf
        {
            get { return _RefNf; }
            set { _RefNf = value; }
        }

        private Entrega _Entregas = new Entrega();
        public Entrega Entregas
        {
            get { return _Entregas; }
            set { _Entregas = value; }
        }

        Utils _Util = new Utils();

        public Utils Util
        {
            get { return _Util; }
            set { _Util = value; }
        }


        string _DigestValue;
        public string DigestValue
        {
            get { return _DigestValue; }
            set { _DigestValue = value; }
        }

        string _SignatureValue;
        public string SignatureValue
        {
            get { return _SignatureValue; }
            set { _SignatureValue = value; }
        }


        string _x509Certificate;
        public string x509Certificate
        {
            get { return _x509Certificate; }
            set { _x509Certificate = value; }
        }

        string _URI;
        public string URI
        {
            get { return _URI; }
            set { _URI = value; }
        }

        Int64 _IDCliente;
        public Int64 IDCliente
        {
            get { return _IDCliente; }
            set { _IDCliente = value; }
        }

        string _indSinc = "0";
        public string indSinc
        {
            get { return _indSinc; }
            set { _indSinc = value; }
        }
    }
}
