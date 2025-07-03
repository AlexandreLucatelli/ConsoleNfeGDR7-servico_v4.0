using System;
using System.Collections.Generic;
using System.Text;
using NFE.Classes.NFE.Objetos;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using NFE.Classes.AcessoDados;
using System.Security.Cryptography.X509Certificates;


namespace NFE.Classes.NFE
{
    public class MontaXMLNfeCancelamento : DB
    {
        public string Conexao;
        public string XmlAss;
        protected Assinatura.AssinaXML objAss = new Assinatura.AssinaXML();
        protected Objetos.Cancela.Cancelamento objCancelamento = new Objetos.Cancela.Cancelamento();

        public String MontaXMLCancelamento(String Id, String xServ, String chNfe, String nProt, String xJust)
        {
            xJust = xJust.Length < 15 ? xJust.PadLeft(15, '_') : xJust;

            Conexao = FncVerificaConexao();

            StringBuilder XML = new StringBuilder();
            XML.Append("<?xml version='1.0' encoding='" + ConfigurationManager.AppSettings["CodificacaoNFE"].ToString() + "' ?>");            
            XML.Append("<cancNFe xmlns='http://www.portalfiscal.inf.br/nfe' versao='1.07'>");                
                XML.Append("<infCanc Id='"+ Id.PadLeft(46, '0')+ "'>");
                    XML.Append("<tpAmb>" + ConfigurationManager.AppSettings["Ambiente"].ToString() + "</tpAmb>");
                    XML.Append("<xServ>" + xServ.PadLeft(8, '0') + "</xServ>");
                    XML.Append("<chNFe>" + chNfe.PadLeft(44, '0') + "</chNFe>");
                    XML.Append("<nProt>" + nProt.PadLeft(15, '0') + "</nProt>");
                    XML.Append("<xJust>" + xJust + "</xJust>");
                XML.Append("</infCanc>");
            XML.Append("</cancNFe>");

            // Autentica o certificado eletronico.
            X509Certificate2 Cert = new X509Certificate2(objCancelamento.CaminhoCert, objCancelamento.SenhaCert);

            XmlAss = objAss.FncAssinarXML(XML.ToString(), "infCanc", Cert);

            
            
            return XmlAss;
        }


        #region VERSÃO NOVA!

        public String MontaXMLCancelamentoNovo(String Id, String xServ, String chNfe, String nProt, String xJust)
        {
            xJust = xJust.Length < 15 ? xJust.PadLeft(15, '_') : xJust;

            Conexao = FncVerificaConexao();

            StringBuilder XML = new StringBuilder();
            //XML.Append("<?xml version='1.0' encoding='" + ConfigurationManager.AppSettings["CodificacaoNFE"].ToString() + "' ?>");            
            XML.Append("<cancNFe xmlns='http://www.portalfiscal.inf.br/nfe' versao='2.00'>");
            XML.Append("<infCanc Id='" + Id.PadLeft(46, '0') + "'>");
            XML.Append("<tpAmb>" + ConfigurationManager.AppSettings["Ambiente"].ToString() + "</tpAmb>");
            XML.Append("<xServ>" + xServ.PadLeft(8, '0') + "</xServ>");
            XML.Append("<chNFe>" + chNfe.PadLeft(44, '0') + "</chNFe>");
            XML.Append("<nProt>" + nProt.PadLeft(15, '0') + "</nProt>");
            XML.Append("<xJust>" + xJust + "</xJust>");
            XML.Append("</infCanc>");
            XML.Append("</cancNFe>");

            // Autentica o certificado eletronico.
            X509Certificate2 Cert = new X509Certificate2(objCancelamento.CaminhoCert, objCancelamento.SenhaCert);

            XmlAss = objAss.FncAssinarXML(XML.ToString(), "infCanc", Cert);



            return XmlAss;
        }

        public String MontaXMLCancelamentoDistribuicao(String Id, String xServ, String chNfe, String nProt, String xJust, String verAplic, String cStat, String xMotivo, String cUF, String dhRecbto, String IdRet, String nProtRet)
        {
            xJust = xJust.Length < 15 ? xJust.PadLeft(15, '_') : xJust;

            Conexao = FncVerificaConexao();

            StringBuilder XML = new StringBuilder();
            XML.Append("<?xml version='1.0' encoding='" + ConfigurationManager.AppSettings["CodificacaoNFE"].ToString() + "' ?>");
            XML.Append("<procCancNFe versao='2.00'>");
            XML.Append("<cancNFe xmlns='http://www.portalfiscal.inf.br/nfe' versao='2.00'>");
            XML.Append("<infCanc Id='" + Id.PadLeft(46, '0') + "'>");
            XML.Append("<tpAmb>" + ConfigurationManager.AppSettings["Ambiente"].ToString() + "</tpAmb>");
            XML.Append("<xServ>" + xServ.PadLeft(8, '0') + "</xServ>");
            XML.Append("<chNFe>" + chNfe.PadLeft(44, '0') + "</chNFe>");
            XML.Append("<nProt>" + nProt.PadLeft(15, '0') + "</nProt>");
            XML.Append("<xJust>" + xJust + "</xJust>");
            XML.Append("</infCanc>");
            XML.Append("</cancNFe>");

            XML.Append("<retCancNFe xmlns='http://www.portalfiscal.inf.br/nfe' versao='2.00'>");

            if (IdRet.ToUpper() != "NOT TAG")
                XML.Append("<infCanc Id='" + IdRet.PadLeft(46, '0') + "'>");
            else
                XML.Append("<infCanc>");
            XML.Append("<tpAmb>" + ConfigurationManager.AppSettings["Ambiente"].ToString() + "</tpAmb>");
            XML.Append("<verAplic>" + verAplic + "</verAplic>");
            XML.Append("<cStat>" + cStat.PadLeft(3, '0') + "</cStat>");
            XML.Append("<xMotivo>" + xMotivo + "</xMotivo>");
            XML.Append("<cUF>" + chNfe.PadLeft(2, '0') + "</cUF>");
            XML.Append("<chNFe>" + chNfe.PadLeft(44, '0') + "</chNFe>");

            if (dhRecbto.ToUpper() != "NOT TAG")
                XML.Append("<dhRecbto>" + dhRecbto + "</dhRecbto>");

            if (nProtRet.ToUpper() != "NOT TAG")
                XML.Append("<nProt>" + nProtRet.PadLeft(15, '0') + "</nProt>");

            XML.Append("</infCanc>");
            XML.Append("</retCancNFe>");
            XML.Append("</procCancNFe>");


            return XML.ToString();
        }

        #endregion

    }
}
