using System;
using System.Collections.Generic;
using System.Text;
using NFE.Classes.NFE.Objetos;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using NFE.Classes.AcessoDados;

namespace NFE.Classes.NFE
{
    public class MontaXMLNfeRetRecepcao : DB
    {
        public string Conexao;


        #region VERSÃO VELHA!

        public String MontaXMLRetRecepcao(String nRec)
        {
            Conexao = FncVerificaConexao();

            StringBuilder XML = new StringBuilder();

            XML.Append("<?xml version='1.0' encoding='" + ConfigurationManager.AppSettings["CodificacaoNFE"].ToString() + "' ?>");

            XML.Append("<consReciNFe xmlns='http://www.portalfiscal.inf.br/nfe' versao='1.10'>");
            XML.Append("<tpAmb>" + ConfigurationManager.AppSettings["Ambiente"].ToString() + "</tpAmb>");
            XML.Append("<nRec>" + nRec.PadLeft(15, '0') + "</nRec>");
            XML.Append("</consReciNFe>");

            return XML.ToString();

        }

        #endregion


        #region VERSÃO NOVA!

        public String MontaXMLRetRecepcaoNovo(String nRec)
        {
            Conexao = FncVerificaConexao();

            StringBuilder XML = new StringBuilder();

            //XML.Append("<?xml version='1.0' encoding='" + ConfigurationManager.AppSettings["CodificacaoNFE"].ToString() + "' ?>");

            XML.Append("<consReciNFe xmlns='http://www.portalfiscal.inf.br/nfe' versao='4.00'>");
            XML.Append("<tpAmb>" + ConfigurationManager.AppSettings["Ambiente"].ToString() + "</tpAmb>");
            XML.Append("<nRec>" + nRec.PadLeft(15, '0') + "</nRec>");
            XML.Append("</consReciNFe>");

            return XML.ToString();

        }

        #endregion

    }
}
