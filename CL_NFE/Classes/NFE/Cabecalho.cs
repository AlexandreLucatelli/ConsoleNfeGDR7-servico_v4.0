using System;
using System.Collections.Generic;

using System.Text;
using System.Configuration;

namespace NFE.Classes.NFE
{
    public class Cabecalho
    {
        protected CL_NFE.HomologCancelamento2.nfeCabecMsg objCancelamentoWSCab = new CL_NFE.HomologCancelamento2.nfeCabecMsg();
        protected CL_NFE.HomologRecepcao2.nfeCabecMsg objWSCab = new CL_NFE.HomologRecepcao2.nfeCabecMsg();
        protected CL_NFE.HomologRetRecepcao2.nfeCabecMsg objRetWSCab = new CL_NFE.HomologRetRecepcao2.nfeCabecMsg();

        protected CL_NFE.ProducaoCancelamento2.nfeCabecMsg objCancelamentoCab = new CL_NFE.ProducaoCancelamento2.nfeCabecMsg();
        protected CL_NFE.ProducaoRecepcao2.nfeCabecMsg objCab = new CL_NFE.ProducaoRecepcao2.nfeCabecMsg();
        protected CL_NFE.ProducaoRetRecepcao2.nfeCabecMsg objRetCab = new CL_NFE.ProducaoRetRecepcao2.nfeCabecMsg();

        protected CL_NFE.SCANProducaoCancelamento.nfeCabecMsg objSCANCancelamentoCab = new CL_NFE.SCANProducaoCancelamento.nfeCabecMsg();
        protected CL_NFE.SCANProducaoRecepcao.nfeCabecMsg objSCANCab = new CL_NFE.SCANProducaoRecepcao.nfeCabecMsg();
        protected CL_NFE.SCANProducaoRetRecepcao.nfeCabecMsg objSCANRetCab = new CL_NFE.SCANProducaoRetRecepcao.nfeCabecMsg();

        protected CL_NFE.ProducaoEvento.nfeCabecMsg objCCeCab = new CL_NFE.ProducaoEvento.nfeCabecMsg();
        protected CL_NFE.HomologacaoCCe.nfeCabecMsg objHomologCCeCab = new CL_NFE.HomologacaoCCe.nfeCabecMsg();

        #region ANTIGO WEB SERVICE E SCAN USANDO VERSÃO 3.0!!

        public string FncRetornaCabecalho()
        {
            StringBuilder Cabecalho = new StringBuilder();
            Cabecalho.Append("<?xml version='1.0' encoding='" + ConfigurationManager.AppSettings["CodificacaoCabecalho"].ToString() + "'?>");
            Cabecalho.Append("<cabecMsg xmlns='" + ConfigurationManager.AppSettings["NameSpaceNFE"].ToString() + "' versao='" + ConfigurationManager.AppSettings["VersaoCabecalhoNFE"].ToString() + "'>");
            Cabecalho.Append("<versaoDados>" + ConfigurationManager.AppSettings["VersaoDadosNFE"].ToString() + "</versaoDados>");
            Cabecalho.Append("</cabecMsg>");
            return Cabecalho.ToString();
        }

        public string FncRetornaCabecalhoCancelamento()
        {
            StringBuilder Cabecalho = new StringBuilder();
            Cabecalho.Append("<?xml version='1.0' encoding='" + ConfigurationManager.AppSettings["CodificacaoCabecalho"].ToString() + "'?>");
            Cabecalho.Append("<cabecMsg xmlns='" + ConfigurationManager.AppSettings["NameSpaceNFE"].ToString() + "' versao='" + ConfigurationManager.AppSettings["VersaoCabecalhoNFE"].ToString() + "'>");
            Cabecalho.Append("<versaoDados>" + ConfigurationManager.AppSettings["VersaoDadosNFECancelmanto"].ToString() + "</versaoDados>");
            Cabecalho.Append("</cabecMsg>");
            return Cabecalho.ToString();
        }

        #endregion

        #region VERSÃO 4.1!

        #region HOMOLOGAÇÃO!!

        public CL_NFE.HomologCancelamento2.nfeCabecMsg FncRetornaCabecalhoCancelamento2()
        {
            objCancelamentoWSCab.versaoDados = "2.00";
            objCancelamentoWSCab.cUF = "35";

            return objCancelamentoWSCab;
        }

        public CL_NFE.HomologRecepcao2.nfeCabecMsg FncRetornaCabecalho2()
        {
            objWSCab.versaoDados = "3.10";
            objWSCab.cUF = "35";

            return objWSCab;
        }

        public CL_NFE.HomologRetRecepcao2.nfeCabecMsg FncRetornaCabecalhoRet2()
        {
            objRetWSCab.versaoDados = "3.10";
            objRetWSCab.cUF = "35";

            return objRetWSCab;        
        }

        public CL_NFE.HomologacaoCCe.nfeCabecMsg FncRetornaCabecalhoCCeHomolog()
        {
            objHomologCCeCab.versaoDados = "1.00";
            objHomologCCeCab.cUF = "35";

            return objHomologCCeCab;
        }

        #endregion

        #region PRODUÇÃO!!

        public CL_NFE.ProducaoCancelamento2.nfeCabecMsg FncRetornaCabecalhoCancelamentoProd2()
        {
            objCancelamentoCab.versaoDados = "2.00";
            objCancelamentoCab.cUF = "35";

            return objCancelamentoCab;
        }

        public CL_NFE.ProducaoRecepcao2.nfeCabecMsg FncRetornaCabecalhoProd2()
        {
            objCab.versaoDados = "3.10";
            objCab.cUF = "35";

            return objCab;
        }

        public CL_NFE.ProducaoRetRecepcao2.nfeCabecMsg FncRetornaCabecalhoRetProd2()
        {
            objRetCab.versaoDados = "3.10";
            objRetCab.cUF = "35";

            return objRetCab;
        }

        public CL_NFE.ProducaoEvento.nfeCabecMsg FncRetornaCabecalhoCCeProd()
        {
            objCCeCab.versaoDados = "1.00";
            objCCeCab.cUF = "35";

            return objCCeCab;
        }

        //SCAN
        public CL_NFE.SCANProducaoCancelamento.nfeCabecMsg FncRetornaCabecalhoCancelamentoSCAN()
        {
            objSCANCancelamentoCab.versaoDados = "2.00";
            objSCANCancelamentoCab.cUF = "35";

            return objSCANCancelamentoCab;
        }

        public CL_NFE.SCANProducaoRecepcao.nfeCabecMsg FncRetornaCabecalhoSCAN()
        {
            objSCANCab.versaoDados = "2.00";
            objSCANCab.cUF = "35";

            return objSCANCab;
        }

        public CL_NFE.SCANProducaoRetRecepcao.nfeCabecMsg FncRetornaCabecalhoRetSCAN()
        {
            objSCANRetCab.versaoDados = "2.00";
            objSCANRetCab.cUF = "35";

            return objSCANRetCab;
        }
        
        #endregion

        #endregion
    }
}
