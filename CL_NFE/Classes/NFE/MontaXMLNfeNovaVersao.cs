using Microsoft.ApplicationBlocks.Data;
using NFE.Classes.AcessoDados;
using NFE.Classes.NFE.Assinatura;
using NFE.Classes.NFE.Objetos;
using NFE.Classes.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Gdr7.Util;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

namespace NFE.Classes.NFE
{
    public class MontaXMLNfeNovaVersao : DB
    {
        protected SqlDataReader Dr;
        protected SqlDataReader DrNotas;
        protected SqlDataReader DrCont;
        public string Conexao;
        public String Observacao;
        protected NotaFiscalEletronica obj = new NotaFiscalEletronica();
        protected Cabecalho oCabecalho = new Cabecalho();
        protected Utils oUtil = new Utils();
        protected StringBuilder IdNota = new StringBuilder();
        protected XmlNode nodeRetorno;
        public String MotivoNfe;
        public String nProt;
        public String dtRetorno;
        public String StatusNfe;
        public string pastaErro = ConfigurationManager.AppSettings["PastaErro"].ToString();

        //Homologação Sefaz São Paulo 4.00
        protected CL_NFE.HomoNFeAutorizacao4.NFeAutorizacao4 objRecepcao4 = new CL_NFE.HomoNFeAutorizacao4.NFeAutorizacao4();

        // SVC-AN Homologação 4.00
        protected CL_NFE.HomoSVCNFeAutorizacao4.NFeAutorizacao4 objSVCNfeRecepcao4 = new CL_NFE.HomoSVCNFeAutorizacao4.NFeAutorizacao4();

        //Produção Sefaz São Paulo 4.00
        protected CL_NFE.ProdNFeAutorizacao4.NFeAutorizacao4 objRecepcaoProducao4 = new CL_NFE.ProdNFeAutorizacao4.NFeAutorizacao4();

        //Produção SVC-AN 4.00
        protected CL_NFE.ProdSVCNFeAutorizacao4.NFeAutorizacao4 objSVCNfeRecepcaoProducao4 = new CL_NFE.ProdSVCNFeAutorizacao4.NFeAutorizacao4();

        public MontaXMLNfeNovaVersao(String Cnn)
        {
            Conexao = Cnn;
        }
    
        public MontaXMLNfeNovaVersao()
        {
            Conexao = FncVerificaConexao();
        }

        public String GeraXML(NotaFiscalEletronica oNfe)
        {
            StringBuilder infoLote = new StringBuilder();
            X509Certificate2 CertAssina = new X509Certificate2();
            StringBuilder XML = new StringBuilder();
            string[] parametros;
            SqlConnection cnnGeraXML = new SqlConnection(Conexao);

            try
            {
                CertAssina = new X509Certificate2(ConfigurationManager.AppSettings["CaminhoCertificado"].ToString(), ConfigurationManager.AppSettings["SenhaCertificado"].ToString());
            }
            catch (Exception ex)
            {
                StreamWriter ArquivoErro = new StreamWriter(pastaErro + oNfe.cNF + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + "_X509Certificate2.txt", false, Encoding.ASCII);
                ArquivoErro.WriteLine("***************** CertAssina X509Certificate2 *****************");
                ArquivoErro.WriteLine("Erro gerado " + DateTime.Now.ToString());
                ArquivoErro.WriteLine("IDNota: " + oNfe.cNF);
                ArquivoErro.WriteLine("CaminhoCertificado: " + ConfigurationManager.AppSettings["CaminhoCertificado"].ToString());
                ArquivoErro.WriteLine("SenhaCertificado: " + ConfigurationManager.AppSettings["SenhaCertificado"].ToString());

                ArquivoErro.WriteLine(ex.Message);
                ArquivoErro.WriteLine(ex.ToString());
                ArquivoErro.Flush();
                ArquivoErro.Close();
            }

                #region ..:: Monta XML NFE ::..
                XML.Append("<NFe xmlns='" + ConfigurationManager.AppSettings["NameSpaceNFE"].ToString() + "'>");
                XML.Append("<infNFe versao='4.00' Id='" + oNfe.infNfe + "'>");

                #region ..:: ide ::..
                XML.Append("<ide>");
                XML.Append("<cUF>" + oNfe.Ide.cUF.PadLeft(2, '0') + "</cUF>");

                // DIMINUIÇÃO DE CARACTERES
                XML.Append("<cNF>" + oNfe.Ide.cNF.PadLeft(8, '0') + "</cNF>");
                XML.Append("<natOp>" + (oNfe.Ide.natOp.Length > 60 ? oNfe.Ide.natOp.Substring(0, 60) : oNfe.Ide.natOp) + "</natOp>");

                // Alterado por: Bruno Arrais Cavalcante data: 10/08/2018
                // O campo foi reposicionado na versão 4.0. Agora os dados de pagamento são obrigatórios para NFe e NFCe e se encontram no grupo pag
                //XML.Append("<indPag>0</indPag>");

                XML.Append("<mod>" + oNfe.Ide.Mod + "</mod>");
                XML.Append("<serie>" + oNfe.Ide.Serie + "</serie>");
                XML.Append("<nNF>" + oNfe.Ide.nNF + "</nNF>");
                XML.Append("<dhEmi>" + oNfe.Ide.dEmi + "</dhEmi>");
                XML.Append("<dhSaiEnt>" + oNfe.Ide.dEmi + "</dhSaiEnt>");
                XML.Append("<tpNF>" + oNfe.Ide.tpNF + "</tpNF>");
                XML.Append("<idDest>" + oNfe.Ide.idDest + "</idDest>");
                XML.Append("<cMunFG>" + oNfe.Ide.cMunFG.ToString().PadLeft(7, '0') + "</cMunFG>");
                XML.Append("<tpImp>" + oNfe.Ide.tpImp.ToString() + "</tpImp>");
                XML.Append("<tpEmis>" + oNfe.Ide.tpEmis + "</tpEmis>");
                XML.Append("<cDV>" + oNfe.Ide.cDV + "</cDV>");
                XML.Append("<tpAmb>" + oNfe.Ide.tpAmb + "</tpAmb>");
                XML.Append("<finNFe>" + oNfe.RefNf.finNFe + "</finNFe>");
                XML.Append("<indFinal>" + oNfe.Ide.indFinal + "</indFinal>");
                XML.Append("<indPres>" + oNfe.Ide.indPres + "</indPres>");
                XML.Append("<procEmi>" + oNfe.Ide.procEmi + "</procEmi>");
                XML.Append("<verProc>" + oNfe.Ide.verProc + "</verProc>");

                if (oNfe.Ide.tpEmis == "3" || oNfe.Ide.tpEmis == "5")
                {
                    XML.Append("<dhCont>" + oNfe.Ide.dhCont + "</dhCont>");
                    XML.Append("<xJust>" + oNfe.Ide.xJust + "</xJust>");
                }

                //Jeferson - 17/05/2016
                //refECF obrigatório para CFOP de venda a partir de Abril de 2016
                if (oNfe.RefNf.finNFe == "1") // Finalidade Normal
                {
                    SqlDataReader drItens = SqlHelper.ExecuteReader(cnnGeraXML, "stpRetornaDadosItens", oNfe.cNF);

                    while (drItens.Read())
                    {
                        if (!string.IsNullOrWhiteSpace(drItens["modCupom"].ToString()) && !string.IsNullOrWhiteSpace(drItens["nECF"].ToString())
                                && !string.IsNullOrWhiteSpace(drItens["nCOO"].ToString()))
                        {
                            XML.Append("<NFref>");
                            XML.Append("<refECF>");
                            XML.Append("<mod>" + drItens["modCupom"] + "</mod>");
                            XML.Append("<nECF>" + drItens["nECF"] + "</nECF>");
                            XML.Append("<nCOO>" + drItens["nCOO"] + "</nCOO>");
                            XML.Append("</refECF>");
                            XML.Append("</NFref>");
                        }
                    }
                    drItens.Close();
            }
                else if (oNfe.RefNf.finNFe == "2") // Complementar
                {
                    XML.Append("<NFref>");
                    XML.Append("<refNFe>" + oNfe.RefNf.chNfeReferenciada + "</refNFe>"); // Nfe Referenciada
                    XML.Append("</NFref>");
                }
                else if (oNfe.RefNf.finNFe == "4") // Devolução de Mercadoria
                {
                    SqlDataReader drItens = SqlHelper.ExecuteReader(cnnGeraXML, "stpRetornaDadosItens", oNfe.cNF);

                    if (!string.IsNullOrEmpty(oNfe.RefNf.NfeReferenciada))
                    {
                        XML.Append("<NFref>");
                        XML.Append("<refNFe>" + oNfe.RefNf.NfeReferenciada + "</refNFe>"); // Nfe Referenciada
                        XML.Append("</NFref>");
                    }
                    else
                    {
                        while (drItens.Read())
                        {
                            XML.Append("<NFref>");
                            XML.Append("<refECF>");
                            XML.Append("<mod>" + drItens["modCupom"] + "</mod>");
                            XML.Append("<nECF>" + drItens["nECF"] + "</nECF>");
                            XML.Append("<nCOO>" + drItens["nCOO"] + "</nCOO>");
                            XML.Append("</refECF>");
                            XML.Append("</NFref>");
                        }
                    }
                    drItens.Close();
             }

                XML.Append("</ide>");
                #endregion

                #region ..:: emit ::..
                XML.Append("<emit>");
                XML.Append("<CNPJ>" + oNfe.Emit.CNPJ.PadLeft(14, '0').Trim() + "</CNPJ>");
                XML.Append("<xNome>" + oNfe.Util.FncRetiraCaracteresCampoTexto(oNfe.Emit.xNome) + "</xNome>");
                //XML.Append("<xFant>" + oNfe.Util.FncRetiraCaracteresCampoTexto(oNfe.Emit.xFant) + "</xFant>");
                XML.Append("<enderEmit>");
                XML.Append("<xLgr>" + oNfe.Util.FncRetiraCaracteresCampoTexto(oNfe.Emit.EnderEmit.xLgr).TrimStart().TrimEnd() + "</xLgr>");
                XML.Append("<nro>" + oNfe.Emit.EnderEmit.nro.TrimStart().TrimStart().TrimEnd() + "</nro>");
                if (!(oNfe.Emit.EnderEmit.xCpl.Trim() == string.Empty))
                {
                    XML.Append("<xCpl>" + oNfe.Util.FncRetiraCaracteresCampoTexto(oNfe.Emit.EnderEmit.xCpl).TrimEnd() + "</xCpl>");
                }
                XML.Append("<xBairro>" + (oNfe.Emit.EnderEmit.xBairro == string.Empty ? "Nao Encontrado" : oNfe.Util.FncRetiraCaracteresCampoTexto(oNfe.Emit.EnderEmit.xBairro).TrimEnd()) + "</xBairro>");
                XML.Append("<cMun>" + oNfe.Emit.EnderEmit.cMun + "</cMun>");
                XML.Append("<xMun>" + oNfe.Util.FUNC_CARACTER_ESPECIAL(oNfe.Emit.EnderEmit.xMun.TrimEnd()) + "</xMun>");
                XML.Append("<UF>" + oNfe.Emit.EnderEmit.UF.Trim() + "</UF>");
                if (!(oNfe.Emit.EnderEmit.CEP == string.Empty))
                {
                    XML.Append("<CEP>" + oNfe.Emit.EnderEmit.CEP.Trim().Replace("-", "") + "</CEP>");
                }
                if (!(oNfe.Emit.EnderEmit.cPais == "0") || (!(oNfe.Emit.EnderEmit.cPais == string.Empty)))
                {
                    XML.Append("<cPais>" + oNfe.Emit.EnderEmit.cPais + "</cPais>");
                    XML.Append("<xPais>" + oNfe.Util.FUNC_CARACTER_ESPECIAL(oNfe.Emit.EnderEmit.xPais).TrimEnd() + "</xPais>");
                }
                if (!(oNfe.Emit.EnderEmit.fone == string.Empty))
                {
                    XML.Append("<fone>" + (oNfe.Emit.EnderEmit.fone.Trim().Length > 10 ? oNfe.Util.FUNC_CARACTER_ESPECIAL(oNfe.Emit.EnderEmit.fone).Replace(" ", "").Trim().Remove(0, oNfe.Emit.EnderEmit.fone.Trim().Length - 10) : oNfe.Util.FUNC_CARACTER_ESPECIAL(oNfe.Emit.EnderEmit.fone).Replace(" ", "").PadLeft(10, '0').Trim()) + "</fone>");
                }
                XML.Append("</enderEmit>");
                XML.Append("<IE>" + oNfe.Util.FUNC_CARACTER_ESPECIAL(oNfe.Emit.EnderEmit.IE).Trim() + "</IE>");

                //NEW VERSÃO 4.01!!
                //CÓDIGO DE REGIME TRIBUTÁRIO - REGIME NORMAL(3) -FIXO!
                XML.Append("<CRT>" + oNfe.Emit.CRT + "</CRT>");

                XML.Append("</emit>");
                #endregion

                #region ..:: dest ::..

                if (oNfe.Dest.CNPJ == string.Empty)
                {
                    parametros = new string[] { "null", oNfe.Ide.cNF.ToString(), oNfe.infNfe.Substring(3, oNfe.infNfe.Length - 3), "4", "O Endereço do destinatário é inválido" };
                    SqlHelper.ExecuteNonQuery(cnnGeraXML, "stpAtualizaRecNfe", parametros);
            }

                XML.Append("<dest>");

                if (oNfe.Dest.EnderDest.UFCli == "EX")
                {
                    XML.Append("<CNPJ/>");
                }
                else
                {
                    if (oNfe.Dest.CNPJ.Length == 14)
                    {
                        XML.Append("<CNPJ>" + oNfe.Dest.CNPJ.Trim().PadLeft(14, '0') + "</CNPJ>");
                    }
                    else
                    {
                        XML.Append("<CPF>" + oNfe.Dest.CNPJ.Trim().PadLeft(11, '0') + "</CPF>");
                    }
                }

                if (ConfigurationManager.AppSettings["Ambiente"].ToString() == "2")
                {
                    XML.Append("<xNome>NF-E EMITIDA EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL</xNome>");
                }
                else
                {
                    XML.Append("<xNome>" + (oNfe.Util.FUNC_CARACTER_ESPECIAL(oNfe.Dest.xNome).TrimStart().TrimEnd().Length > 60 ? oNfe.Util.FUNC_CARACTER_ESPECIAL(oNfe.Dest.xNome).Substring(0, 60).TrimStart().TrimEnd() : oNfe.Util.FUNC_CARACTER_ESPECIAL(oNfe.Dest.xNome).TrimStart().TrimEnd()) + "</xNome>");
                }

                XML.Append("<enderDest>");
                XML.Append("<xLgr>" + oNfe.Util.FncRetiraCaracteresCampoTexto(oNfe.Dest.EnderDest.xLgr).TrimStart().TrimEnd() + "</xLgr>");
                XML.Append("<nro>" + oNfe.Util.FUNC_CARACTER_ESPECIAL(oNfe.Dest.EnderDest.nro).TrimStart().TrimEnd() + "</nro>");

                if (!(oNfe.Dest.EnderDest.xCpl.Trim() == string.Empty))
                {
                    XML.Append("<xCpl>" + oNfe.Util.FncRetiraCaracteresCampoTexto(oNfe.Dest.EnderDest.xCpl.TrimStart().TrimEnd()).TrimStart() + "</xCpl>");
                }

                XML.Append("<xBairro>" + (oNfe.Dest.EnderDest.xBairro == string.Empty ? "Nao Encontrado" : oNfe.Util.FncRetiraCaracteresCampoTexto(oNfe.Dest.EnderDest.xBairro).TrimStart().TrimEnd()) + "</xBairro>");

                if (oNfe.Dest.EnderDest.UFCli == "EX")
                {
                    XML.Append("<cMun>9999999</cMun>");
                    XML.Append("<xMun>Exterior</xMun>");
                }
                else
                {
                    XML.Append("<cMun>" + oNfe.Dest.EnderDest.cMun.TrimStart().TrimEnd() + "</cMun>");
                    XML.Append("<xMun>" + oNfe.Util.FUNC_CARACTER_ESPECIAL(oNfe.Dest.EnderDest.xMun).TrimStart().TrimEnd() + "</xMun>");
                }

                if (oNfe.Dest.EnderDest.UFCli == "EX")
                {
                    XML.Append("<UF>" + oNfe.Dest.EnderDest.UFCli.Trim() + "</UF>");
                }
                else
                {
                    XML.Append("<UF>" + oNfe.Dest.EnderDest.UF.Trim() + "</UF>");
                }

                if (!(oNfe.Dest.EnderDest.CEP == string.Empty))
                {
                    XML.Append("<CEP>" + oNfe.Util.FUNC_CARACTER_ESPECIAL(oNfe.Dest.EnderDest.CEP).Trim().PadLeft(8, '0') + "</CEP>");
                }

                if (oNfe.Dest.EnderDest.UFCli == "EX")
                {
                    XML.Append("<cPais>" + Convert.ToInt32(oNfe.Dest.EnderDest.cPais).ToString() + "</cPais>");
                    XML.Append("<xPais>" + oNfe.Util.FUNC_CARACTER_ESPECIAL(oNfe.Dest.EnderDest.xPais) + "</xPais>");
                }
                else
                {
                    XML.Append("<cPais>1058</cPais>");
                    XML.Append("<xPais>Brasil</xPais>");
                }

                if (!(oNfe.Dest.EnderDest.fone == string.Empty))
                {
                    XML.Append("<fone>" + (oNfe.Dest.EnderDest.fone.Trim().Length > 10 ? oNfe.Util.FUNC_CARACTER_ESPECIAL(oNfe.Dest.EnderDest.fone).Replace(" ", "").Trim() : oNfe.Util.FUNC_CARACTER_ESPECIAL(oNfe.Dest.EnderDest.fone).Replace(" ", "").PadLeft(10, '0').Trim()).Substring(0, 10) + "</fone>");
                }
                XML.Append("</enderDest>");
                XML.Append("<indIEDest>" + oNfe.Dest.indIEDest + "</indIEDest>");

                if (oNfe.Dest.indIEDest != "2" && oNfe.Dest.indIEDest != "9")
                {
                    if (oNfe.Dest.CNPJ.Length == 11)
                    {
                        XML.Append("<IE/>");
                    }
                    else
                    {
                        if (oNfe.Dest.EnderDest.IE.Trim() == string.Empty)
                        {
                            XML.Append("<IE/>");
                        }
                        else if (oNfe.Dest.EnderDest.IE.ToUpper().Trim() == "ISENTO")
                        {
                            XML.Append("<IE/>");
                        }
                        else
                        {
                            XML.Append("<IE>" + oNfe.Util.FUNC_CARACTER_ESPECIAL(oNfe.Dest.EnderDest.IE).Trim() + "</IE>");
                        }
                    }
                }

                //NEW VERSÃO 4.01 NFe!
                if (oNfe.Dest.email.Trim() != "")
                    XML.Append("<email>" + oNfe.Dest.email.Trim() + "</email>");

                XML.Append("</dest>");

                #endregion

                //#region ..:: Entregas ::..
                //if (!(oNfe.Entregas.CNPJ == string.Empty))
                //{
                //    if (oNfe.IcmsTot.vNF > 0)
                //    {
                //        XML.Append("<entrega>");
                //        XML.Append("<CNPJ>" + oNfe.Entregas.CNPJ + "</CNPJ>");
                //        XML.Append("<xLgr>" + oNfe.Util.FncRetiraCaracteresCampoTexto(oNfe.Entregas.xLgr).TrimStart().TrimEnd() + "</xLgr>");
                //        XML.Append("<nro>" + oNfe.Util.FUNC_CARACTER_ESPECIAL(oNfe.Entregas.nro).TrimStart().TrimEnd() + "</nro>");
                //        if (!(oNfe.Entregas.xLgr == string.Empty))
                //        {
                //            XML.Append("<xCpl>" + oNfe.Util.FUNC_CARACTER_ESPECIAL(oNfe.Entregas.xLgr) + "</xCpl>");
                //        }
                //        XML.Append("<xBairro>" + (oNfe.Entregas.xBairro == string.Empty ? "Nao Encontrado" : oNfe.Util.FncRetiraCaracteresCampoTexto(oNfe.Entregas.xBairro)) + "</xBairro>");

                //        if (!(oNfe.Entregas.cMun == string.Empty))
                //        {
                //            XML.Append("<cMun>" + oNfe.Entregas.cMun + "</cMun>");
                //            XML.Append("<xMun>" + oNfe.Util.FUNC_CARACTER_ESPECIAL(oNfe.Entregas.xMun).TrimEnd() + "</xMun>");
                //        }

                //        XML.Append("<UF>" + oNfe.Entregas.UF.Trim() + "</UF>");
                //        XML.Append("</entrega>");
                //    }
                //}
                //#endregion

                #region ..:: detItem ::..

                Dr = SqlHelper.ExecuteReader(cnnGeraXML, "stpRetornaTotalItensNf", oNfe.cNF);
                int Contator = 1;

                decimal vICMS = 0,
                       VLProduto = 0,
                       VLFrete = 0,
                       VLSeguro = 0,
                       VLDesconto = 0,
                       VBC = 0,
                       OutrasDespesas = 0,
                       VLIPI = 0,
                       VLPIS = 0,
                       VLCOFINS = 0,
                       VLII = 0,
                       vBCST = 0,
                       vICMSST = 0;

                while (Dr.Read())
                {
                    oNfe.Det.Ipost.Icms.ICMs00.orig = Dr["SitTributA"] is DBNull ? "0" : Dr["SitTributA"].ToString();
                    oNfe.Det.Ipost.Icms.ICMs10.orig = Dr["SitTributA"] is DBNull ? "0" : Dr["SitTributA"].ToString();
                    oNfe.Det.Ipost.Icms.ICMs20.orig = Dr["SitTributA"] is DBNull ? 0 : decimal.Parse(Dr["SitTributA"].ToString());
                    oNfe.Det.Ipost.Icms.ICMs40.orig = Dr["SitTributA"] is DBNull ? 0 : decimal.Parse(Dr["SitTributA"].ToString());
                    oNfe.Det.Ipost.Icms.ICMs51.orig = Dr["SitTributA"] is DBNull ? 0 : decimal.Parse(Dr["SitTributA"].ToString());
                    oNfe.Det.Ipost.Icms.ICMs60.orig = Dr["SitTributA"] is DBNull ? "0" : Dr["SitTributA"].ToString();
                    oNfe.Det.Ipost.Icms.ICMs70.orig = Dr["SitTributA"] is DBNull ? 0 : decimal.Parse(Dr["SitTributA"].ToString());
                    oNfe.Det.Ipost.Icms.ICMs90.orig = Dr["SitTributA"] is DBNull ? "0" : Dr["SitTributA"].ToString();

                oNfe.Det.Produto.cProd = Dr["IDProduto"] is DBNull ? "0" : Dr["IDProduto"].ToString();
                    oNfe.Det.Produto.xProd = Dr["Descricao"] is DBNull ? string.Empty : Dr["Descricao"].ToString();

                    //INCLUÍDO 29/11/2011!!                        
                    string Xped = Dr["NumeroPedidoCliente"] is DBNull ? string.Empty : oNfe.Util.FUNC_CARACTER_ESPECIAL(System.Text.RegularExpressions.Regex.Replace(Dr["NumeroPedidoCliente"].ToString(), "<[^<>]+>", String.Empty).Replace("\n", String.Empty).Replace("\r", String.Empty));

                    oNfe.Det.Produto.xPed = Xped.Length > 15 ? Xped.TrimEnd().TrimStart().Substring(0, 15).TrimEnd().TrimStart() : Xped.TrimEnd().TrimStart();

                    oNfe.Det.Produto.NCM = Dr["IDNCM"] is DBNull ? string.Empty : Dr["IDNCM"].ToString().PadLeft(8, '0');
                    oNfe.Det.Produto.CEST = Dr["CEST"] is DBNull ? "0" : Dr["CEST"].ToString();
                    oNfe.Det.Produto.CFOP = Dr["CFop"] is DBNull ? string.Empty : Dr["CFop"].ToString().PadLeft(4, '0');
                    oNfe.Det.Produto.uCom = Dr["Unidade"] is DBNull ? string.Empty : Dr["Unidade"].ToString();
                    oNfe.Det.Produto.qCom = Dr["Qtde"] is DBNull ? 0 : decimal.Parse(Dr["Qtde"].ToString());
                    oNfe.Det.Produto.vUnCom = Dr["Vlunitario"] is DBNull ? 0 : decimal.Parse(Dr["Vlunitario"].ToString());
                    oNfe.Det.Produto.vUnTrib = Dr["Vlunitario"] is DBNull ? 0 : decimal.Parse(Dr["Vlunitario"].ToString());
                    oNfe.Det.Produto.vProd = Dr["ValorTotal"] is DBNull ? 0 : decimal.Parse(Dr["ValorTotal"].ToString());
                    oNfe.Det.Produto.vFrete = Dr["ValorFrete"] is DBNull ? 0 : decimal.Parse(Dr["ValorFrete"].ToString());
                    oNfe.Det.Produto.vSeg = Dr["ValorSeguro"] is DBNull ? 0 : decimal.Parse(Dr["ValorSeguro"].ToString());
                    oNfe.Det.Produto.uTrib = Dr["unidade"] is DBNull ? "UN" : Dr["unidade"].ToString();
                    oNfe.Det.Produto.qTrib = Dr["Qtde"] is DBNull ? 0 : decimal.Parse(Dr["Qtde"].ToString());
                    oNfe.Det.Produto.vDesc = Dr["Desconto"] is DBNull ? 0 : decimal.Parse(Dr["Desconto"].ToString());
                    oNfe.Det.Produto.SitTributB = Dr["SitTributB"] is DBNull ? "00" : Dr["SitTributB"].ToString();
                    oNfe.Det.Produto.vOutras = Dr["OutrasDespesas"] is DBNull ? 0 : decimal.Parse(Dr["OutrasDespesas"].ToString());

                // II - TAG de grupo do Imposto de Importação
                //oNfe.Det.Ipost.II.vBC = Dr["BaseII"] is DBNull ? 0 : decimal.Parse(Dr["BaseII"].ToString());
                //oNfe.Det.Ipost.II.vDespAdu = Dr["vDespAdu"] is DBNull ? 0 : decimal.Parse(Dr["vDespAdu"].ToString());
                //oNfe.Det.Ipost.II.vII = Dr["vII"] is DBNull ? 0 : decimal.Parse(Dr["vII"].ToString());
                //oNfe.Det.Ipost.II.vIOF = Dr["vIOF"] is DBNull ? 0 : decimal.Parse(Dr["vIOF"].ToString());

                // PIS 
                    oNfe.Det.Ipost.Pis.CST = Dr["CodPIS"] is DBNull ? "" : Dr["CodPIS"].ToString();
                    oNfe.Det.Ipost.Pis.vBC = Dr["BasePIS"] is DBNull ? 0 : decimal.Parse(Dr["BasePIS"].ToString());
                    oNfe.Det.Ipost.Pis.vPis = Dr["vPIS"] is DBNull ? 0 : decimal.Parse(Dr["vPIS"].ToString());
                    oNfe.Det.Ipost.Pis.pPIS = Dr["AliqPIS"] is DBNull ? 0 : decimal.Parse(Dr["AliqPIS"].ToString());
                    oNfe.Det.Ipost.Pis.qBCProd = Dr["Qtde"] is DBNull ? 0 : decimal.Parse(Dr["Qtde"].ToString());
                    oNfe.Det.Ipost.Pis.vAliqProd = 0;

                    // Cofins
                    oNfe.Det.Ipost.Cofins.CST = Dr["CodCOFINS"] is DBNull ? "" : Dr["CodCOFINS"].ToString();
                    oNfe.Det.Ipost.Cofins.vBC = Dr["BaseCOFINS"] is DBNull ? 0 : decimal.Parse(Dr["BaseCOFINS"].ToString());
                    oNfe.Det.Ipost.Cofins.vCOFINS = Dr["vCOFINS"] is DBNull ? 0 : decimal.Parse(Dr["vCOFINS"].ToString());
                    oNfe.Det.Ipost.Cofins.pCOFINS = Dr["AliqCOFINS"] is DBNull ? 0 : decimal.Parse(Dr["AliqCOFINS"].ToString());
                    oNfe.Det.Ipost.Cofins.qBCProd = Dr["Qtde"] is DBNull ? 0 : decimal.Parse(Dr["Qtde"].ToString());
                    oNfe.Det.Ipost.Cofins.vAliqProd = 0;

                    //IPI
                    oNfe.Det.Ipost.Ipi.IPITrib.vBC = Dr["BaseIPI"] is DBNull ? 0 : decimal.Parse(Dr["BaseIPI"].ToString());
                    oNfe.Det.Ipost.Ipi.IPITrib.pIPI = Dr["AliqIPI"] is DBNull ? 0 : decimal.Parse(Dr["AliqIPI"].ToString());
                    oNfe.Det.Ipost.Ipi.IPITrib.vIPI = Dr["TotalIPI"] is DBNull ? 0 : decimal.Parse(Dr["TotalIPI"].ToString());
                    oNfe.Det.Ipost.Ipi.IPITrib.CST = Dr["SitTribIPI"] is DBNull ? "50" : Dr["SitTribIPI"].ToString();

                    if (oNfe.Det.Produto.SitTributB == "00")
                    {
                        oNfe.Det.Ipost.Icms.ICMs00.vBC = Dr["BaseICMS"] is DBNull ? 0 : decimal.Parse(Dr["BaseICMS"].ToString());
                        oNfe.Det.Ipost.Icms.ICMs00.pICMS = Dr["AliqICMS"] is DBNull ? 0 : decimal.Parse(Dr["AliqICMS"].ToString());
                        oNfe.Det.Ipost.Icms.ICMs00.vICMS = Dr["ValorICMS"] is DBNull ? 0 : decimal.Parse(Dr["ValorICMS"].ToString());

                        vICMS += (oNfe.Det.Ipost.Icms.ICMs00.vICMS);
                        VBC += (oNfe.Det.Ipost.Icms.ICMs00.vBC);

                    }
                    else if (oNfe.Det.Produto.SitTributB == "10")
                    {
                        oNfe.Det.Ipost.Icms.ICMs10.CST = "10";
                        oNfe.Det.Ipost.Icms.ICMs10.modBC = Dr["modBC"] is DBNull ? "" : Dr["modBC"].ToString();
                        oNfe.Det.Ipost.Icms.ICMs10.vBC = Dr["BaseICMS"] is DBNull ? 0 : decimal.Parse(Dr["BaseICMS"].ToString());
                        oNfe.Det.Ipost.Icms.ICMs10.pICMS = Dr["AliqICMS"] is DBNull ? 0 : decimal.Parse(Dr["AliqICMS"].ToString());
                        oNfe.Det.Ipost.Icms.ICMs10.vICMS = Dr["ValorICMS"] is DBNull ? 0 : decimal.Parse(Dr["ValorICMS"].ToString());
                        oNfe.Det.Ipost.Icms.ICMs10.modBCST = Dr["modBCST"] is DBNull ? "" : Dr["modBCST"].ToString();
                        oNfe.Det.Ipost.Icms.ICMs10.pMVAST = Dr["pMVAST"] is DBNull ? 0 : decimal.Parse(Dr["pMVAST"].ToString());
                        oNfe.Det.Ipost.Icms.ICMs10.pRedBCST = Dr["pRedBCST"] is DBNull ? 0 : decimal.Parse(Dr["pRedBCST"].ToString());
                        oNfe.Det.Ipost.Icms.ICMs10.vBCST = Dr["vBCST"] is DBNull ? 0 : decimal.Parse(Dr["vBCST"].ToString());
                        oNfe.Det.Ipost.Icms.ICMs10.pICMSST = Dr["pICMSST"] is DBNull ? 0 : decimal.Parse(Dr["pICMSST"].ToString());
                        oNfe.Det.Ipost.Icms.ICMs10.vICMSST = Dr["vICMSST"] is DBNull ? 0 : decimal.Parse(Dr["vICMSST"].ToString());

                        vICMS += (oNfe.Det.Ipost.Icms.ICMs10.vICMS);
                        VBC += (oNfe.Det.Ipost.Icms.ICMs10.vBC);
                        vBCST += (oNfe.Det.Ipost.Icms.ICMs10.vBCST);
                        vICMSST += oNfe.Det.Ipost.Icms.ICMs10.vICMSST;

                    }
                    else if (oNfe.Det.Produto.SitTributB == "20")
                    {
                        oNfe.Det.Ipost.Icms.ICMs20.vBC = Dr["BaseICMS"] is DBNull ? 0 : decimal.Parse(Dr["BaseICMS"].ToString());
                        oNfe.Det.Ipost.Icms.ICMs20.pICMS = Dr["AliqICMS"] is DBNull ? 0 : decimal.Parse(Dr["AliqICMS"].ToString());
                        oNfe.Det.Ipost.Icms.ICMs20.vICMS = Dr["ValorICMS"] is DBNull ? 0 : decimal.Parse(Dr["ValorICMS"].ToString());

                        vICMS += (oNfe.Det.Ipost.Icms.ICMs20.vICMS);
                        VBC += (oNfe.Det.Ipost.Icms.ICMs20.vBC);

                    }
                    else if (oNfe.Det.Produto.SitTributB == "51")
                    {
                        oNfe.Det.Ipost.Icms.ICMs51.vBC = Dr["BaseICMS"] is DBNull ? 0 : decimal.Parse(Dr["BaseICMS"].ToString());
                        oNfe.Det.Ipost.Icms.ICMs51.pICMS = Dr["AliqICMS"] is DBNull ? 0 : decimal.Parse(Dr["AliqICMS"].ToString());
                        oNfe.Det.Ipost.Icms.ICMs51.vICMS = Dr["ValorICMS"] is DBNull ? 0 : decimal.Parse(Dr["ValorICMS"].ToString());

                        vICMS += (oNfe.Det.Ipost.Icms.ICMs51.vICMS);
                        VBC += (oNfe.Det.Ipost.Icms.ICMs51.vBC);
                    }
                else if (oNfe.Det.Produto.SitTributB == "90")
                {
                    oNfe.Det.Ipost.Icms.ICMs90.CST = "90";
                    oNfe.Det.Ipost.Icms.ICMs90.modBC = Dr["modBC"] is DBNull ? "3" : Dr["modBC"].ToString();
                    oNfe.Det.Ipost.Icms.ICMs90.vBC = Dr["BaseICMS"] is DBNull ? 0 : decimal.Parse(Dr["BaseICMS"].ToString());
                    oNfe.Det.Ipost.Icms.ICMs90.pICMS = Dr["AliqICMS"] is DBNull ? 0 : decimal.Parse(Dr["AliqICMS"].ToString());
                    oNfe.Det.Ipost.Icms.ICMs90.vICMS = Dr["ValorICMS"] is DBNull ? 0 : decimal.Parse(Dr["ValorICMS"].ToString());
                    oNfe.Det.Ipost.Icms.ICMs90.modBCST = Dr["modBCST"] is DBNull ? "" : Dr["modBCST"].ToString();
                    oNfe.Det.Ipost.Icms.ICMs90.pMVAST = Dr["pMVAST"] is DBNull ? 0 : decimal.Parse(Dr["pMVAST"].ToString());
                    oNfe.Det.Ipost.Icms.ICMs90.pRedBCST = Dr["pRedBCST"] is DBNull ? 0 : decimal.Parse(Dr["pRedBCST"].ToString());
                    oNfe.Det.Ipost.Icms.ICMs90.vBCST = Dr["vBCST"] is DBNull ? 0 : decimal.Parse(Dr["vBCST"].ToString());
                    oNfe.Det.Ipost.Icms.ICMs90.pICMSST = Dr["pICMSST"] is DBNull ? 0 : decimal.Parse(Dr["pICMSST"].ToString());
                    oNfe.Det.Ipost.Icms.ICMs90.vICMSST = Dr["vICMSST"] is DBNull ? 0 : decimal.Parse(Dr["vICMSST"].ToString());

                    vICMS += (oNfe.Det.Ipost.Icms.ICMs90.vICMS);
                    VBC += (oNfe.Det.Ipost.Icms.ICMs90.vBC);
                    vBCST += (oNfe.Det.Ipost.Icms.ICMs90.vBCST);
                    vICMSST += oNfe.Det.Ipost.Icms.ICMs90.vICMSST;
                }

                oNfe.Det.nItem = Contator.ToString();

                    VLProduto += oNfe.Det.Produto.vProd;
                    VLFrete += oNfe.Det.Produto.vFrete;
                    VLSeguro += oNfe.Det.Produto.vSeg;
                    VLDesconto += oNfe.Det.Produto.vDesc;
                    VLIPI += oNfe.Det.Ipost.Ipi.IPITrib.vIPI;
                    VLPIS += oNfe.Det.Ipost.Pis.vPis;
                    VLCOFINS += oNfe.Det.Ipost.Cofins.vCOFINS;
                    VLII += oNfe.Det.Ipost.II.vII;

                    oNfe.IcmsTot.vNF = (VLProduto + VLFrete + OutrasDespesas + VLSeguro + VLIPI + vICMSST) - VLDesconto;

                    OutrasDespesas += Dr["OutrasDespesas"] is DBNull ? 0 : decimal.Parse(Dr["OutrasDespesas"].ToString());

                    if (oNfe.Det.Produto.cProd == string.Empty || oNfe.Det.Produto.cProd == "0")
                    {
                        parametros = new string[] { "null", oNfe.Ide.cNF.ToString(), oNfe.infNfe.Substring(3, oNfe.infNfe.Length - 3), "4", "Não existe produtos associados a esse número de nota" };
                    }

                    XML.Append("<det nItem='" + Contator + "'>");
                    XML.Append("<prod>");
                    XML.Append("<cProd>" + oNfe.Det.Produto.cProd + "</cProd>");
                    XML.Append("<cEAN>SEM GTIN</cEAN>");
                    XML.Append("<xProd>" + oNfe.Util.FncRetiraCaracteresCampoTexto((oNfe.Det.Produto.xProd.Length > 59 ? oNfe.Det.Produto.xProd.Substring(0, 59) : oNfe.Det.Produto.xProd)).TrimStart().TrimEnd() + "</xProd>");
                    if (!(oNfe.Det.Produto.NCM == string.Empty))
                    {
                        XML.Append("<NCM>" + oNfe.Det.Produto.NCM + "</NCM>");
                    }

                    //// Alterado por: Bruno Arrais Cavalcante data: 13/06/2018
                    ////O campo CEST e os opcionais indEscala e CNPJFab foram movidos para um grupo próprio na NFe 4.0
                    XML.Append("<CEST>" + oNfe.Det.Produto.CEST + "</CEST>");
                    //XML.Append("< indEscala > S </ indEscala >");
                    //XML.Append("< CNPJFab > CNPJFab1 </ CNPJFab >");

                    XML.Append("<CFOP>" + oNfe.Det.Produto.CFOP.Replace(",", "") + "</CFOP>");
                    XML.Append("<uCom>" + oNfe.Util.FUNC_CARACTER_ESPECIAL(oNfe.Det.Produto.uCom.Replace(",", "")) + "</uCom>");
                    XML.Append("<qCom>" + string.Format("{0:0.000}", decimal.Parse(oNfe.Det.Produto.qCom.ToString())).Replace(",", ".") + "</qCom>");
                    XML.Append("<vUnCom>" + string.Format("{0:0.0000}", decimal.Parse(oNfe.Det.Produto.vUnCom.ToString())).Replace(",", ".") + "</vUnCom>");
                    XML.Append("<vProd>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Produto.vProd.ToString())).Replace(",", ".") + "</vProd>");
                    XML.Append("<cEANTrib>SEM GTIN</cEANTrib>");
                    XML.Append("<uTrib>" + oNfe.Util.FUNC_CARACTER_ESPECIAL(oNfe.Det.Produto.uTrib) + "</uTrib>");
                    XML.Append("<qTrib>" + string.Format("{0:0.000}", decimal.Parse(oNfe.Det.Produto.qTrib.ToString())).Replace(",", ".") + "</qTrib>");
                    XML.Append("<vUnTrib>" + string.Format("{0:0.0000}", decimal.Parse(oNfe.Det.Produto.vUnTrib.ToString())).Replace(",", ".") + "</vUnTrib>");

                    if (oNfe.Det.Produto.vDesc > 0)
                    {
                        XML.Append("<vDesc>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Produto.vDesc.ToString())).Replace(",", ".") + "</vDesc>");
                    }

                    if (oNfe.Det.Produto.vFrete > 0)
                    {
                        XML.Append("<vFrete>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Produto.vFrete.ToString())).Replace(",", ".") + "</vFrete>");
                    }

                    if (oNfe.Det.Produto.vOutras > 0)
                    {
                        XML.Append("<vOutro>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Produto.vOutras.ToString())).Replace(",", ".") + "</vOutro>");
                    }
                    XML.Append("<indTot>" + oNfe.Det.Produto.indTot + "</indTot>");

                    ////IMPORTAÇÃO!!
                    //if (oNfe.Det.Produto.CFOP.Replace(",", "").Substring(0, 1).ToString() == "3")
                    //{

                    //    XML.Append("<DI>");
                    //    XML.Append("<nDI>" + oNfe.NumDocDI.ToString() + "</nDI>");
                    //    XML.Append("<dDI>" + oNfe.DataRegistroDI.ToString() + "</dDI>");
                    //    XML.Append("<xLocDesemb>" + oNfe.LocalDesembDI.ToString() + "</xLocDesemb>");
                    //    XML.Append("<UFDesemb>" + oNfe.UFDesembDI.ToString() + "</UFDesemb>");
                    //    XML.Append("<dDesemb>" + oNfe.DataRegistroDI.ToString() + "</dDesemb>");

                    //    string cExportador = oNfe.IDCliente.ToString();

                    //    XML.Append("<cExportador>" + cExportador + "</cExportador>"); // <-- IDCLIENTE / IDFORNECEDOR

                    //    XML.Append("<adi>");
                    //    XML.Append("<nAdicao>" + Contator.ToString() + "</nAdicao>");
                    //    XML.Append("<nSeqAdic>" + Contator.ToString() + "</nSeqAdic>");
                    //    XML.Append("<cFabricante>" + oNfe.IDFabEstrangeiro.ToString() + "</cFabricante>"); // <-- IDFabEstrangeiro
                    //    XML.Append("</adi>");
                    //    XML.Append("</DI>");
                    //}

                    //NÚMERO DO PEDIDO DE COMPRA - ADICIONADO 29/11/2011!!
                    //if (!string.IsNullOrEmpty(oNfe.Det.Produto.xPed))
                    //    XML.Append("<xPed>" + oNfe.Det.Produto.xPed + "</xPed>");

                    XML.Append("</prod>");

                    XML.Append("<imposto>");
                    XML.Append("<ICMS>");

                    if (oNfe.Det.Produto.SitTributB == "20")
                    {
                        XML.Append("<ICMS20>");
                        XML.Append("<orig>" + oNfe.Det.Ipost.Icms.ICMs20.orig + "</orig>");
                        XML.Append("<CST>20</CST>");
                        XML.Append("<modBC>" + oNfe.Det.Ipost.Icms.ICMs20.modBC + "</modBC>");
                        XML.Append("<pRedBC>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Icms.ICMs20.pRedBC.ToString())).Replace(",", ".") + "</pRedBC>");
                        XML.Append("<vBC>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Icms.ICMs20.vBC.ToString())).Replace(",", ".") + "</vBC>");
                        XML.Append("<pICMS>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Icms.ICMs20.pICMS.ToString())).Replace(",", ".") + "</pICMS>");
                        XML.Append("<vICMS>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Icms.ICMs20.vICMS.ToString())).Replace(",", ".") + "</vICMS>");
                        XML.Append("</ICMS20>");
                    }
                    else if (oNfe.Det.Produto.SitTributB == "10")
                    {
                        XML.Append("<ICMS10>");
                        XML.Append("<orig>" + oNfe.Det.Ipost.Icms.ICMs10.orig + "</orig>");
                        XML.Append("<CST>10</CST>");
                        XML.Append("<modBC>" + oNfe.Det.Ipost.Icms.ICMs10.modBC + "</modBC>");
                        XML.Append("<vBC>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Icms.ICMs10.vBC.ToString())).Replace(",", ".") + "</vBC>");
                        XML.Append("<pICMS>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Icms.ICMs10.pICMS.ToString())).Replace(",", ".") + "</pICMS>");
                        XML.Append("<vICMS>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Icms.ICMs10.vICMS.ToString())).Replace(",", ".") + "</vICMS>");
                        XML.Append("<modBCST>" + oNfe.Det.Ipost.Icms.ICMs10.modBCST + "</modBCST>");
                        XML.Append("<vBCST>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Icms.ICMs10.vBCST.ToString())).Replace(",", ".") + "</vBCST>");
                        XML.Append("<pICMSST>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Icms.ICMs10.pICMSST.ToString())).Replace(",", ".") + "</pICMSST>");
                        XML.Append("<vICMSST>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Icms.ICMs10.vICMSST.ToString())).Replace(",", ".") + "</vICMSST>");
                        XML.Append("</ICMS10>");
                    }

                    //******************************************
                    //TODOS ABAIXO PERTENCEM AO GRUPO ICMS40!!
                    else if (oNfe.Det.Produto.SitTributB == "40")
                    {
                        XML.Append("<ICMS40>");
                        XML.Append("<orig>" + oNfe.Det.Ipost.Icms.ICMs40.orig + "</orig>");
                        XML.Append("<CST>40</CST>");
                        XML.Append("</ICMS40>");
                    }
                    else if (oNfe.Det.Produto.SitTributB == "41")
                    {
                        XML.Append("<ICMS40>");
                        XML.Append("<orig>" + oNfe.Det.Ipost.Icms.ICMs40.orig + "</orig>");
                        XML.Append("<CST>41</CST>");
                        XML.Append("</ICMS40>");
                    }
                    else if (oNfe.Det.Produto.SitTributB == "50")
                    {
                        XML.Append("<ICMS40>");
                        XML.Append("<orig>" + oNfe.Det.Ipost.Icms.ICMs40.orig + "</orig>");
                        XML.Append("<CST>50</CST>");
                        XML.Append("</ICMS40>");
                    }
                    //END TODOS ACIMA PERTENCEM AO GRUPO ICMS40!!
                    //*********************************************

                    else if (oNfe.Det.Produto.SitTributB == "51")
                    {
                        XML.Append("<ICMS51>");
                        XML.Append("<orig>" + oNfe.Det.Ipost.Icms.ICMs51.orig + "</orig>");
                        XML.Append("<CST>51</CST>");
                        XML.Append("<modBC>" + oNfe.Det.Ipost.Icms.ICMs51.modBC + "</modBC>");
                        XML.Append("<pRedBC>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Icms.ICMs51.pRedBC.ToString())).Replace(",", ".") + "</pRedBC>");
                        XML.Append("<vBC>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Icms.ICMs51.vBC.ToString())).Replace(",", ".") + "</vBC>");
                        XML.Append("<pICMS>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Icms.ICMs51.pICMS.ToString())).Replace(",", ".") + "</pICMS>");
                        XML.Append("<vICMS>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Icms.ICMs51.vICMS.ToString())).Replace(",", ".") + "</vICMS>");
                        XML.Append("</ICMS51>");
                    }
                    else if (oNfe.Det.Produto.SitTributB == "60")
                    {
                        XML.Append("<ICMS60>");
                        XML.Append("<orig>" + oNfe.Det.Ipost.Icms.ICMs60.orig.ToString() + "</orig>");
                        XML.Append("<CST>60</CST>");
                        XML.Append("</ICMS60>");
                    }
                else if (oNfe.Det.Produto.SitTributB == "90")
                {
                    XML.Append("<ICMS90>");
                    XML.Append("<orig>" + oNfe.Det.Ipost.Icms.ICMs90.orig + "</orig>");
                    XML.Append("<CST>90</CST>");
                    XML.Append("<modBC>" + oNfe.Det.Ipost.Icms.ICMs90.modBC + "</modBC>");
                    XML.Append("<vBC>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Icms.ICMs90.vBC.ToString())).Replace(",", ".") + "</vBC>");
                    XML.Append("<pICMS>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Icms.ICMs90.pICMS.ToString())).Replace(",", ".") + "</pICMS>");
                    XML.Append("<vICMS>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Icms.ICMs90.vICMS.ToString())).Replace(",", ".") + "</vICMS>");
                    //Essa Situação Tributária pode ter ou não a incidência de ST. Dessa forma, caso o campo modBCST tenha sido preenchido, preenche as informações no XML
                    if(oNfe.Det.Ipost.Icms.ICMs90.modBCST != "")
                    { 
                    XML.Append("<modBCST>" + oNfe.Det.Ipost.Icms.ICMs90.modBCST + "</modBCST>");
                    XML.Append("<vBCST>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Icms.ICMs90.vBCST.ToString())).Replace(",", ".") + "</vBCST>");
                    XML.Append("<pICMSST>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Icms.ICMs90.pICMSST.ToString())).Replace(",", ".") + "</pICMSST>");
                    XML.Append("<vICMSST>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Icms.ICMs90.vICMSST.ToString())).Replace(",", ".") + "</vICMSST>");
                    }
                    XML.Append("</ICMS90>");
                }
                else
                    {
                        XML.Append("<ICMS00>");
                        XML.Append("<orig>" + oNfe.Det.Ipost.Icms.ICMs00.orig + "</orig>");
                        XML.Append("<CST>00</CST>");
                        XML.Append("<modBC>" + oNfe.Det.Ipost.Icms.ICMs00.modBC + "</modBC>");
                        XML.Append("<vBC>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Icms.ICMs00.vBC.ToString())).Replace(",", ".") + "</vBC>");
                        XML.Append("<pICMS>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Icms.ICMs00.pICMS.ToString())).Replace(",", ".") + "</pICMS>");
                        XML.Append("<vICMS>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Icms.ICMs00.vICMS.ToString())).Replace(",", ".") + "</vICMS>");
                        XML.Append("</ICMS00>");
                    }

                    XML.Append("</ICMS>");

                    // A TAG IPI SÓ A PARECE QUANDO FOI INFORMADO ALGUM VALOR DESTE IMPOSTO
                    if (oNfe.Det.Ipost.Ipi.IPITrib.vIPI > 0)
                    {
                        XML.Append("<IPI>");
                        if (oNfe.Dest.EnderDest.UFCli == "EX")
                        {
                            XML.Append("<clEnq>999</clEnq>");
                        }

                        XML.Append("<cEnq>" + oNfe.Det.Ipost.Ipi.cEnq + "</cEnq>");

                        if (
                            oNfe.Det.Ipost.Ipi.IPITrib.CST == "01" ||
                            oNfe.Det.Ipost.Ipi.IPITrib.CST == "02" ||
                            oNfe.Det.Ipost.Ipi.IPITrib.CST == "03" ||
                            oNfe.Det.Ipost.Ipi.IPITrib.CST == "04" ||
                            oNfe.Det.Ipost.Ipi.IPITrib.CST == "05" ||
                            oNfe.Det.Ipost.Ipi.IPITrib.CST == "20" ||
                            oNfe.Det.Ipost.Ipi.IPITrib.CST == "51" ||
                            oNfe.Det.Ipost.Ipi.IPITrib.CST == "52" ||
                            oNfe.Det.Ipost.Ipi.IPITrib.CST == "53" ||
                            oNfe.Det.Ipost.Ipi.IPITrib.CST == "54" ||
                            oNfe.Det.Ipost.Ipi.IPITrib.CST == "55"
                            )
                        {
                            XML.Append("<IPINT>");
                            XML.Append("<CST>" + oNfe.Det.Ipost.Ipi.IPITrib.CST + "</CST>");
                            XML.Append("</IPINT>");
                        }
                        else
                        {
                            XML.Append("<IPITrib>");
                            XML.Append("<CST>" + oNfe.Det.Ipost.Ipi.IPITrib.CST + "</CST>");
                            XML.Append("<vBC>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Ipi.IPITrib.vBC.ToString())).Replace(",", ".") + "</vBC>");
                            XML.Append("<pIPI>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Ipi.IPITrib.pIPI.ToString())).Replace(",", ".") + "</pIPI>");
                            XML.Append("<vIPI>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Ipi.IPITrib.vIPI.ToString())).Replace(",", ".") + "</vIPI>");
                            XML.Append("</IPITrib>");
                        }
                        XML.Append("</IPI>");
                    }

                    // II - TAG de grupo do Imposto de Importação
                    //if (oNfe.Dest.EnderDest.UFCli == "EX")
                    //{
                    //    XML.Append("<II>");
                    //    XML.Append("<vBC>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.II.vBC.ToString())).Replace(",", ".") + "</vBC>");
                    //    XML.Append("<vDespAdu>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.II.vDespAdu.ToString())).Replace(",", ".") + "</vDespAdu>");
                    //    XML.Append("<vII>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.II.vII.ToString())).Replace(",", ".") + "</vII>");
                    //    XML.Append("<vIOF>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.II.vIOF.ToString())).Replace(",", ".") + "</vIOF>");
                    //    XML.Append("</II>");
                    //}

                    // PIS
                    XML.Append("<PIS>");
                    // CST - Código Situação Tributaria
                    // 01 - Operação Tributável - Base de Cálculo = Valor da Operação Alíquota Normal (Cumulativo / Não Cumulativo)
                    // 02 - Operação Tributável - Base de Cálculo = Valor da Operação (Alíquota Diferenciada)
                    if ((oNfe.Det.Ipost.Pis.CST == "01") || (oNfe.Det.Ipost.Pis.CST == "02"))
                    {
                        XML.Append("<PISAliq>");
                        XML.Append("<CST>" + oNfe.Det.Ipost.Pis.CST + "</CST>");
                        XML.Append("<vBC>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Pis.vBC.ToString())).Replace(",", ".") + "</vBC>");
                        XML.Append("<pPIS>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Pis.pPIS.ToString())).Replace(",", ".") + "</pPIS>");
                        XML.Append("<vPIS>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Pis.vPis.ToString())).Replace(",", ".") + "</vPIS>");
                        XML.Append("</PISAliq>");
                    }

                    // 03 - Operação Tributável - Base de Cálculo = Quantidade Vendida x Alíquota por Unidade de Produto
                    if (oNfe.Det.Ipost.Pis.CST == "03")
                    {
                        XML.Append("<PISQtde>");
                        XML.Append("<CST>" + oNfe.Det.Ipost.Pis.CST + "</CST>");
                        XML.Append("<qBCProd>" + string.Format("{0:0.0000}", decimal.Parse(oNfe.Det.Ipost.Pis.qBCProd.ToString())).Replace(",", ".") + "</qBCProd>");
                        XML.Append("<vAliqProd>" + string.Format("{0:0.0000}", decimal.Parse(oNfe.Det.Ipost.Pis.vAliqProd.ToString())).Replace(",", ".") + "</vAliqProd>");
                        XML.Append("<vPIS>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Pis.vPis.ToString())).Replace(",", ".") + "</vPIS>");
                        XML.Append("</PISQtde>");
                    }

                    // 04 - Operação Tributável - Tributação Monofásica - (Alíquota Zero)
                    // 06 - Operação Tributável - Alíquota Zero
                    // 07 - Operação Isenta da Contribuição
                    // 08 - Operação sem incidência da Contribuição
                    // 09 - Operação com Suspensão da Contribuição
                    if ((oNfe.Det.Ipost.Pis.CST == "04") || (oNfe.Det.Ipost.Pis.CST == "06") || (oNfe.Det.Ipost.Pis.CST == "07") || (oNfe.Det.Ipost.Pis.CST == "08") || (oNfe.Det.Ipost.Pis.CST == "09"))
                    {
                        XML.Append("<PISNT>");
                        XML.Append("<CST>" + oNfe.Det.Ipost.Pis.CST + "</CST>");
                        XML.Append("</PISNT>");
                    }

                    // 99 - Outras Operações
                    if ((oNfe.Det.Ipost.Pis.CST == "99") || (oNfe.Det.Ipost.Pis.CST == "49"))
                    {
                        XML.Append("<PISOutr>");
                        XML.Append("<CST>" + oNfe.Det.Ipost.Pis.CST + "</CST>");
                        XML.Append("<vBC>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Pis.vBC.ToString())).Replace(",", ".") + "</vBC>");
                        XML.Append("<pPIS>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Pis.pPIS.ToString())).Replace(",", ".") + "</pPIS>");
                        XML.Append("<vPIS>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Pis.vPis.ToString())).Replace(",", ".") + "</vPIS>");
                        XML.Append("</PISOutr>");
                    }
                    XML.Append("</PIS>");

                    XML.Append("<COFINS>");
                    // CST - Código Situação Tributaria
                    // 01 - Operação Tributável - Base de Cálculo = Valor da Operação Alíquota Normal (Cumulativo / Não Cumulativo)
                    // 02 - Operação Tributável - Base de Cálculo = Valor da Operação (Alíquota Diferenciada)
                    if ((oNfe.Det.Ipost.Cofins.CST == "01") || (oNfe.Det.Ipost.Cofins.CST == "02"))
                    {
                        XML.Append("<COFINSAliq>");
                        XML.Append("<CST>" + oNfe.Det.Ipost.Cofins.CST + "</CST>");
                        XML.Append("<vBC>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Cofins.vBC.ToString())).Replace(",", ".") + "</vBC>");
                        XML.Append("<pCOFINS>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Cofins.pCOFINS.ToString())).Replace(",", ".") + "</pCOFINS>");
                        XML.Append("<vCOFINS>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Cofins.vCOFINS.ToString())).Replace(",", ".") + "</vCOFINS>");
                        XML.Append("</COFINSAliq>");
                    }

                    // 03 - Operação Tributável - Base de Cálculo = Quantidade Vendida x Alíquota por Unidade de Produto
                    if (oNfe.Det.Ipost.Cofins.CST == "03")
                    {
                        XML.Append("<COFINSQtde>");
                        XML.Append("<CST>" + oNfe.Det.Ipost.Cofins.CST + "</CST>");
                        XML.Append("<qBCProd>" + string.Format("{0:0.0000}", decimal.Parse(oNfe.Det.Ipost.Cofins.qBCProd.ToString())).Replace(",", ".") + "</qBCProd>");
                        XML.Append("<vAliqProd>" + string.Format("{0:0.0000}", decimal.Parse(oNfe.Det.Ipost.Cofins.vAliqProd.ToString())).Replace(",", ".") + "</vAliqProd>");
                        XML.Append("<vCOFINS>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Cofins.vCOFINS.ToString())).Replace(",", ".") + "</vCOFINS>");
                        XML.Append("</COFINSQtde>");
                    }

                    // 04 - Operação Tributável - Tributação Monofásica - (Alíquota Zero)
                    // 06 - Operação Tributável - Alíquota Zero
                    // 07 - Operação Isenta da Contribuição
                    // 08 - Operação sem incidência da Contribuição
                    // 09 - Operação com Suspensão da Contribuição
                    if ((oNfe.Det.Ipost.Cofins.CST == "04") || (oNfe.Det.Ipost.Cofins.CST == "06") || (oNfe.Det.Ipost.Cofins.CST == "07") || (oNfe.Det.Ipost.Cofins.CST == "08") || (oNfe.Det.Ipost.Cofins.CST == "09"))
                    {
                        XML.Append("<COFINSNT>");
                        XML.Append("<CST>" + oNfe.Det.Ipost.Cofins.CST + "</CST>");
                        XML.Append("</COFINSNT>");
                    }

                    // 99 - Outras Operações
                    if ((oNfe.Det.Ipost.Cofins.CST == "99") || (oNfe.Det.Ipost.Cofins.CST == "49"))
                    {
                        XML.Append("<COFINSOutr>");
                        XML.Append("<CST>" + oNfe.Det.Ipost.Cofins.CST + "</CST>");
                        XML.Append("<vBC>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Cofins.vBC.ToString())).Replace(",", ".") + "</vBC>");
                        XML.Append("<pCOFINS>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Cofins.pCOFINS.ToString())).Replace(",", ".") + "</pCOFINS>");
                        XML.Append("<vCOFINS>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Det.Ipost.Cofins.vCOFINS.ToString())).Replace(",", ".") + "</vCOFINS>");
                        XML.Append("</COFINSOutr>");
                    }
                    XML.Append("</COFINS>");

                    XML.Append("</imposto>");
                    XML.Append("<infAdProd>FCP 0 BC 0.0 = 0.00</infAdProd>");
                    XML.Append("</det>");

                    Contator++;
                }
                Dr.Close();

            #endregion

            #region ..:: total ::..
                oNfe.IcmsTot.vProd = VLProduto;
                oNfe.IcmsTot.vST = vICMSST;
                oNfe.IcmsTot.vBC = VBC;
                oNfe.IcmsTot.vICMS = vICMS;
                oNfe.IcmsTot.vBCST = vBCST;
                oNfe.IcmsTot.vFrete = VLFrete;
                oNfe.IcmsTot.vDesc = VLDesconto;
                oNfe.IcmsTot.vSeg = VLSeguro;
                oNfe.IcmsTot.vIPI = VLIPI;
                oNfe.IcmsTot.vPIS = VLPIS;
                oNfe.IcmsTot.vCOFINS = VLCOFINS;
                oNfe.IcmsTot.vII = VLII;
                oNfe.IcmsTot.vOutro = OutrasDespesas;

                oNfe.IcmsTot.vNF = (VLProduto + VLFrete + OutrasDespesas + VLSeguro + VLIPI + vICMSST) - VLDesconto;

                XML.Append("<total>");
                XML.Append("<ICMSTot>");
                XML.Append("<vBC>" + string.Format("{0:0.00}", decimal.Parse(oNfe.IcmsTot.vBC.ToString())).Replace(",", ".") + "</vBC>");
                XML.Append("<vICMS>" + string.Format("{0:0.00}", decimal.Parse(oNfe.IcmsTot.vICMS.ToString())).Replace(",", ".") + "</vICMS>");
                XML.Append("<vICMSDeson>" + "0.00" + "</vICMSDeson>");

                // Alterado por: Bruno Arrais Cavalcante data: 10/08/2018
                // Os campos de FCP são opcionais, porém no total da NFe eles são obrigatórios, mesmo que zerados
                //XML.Append("<vFCPUFDest>" + string.Format("{0:0.00}", vTotalFCPUFDest).Replace(",", ".") + "</vFCPUFDest >");
                XML.Append("<vFCPUFDest>0.00</vFCPUFDest >");

                // Alterado por: Bruno Arrais Cavalcante data: 10/08/2018
                // Os campos de FCP são opcionais, porém no total da NFe eles são obrigatórios, mesmo que zerados
                //XML.Append("<vFCP>0.00</vFCP>"); //Valor Total do FCP (Fundo de Combate à Pobreza)
                //XML.Append("<vFCP>" + string.Format("{0:0.00}", vTotalFCPUFDest).Replace(",", ".") + "</vFCP>"); //Valor Total do FCP (Fundo de Combate à Pobreza)
                XML.Append("<vFCP>0.00</vFCP>"); //Valor Total do FCP (Fundo de Combate à Pobreza)

                XML.Append("<vBCST>" + string.Format("{0:0.00}", decimal.Parse(oNfe.IcmsTot.vBCST.ToString())).Replace(",", ".") + "</vBCST>");
                XML.Append("<vST>" + string.Format("{0:0.00}", decimal.Parse(oNfe.IcmsTot.vST.ToString())).Replace(",", ".") + "</vST>");

                // Alterado por: Bruno Arrais Cavalcante data: 10/08/2018
                // Os campos de FCP são opcionais, porém no total da NFe eles são obrigatórios, mesmo que zerados
                XML.Append("<vFCPST>" + "0.00" + "</vFCPST>"); //Valor Total do FCP (Fundo de Combate à Pobreza) retido por substituição tributária
                XML.Append("<vFCPSTRet>" + "0.00" + "</vFCPSTRet>"); //Valor Total do FCP retido anteriormente por Substituição Tributária    

                XML.Append("<vProd>" + string.Format("{0:0.00}", decimal.Parse(oNfe.IcmsTot.vProd.ToString())).Replace(",", ".") + "</vProd>");
                XML.Append("<vFrete>" + string.Format("{0:0.00}", decimal.Parse(oNfe.IcmsTot.vFrete.ToString())).Replace(",", ".") + "</vFrete>");
                XML.Append("<vSeg>" + string.Format("{0:0.00}", decimal.Parse(oNfe.IcmsTot.vSeg.ToString())).Replace(",", ".") + "</vSeg>");
                XML.Append("<vDesc>" + string.Format("{0:0.00}", decimal.Parse(oNfe.IcmsTot.vDesc.ToString())).Replace(",", ".") + "</vDesc>");
                XML.Append("<vII>" + string.Format("{0:0.00}", decimal.Parse(oNfe.IcmsTot.vII.ToString())).Replace(",", ".") + "</vII>");
                XML.Append("<vIPI>" + string.Format("{0:0.00}", decimal.Parse(oNfe.IcmsTot.vIPI.ToString())).Replace(",", ".") + "</vIPI>");

                if (oNfe.RefNf.finNFe == "4")
                {
                    // Alterado por: Bruno Arrais Cavalcante data: 13/06/2018
                    // O campo foi adicionado no grupo de total da versão 4.0. Corresponde ao total da soma dos campos de vIPIDevol dos produtos. É obrigatório, mesmo que zerado
                    XML.Append("<vIPIDevol>" + string.Format("{0:0.00}", "0") + "</vIPIDevol>"); //Valor Total do IPI devolvido
                }
                else
                {
                    XML.Append("<vIPIDevol>0.00</vIPIDevol>"); //Valor Total do IPI devolvido
                }

                XML.Append("<vPIS>" + string.Format("{0:0.00}", decimal.Parse(oNfe.IcmsTot.vPIS.ToString())).Replace(",", ".") + "</vPIS>");
                XML.Append("<vCOFINS>" + string.Format("{0:0.00}", decimal.Parse(oNfe.IcmsTot.vCOFINS.ToString())).Replace(",", ".") + "</vCOFINS>");
                XML.Append("<vOutro>" + string.Format("{0:0.00}", decimal.Parse(oNfe.IcmsTot.vOutro.ToString())).Replace(",", ".") + "</vOutro>");
                XML.Append("<vNF>" + string.Format("{0:0.00}", decimal.Parse(oNfe.IcmsTot.vNF.ToString())).Replace(",", ".") + "</vNF>");
                XML.Append("</ICMSTot>");
                XML.Append("</total>");
                #endregion

                #region ..:: transp ::..
                Dr = SqlHelper.ExecuteReader(cnnGeraXML, "stpRetornaTransportadoraNF", oNfe.cNF);

                if (Dr.Read())
                {
                    oNfe.TRansp.transportadora.CNPJ = Dr["CNPJ"] is DBNull ? string.Empty : Dr["CNPJ"].ToString().PadLeft(14, '0');
                    oNfe.TRansp.transportadora.xNome = Dr["Razao"] is DBNull ? string.Empty : oNfe.Util.FncRetiraCaracteresCampoTexto(Dr["Razao"].ToString().TrimStart().TrimEnd());
                    oNfe.TRansp.transportadora.IE = Dr["IE"] is DBNull ? string.Empty : oNfe.Util.FUNC_CARACTER_ESPECIAL(Dr["IE"].ToString());
                    oNfe.TRansp.transportadora.xEnder = Dr["Logradouro"] is DBNull ? string.Empty : oNfe.Util.FncRetiraCaracteresCampoTexto(Dr["Logradouro"].ToString()).TrimStart().TrimEnd();
                    oNfe.TRansp.transportadora.xMun = Dr["Municipio"] is DBNull ? string.Empty : Dr["Municipio"].ToString().TrimEnd();
                    oNfe.TRansp.transportadora.UF = Dr["sigla"] is DBNull ? string.Empty : Dr["sigla"].ToString();
                }

                Dr.Close();
 
                XML.Append("<transp>");
                XML.Append("<modFrete>" + oNfe.TRansp.modFrete + "</modFrete>");

                if (!(oNfe.TRansp.IdTransportadora == 0))
                {
                    if (!(oNfe.TRansp.transportadora.CNPJ == "00000000000000"))
                    {
                        if (!(oNfe.TRansp.transportadora.CNPJ == string.Empty))
                        {
                            XML.Append("<transporta>");
                            XML.Append("<CNPJ>" + oNfe.TRansp.transportadora.CNPJ.PadLeft(14, '0').TrimEnd() + "</CNPJ>");
                            XML.Append("<xNome>" + (oNfe.Util.FncRetiraCaracteresCampoTexto(oNfe.TRansp.transportadora.xNome.TrimEnd()).Length > 60 ? oNfe.Util.FncRetiraCaracteresCampoTexto(oNfe.TRansp.transportadora.xNome.TrimStart().TrimEnd()).Substring(0, 60) : oNfe.Util.FncRetiraCaracteresCampoTexto(oNfe.TRansp.transportadora.xNome.TrimStart().TrimEnd())) + "</xNome>");
                            if (oNfe.TRansp.transportadora.IE.Trim() == "")
                            {
                                XML.Append("<IE/>");
                            }
                            else
                            {
                                XML.Append("<IE>" + oNfe.Util.FUNC_CARACTER_ESPECIAL(oNfe.TRansp.transportadora.IE).TrimStart().TrimEnd() + "</IE>");
                            }

                            if (oNfe.TRansp.transportadora.xEnder.Trim() == "")
                            {
                                XML.Append("<xEnder />");
                            }
                            else
                            {
                                XML.Append("<xEnder>" + (oNfe.TRansp.transportadora.xEnder.Trim().Length > 60 ? oNfe.Util.FncRetiraCaracteresCampoTexto(oNfe.TRansp.transportadora.xEnder.Trim().Substring(0, 60)).TrimStart().TrimEnd() : oNfe.Util.FncRetiraCaracteresCampoTexto(oNfe.TRansp.transportadora.xEnder).TrimStart().TrimEnd()) + "</xEnder>");
                            }

                            if (oNfe.TRansp.transportadora.xEnder.Trim() == "")
                            {
                                XML.Append("<xMun />");
                            }
                            else
                            {
                                XML.Append("<xMun>" + oNfe.Util.FUNC_CARACTER_ESPECIAL(oNfe.TRansp.transportadora.xMun.TrimStart().TrimEnd()) + "</xMun>");
                            }

                            if (oNfe.TRansp.transportadora.UF.Trim() == "")
                            {
                                XML.Append("<UF />");
                            }
                            else
                            {
                                XML.Append("<UF>" + oNfe.TRansp.transportadora.UF.Trim() + "</UF>");
                            }


                            XML.Append("</transporta>");
                        }
                    }

                    if (!(oNfe.TRansp.VeicTransp.placa == "0"))
                    {
                        XML.Append("<veicTransp>");
                        XML.Append("<placa>" + oNfe.Util.FUNC_CARACTER_ESPECIAL(oNfe.TRansp.VeicTransp.placa).Trim().Replace(" ", "") + "</placa>");
                        XML.Append("<UF>" + oNfe.Util.FUNC_CARACTER_ESPECIAL(oNfe.TRansp.VeicTransp.UF).Trim() + "</UF>");
                        if (!(oNfe.TRansp.VeicTransp.RNTC == string.Empty))
                        {
                            XML.Append("<RNTC>" + oNfe.Util.FUNC_CARACTER_ESPECIAL(oNfe.TRansp.VeicTransp.RNTC).Trim() + "</RNTC>");
                        }
                        XML.Append("</veicTransp>");
                    }

                    if (!(oNfe.TRansp.Reboque.placa == string.Empty))
                    {
                        XML.Append("<reboque>");
                        XML.Append("<placa>" + oNfe.TRansp.Reboque.placa.Replace(" ", "") + "</placa>");
                        XML.Append("<UF>" + oNfe.TRansp.Reboque.UF + "</UF>");
                        if (!(oNfe.TRansp.Reboque.RNTC == string.Empty))
                        {
                            XML.Append("<RNTC>" + oNfe.TRansp.Reboque.RNTC + "</RNTC>");
                        }
                        XML.Append("</reboque>");
                    }

                    XML.Append("<vol>");
                    XML.Append("<qVol>" + oNfe.TRansp.Vol.qVol + "</qVol>");
                    XML.Append("<esp>" + oNfe.TRansp.Vol.esp + "</esp>");
                    XML.Append("<marca>" + oNfe.TRansp.Vol.marca + "</marca>");
                    XML.Append("<nVol>" + (oNfe.TRansp.Vol.nVol == string.Empty ? "0" : oNfe.TRansp.Vol.nVol) + "</nVol>");
                    XML.Append("<pesoL>" + string.Format("{0:0.000}", decimal.Parse(oNfe.TRansp.Vol.pesoL.ToString())).Replace(",", ".") + "</pesoL>");
                    XML.Append("<pesoB>" + string.Format("{0:0.000}", decimal.Parse(oNfe.TRansp.Vol.pesoB.ToString())).Replace(",", ".") + "</pesoB>");

                    if (!(oNfe.TRansp.Vol.Lacres.nLacre == string.Empty))
                    {
                        XML.Append("<lacres>");
                        XML.Append("<nLacre>" + oNfe.Util.FUNC_CARACTER_ESPECIAL(oNfe.TRansp.Vol.Lacres.nLacre) + "</nLacre>");
                        XML.Append("</lacres>");
                    }
                    XML.Append("</vol>");
                }
                XML.Append("</transp>");

            //DataSet DsFaturas = new DataSet();
            //DsFaturas = SqlHelper.ExecuteDataset(Conexao, "stpRetornaFaturas", oNfe.cNF);
            //int QtdeFaturas = 0;
            //QtdeFaturas = DsFaturas.Tables[0].Rows.Count;

            //if (QtdeFaturas > 0)
            //{
            //    XML.Append("<cobr>");
            //    foreach (DataRow DrFaturas in DsFaturas.Tables[0].Rows)
            //    {
            //        oNfe.TRansp.Cobr.DUP.nDup = oNfe.nNF + "-" + ((DrFaturas["Parcela"] is DBNull ? "1" : DrFaturas["Parcela"].ToString()) + "/" + QtdeFaturas.ToString());
            //        oNfe.TRansp.Cobr.DUP.dVenc = DateTime.Parse(DrFaturas["dtVencimento"].ToString()).ToString("yyyy-MM-dd");
            //        oNfe.TRansp.Cobr.DUP.vDup = string.Format("{0:0.00}", decimal.Parse(DrFaturas["Valor"].ToString())).Replace(",", ".");
            //        XML.Append("<dup>");
            //        XML.Append("<nDup>" + oNfe.TRansp.Cobr.DUP.nDup + "</nDup>");
            //        XML.Append("<dVenc>" + oNfe.TRansp.Cobr.DUP.dVenc + "</dVenc>");
            //        if (oNfe.IcmsTot.vNF > 0) // NFE COM VALOR
            //        {
            //            XML.Append("<vDup>" + oNfe.TRansp.Cobr.DUP.vDup + "</vDup>");
            //        }
            //        XML.Append("</dup>");
            //    }
            //    XML.Append("</cobr>");
            //}

            //DsFaturas.Dispose();

            #endregion

            #region ..:: Informacao Pagamento ::..            

            Boolean pag = false;
            Boolean troco = false;

            //Dados Pagamento obrigatório apenas para (NFC-e) NT 2012/004
            // Alterado por: Bruno Arrais Cavalcante data: 12/08/2018
            // O grupo de Pagamentos foi reestruturado e passou a ser obrigatório para NFe 4.0
            Dr = SqlHelper.ExecuteReader(cnnGeraXML, "stpRetornaDadosPag", oNfe.cNF);

            XML.Append("<pag>");

            while (Dr.Read())
            {
                pag = true;

                //oNfe.Pag.indPag = Dr["indPag"] is DBNull ? 0 : int.Parse(Dr["indPag"].ToString());
                oNfe.Pag.tPag = Dr["tPag"].ToString();
                oNfe.Pag.vPag = Dr["vPag"] is DBNull ? 0 : decimal.Parse(Dr["vPag"].ToString());
                oNfe.Pag.CNPJ = Dr["CNPJ"].ToString();
                oNfe.Pag.tBand  = Dr["tBand"].ToString();
                oNfe.Pag.vTroco = Dr["vTroco"] is DBNull ? 0 : decimal.Parse(Dr["vTroco"].ToString());                
                
                XML.Append("<detPag>");
                //XML.Append("<indPag>" + oNfe.Pag.indPag.ToString() + "</indPag>");
                XML.Append("<tPag>" + oNfe.Pag.tPag.ToString() + "</tPag>");

                if (oNfe.Pag.tPag != "90")
                {
                    XML.Append("<vPag>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Pag.vPag.ToString())).Replace(",", ".") + "</vPag>");
                }
                else
                {
                    XML.Append("<vPag>0.00</vPag>");
                }

                if (oNfe.Pag.tPag.ToString() == "03" || oNfe.Pag.tPag.ToString() == "04")
                {
                    XML.Append("<card>");
                    XML.Append("<tpIntegra>2</tpIntegra>");
                    XML.Append("<CNPJ>" + oNfe.Pag.CNPJ.ToString() + "</CNPJ >");
                    XML.Append("<tBand>" + oNfe.Pag.tBand.ToString() + "</tBand>");
                    //XML.Append("<cAut></cAut>");
                    XML.Append("</card>");
                }

                if (oNfe.Pag.tPag != "90")
                {
                    troco = true;
                }

                XML.Append("</detPag>");
            }

            Dr.Close();

            if (pag == false)
            {
                XML.Append("<detPag>");
                //XML.Append("<indPag>" + oNfe.Pag.indPag.ToString() + "</indPag>");
                XML.Append("<tPag>90</tPag>");
                XML.Append("<vPag>0.00</vPag>");
                XML.Append("</detPag>");
            }

            if (troco == true)
            {
                XML.Append("<vTroco>" + string.Format("{0:0.00}", decimal.Parse(oNfe.Pag.vTroco.ToString())).Replace(",", ".") + "</vTroco>");
            }

            XML.Append("</pag>");            

            #endregion

            #region ..:: Informacao Adicional ::..

            string 
                    DadosAdicionais = string.Empty;

            Dr = SqlHelper.ExecuteReader(cnnGeraXML, "stpRetornaDadosAdicionais", oNfe.cNF.ToString());

            if (Dr.Read())
            {
                DadosAdicionais = "OBS: " + (Dr["DadosAdicionais"] is DBNull ? string.Empty : Dr["DadosAdicionais"].ToString().Trim() == string.Empty ? string.Empty : Dr["DadosAdicionais"].ToString()).ToString();
            }

            Dr.Close();

            #region ..:: infAdic ::..
            XML.Append("<infAdic>");
            XML.Append("<infCpl>" + oNfe.Util.FncRetiraCaracteresCampoTexto(DadosAdicionais.Replace("\r\n", " ").TrimEnd()) + "</infCpl>");
            XML.Append("</infAdic>");
            #endregion

            #endregion

            XML.Append("</infNFe>");

            #region ..:: Informações Suplementares da NFe ::..
            // Somente para NFCe
            //// Alterado por: Bruno Arrais Cavalcante data: 13/06/2018
            //// O campo foi adicionado na versão 4.0 para incluir o link de consulta da Sefaz estadual onde a NFCe foi emitida
            //XML.Append("<infNFeSupl>");
            //XML.Append("<urlChave> urlChave____________1 </urlChave >"); // Texto com a URL de consulta por chave de acesso a ser impressa no DANFE NFC-e.
            //XML.Append("</ infNFeSupl>");
            #endregion

            XML.Append("</NFe>");

            #endregion

            try
            {
                #region ..:: Assina Nfe ::..
                Assinatura.AssinaXML ObjXml = new Assinatura.AssinaXML();
                String XmlNfeAss = ObjXml.FncAssinarXML(XML.ToString(), "infNFe", CertAssina);
                #endregion

                // Motivo desse armazenamento: esse xml é usado em na RetRecepcao, ele é usado para
                // geração do arquivo de Distribuição para o Destinatário.
                obj.Util.FncGravaXML(XmlNfeAss.ToString(), obj.PastaXMLRecepcaoCliente + oNfe.cNF + ".xml");

                #region ..:: adiciona Nfe no Lote ::..
                infoLote.Append("<enviNFe xmlns='" + ConfigurationManager.AppSettings["NameSpaceNFE"].ToString() + "' versao='" + ConfigurationManager.AppSettings["VersaoDadosNFENova"].ToString() + "'>");
                infoLote.Append("<idLote>" + oNfe.cNF + "</idLote>");
                infoLote.Append("<indSinc>" + oNfe.indSinc + "</indSinc>");
                infoLote.Append(XmlNfeAss);
                infoLote.Append("</enviNFe>");
                #endregion
              
            }
            catch (Exception ex)
            {
                StreamWriter ArquivoErro = new StreamWriter(pastaErro + oNfe.cNF + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + "_AssinaNfe.txt", false, Encoding.ASCII);
                ArquivoErro.WriteLine("***************** Assina Nfe *****************");
                ArquivoErro.WriteLine("Erro gerado " + DateTime.Now.ToString());
                ArquivoErro.WriteLine("IDNota: " + oNfe.cNF);
                ArquivoErro.WriteLine("CaminhoCertificado: " + ConfigurationManager.AppSettings["CaminhoCertificado"].ToString());
                ArquivoErro.WriteLine("SenhaCertificado: " + ConfigurationManager.AppSettings["SenhaCertificado"].ToString());

                ArquivoErro.WriteLine(ex.Message);
                ArquivoErro.WriteLine(ex.ToString());
                ArquivoErro.Flush();
                ArquivoErro.Close();
            }

            if (cnnGeraXML.State == ConnectionState.Open)
            {
                cnnGeraXML.Close();
                cnnGeraXML.Dispose();
            }
            return infoLote.ToString();
        }

        public void MontaXML()
        {
            NotaFiscalEletronica oNfe = new NotaFiscalEletronica();
            X509Certificate2 CertAssina = new X509Certificate2(ConfigurationManager.AppSettings["CaminhoCertificado"].ToString(), ConfigurationManager.AppSettings["SenhaCertificado"].ToString());
            string[] parametros;
            SqlConnection cnnMontaXML = new SqlConnection(Conexao);
            SqlConnection cnnMontaXML2 = new SqlConnection(Conexao);
            string strAmbiente = ConfigurationManager.AppSettings["Ambiente"].ToString();

            // Coloque isso antes de qualquer chamada ao webservice
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            try
            {

                #region Parte Responsável por selecionar as informações da tabela de notas

                DrNotas = SqlHelper.ExecuteReader(cnnMontaXML, "stpRetornaNotaFiscal", null); // Lista Notas Fiscais

                if (DrNotas.HasRows)
                {
                    #region Parte Responsável por formar o ID do cabeçalho da Nota e acrescentar o digito no mesmo

                    Dr = SqlHelper.ExecuteReader(cnnMontaXML2, "stpRetornaParametrosNFE", null);

                    if (Dr.Read())
                    {
                        oNfe.cUf = Dr["Cuf"] is DBNull ? "0" : Dr["Cuf"].ToString();
                        oNfe.CNPJ = Dr["cnpj"] is DBNull ? "0".PadLeft(14, '0') : Dr["cnpj"].ToString();
                        oNfe.mod = Dr["mod"] is DBNull ? "0" : Dr["mod"].ToString();
                        //oNfe.serie = Dr["serie"] is DBNull ? "0" : Dr["serie"].ToString();

                        oNfe.Det.Ipost.Pis.CST = Dr["PisCST"] is DBNull ? "01" : Dr["PisCST"].ToString();
                        oNfe.Det.Ipost.Cofins.CST = Dr["CofinsCST"] is DBNull ? "01" : Dr["CofinsCST"].ToString();
                        /*     O Campo ICMS 00 tem os valroes abaixo
                               0 – Nacional;
                               1 – Estrangeira – Importação direta;
                               2 – Estrangeira – Adquirida no
                               mercado interno.*/

                        oNfe.Det.Ipost.Ipi.clEnq = Dr["IPIClasseEnquadramento"] is DBNull ? "999" : Dr["IPIClasseEnquadramento"].ToString();
                        oNfe.Det.Ipost.Ipi.cEnq = Dr["IPICodEnquadramento"] is DBNull ? "999" : Dr["IPICodEnquadramento"].ToString();

                        oNfe.Det.Ipost.Icms.ICMs00.CST = Dr["ICMSCst"] is DBNull ? "00" : Dr["ICMSCst"].ToString();
                        oNfe.Det.Ipost.Icms.ICMs00.modBC = Dr["ModBC"] is DBNull ? "3" : Dr["ModBC"].ToString();

                        oNfe.Det.Ipost.Icms.ICMs20.CST = Dr["ICMS20CST"] is DBNull ? 20 : decimal.Parse(Dr["ICMS20CST"].ToString());
                        oNfe.Det.Ipost.Icms.ICMs20.modBC = Dr["ICMS20ModBC"] is DBNull ? 3 : decimal.Parse(Dr["ICMS20ModBC"].ToString());
                        oNfe.Det.Ipost.Icms.ICMs20.pRedBC = Dr["ICMS20pRedBC"] is DBNull ? 0 : decimal.Parse(Dr["ICMS20pRedBC"].ToString());

                        oNfe.Det.Ipost.Icms.ICMs40.CST = Dr["ICMS40CST"] is DBNull ? 40 : decimal.Parse(Dr["ICMS40CST"].ToString());

                        //NEW - VALORES DE CST PARA GRUPO ICMS40!!
                        oNfe.Det.Ipost.Icms.ICMs40.CST41 = Dr["ICMS41CST"] is DBNull ? 41 : float.Parse(Dr["ICMS41CST"].ToString());
                        oNfe.Det.Ipost.Icms.ICMs40.CST50 = Dr["ICMS50CST"] is DBNull ? 50 : float.Parse(Dr["ICMS50CST"].ToString());

                        oNfe.Det.Ipost.Icms.ICMs51.CST = Dr["ICMS51CST"] is DBNull ? 51 : decimal.Parse(Dr["ICMS51CST"].ToString());
                        oNfe.Det.Ipost.Icms.ICMs51.modBC = Dr["ICMS51ModBC"] is DBNull ? 3 : decimal.Parse(Dr["ICMS51ModBC"].ToString());
                        oNfe.Det.Ipost.Icms.ICMs51.pRedBC = Dr["ICMS51pRedBC"] is DBNull ? 0 : decimal.Parse(Dr["ICMS51pRedBC"].ToString());

                        oNfe.Det.Ipost.Icms.ICMs70.CST = Dr["ICMS70CST"] is DBNull ? 51 : decimal.Parse(Dr["ICMS70CST"].ToString());
                        oNfe.Det.Ipost.Icms.ICMs70.modBC = Dr["ICMS70ModBC"] is DBNull ? 3 : decimal.Parse(Dr["ICMS70ModBC"].ToString());
                        oNfe.Det.Ipost.Icms.ICMs70.pRedBC = Dr["ICMS70pRedBC"] is DBNull ? 0 : decimal.Parse(Dr["ICMS70pRedBC"].ToString());

                        //ICMS60CST - Valor padrão
                        oNfe.Det.Ipost.Icms.ICMs60.CST = Dr["ICMSCst"] is DBNull ? "00" : Dr["ICMSCst"].ToString();

                        oNfe.RefNf.cUF = Dr["Cuf"] is DBNull ? "0" : Dr["Cuf"].ToString();
                        oNfe.RefNf.CNPJ = Dr["cnpj"] is DBNull ? string.Empty : Dr["cnpj"].ToString();
                        oNfe.RefNf.tpImp = Dr["tpImp"] is DBNull ? "1" : Dr["tpImp"].ToString();
                        oNfe.RefNf.tpAmb = strAmbiente;

                        oNfe.RefNf.procEmi = Dr["procEmiss"] is DBNull ? "0" : Dr["procEmiss"].ToString();
                        oNfe.RefNf.AAMM = DateTime.Now.ToString("yyMM");
                        oNfe.RefNf.mod = Dr["refNFMod"] is DBNull ? "01" : Dr["refNFMod"].ToString();
                        oNfe.RefNf.serie = Dr["refNFSerie"] is DBNull ? "0" : Dr["refNFSerie"].ToString();

                        oNfe.Ide.Mod = Dr["mod"] is DBNull ? "55" : Dr["mod"].ToString();
                        oNfe.Ide.tpImp = Dr["tpImp"] is DBNull ? 1 : int.Parse(Dr["tpImp"].ToString());
                        oNfe.Ide.tpEmis = Dr["tpEmis"] is DBNull ? "1" : Dr["tpEmis"].ToString();
                        oNfe.Ide.Serie = Dr["serie"] is DBNull ? "0" : Dr["serie"].ToString();
                        oNfe.Ide.tpAmb = strAmbiente;
                        oNfe.Ide.procEmi = Dr["procEmiss"] is DBNull ? "0" : Dr["procEmiss"].ToString();
                        //oNfe.Ide.dSaiEnt = DateTime.Now.ToString("yyyy-MM-dd");
                        oNfe.Ide.verProc = Dr["VerProc"] is DBNull ? "NF-eletronica.com" : Dr["VerProc"].ToString();
                        oNfe.Ide.cUF = Dr["Cuf"] is DBNull ? "0" : Dr["Cuf"].ToString();
                        oNfe.Ide.cMunFG = Dr["cMunFg"] is DBNull ? 0 : int.Parse(Dr["cMunFg"].ToString());
                        oNfe.Ide.tpImp = Dr["tpImp"] is DBNull ? 1 : int.Parse(Dr["tpImp"].ToString());
                        //oNfe.Ide.tpEmis = Dr["tpEmis"] is DBNull ? "1" : Dr["tpEmis"].ToString();
                        //oNfe.Ide.indPag = Dr["IndPag"] is DBNull ? 0 : int.Parse(Dr["IndPag"].ToString());

                        //NEW VERSÃO NFe 4.01!

                        oNfe.Ide.dhCont = Dr["dhCont"] is DBNull ? string.Empty : string.Format("{0:yyyy-MM-ddTHH:mm:ss}{1}", Convert.ToDateTime(Dr["dhCont"].ToString()).AddMinutes(-3), TimeZone.CurrentTimeZone.GetUtcOffset(Convert.ToDateTime(Dr["dhCont"].ToString())).ToString().Substring(0, 6));
                        oNfe.Ide.xJust = Dr["xJust"] is DBNull ? string.Empty : Dr["xJust"].ToString();

                        oNfe.Emit.EnderEmit.cMun = Dr["Cuf"] is DBNull ? "0" : Dr["Cuf"].ToString();
                        oNfe.Emit.EnderEmit.xPais = Dr["DescPais"] is DBNull ? string.Empty : Dr["DescPais"].ToString();
                        oNfe.Emit.EnderEmit.cPais = Dr["CodPais"] is DBNull ? string.Empty : Dr["CodPais"].ToString();

                        oNfe.Dest.EnderDest.xPais = Dr["DescPais"] is DBNull ? string.Empty : Dr["DescPais"].ToString();
                        oNfe.Dest.EnderDest.cPais = Dr["CodPais"] is DBNull ? string.Empty : Dr["CodPais"].ToString();
                    }
                    Dr.Close();

                    oNfe.AAMM = DateTime.Now.ToString("yyMM");

                    #endregion

                    #region Parte Responsável selecionar os dados da empresa Emissora

                    Dr = SqlHelper.ExecuteReader(cnnMontaXML2, "stpRetornaDadosEmpresa", null);

                    if (Dr.Read())
                    {
                        oNfe.Emit.CNPJ = Dr["CNPJ"] is DBNull ? "0" : Dr["CNPJ"].ToString().Replace(".", "").Replace("/", "").PadLeft(14, '0').Replace("-", "");
                        oNfe.Emit.xNome = Dr["Empresa"] is DBNull ? string.Empty : Dr["Empresa"].ToString();
                        oNfe.Emit.xFant = Dr["Fantasia"] is DBNull ? string.Empty : Dr["Fantasia"].ToString();
                        oNfe.Emit.EnderEmit.IE = Dr["IE"] is DBNull ? string.Empty : Dr["IE"].ToString();
                        oNfe.Emit.EnderEmit.xLgr = Dr["Endereco"] is DBNull ? string.Empty : (Dr["Endereco"].ToString().Length > 60 ? Dr["Endereco"].ToString().Substring(0, 60) : Dr["Endereco"].ToString().Trim());
                        oNfe.Emit.EnderEmit.nro = Dr["Numero"] is DBNull ? string.Empty : Dr["Numero"].ToString();
                        oNfe.Emit.EnderEmit.xCpl = Dr["Complemento"] is DBNull ? string.Empty : Dr["Complemento"].ToString();
                        oNfe.Emit.EnderEmit.xBairro = Dr["Bairro"] is DBNull ? string.Empty : (Dr["Bairro"].ToString().Length > 60 ? Dr["Bairro"].ToString().Substring(0, 60) : Dr["Bairro"].ToString().Trim());
                        oNfe.Emit.EnderEmit.cMun = Dr["CodMunicipio"] is DBNull ? string.Empty : Dr["CodMunicipio"].ToString().PadLeft(7, '0');
                        oNfe.Emit.EnderEmit.xMun = Dr["Municipio"] is DBNull ? string.Empty : (Dr["Municipio"].ToString().Length > 60 ? Dr["Municipio"].ToString().Substring(0, 60) : Dr["Municipio"].ToString().Trim());
                        oNfe.Emit.EnderEmit.UF = Dr["UF"] is DBNull ? string.Empty : Dr["UF"].ToString().Length > 2 ? Dr["UF"].ToString().Substring(0, 2) : Dr["UF"].ToString().Trim();
                        oNfe.Emit.EnderEmit.CEP = Dr["CEP"] is DBNull ? string.Empty : Dr["CEP"].ToString().Replace("-", "").Length > 8 ? Dr["CEP"].ToString().Replace("-", "").Substring(0, 8) : Dr["CEP"].ToString().Replace("-", "").Trim();
                        oNfe.Emit.EnderEmit.fone = Dr["Telefone"] is DBNull ? string.Empty : Dr["Telefone"].ToString().Replace(".", "").Replace("/", "").Replace("(", "").Replace(")", "").Replace(" ", "").Trim();
                    }
                    Dr.Close();

                    #endregion


                }

                while (DrNotas.Read())
                {
                    oNfe.cNF = DrNotas["IdNota"] is DBNull ? "0" : DrNotas["IdNota"].ToString();

                    oNfe.nNF = DrNotas["Numero"] is DBNull ? "1" : DrNotas["Numero"].ToString();

                    oNfe.RefNf.chNfeReferenciada = DrNotas["chNFeReferenciada"] is DBNull ? "0" : DrNotas["chNFeReferenciada"].ToString();
                    oNfe.RefNf.finNFe = DrNotas["finNfe"] is DBNull ? "1" : DrNotas["finNfe"].ToString();

                    oNfe.TRansp.modFrete = DrNotas["modFrete"] is DBNull ? 1 : int.Parse(DrNotas["modFrete"].ToString());
                    oNfe.TRansp.IdTransportadora = DrNotas["idTransportadora"] is DBNull ? 0 : int.Parse(DrNotas["idTransportadora"].ToString());
                    oNfe.TRansp.VeicTransp.placa = DrNotas["PlacaVeic"].ToString() == "" ? "0" : DrNotas["PlacaVeic"].ToString();
                    oNfe.TRansp.VeicTransp.UF = DrNotas["UfVeiculo"] is DBNull ? "0" : DrNotas["UfVeiculo"].ToString();

                    oNfe.TRansp.Vol.qVol = DrNotas["Volumes"] is DBNull ? "0" : DrNotas["Volumes"].ToString();
                    oNfe.TRansp.Vol.esp = DrNotas["Especie"] is DBNull ? string.Empty : DrNotas["Especie"].ToString();
                    oNfe.TRansp.Vol.marca = DrNotas["Marca"].ToString() == "" ? "Indefinido" : DrNotas["Marca"].ToString();

                    oNfe.TRansp.Vol.pesoL = DrNotas["PesoLiquido"] is DBNull ? 0 : decimal.Parse(DrNotas["PesoLiquido"].ToString().Replace(".", ","));
                    oNfe.TRansp.Vol.pesoB = DrNotas["PesoBruto"] is DBNull ? 0 : decimal.Parse(DrNotas["PesoBruto"].ToString().Replace(".", ","));

                    oNfe.IDCliente = Convert.ToInt64(DrNotas["IdCliente"] is DBNull ? 0 : int.Parse(DrNotas["IdCliente"].ToString()));

                    oNfe.Ide.nNF = DrNotas["Numero"] is DBNull ? "0" : DrNotas["Numero"].ToString();
                    oNfe.Ide.natOp = DrNotas["DescNaturezaOP"] is DBNull ? "0" : oUtil.FUNC_CARACTER_ESPECIAL(DrNotas["DescNaturezaOP"].ToString());
                    oNfe.Ide.tpNF = DrNotas["TpNf"] is DBNull ? 0 : int.Parse(DrNotas["TpNf"].ToString());
                    oNfe.Ide.tpEmis = DrNotas["tpEmis"] is DBNull ? "1" : DrNotas["tpEmis"].ToString();
                    oNfe.Ide.Serie = DrNotas["serie"] is DBNull ? "0" : DrNotas["serie"].ToString();

                    //oNfe.Ide.dEmi = DateTime.Now.ToString("yyyy-MM-dd");
                    oNfe.Ide.cNF = DrNotas["IdNota"] is DBNull ? "0" : DrNotas["IdNota"].ToString();

                    oNfe.Ide.dhCont = DrNotas["dhCont"] is DBNull ? string.Empty : Convert.ToDateTime(DrNotas["dhCont"].ToString()).ToString("yyyy-MM-ddTHH:mm:ss");
                    oNfe.Ide.xJust = DrNotas["xJust"] is DBNull ? string.Empty : DrNotas["xJust"].ToString();

                    IdNota = null;
                    IdNota = new StringBuilder();

                    //NOVO LAYOUT 2.00!
                    IdNota.Append(
                                  oNfe.cUf.PadLeft(2, '0') +
                                  oNfe.AAMM +
                                  oNfe.CNPJ.PadLeft(14, '0') +
                                  oNfe.mod.PadLeft(2, '0') +
                                  oNfe.Ide.Serie.PadLeft(3, '0') +
                                  oNfe.nNF.PadLeft(9, '0') +
                                  oNfe.Ide.tpEmis +
                                  oNfe.cNF.PadLeft(8, '0')
                                  );

                    IdNota.Append(oUtil.FncRetornaDigitoNota(IdNota));
                    oNfe.infNfe = "NFe" + IdNota.ToString();
                    oNfe.RefNf.cDV = oNfe.infNfe.Substring(oNfe.infNfe.Length - 1, 1);
                    oNfe.Ide.cDV = oNfe.infNfe.Substring(oNfe.infNfe.Length - 1, 1);

                #endregion

                    #region Propriedades Versão 3.10
                    oNfe.Ide.idDest = DrNotas["idDest"] is DBNull ? "0" : DrNotas["idDest"].ToString();
                    oNfe.Ide.indFinal = DrNotas["indFinal"] is DBNull ? "0" : DrNotas["indFinal"].ToString();
                    oNfe.Ide.indPres = DrNotas["indPres"] is DBNull ? "0" : DrNotas["indPres"].ToString();
                    oNfe.Dest.indIEDest = DrNotas["indIEDest"] is DBNull ? "0" : DrNotas["indIEDest"].ToString();
                    oNfe.RefNf.NfeReferenciada = DrNotas["NfeReferenciada"] is DBNull ? "" : DrNotas["NfeReferenciada"].ToString();
                    #endregion

                    #region Parte Responsável selecionar os dados da empresa recebedora

                    Dr = SqlHelper.ExecuteReader(cnnMontaXML2, "stpRetornaDadoCliente", oNfe.cNF);

                    if (Dr.Read())
                    {
                        oNfe.Dest.CNPJ = Dr["CNPJ"] is DBNull ? "0" : Dr["CNPJ"].ToString().Replace(".", "").Replace("/", "").Replace("-", "");
                        oNfe.Dest.xNome = Dr["Empresa"] is DBNull ? string.Empty : oNfe.Util.FncRetiraCaracteresCampoTexto(Dr["Empresa"].ToString());
                        oNfe.Dest.EnderDest.IE = Dr["IE"] is DBNull ? string.Empty : Dr["IE"].ToString();
                        oNfe.Dest.EnderDest.xLgr = Dr["Flogradouro"] is DBNull ? string.Empty : (Dr["Flogradouro"].ToString().Length > 60 ? oNfe.Util.FncRetiraCaracteresCampoTexto(Dr["Flogradouro"].ToString().Substring(0, 60)) : oNfe.Util.FncRetiraCaracteresCampoTexto(Dr["Flogradouro"].ToString().Trim()));
                        oNfe.Dest.EnderDest.nro = Dr["FNumero"] is DBNull ? string.Empty : Dr["FNumero"].ToString();
                        oNfe.Dest.EnderDest.xBairro = Dr["FBairro"] is DBNull ? string.Empty : (Dr["FBairro"].ToString().Length > 60 ? Dr["FBairro"].ToString().Substring(0, 60) : Dr["FBairro"].ToString().Trim());
                        oNfe.Dest.EnderDest.cMun = Dr["CodMunicipio"] is DBNull ? "0" : Dr["CodMunicipio"].ToString().PadLeft(7, '0');
                        oNfe.Dest.EnderDest.xMun = Dr["FMunicipio"] is DBNull ? string.Empty : (Dr["FMunicipio"].ToString().Length > 60 ? Dr["FMunicipio"].ToString().Substring(0, 60) : Dr["FMunicipio"].ToString().Trim());
                        oNfe.Dest.EnderDest.UF = Dr["FUF"] is DBNull ? string.Empty : Dr["FUF"].ToString().Length > 2 ? Dr["FUF"].ToString().Substring(0, 2) : Dr["FUF"].ToString().Trim();
                        oNfe.Dest.EnderDest.UFCli = Dr["UFCli"] is DBNull ? string.Empty : Dr["UFCli"].ToString().Length > 2 ? Dr["UFCli"].ToString().Substring(0, 2) : Dr["UFCli"].ToString().Trim();
                        oNfe.Dest.EnderDest.CEP = Dr["FCEP"] is DBNull ? string.Empty : Dr["FCEP"].ToString().Replace("-", "").Length > 8 ? Dr["FCEP"].ToString().Replace("-", "").Substring(0, 8) : Dr["FCEP"].ToString().Replace("-", "").Trim();
                        oNfe.Dest.EnderDest.xCpl = Dr["FComplemento"] is DBNull ? string.Empty : (Dr["FComplemento"].ToString().Length > 60 ? Dr["FComplemento"].ToString().Trim().Substring(0, 60) : Dr["FComplemento"].ToString());
                        oNfe.Dest.EnderDest.fone = Dr["Telefone"] is DBNull ? string.Empty : oNfe.Util.FUNC_CARACTER_ESPECIAL(Dr["Telefone"].ToString()).Trim();
                        oNfe.Dest.EnderDest.cPais = Dr["CodPais"] is DBNull ? string.Empty : Dr["CodPais"].ToString().Trim();
                        oNfe.Dest.EnderDest.xPais = Dr["Pais"] is DBNull ? string.Empty : oUtil.FncRetiraCaracteresCampoTexto(Dr["Pais"].ToString()).TrimStart().TrimEnd();

                        //NEW VERSÃO 4.01 NFe!
                        oNfe.Dest.email = Dr["EmailNFe"] is DBNull ? string.Empty : Dr["EmailNFe"].ToString();
                    }
                    Dr.Close();
 
                    #endregion

                    String strXml = GeraXML(oNfe);
                                        
                    #region Responsável por gravar as informações e atualizar o banco de dados

                    obj.Util.FncGravaXML(strXml, obj.PastaXMLRecepcao + oNfe.cNF + ".xml");

                    #region CONVERTE STRING EM XML NODE!!

                    XmlTextReader xmlReader = new XmlTextReader(new
                        StringReader(strXml));

                    // if you already have an XmlDocument then use that, otherwise
                    // create one
                    XmlDocument xmlDocument = new XmlDocument();
                    XmlNode nodeEnvio = xmlDocument.ReadNode(xmlReader);

                    #endregion

                    string RetornoRecepcao = string.Empty;

                    // ############################################################################################
                    // 5 – Contingência FS-DA - emissão em contingência com impressão do DANFE em Formulário de 
                    //     Segurança para Impressão de Documento Auxiliar de Documento Fiscal Eletrônico (FS-DA).
                    //
                    // Caso o Tipo de Emissão for 5, o XML é gravado porém não é enviado p/ nenhum servidor
                    // para validação.
                    // ############################################################################################

                    if (oNfe.Ide.tpEmis == "5")
                    {
                        string[] ParamsCont = {
                                              "7",
                                             oNfe.cNF,
                                             null,
                                             null,
                                             null,
                                             null,
                                             null,
                                             oNfe.infNfe.Substring(3, oNfe.infNfe.Length - 3),
                                             null,
                                             null
                                          };

                        // Atualiza o blnXMLContigencia
                        SqlHelper.ExecuteNonQuery(cnnMontaXML2, "StpNfe", ParamsCont);

                        // Gera historico
                        Observacao = "FS-DA (Formulário de Segurança)";
                        string[] ParamHistorico = {
                                                    "8",
                                                    oNfe.cNF,
                                                    "2",
                                                    null,
                                                    null,
                                                    null,
                                                    null,
                                                    null,
                                                    oNfe.Ide.tpEmis,
                                                    Observacao
                                                };
                        SqlHelper.ExecuteNonQuery(cnnMontaXML2, "StpNfe", ParamHistorico);
                    }
                    else
                    {
                        if (ConfigurationManager.AppSettings["Ambiente"].ToString() == "2") //Ambiente de HOMOLOGAÇÃO
                        {
                            if (oNfe.Ide.tpEmis == "1") // 1-Normal
                            {
                                objRecepcao4.ClientCertificates.Add(CertAssina);

                                //objRecepcao.nfeCabecMsgValue = oCabecalho.FncRetornaCabecalho2();
                                //nodeRetorno = objRecepcao.nfeAutorizacaoLote(nodeEnvio);
                                nodeRetorno = objRecepcao4.nfeAutorizacaoLote(nodeEnvio);

                                RetornoRecepcao = nodeRetorno.OuterXml;
                            }
                            else if (oNfe.Ide.tpEmis == "3") // 3 – Contingência SCAN
                            {
                                //objSCANRecepcao.ClientCertificates.Add(CertAssina);
                                //RetornoRecepcao = objSCANRecepcao.nfeRecepcaoLote(oCabecalho.FncRetornaCabecalho(), infoLote.ToString());
                            }
                        }
                        else  //Ambiente de PRODUÇÃO
                        {
                            if (oNfe.Ide.tpEmis == "1") // 1-Normal
                            {
                                objRecepcaoProducao4.ClientCertificates.Add(CertAssina);

                                //objRecepcaoProducao.nfeCabecMsgValue = oCabecalho.FncRetornaCabecalhoProd2();
                                //nodeRetorno = objRecepcaoProducao.nfeAutorizacaoLote(nodeEnvio);
                                nodeRetorno = objRecepcaoProducao4.nfeAutorizacaoLote(nodeEnvio);

                                RetornoRecepcao = nodeRetorno.OuterXml;
                            }
                            else if (oNfe.Ide.tpEmis == "3") // 3 – Contingência SCAN
                            {
                                objSVCNfeRecepcaoProducao4.ClientCertificates.Add(CertAssina);

                                //objSCANRecepcaoProducao.nfeCabecMsgValue = oCabecalho.FncRetornaCabecalhoSCAN();
                                //nodeRetorno = objSCANRecepcaoProducao.nfeRecepcaoLote2(nodeEnvio);
                                nodeRetorno = objSVCNfeRecepcaoProducao4.nfeAutorizacaoLote(nodeEnvio);

                                RetornoRecepcao = nodeRetorno.OuterXml;
                            }
                        }

                        // Grava o XML da Nfe
                        obj.Util.FncGravaXML(RetornoRecepcao, obj.PastaXMLRecepcaoRetorno + oNfe.cNF + ".xml");
                        XmlDocument objXml = new XmlDocument();
                        objXml.LoadXml(RetornoRecepcao);

                        String cStat;
                        cStat = objXml.GetElementsByTagName("cStat").Item(0).InnerXml;

                        if (cStat == "103") // Lote recebido com sucesso
                        {
                            string nRec = objXml.GetElementsByTagName("nRec").Item(0).InnerXml;
                            parametros = null;
                            parametros = new string[] { nRec.ToString(), oNfe.Ide.cNF.ToString(), oNfe.infNfe.Substring(3, oNfe.infNfe.Length - 3), "2", "null" };
                            SqlHelper.ExecuteNonQuery(cnnMontaXML2, "stpAtualizaRecNfe", parametros);

                            Observacao = "StatusNFe: 2, nRec: " + nRec.ToString() + ", cNF: " + oNfe.Ide.cNF.ToString() + ", infNfe: " + oNfe.infNfe.ToString();

                            string[] ParamHistorico = {
                                                        "8",
                                                        oNfe.cNF,
                                                        "2",
                                                        null,
                                                        null,
                                                        null,
                                                        null,
                                                        null,
                                                        oNfe.Ide.tpEmis,
                                                        Observacao
                                                    };
                            SqlHelper.ExecuteNonQuery(cnnMontaXML2, "StpNfe", ParamHistorico);
                        }
                        else if (cStat == "104")// Lote Processado - Aqui provavelmente ocorreu alguma rejeição, pois a nf-e não foi aprovada
                        {
                            // Resgata os valores do Node 1                    
                            cStat = objXml.GetElementsByTagName("cStat").Item(1).InnerXml.ToString();
                            MotivoNfe = objXml.GetElementsByTagName("xMotivo").Item(1).InnerXml.ToString();
                            dtRetorno = objXml.GetElementsByTagName("dhRecbto").Item(1).InnerXml.ToString();

                            if (cStat == "100") // Autorizado o uso da NF-e
                            {
                                StatusNfe = "3";
                                nProt = objXml.GetElementsByTagName("nProt").Item(0).InnerXml.ToString();

                                string[] ParamAtualizaNfeAprova = { "2", oNfe.cNF, StatusNfe, cStat, MotivoNfe, nProt, dtRetorno == "" ? null : dtRetorno, null, null, null };
                                SqlHelper.ExecuteNonQuery(cnnMontaXML2, "StpNfe", ParamAtualizaNfeAprova);

                                // Gera Arquivo de Distribuição                           
                                FncGeraXMLCliente(Convert.ToInt32(oNfe.cNF));

                            }
                            else //Se retorno qualquer outro cStat, a NFe foi rejeitada
                            {
                                // Atualiza o status da NFe para Rejeitada
                                StatusNfe = "4"; // Rejeitada

                                string[] ParamAtualizaNfe = { "2", oNfe.cNF, StatusNfe, cStat, MotivoNfe, nProt, dtRetorno == "" ? null : dtRetorno, null, null, null };
                                SqlHelper.ExecuteNonQuery(cnnMontaXML2, "StpNfe", ParamAtualizaNfe);
                            }
                        }
                        else
                        {
                            string xMotivo = objXml.GetElementsByTagName("xMotivo").Item(0).InnerXml;
                            parametros = null;
                            parametros = new string[] { oNfe.Ide.cNF.ToString(), xMotivo.ToString() };
                            SqlHelper.ExecuteNonQuery(cnnMontaXML2, "stpAtualizaRejeicaoNfe", parametros);
                         }
                    }

                    #endregion
                }

                DrNotas.Close();
             }
            catch (Exception ex)
            {
                 StreamWriter ArquivoErro = new StreamWriter(pastaErro + oNfe.cNF + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".txt", false, Encoding.ASCII);
                ArquivoErro.WriteLine("***************** RECEPCAO *****************");
                ArquivoErro.WriteLine("Erro gerado " + DateTime.Now.ToString());
                ArquivoErro.WriteLine("IDNota: " + oNfe.cNF);
                ArquivoErro.WriteLine(ex.Message);
                ArquivoErro.WriteLine(ex.ToString());
                ArquivoErro.WriteLine(ex.StackTrace.ToString());
                ArquivoErro.Flush();
                ArquivoErro.Close();
            }
            finally
            {
                if (!DrNotas.IsClosed)
                {
                    DrNotas.Close();
                }
                if (!Dr.IsClosed)
                {
                    Dr.Close();
                }
                if (cnnMontaXML.State == ConnectionState.Open)
                {
                    cnnMontaXML.Close();
                    cnnMontaXML.Dispose();
                }
                if (cnnMontaXML2.State == ConnectionState.Open)
                {
                    cnnMontaXML2.Close();
                    cnnMontaXML2.Dispose();
                }
            }
        }

        public void FncGeraXMLCliente(Int32 intID)
        {
            XmlDocument XmlNfe = new XmlDocument();
            XmlDocument XmlAutorizacao = new XmlDocument();
            StringBuilder infoXmlCliente = new StringBuilder();
            NFE.Objetos.NotaFiscalEletronica obj = new NFE.Objetos.NotaFiscalEletronica();
            NFE.Objetos.Ret_Recepcao.RetRecepcao objRetRecepcao = new NFE.Objetos.Ret_Recepcao.RetRecepcao();

            try
            {
                XmlNfe.Load(obj.PastaXMLRecepcao + intID + ".xml");

                String XmlNfeAss = XmlNfe.GetElementsByTagName("NFe")[0].OuterXml;

                XmlAutorizacao.Load(obj.PastaXMLRecepcaoRetorno + intID + ".xml");

                infoXmlCliente.Append("<?xml version='1.0' encoding='" + ConfigurationManager.AppSettings["CodificacaoNFE"].ToString() + "' ?>");
                infoXmlCliente.Append("<nfeProc xmlns='" + ConfigurationManager.AppSettings["NameSpaceNFE"].ToString() + "' versao='" + ConfigurationManager.AppSettings["VersaoDadosNFENova"].ToString() + "'>");
                // Nfe Assinada                
                infoXmlCliente.Append(XmlNfeAss);
                // Retorno com autorização de uso
                infoXmlCliente.Append(XmlAutorizacao.GetElementsByTagName("protNFe").Item(0).OuterXml);
                infoXmlCliente.Append("</nfeProc>");

                oUtil.FncGravaXML(infoXmlCliente, objRetRecepcao.PastaRetRecepcaoCliente + intID + ".xml");

                //FncGeraPDF(ConfigurationManager.AppSettings["PastaPDF"], intID);

            }
            catch (Exception ex)
            {
                StreamWriter ArquivoErro = new StreamWriter(pastaErro + intID.ToString() + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".txt", false, Encoding.ASCII);
                ArquivoErro.WriteLine("***************** ERRO AO GERAR ARQUIVO DO CLIENTE *****************");
                ArquivoErro.WriteLine("Erro gerado " + DateTime.Now.ToString());
                ArquivoErro.WriteLine("IDNota: " + intID);
                ArquivoErro.WriteLine(ex.Message);
                ArquivoErro.WriteLine(ex.ToString());
                ArquivoErro.Flush();
                ArquivoErro.Close();
            }
        }

        public void MontaXMLContingencia()
        {
            AssinaXML ObjAss = new AssinaXML();
            X509Certificate2 CertAssina = new X509Certificate2(ConfigurationManager.AppSettings["CaminhoCertificado"].ToString(), ConfigurationManager.AppSettings["SenhaCertificado"].ToString());
            XmlDocument XmlCont = new XmlDocument();
            SqlDataReader DRCont;
            SqlDataReader DRNota;
            string RetornoRecepcao = string.Empty;
            Int32 idNotaCont = 0;
            Conexao = FncVerificaConexao();
            SqlConnection cnnMontaXMLContingencia = new SqlConnection(Conexao);
            SqlConnection cnnMontaXMLContingencia2 = new SqlConnection(Conexao);

            string[] Params = {
                                  "6",
                                 null,       
                                 null,          
                                 null,          
                                 null,          
                                 null,    
                                 null,
                                 null,
                                 null,
                                 null
                              };

            DRCont = SqlHelper.ExecuteReader(cnnMontaXMLContingencia, "StpNfe", Params);

            if (DRCont.Read())
            {
                if (DRCont["TpEmis"].ToString() == "1") // 1-Normal
                {
                    DRNota = SqlHelper.ExecuteReader(cnnMontaXMLContingencia2, "stpRetornaNotaFiscalContingente");
                    while (DRNota.Read())
                    {
                        idNotaCont = Convert.ToInt32(DRNota["IDNota"].ToString());
                        obj.cNF = DRNota["chNfe"].ToString();

                        XmlCont.Load(obj.PastaXMLRecepcao + idNotaCont.ToString() + ".xml");

                        #region CONVERTE STRING EM XML NODE!!

                        XmlTextReader xmlReader = new XmlTextReader(new
                            StringReader(XmlCont.InnerXml));

                        // if you already have an XmlDocument then use that, otherwise
                        // create one
                        XmlDocument xmlDocument = new XmlDocument();
                        XmlNode nodeEnvio = xmlDocument.ReadNode(xmlReader);

                        #endregion

                        if (ConfigurationManager.AppSettings["Ambiente"].ToString() == "2")
                        {
                            objRecepcao4.ClientCertificates.Add(CertAssina);

                            //objRecepcao.nfeCabecMsgValue = oCabecalho.FncRetornaCabecalho2();
                            //nodeRetorno = objRecepcao.nfeAutorizacaoLote(nodeEnvio);
                            nodeRetorno = objRecepcao4.nfeAutorizacaoLote(nodeEnvio);

                            RetornoRecepcao = nodeRetorno.OuterXml;
                        }
                        else
                        {
                            objRecepcaoProducao4.ClientCertificates.Add(CertAssina);

                            //objRecepcaoProducao.nfeCabecMsgValue = oCabecalho.FncRetornaCabecalhoProd2();
                            //nodeRetorno = objRecepcaoProducao.nfeAutorizacaoLote(nodeEnvio);
                            nodeRetorno = objRecepcaoProducao4.nfeAutorizacaoLote(nodeEnvio);

                            RetornoRecepcao = nodeRetorno.OuterXml;
                        }

                        // Grava o XML da Nfe
                        obj.Util.FncGravaXML(RetornoRecepcao, obj.PastaXMLRecepcaoRetorno + idNotaCont.ToString() + ".xml");
                        XmlDocument objXml = new XmlDocument();
                        objXml.LoadXml(RetornoRecepcao);

                        string nRec = objXml.GetElementsByTagName("nRec").Item(0).InnerXml;
                        
                        string[] parametros = new string[] { 
                                                            nRec.ToString(), 
                                                            idNotaCont.ToString(), 
                                                            obj.cNF.Substring(3, obj.cNF.Length - 3), 
                                                            "2", 
                                                            "null" 
                                                            };
                        SqlHelper.ExecuteNonQuery(Conexao, "stpAtualizaRecNfe", parametros);

                        Observacao = "StatusNFe: 2, nRec: " + nRec.ToString();
                        
                        string[] ParamHistorico = {
                                                    "8",          
                                                    idNotaCont.ToString(),       
                                                    "2",          
                                                    null,          
                                                    null,          
                                                    null,    
                                                    null,
                                                    null,
                                                    DRNota["TpEmis"].ToString(),
                                                    Observacao
                                                };
                        SqlHelper.ExecuteNonQuery(Conexao, "StpNfe", ParamHistorico);
                    }
                    DRNota.Close();
                    DRNota.Dispose();

                    if (cnnMontaXMLContingencia2.State == ConnectionState.Open)
                    {
                        cnnMontaXMLContingencia2.Close();
                        cnnMontaXMLContingencia2.Dispose();
                    }
                }
            }
            DRCont.Close();
            if (cnnMontaXMLContingencia.State == ConnectionState.Open)
            {
                cnnMontaXMLContingencia.Close();
                cnnMontaXMLContingencia.Dispose();
            }
        }

        public void TesteString(string str)
        {
            str = oUtil.FUNC_CARACTER_ESPECIAL(str);
        }
    }
}
