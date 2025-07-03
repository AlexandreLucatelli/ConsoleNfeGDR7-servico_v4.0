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
    public class MontaXMLNfeConsulta : DB
    {

        protected SqlDataReader Dr;
        public string Conexao;

        public void MontaXMLConsulta(NFE.Objetos.NotaFiscalEletronica Nfe)
        {
            Conexao = FncVerificaConexao();

            StringBuilder XML = new StringBuilder();

            XML.Append("<?xml version='1.0' encoding='" + ConfigurationManager.AppSettings["CodificacaoNFE"].ToString() + "' ?>");

            XML.Append("<conSitNFe>");
                XML.Append("<versao>" + Nfe.ConsultaNfe.versao.PadLeft(2, '0') + "</versao>");
                XML.Append("<tpAmb>" + Nfe.ConsultaNfe.tpAmb.ToString() + "</tpAmb>");
                XML.Append("<xServ>" + Nfe.ConsultaNfe.xServ.PadLeft(9, '0') + "</xServ>");
                XML.Append("<chNFe>" + Nfe.ConsultaNfe.chNFe.PadLeft(44, '0') + "</chNFe>");
            XML.Append("</conSitNFe>");

        }

    }
}
