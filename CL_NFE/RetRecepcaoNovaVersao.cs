using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using NFE.Classes.NFE.Objetos;
using NFE.Classes.Util;
using Microsoft.ApplicationBlocks.Data;
using NFE.Classes.AcessoDados;
using System.Configuration;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Data.SqlClient;
using NFE.Classes.NFE.Assinatura;
using System.Text.RegularExpressions;

namespace NFE
{
    public class RetRecepcaoNovaVersao : DB
    {
        public String Conexao;
        public String RetornoRetRecepcao;
        public String cStat;
        public String MotivoNfe;
        public String nProt;
        public String dtRetorno;
        public String IDNota;
        public String MsgError;
        public String StatusNfe;
        public String Observacao;
        protected SqlDataReader Dr;
        protected SqlDataReader DrNotas;
        protected SqlDataReader DrDadosAdd;
        protected SqlDataReader DrDest;
        protected SqlDataReader DrTrans;
        protected SqlDataReader DrCalcImpos;
        protected StringBuilder IdNota = new StringBuilder();
        protected Utils oUtil = new Utils();
        protected XmlNode nodeRetorno;
        public string XmlAss;
        protected AssinaXML objAss = new AssinaXML();

         public RetRecepcaoNovaVersao(String Cnn)
        {
            Conexao = Cnn;
        }

         public RetRecepcaoNovaVersao()
        {
            Conexao = FncVerificaConexao();
        }

        public String CarregaNumeroOF(Int64 intIDNF)
        {
            string NumeroOF = "";
            Dr = SqlHelper.ExecuteReader(Conexao, CommandType.Text, "stpNotasFiscais @intOperacao=90, @intID=" + intIDNF.ToString());

            while (Dr.Read())
            {
                if (NumeroOF == "")
                    NumeroOF += Dr["NumeroOF"].ToString();
                else
                    NumeroOF += " / " + Dr["NumeroOF"].ToString();
            }

            return NumeroOF;
        }

        public String CarregaNrPedidoCli(Int64 intIDNF)
        {
            string NrPedidoCli = "";
            Dr = SqlHelper.ExecuteReader(Conexao, CommandType.Text, "stpNotasFiscais @intOperacao=91, @intID=" + intIDNF.ToString());

            while (Dr.Read())
            {
                if (NrPedidoCli == "")
                    NrPedidoCli += Dr["PedidoCliente"].ToString();
                else
                    NrPedidoCli += " / " + Dr["PedidoCliente"].ToString();
            }

            return NrPedidoCli;
        }

        public void FncGeraXMLCliente(Int32 intID)
        {
            XmlDocument XmlNfe = new XmlDocument();
            XmlDocument XmlAutorizacao = new XmlDocument();
            XmlDocument XmlProc = new XmlDocument();
            StringBuilder infoXmlCliente = new StringBuilder();
            NFE.Classes.NFE.Objetos.NotaFiscalEletronica obj = new NFE.Classes.NFE.Objetos.NotaFiscalEletronica();
            NFE.Classes.NFE.Objetos.Ret_Recepcao.RetRecepcao objRetRecepcao = new NFE.Classes.NFE.Objetos.Ret_Recepcao.RetRecepcao();
            AssinaXML ObjAss = new AssinaXML();
            X509Certificate2 CertAssina = new X509Certificate2(ConfigurationManager.AppSettings["CaminhoCertificado"].ToString(), ConfigurationManager.AppSettings["SenhaCertificado"].ToString());

            try
            {
                XmlNfe.Load(obj.PastaXMLRecepcaoCliente + intID + ".xml");

                //String XmlNfeAss = ObjAss.FncAssinarXML(XmlNfe.OuterXml, "infNFe", CertAssina);
                String XmlNfeAss = XmlNfe.OuterXml;

                XmlAutorizacao.Load(objRetRecepcao.PastaRetRecepcao + intID + ".xml");

                infoXmlCliente.Append("<?xml version='1.0' encoding='" + ConfigurationManager.AppSettings["CodificacaoNFE"].ToString() + "' ?>");
                infoXmlCliente.Append("<nfeProc xmlns='" + ConfigurationManager.AppSettings["NameSpaceNFE"].ToString() + "' versao='" + ConfigurationManager.AppSettings["VersaoDadosNFENova"].ToString() + "'>");
                // Nfe Assinada                
                infoXmlCliente.Append(XmlNfeAss);
                // Retorno com autorização de uso
                infoXmlCliente.Append(XmlAutorizacao.GetElementsByTagName("protNFe").Item(0).OuterXml);
                infoXmlCliente.Append("</nfeProc>");

                oUtil.FncGravaXML(infoXmlCliente, objRetRecepcao.PastaRetRecepcaoCliente + intID + ".xml");

                FncGeraPDF(ConfigurationManager.AppSettings["PastaPDF"], intID);

            }
            catch (Exception ex)
            {
                StreamWriter ArquivoErro = new StreamWriter(ConfigurationManager.AppSettings["PastaErro"].ToString() + IDNota + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".txt", false, Encoding.ASCII);
                ArquivoErro.WriteLine("***************** ERRO AO GERAR ARQUIVO DO CLIENTE *****************");
                ArquivoErro.WriteLine("Erro gerado " + DateTime.Now.ToString());
                ArquivoErro.WriteLine("IDNota: " + IDNota);
                ArquivoErro.WriteLine(ex.Message);
                ArquivoErro.WriteLine(ex.ToString());
                ArquivoErro.Flush();
                ArquivoErro.Close();
            }
        }

        public void FncGeraPDF(String pathPDF, Int64 IDNF)
        {
            try
            {
                String PathLogo = ConfigurationManager.AppSettings["ImageLogo"].ToString();
                String PathImageP = ConfigurationManager.AppSettings["ImageP"].ToString();
                String PathImageB = ConfigurationManager.AppSettings["ImageB"].ToString();
                String PathImageFundo = ConfigurationManager.AppSettings["Image"].ToString();
                                
                System.Gdr7.Util.NFE objNFe = new System.Gdr7.Util.NFE();
                System.Gdr7.Util.NFE.EntityNFe Entity = new System.Gdr7.Util.NFE.EntityNFe();


                System.Gdr7.Util.NFE.EntityNFe NFe = new System.Gdr7.Util.NFE.EntityNFe();

                #region DADOS GERAIS

                string[] ParamsDrNotas = {
                                        "1",
                                        IDNF.ToString()
                                    };

                DrNotas = SqlHelper.ExecuteReader(Conexao, "stpRetornaDadosDanfe", ParamsDrNotas); // Lista Notas Fiscais

                while (DrNotas.Read())
                {
                    NFe.ChaveAcesso = DrNotas["chNfe"] is DBNull ? "" : DrNotas["chNfe"].ToString();
                    NFe.Serie = DrNotas["serie"] is DBNull ? "0" : DrNotas["serie"].ToString();
                    NFe.TipoEmissao = DrNotas["tpNF"] is DBNull ? "1" : DrNotas["tpNF"].ToString();
                    NFe.NumeroNF = DrNotas["Numero"] is DBNull ? "0" : DrNotas["Numero"].ToString();
					NFe.Transportador.FretePorConta = DrNotas["ModFrete"] is DBNull ? " " : DrNotas["ModFrete"].ToString();

                    #region ..:: IDE ::..

                    NFe.IDE.natOp = DrNotas["DescNaturezaOP"] is DBNull ? "0" : oUtil.FUNC_CARACTER_ESPECIAL(DrNotas["DescNaturezaOP"].ToString());
                    NFe.IDE.RazaoSocial = DrNotas["RazaoSocial"] is DBNull ? "0" : oUtil.FUNC_CARACTER_ESPECIAL(DrNotas["RazaoSocial"].ToString());


                    if (DrNotas["nProt"] != null)
                    {
                        NFe.IDE.Protocolo = DrNotas["nProt"] is DBNull ? "0" : DrNotas["nProt"].ToString();
                    }

                    NFe.IDE.IE = DrNotas["IE"] is DBNull ? "0" : DrNotas["IE"].ToString();
                    NFe.IDE.IESubstTrib = "";
                    NFe.IDE.CNPJ = DrNotas["CNPJ"] is DBNull ? "0" : DrNotas["CNPJ"].ToString();

                    #endregion

                    if (DrNotas["IDStatusNFe"].ToString() == "7")
                    {
                        PathImageFundo = PathImageFundo + "BackNFeCancelada.jpg";
                    }
                    else if (DrNotas["IDStatusNFe"].ToString() == "3")
                    {
                        PathImageFundo = "";
                    }
                    else {
                        PathImageFundo = PathImageFundo + "BackNFeNaoAprovada.jpg";
                    }

                }

                #endregion

                #region Destinatario

                string[] ParamsDrDest = {
                                    "2",
                                    IDNF.ToString()
                                };
                DrDest = SqlHelper.ExecuteReader(Conexao, "stpRetornaDadosDanfe", ParamsDrDest);

                if (DrDest.Read())
                {
                    NFe.Dest.RazaoSocial = DrDest["Empresa"] is DBNull ? "0" : DrDest["Empresa"].ToString();

                    if (Convert.ToString(DrDest["CNPJ"]).Length > 11)

                        NFe.Dest.CNPJ = string.Format(@"{0:00\.000\.000\/0000\-00}", DrDest["CNPJ"] is DBNull ? "0" : DrDest["CNPJ"].ToString());
                    else

                        NFe.Dest.CNPJ = string.Format(@"{0:000\.000\.000\-00}", DrDest["CNPJ"] is DBNull ? "0" : DrDest["CNPJ"].ToString());

                    NFe.Dest.DataEmissao = DrDest["DtEmissao"] is DBNull ? "0" : DrDest["DtEmissao"].ToString();
                    NFe.Dest.Endereco = DrDest["xLgr"] is DBNull ? "0" : DrDest["xLgr"].ToString();
                    NFe.Dest.Bairro = DrDest["FBairro"] is DBNull ? "0" : DrDest["FBairro"].ToString();
                    NFe.Dest.CEP = DrDest["FCEP"] is DBNull ? "0" : DrDest["FCEP"].ToString();
                    NFe.Dest.DataSaidaEntrada = "";
                    NFe.Dest.Municipio = DrDest["FMunicipio"] is DBNull ? "0" : DrDest["FMunicipio"].ToString();
                    NFe.Dest.UF = DrDest["FUF"] is DBNull ? "0" : DrDest["FUF"].ToString();

                    if (Convert.ToString(DrDest["Telefone"]).Length > 1)
                    {
                        NFe.Dest.Telefone = DrDest["Telefone"] is DBNull ? "0" : DrDest["Telefone"].ToString();
                    }

                    if (Convert.ToString(DrDest["IE"]).Length > 1)
                    {
                        NFe.Dest.IE = DrDest["IE"] is DBNull ? "0" : DrDest["IE"].ToString();
                    }

                    NFe.Dest.HoraSaida = "";
                }
                DrDest.Close();

                #endregion

                #region ..:: Cálculos de Impostos ::..

                string[] ParamsDrCalcImpos = {
                                    "3",
                                    IDNF.ToString()
                                };
                DrCalcImpos = SqlHelper.ExecuteReader(Conexao, "stpRetornaDadosDanfe", ParamsDrCalcImpos);

                if (DrCalcImpos.Read())
                {
                    NFe.CalcImposto.bcICMS = DrCalcImpos["BaseICMS"] is DBNull ? "0" : DrCalcImpos["BaseICMS"].ToString();
                    NFe.CalcImposto.ValorICMS = DrCalcImpos["ValorICMS"] is DBNull ? "0" : DrCalcImpos["ValorICMS"].ToString();

                    NFe.CalcImposto.ValorTotalProd = DrCalcImpos["ValorTotalProd"] is DBNull ? "0" : DrCalcImpos["ValorTotalProd"].ToString();
                    NFe.CalcImposto.ValorFrete = DrCalcImpos["ValorFrete"] is DBNull ? "0" : DrCalcImpos["ValorFrete"].ToString();
                    NFe.CalcImposto.ValorSeguro = DrCalcImpos["ValorSeguro"] is DBNull ? "0" : DrCalcImpos["ValorSeguro"].ToString();

                    NFe.CalcImposto.ValorIPI = DrCalcImpos["TotalIPI"] is DBNull ? "0" : DrCalcImpos["TotalIPI"].ToString();
                    NFe.CalcImposto.ValorTotalNF = DrCalcImpos["ValorTotal"] is DBNull ? "0" : DrCalcImpos["ValorTotal"].ToString();
                    NFe.CalcImposto.Desconto = DrCalcImpos["DescontoItem"] is DBNull ? "0" : DrCalcImpos["DescontoItem"].ToString();
                    NFe.CalcImposto.DespAssecorias = DrCalcImpos["DespAcessorias"] is DBNull ? "0" : DrCalcImpos["DespAcessorias"].ToString();
                }

                DrCalcImpos.Close();

                #endregion

                #region ..:: Transportador ::..


                string[] ParamsDrTrans = {
                                    "5",
                                    IDNF.ToString()
                                };
                DrTrans = SqlHelper.ExecuteReader(Conexao, "stpRetornaDadosDanfe", ParamsDrTrans);

                if (DrTrans.Read())
                {
                    NFe.Transportador.FretePorConta = DrTrans["ModFrete"] is DBNull ? "0" : DrTrans["ModFrete"].ToString();
                    NFe.Transportador.RazaoSocial = DrTrans["Razao"] is DBNull ? "0" : DrTrans["Razao"].ToString();
                    NFe.Transportador.CNPJCPF = DrTrans["CNPJ"] is DBNull ? "0" : DrTrans["CNPJ"].ToString();
                    NFe.Transportador.Endereco = DrTrans["Logradouro"] is DBNull ? "0" : DrTrans["Logradouro"].ToString();
                    NFe.Transportador.Municipio = DrTrans["Municipio"] is DBNull ? "0" : DrTrans["Municipio"].ToString();
                    NFe.Transportador.UFEndereco = DrTrans["sigla"] is DBNull ? "0" : DrTrans["sigla"].ToString();
                    NFe.Transportador.IE = DrTrans["IE"] is DBNull ? "0" : DrTrans["IE"].ToString();
                    NFe.Transportador.PlacaVeiculo = DrTrans["PlacaVeic"] is DBNull ? "0" : DrTrans["PlacaVeic"].ToString();
                    NFe.Transportador.UFVeiculo = DrTrans["UFVeiculo"] is DBNull ? "0" : DrTrans["UFVeiculo"].ToString();
                    NFe.Transportador.Quantidade = DrTrans["Quantidade"] is DBNull ? "0" : DrTrans["Quantidade"].ToString();
                    NFe.Transportador.Especie = DrTrans["Especie"] is DBNull ? "0" : DrTrans["Especie"].ToString();
                    NFe.Transportador.Marca = DrTrans["Marca"] is DBNull ? "0" : DrTrans["Marca"].ToString();
                    NFe.Transportador.PesoBruto = DrTrans["pesobruto"] is DBNull ? "0" : DrTrans["pesobruto"].ToString();
                    NFe.Transportador.PesoLiquido = DrTrans["Pesoliquido"] is DBNull ? "0" : DrTrans["Pesoliquido"].ToString();

                }

                DrTrans.Close();

                #endregion

                #region ..:: Produtos ::..
                String nmTable = "";

                Dr = SqlHelper.ExecuteReader(Conexao, "stpRetornaTotalItensNf", IDNF);

                while (Dr.Read())
                {
                    System.Gdr7.Util.NFE.Produto entityProduto = new System.Gdr7.Util.NFE.Produto();

                    entityProduto.Cod = Dr["NumeroLote"] is DBNull ? "" : Dr["NumeroLote"].ToString();
                    entityProduto.Peso = Dr["Peso"] is DBNull ? "0" : Dr["Peso"].ToString();
                    entityProduto.Descricao = Dr["Descricao"] is DBNull ? string.Empty : Dr["Descricao"].ToString();
                    entityProduto.NCM_SB = Dr["IDNCM"] is DBNull ? string.Empty : Dr["IDNCM"].ToString().PadLeft(8, '0');
                    entityProduto.CFOP = Dr["CFop"] is DBNull ? string.Empty : Dr["CFop"].ToString().PadLeft(4, '0');
                    entityProduto.UN = Dr["unidade"] is DBNull ? "UN" : Dr["unidade"].ToString();
                    entityProduto.Quantidade = Dr["Qtde"] is DBNull ? "0" : Dr["Qtde"].ToString();
                    entityProduto.ValorUnitario = Dr["Vlunitario"] is DBNull ? "0" : Dr["Vlunitario"].ToString();
                    entityProduto.ValorTotal = Dr["ValorTotal"] is DBNull ? "0" : Dr["ValorTotal"].ToString();

                    entityProduto.ValorIPI = Dr["TotalIPI"] is DBNull ? "0" : Dr["TotalIPI"].ToString();
                    entityProduto.AliqIPI = Dr["AliqIPI"] is DBNull ? "0" : Dr["AliqIPI"].ToString();

                    entityProduto.bcICMS = Dr["BaseICMS"] is DBNull ? "0" : Dr["BaseICMS"].ToString();
                    entityProduto.ValorICMS = Dr["ValorICMS"] is DBNull ? "0" : Dr["ValorICMS"].ToString();
                    entityProduto.AliqICMS = Dr["AliqICMS"] is DBNull ? "0" : Dr["AliqICMS"].ToString();
                    entityProduto.CST = Dr["CST"] is DBNull ? "" : Dr["CST"].ToString();

                    Entity.Produtos.Add(entityProduto);
                }

                Dr.Close();

                NFe.Produtos = Entity.Produtos;

                int qtdProd = NFe.Produtos.Count - 17;
                int qtdPag = qtdProd % 60 == 0 ? qtdProd / 60 : (qtdProd / 60) + 1;

                if (qtdProd < 0)
                    qtdPag = 1;


                #endregion

                #region ..:: Dados Adicionais ::..

                string[] ParamsDrDadosAdd = {
                                    "4",
                                    IDNF.ToString()
                                };
                DrDadosAdd = SqlHelper.ExecuteReader(Conexao, "stpRetornaDadosDanfe", ParamsDrDadosAdd);

                if (DrDadosAdd.Read())
                {
                    NFe.DadosAdicionais.Observacao = DrDadosAdd["Observacoes"] is DBNull ? "0" : DrDadosAdd["Observacoes"].ToString();
                }

                DrDadosAdd.Close();
                #endregion

                objNFe.GeraPDF(PathLogo, pathPDF, PathImageP, PathImageB, PathImageFundo, NFe, IDNF, 1);
                if (DrNotas.IsClosed == false) { DrNotas.Close(); }
            }
            catch (Exception ex)
            {
                StreamWriter ArquivoErro = new StreamWriter(ConfigurationManager.AppSettings["PastaErro"].ToString() + IDNota + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".txt", false, Encoding.ASCII);
                ArquivoErro.WriteLine("***************** ERRO AO GERAR PDF DO CLIENTE *****************");
                ArquivoErro.WriteLine("Erro gerado " + DateTime.Now.ToString());
                ArquivoErro.WriteLine("IDNota: " + IDNota);
                ArquivoErro.WriteLine(ex.Message);
                ArquivoErro.WriteLine(ex.ToString());
                ArquivoErro.Flush();
                ArquivoErro.Close();
            }
        }
        public void FncGeraPDF(String pathPDF, Int64 IDNF, Boolean bCanhoto)
        {
            try
            {
                String PathLogo = ConfigurationManager.AppSettings["ImageLogo"].ToString();
                String PathImageP = ConfigurationManager.AppSettings["ImageP"].ToString();
                String PathImageB = ConfigurationManager.AppSettings["ImageB"].ToString();
                String PathImageFundo = ConfigurationManager.AppSettings["Image"].ToString();

                System.Gdr7.Util.NFE objNFe = new System.Gdr7.Util.NFE();
                System.Gdr7.Util.NFE.EntityNFe Entity = new System.Gdr7.Util.NFE.EntityNFe();


                System.Gdr7.Util.NFE.EntityNFe NFe = new System.Gdr7.Util.NFE.EntityNFe();


                #region DADOS GERAIS

                string[] ParamsDrNotas = {
                                        "1",
                                        IDNF.ToString()
                                    };

                DrNotas = SqlHelper.ExecuteReader(Conexao, "stpRetornaDadosDanfe", ParamsDrNotas); // Lista Notas Fiscais

                while (DrNotas.Read())
                {
                    NFe.ChaveAcesso = DrNotas["chNfe"] is DBNull ? "" : DrNotas["chNfe"].ToString();
                    NFe.Serie = DrNotas["serie"] is DBNull ? "0" : DrNotas["serie"].ToString();
                    NFe.TipoEmissao = DrNotas["tpNF"] is DBNull ? "1" : DrNotas["tpNF"].ToString();
                    NFe.NumeroNF = DrNotas["Numero"] is DBNull ? "0" : DrNotas["Numero"].ToString();

                    #region ..:: IDE ::..

                    NFe.IDE.natOp = DrNotas["DescNaturezaOP"] is DBNull ? "0" : oUtil.FUNC_CARACTER_ESPECIAL(DrNotas["DescNaturezaOP"].ToString());
                    NFe.IDE.RazaoSocial = DrNotas["RazaoSocial"] is DBNull ? "0" : oUtil.FUNC_CARACTER_ESPECIAL(DrNotas["RazaoSocial"].ToString());


                    if (DrNotas["nProt"] != null)
                    {
                        NFe.IDE.Protocolo = DrNotas["nProt"] is DBNull ? "0" : DrNotas["nProt"].ToString();
                    }

                    NFe.IDE.IE = DrNotas["IE"] is DBNull ? "0" : DrNotas["IE"].ToString();
                    NFe.IDE.IESubstTrib = "";
                    NFe.IDE.CNPJ = DrNotas["CNPJ"] is DBNull ? "0" : DrNotas["CNPJ"].ToString();

					NFe.Transportador.FretePorConta = DrNotas["ModFrete"] is DBNull ? " " : DrNotas["ModFrete"].ToString();
                    #endregion

                    if (DrNotas["IDStatusNFe"].ToString() == "7")
                    {
                        PathImageFundo = PathImageFundo + "BackNFeCancelada.jpg";
                    }
                    else if (DrNotas["IDStatusNFe"].ToString() == "3")
                    {
                        PathImageFundo = "";
                    }
                    else
                    {
                        PathImageFundo = PathImageFundo + "BackNFeNaoAprovada.jpg";
                    }

                }

                #endregion

                #region Destinatario

                string[] ParamsDrDest = {
                                    "2",
                                    IDNF.ToString()
                                };
                DrDest = SqlHelper.ExecuteReader(Conexao, "stpRetornaDadosDanfe", ParamsDrDest);

                if (DrDest.Read())
                {
                    NFe.Dest.RazaoSocial = DrDest["Empresa"] is DBNull ? "0" : DrDest["Empresa"].ToString();

                    if (Convert.ToString(DrDest["CNPJ"]).Length > 11)

                        NFe.Dest.CNPJ = string.Format(@"{0:00\.000\.000\/0000\-00}", DrDest["CNPJ"] is DBNull ? "0" : DrDest["CNPJ"].ToString());
                    else

                        NFe.Dest.CNPJ = string.Format(@"{0:000\.000\.000\-00}", DrDest["CNPJ"] is DBNull ? "0" : DrDest["CNPJ"].ToString());

                    NFe.Dest.DataEmissao = DrDest["DtEmissao"] is DBNull ? "0" : DrDest["DtEmissao"].ToString();
                    NFe.Dest.Endereco = DrDest["xLgr"] is DBNull ? "0" : DrDest["xLgr"].ToString();
                    NFe.Dest.Bairro = DrDest["FBairro"] is DBNull ? "0" : DrDest["FBairro"].ToString();
                    NFe.Dest.CEP = DrDest["FCEP"] is DBNull ? "0" : DrDest["FCEP"].ToString();
                    NFe.Dest.DataSaidaEntrada = "";
                    NFe.Dest.Municipio = DrDest["FMunicipio"] is DBNull ? "0" : DrDest["FMunicipio"].ToString();
                    NFe.Dest.UF = DrDest["FUF"] is DBNull ? "0" : DrDest["FUF"].ToString();

                    if (Convert.ToString(DrDest["Telefone"]).Length > 1)
                    {
                        NFe.Dest.Telefone = DrDest["Telefone"] is DBNull ? "0" : DrDest["Telefone"].ToString();
                    }

                    if (Convert.ToString(DrDest["IE"]).Length > 1)
                    {
                        NFe.Dest.IE = DrDest["IE"] is DBNull ? "0" : DrDest["IE"].ToString();
                    }

                    NFe.Dest.HoraSaida = "";
                }
                DrDest.Close();

                #endregion

                #region ..:: Cálculos de Impostos ::..

                string[] ParamsDrCalcImpos = {
                                    "3",
                                    IDNF.ToString()
                                };
                DrCalcImpos = SqlHelper.ExecuteReader(Conexao, "stpRetornaDadosDanfe", ParamsDrCalcImpos);

                if (DrCalcImpos.Read())
                {
                    NFe.CalcImposto.bcICMS = DrCalcImpos["BaseICMS"] is DBNull ? "0" : DrCalcImpos["BaseICMS"].ToString();
                    NFe.CalcImposto.ValorICMS = DrCalcImpos["ValorICMS"] is DBNull ? "0" : DrCalcImpos["ValorICMS"].ToString();

                    NFe.CalcImposto.bcICMSSubst = DrCalcImpos["BaseICMSST"] is DBNull ? "0" : DrCalcImpos["BaseICMSST"].ToString();
                    NFe.CalcImposto.VAlorICMSSubst = DrCalcImpos["ValorICMSST"] is DBNull ? "0" : DrCalcImpos["ValorICMSST"].ToString();

                    NFe.CalcImposto.ValorTotalProd = DrCalcImpos["ValorTotalProd"] is DBNull ? "0" : DrCalcImpos["ValorTotalProd"].ToString();
                    NFe.CalcImposto.ValorFrete = DrCalcImpos["ValorFrete"] is DBNull ? "0" : DrCalcImpos["ValorFrete"].ToString();
                    NFe.CalcImposto.ValorSeguro = DrCalcImpos["ValorSeguro"] is DBNull ? "0" : DrCalcImpos["ValorSeguro"].ToString();

                    NFe.CalcImposto.ValorIPI = DrCalcImpos["TotalIPI"] is DBNull ? "0" : DrCalcImpos["TotalIPI"].ToString();
                    NFe.CalcImposto.ValorTotalNF = DrCalcImpos["ValorTotal"] is DBNull ? "0" : DrCalcImpos["ValorTotal"].ToString();
                    NFe.CalcImposto.Desconto = DrCalcImpos["DescontoItem"] is DBNull ? "0" : DrCalcImpos["DescontoItem"].ToString();
                    NFe.CalcImposto.DespAssecorias = DrCalcImpos["DespAcessorias"] is DBNull ? "0" : DrCalcImpos["DespAcessorias"].ToString();
                }

                DrCalcImpos.Close();

                #endregion

                #region ..:: Transportador ::..


                string[] ParamsDrTrans = {
                                    "5",
                                    IDNF.ToString()
                                };
                DrTrans = SqlHelper.ExecuteReader(Conexao, "stpRetornaDadosDanfe", ParamsDrTrans);

                if (DrTrans.Read())
                {
                    NFe.Transportador.FretePorConta = DrTrans["ModFrete"] is DBNull ? "0" : DrTrans["ModFrete"].ToString();
                    NFe.Transportador.RazaoSocial = DrTrans["Razao"] is DBNull ? "0" : DrTrans["Razao"].ToString();
                    NFe.Transportador.CNPJCPF = DrTrans["CNPJ"] is DBNull ? "0" : DrTrans["CNPJ"].ToString();
                    NFe.Transportador.Endereco = DrTrans["Logradouro"] is DBNull ? "0" : DrTrans["Logradouro"].ToString();
                    NFe.Transportador.Municipio = DrTrans["Municipio"] is DBNull ? "0" : DrTrans["Municipio"].ToString();
                    NFe.Transportador.UFEndereco = DrTrans["sigla"] is DBNull ? "0" : DrTrans["sigla"].ToString();
                    NFe.Transportador.IE = DrTrans["IE"] is DBNull ? "0" : DrTrans["IE"].ToString();
                    NFe.Transportador.PlacaVeiculo = DrTrans["PlacaVeic"] is DBNull ? "0" : DrTrans["PlacaVeic"].ToString();
                    NFe.Transportador.UFVeiculo = DrTrans["UFVeiculo"] is DBNull ? "0" : DrTrans["UFVeiculo"].ToString();
                    NFe.Transportador.Quantidade = DrTrans["Quantidade"] is DBNull ? "0" : DrTrans["Quantidade"].ToString();
                    NFe.Transportador.Especie = DrTrans["Especie"] is DBNull ? "0" : DrTrans["Especie"].ToString();
                    NFe.Transportador.Marca = DrTrans["Marca"] is DBNull ? "0" : DrTrans["Marca"].ToString();
                    NFe.Transportador.PesoBruto = DrTrans["pesobruto"] is DBNull ? "0" : DrTrans["pesobruto"].ToString();
                    NFe.Transportador.PesoLiquido = DrTrans["Pesoliquido"] is DBNull ? "0" : DrTrans["Pesoliquido"].ToString();

                }

                DrTrans.Close();

                #endregion

                #region ..:: Produtos ::..
                String nmTable = "";

                Dr = SqlHelper.ExecuteReader(Conexao, "stpRetornaTotalItensNf", IDNF);

                while (Dr.Read())
                {
                    System.Gdr7.Util.NFE.Produto entityProduto = new System.Gdr7.Util.NFE.Produto();

                    entityProduto.Cod = Dr["NumeroLote"] is DBNull ? "" : Dr["NumeroLote"].ToString();
                    entityProduto.Peso = Dr["Peso"] is DBNull ? "0" : Dr["Peso"].ToString();
                    string sInput = Dr["Descricao"] is DBNull ? string.Empty : Dr["Descricao"].ToString();
                    Regex rgx = new Regex("[^a-zA-Z0-9 -]", RegexOptions.None);
                    entityProduto.Descricao = rgx.Replace(sInput, "");
                    entityProduto.NCM_SB = Dr["IDNCM"] is DBNull ? string.Empty : Dr["IDNCM"].ToString().PadLeft(8, '0');
                    entityProduto.CFOP = Dr["CFop"] is DBNull ? string.Empty : Dr["CFop"].ToString().PadLeft(4, '0');
                    entityProduto.UN = Dr["unidade"] is DBNull ? "UN" : Dr["unidade"].ToString();
                    entityProduto.Quantidade = Dr["Qtde"] is DBNull ? "0" : Dr["Qtde"].ToString();
                    entityProduto.ValorUnitario = Dr["Vlunitario"] is DBNull ? "0" : Dr["Vlunitario"].ToString();
                    entityProduto.ValorTotal = Dr["ValorTotal"] is DBNull ? "0" : Dr["ValorTotal"].ToString();

                    entityProduto.ValorIPI = Dr["TotalIPI"] is DBNull ? "0" : Dr["TotalIPI"].ToString();
                    entityProduto.AliqIPI = Dr["AliqIPI"] is DBNull ? "0" : Dr["AliqIPI"].ToString();

                    entityProduto.bcICMS = Dr["BaseICMS"] is DBNull ? "0" : Dr["BaseICMS"].ToString();
                    entityProduto.ValorICMS = Dr["ValorICMS"] is DBNull ? "0" : Dr["ValorICMS"].ToString();
                    entityProduto.AliqICMS = Dr["AliqICMS"] is DBNull ? "0" : Dr["AliqICMS"].ToString();
                    entityProduto.CST = Dr["CST"] is DBNull ? "" : Dr["CST"].ToString();

                    Entity.Produtos.Add(entityProduto);
                }

                Dr.Close();

                NFe.Produtos = Entity.Produtos;

                int qtdProd = NFe.Produtos.Count - 17;
                int qtdPag = qtdProd % 60 == 0 ? qtdProd / 60 : (qtdProd / 60) + 1;

                if (qtdProd < 0)
                    qtdPag = 1;


                #endregion

                #region ..:: Dados Adicionais ::..

                string[] ParamsDrDadosAdd = {
                                    "4",
                                    IDNF.ToString()
                                };
                DrDadosAdd = SqlHelper.ExecuteReader(Conexao, "stpRetornaDadosDanfe", ParamsDrDadosAdd);

                if (DrDadosAdd.Read())
                {
                    NFe.DadosAdicionais.Observacao = DrDadosAdd["Observacoes"] is DBNull ? "0" : DrDadosAdd["Observacoes"].ToString();
                }

                DrDadosAdd.Close();
                #endregion
                objNFe.GeraPDF(PathLogo, pathPDF, PathImageP, PathImageB, PathImageFundo, NFe, IDNF, 1, bCanhoto);
                

            }
            catch (Exception ex)
            {
                StreamWriter ArquivoErro = new StreamWriter(ConfigurationManager.AppSettings["PastaErro"].ToString() + IDNota + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".txt", false, Encoding.ASCII);
                ArquivoErro.WriteLine("***************** ERRO AO GERAR PDF DO CLIENTE *****************");
                ArquivoErro.WriteLine("Erro gerado " + DateTime.Now.ToString());
				ArquivoErro.WriteLine("IDNota: " + IDNota);
				ArquivoErro.WriteLine("Canhoto: " + bCanhoto.ToString());
                ArquivoErro.WriteLine(ex.Message);
                ArquivoErro.WriteLine(ex.ToString());
                ArquivoErro.Flush();
                ArquivoErro.Close();
            }
        } 
        public void FncRetRecepcao()
        {
            try
            {
                NFE.Classes.NFE.Objetos.NotaFiscalEletronica obj = new NFE.Classes.NFE.Objetos.NotaFiscalEletronica();

                CL_NFE.HomologRetRecepcao2.NfeRetAutorizacao objRetRecepcaoWS = new CL_NFE.HomologRetRecepcao2.NfeRetAutorizacao();
                CL_NFE.ProducaoRetRecepcao2.NfeRetAutorizacao objRetRecepcaoProducaoWS = new CL_NFE.ProducaoRetRecepcao2.NfeRetAutorizacao();

                CL_NFE.SCANHomologRetRecepcao.NfeRetRecepcao2 objSCANRetRecepcaoWS = new CL_NFE.SCANHomologRetRecepcao.NfeRetRecepcao2();
                CL_NFE.SCANProducaoRetRecepcao.NfeRetRecepcao2 objSCANRetRecepcaoProducaoWS = new CL_NFE.SCANProducaoRetRecepcao.NfeRetRecepcao2();

                NFE.Classes.NFE.Objetos.Ret_Recepcao.RetRecepcao objRetRecepcao = new NFE.Classes.NFE.Objetos.Ret_Recepcao.RetRecepcao();
                NFE.Classes.NFE.MontaXMLNfeRetRecepcao objMontaRetRecepcao = new NFE.Classes.NFE.MontaXMLNfeRetRecepcao();

                // Homologação Autorização 4.00
                CL_NFE.HomoNFeRetAutorizacao4.NFeRetAutorizacao4 objRetRecepcao4 = new CL_NFE.HomoNFeRetAutorizacao4.NFeRetAutorizacao4();

                // Homologação SVC-AN Autorização 4.00
                CL_NFE.HomoSVCNFeRetAutorizacao4.NFeRetAutorizacao4 objSVCRetRecepcaoWS4 = new CL_NFE.HomoSVCNFeRetAutorizacao4.NFeRetAutorizacao4();

                // Produção Autorização 4.00
                CL_NFE.ProdNFeRetAutorizacao4.NFeRetAutorizacao4 objRetRecepcaoProducaoWS4 = new CL_NFE.ProdNFeRetAutorizacao4.NFeRetAutorizacao4();

                // Produção SVC-AN Autorização 4.00
                CL_NFE.ProdSVCNFeRetAutorizacao4.NFeRetAutorizacao4 objSVCRetRecepcaoProducaoWS4 = new CL_NFE.ProdSVCNFeRetAutorizacao4.NFeRetAutorizacao4();

                NFE.Classes.NFE.Cabecalho objCabec = new NFE.Classes.NFE.Cabecalho();
                NFE.Classes.Util.Utils objUtil = new NFE.Classes.Util.Utils();
                NFE.Classes.NFE.Assinatura.AssinaXML objAss = new NFE.Classes.NFE.Assinatura.AssinaXML();
                CL_NFE.Classes.NFE.Email objEmail = new CL_NFE.Classes.NFE.Email();
                
                X509Certificate2 CertAssina = new X509Certificate2(ConfigurationManager.AppSettings["CaminhoCertificado"].ToString(), ConfigurationManager.AppSettings["SenhaCertificado"].ToString());
                
                DataSet Ds = new DataSet();
                DataSet Ds2 = new DataSet();
                XmlDocument XmlDoc = new XmlDocument();
                XmlDocument XmlCliente = new XmlDocument();
                Conexao = FncVerificaConexao();

                // Recupera Nfe's a serem consultadas    
                string[] ParamRetornoNfe = { "1", null, null, null, null, null, null, null, null, null };
                Ds = SqlHelper.ExecuteDataset(Conexao, "StpNfe", ParamRetornoNfe);

                // Percorre as Nfe's verificando os Status "cStat"
                foreach (DataRow Dr in Ds.Tables[0].Rows)
                {
                    try
                    {
                        IDNota = Dr["ID"].ToString();

                        // Estabelece conexão com WebService "RetRecepcao" e Autentica o certificado eletronico.
                        X509Certificate Cert = new X509Certificate(objRetRecepcao.CaminhoCert, objRetRecepcao.SenhaCert);


                        #region CONVERTE STRING EM XML NODE!!

                        XmlTextReader xmlReader = new XmlTextReader(new
                            StringReader(objMontaRetRecepcao.MontaXMLRetRecepcaoNovo(Dr["nRec"].ToString())));

                        // if you already have an XmlDocument then use that, otherwise
                        // create one
                        XmlDocument xmlDocument = new XmlDocument();
                        XmlNode nodeEnvio = xmlDocument.ReadNode(xmlReader);

                        #endregion


                        // Retorno Xml para envio do RetRecepcao
                        if (ConfigurationManager.AppSettings["Ambiente"].ToString() == "2")
                        {
                            // 1-Normal 
                            // 5-Contingencia 
                            if ((Dr["tpEmis"].ToString() == "1") || (Dr["tpEmis"].ToString() == "5"))
                            {
                                // Adiciona o Certificado ao WebService RetRecepcao
                                //objRetRecepcaoWS.ClientCertificates.Add(Cert);
                                objRetRecepcao4.ClientCertificates.Add(Cert);

                                //objRetRecepcaoWS.nfeCabecMsgValue = objCabec.FncRetornaCabecalhoRet2();
                                //nodeRetorno = objRetRecepcaoWS.nfeRetAutorizacaoLote(nodeEnvio);
                                nodeRetorno = objRetRecepcao4.nfeRetAutorizacaoLote(nodeEnvio);

                                RetornoRetRecepcao = nodeRetorno.OuterXml;
                            }
                            else if (Dr["tpEmis"].ToString() == "3") // 3 – Contingência SCAN
                            {
                                // Adiciona o Certificado ao WebService RetRecepcao
                                //objSCANRetRecepcaoWS.ClientCertificates.Add(Cert);

                                //objSCANRetRecepcaoWS.nfeCabecMsgValue = objCabec.FncRetornaCabecalhoRetSCAN();
                                //nodeRetorno = objSCANRetRecepcaoWS.nfeRetRecepcao2(nodeEnvio);
                            }
                        }
                        else
                        {
                            // 1-Normal 
                            // 5-Contingencia 
                            if ((Dr["tpEmis"].ToString() == "1") || (Dr["tpEmis"].ToString() == "5"))
                            {
                                // Adiciona o Certificado ao WebService RetRecepcao
                                //objRetRecepcaoProducaoWS.ClientCertificates.Add(Cert);
                                objRetRecepcaoProducaoWS4.ClientCertificates.Add(Cert);

                                //objRetRecepcaoProducaoWS.nfeCabecMsgValue = objCabec.FncRetornaCabecalhoRetProd2();
                                nodeRetorno = objRetRecepcaoProducaoWS4.nfeRetAutorizacaoLote(nodeEnvio);

                                RetornoRetRecepcao = nodeRetorno.OuterXml;
                            }
                            else if (Dr["tpEmis"].ToString() == "3") // 3 – Contingência SCAN
                            {
                                // Adiciona o Certificado ao WebService RetRecepcao
                                //objSCANRetRecepcaoProducaoWS.ClientCertificates.Add(Cert);
                                objSVCRetRecepcaoProducaoWS4.ClientCertificates.Add(Cert);
                                
                                //objSCANRetRecepcaoProducaoWS.nfeCabecMsgValue = objCabec.FncRetornaCabecalhoRetSCAN();

                                //nodeRetorno = objSCANRetRecepcaoProducaoWS.nfeRetRecepcao2(nodeEnvio);
                                nodeRetorno = objSVCRetRecepcaoProducaoWS4.nfeRetAutorizacaoLote(nodeEnvio);

                                RetornoRetRecepcao = nodeRetorno.OuterXml;
                            }

                        }

                        XmlDoc.LoadXml(RetornoRetRecepcao);

                        // Grava arquivo de retorno
                        objUtil.FncGravaXML(objUtil.FUNC_CARACTER_ACENTO(RetornoRetRecepcao), objRetRecepcao.PastaRetRecepcao + IDNota + ".xml");

                        // Resgata os valores no Node 0
                        cStat = XmlDoc.GetElementsByTagName("cStat").Item(0).InnerXml.ToString();
                        MotivoNfe = XmlDoc.GetElementsByTagName("xMotivo").Item(0).InnerXml.ToString();

                        if (cStat == "105") // Lote em processamento
                        {
                            //O aplicativo do deverá fazer uma nova consulta
                            StatusNfe = "2"; // 2 - Remetida                            
                            MotivoNfe = "Lote em processamento";
                        }
                        else if (cStat == "106") // Lote não localizado
                        {
                            //O aplicativo do deverá fazer uma nova consulta
                            StatusNfe = "1"; // 1 - Finalizado                            
                            MotivoNfe = "Lote não localizado";
                        }
                        else if (cStat == "104")// Lote Processado
                        {
                            // Resgata os valores do Node 1                    
                            cStat = XmlDoc.GetElementsByTagName("cStat").Item(1).InnerXml.ToString();
                            MotivoNfe = XmlDoc.GetElementsByTagName("xMotivo").Item(1).InnerXml.ToString();
                        }

                        if (cStat == "100") // Autorizado o uso da NF-e
                        {
                            StatusNfe = "3"; // 3 - Aprovada
                            nProt = XmlDoc.GetElementsByTagName("nProt").Item(0).InnerXml.ToString();
                            dtRetorno = XmlDoc.GetElementsByTagName("dhRecbto").Item(0).InnerXml.ToString();

                            string[] ParamAtualizaNfeAprova = { "2", IDNota, StatusNfe, cStat, MotivoNfe, nProt, dtRetorno == "" ? null : dtRetorno, null, null, null };
                            SqlHelper.ExecuteNonQuery(Conexao, "StpNfe", ParamAtualizaNfeAprova);

                            // Gera Arquivo de Distribuição                           
                            FncGeraXMLCliente(Convert.ToInt32(IDNota));
                            
                            if ((!(Dr["EmailNFe"] is DBNull)) || (!(Dr["EmailNFe"].ToString().Trim() == "")))
                            {
                                string[] EmailNFe = new string[2];

                                EmailNFe[0] = Dr["EmailNFe"].ToString();
                                //EmailNFe[1] = Dr["EmailTransportadora"].ToString();

                                //objEmail.EnviaEmailNFe(EmailNFe, Dr["RazaoSocial"].ToString(), Dr["Numero"].ToString(), Dr["chNFe"].ToString(), Dr["ID"].ToString());
                            }
                        }
                        else
                        {
                            if (cStat == "105") // Lote em processamento
                            {
                                //O aplicativo do deverá fazer uma nova consulta
                                StatusNfe = "2"; // 2 - Remetida   
                            }
                            else if (cStat == "")
                            {
                                //O aplicativo do deverá fazer uma nova consulta
                                StatusNfe = "2"; // 2 - Remetida 
                                MotivoNfe = "";
                            }
                            else if (cStat == "204" || cStat == "539") // Rejeição por Duplicidade
                            {
                                StreamWriter ArquivoErro = new StreamWriter(ConfigurationManager.AppSettings["PastaErro"].ToString() + "Duplicidade_" + IDNota + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".txt", false, Encoding.ASCII);
                                ArquivoErro.WriteLine("*****************  DUPLICIDADE *****************");
                                ArquivoErro.WriteLine(string.Format(" Iniciando else if (cStat == '{0}') ", cStat));
                                 
                                try
                                {

                                    MotivoNfe = MotivoNfe.Replace("Rejeição:", "Rejeição - ");
                                    MotivoNfe = MotivoNfe.Replace("chNFe:", "chNFe - ");

                                    String[] arr = MotivoNfe.Split(':');
                                    String nRecDuplicidade = arr[1].ToString().Replace("]", "");

                                    //O aplicativo do deverá fazer uma nova consulta
                                    StatusNfe = "2"; // 2 - Remetida 

                                    string[] ParamAtualizaDuplicidade = { IDNota, nRecDuplicidade, StatusNfe };

                                    SqlHelper.ExecuteNonQuery(Conexao, "STP_ATUALIZA_NREC_DUPLICIDADE", ParamAtualizaDuplicidade);

                                }
                                catch (Exception exDuplicidade)
                                {
                                    ArquivoErro.WriteLine("Erro gerado " + DateTime.Now.ToString());
                                    ArquivoErro.WriteLine("IDNota: " + IDNota);
                                    ArquivoErro.WriteLine(exDuplicidade.Message);
                                    ArquivoErro.WriteLine(exDuplicidade.ToString());                                   
                                }

                                ArquivoErro.WriteLine(" Fim do else if (cStat == '204') ");

                                ArquivoErro.Flush();
                                ArquivoErro.Close();

                                continue;
                            }
                            else
                            {
                                StatusNfe = "4"; // 4 - Rejeitado
                            }
                        }

                        string[] ParamAtualizaNfe = { "2", IDNota, StatusNfe, cStat, MotivoNfe, nProt, dtRetorno == "" ? null : dtRetorno, null, null, null };
                        SqlHelper.ExecuteNonQuery(Conexao, "StpNfe", ParamAtualizaNfe);

                        if (StatusNfe == "4" || StatusNfe == "8")
                        {

                            string[] ParamRetornoDadosItens = { "9", IDNota, null, null, null, null, null, null, null, null };
                            Ds2 = SqlHelper.ExecuteDataset(Conexao, "StpNfe", ParamRetornoDadosItens);

                            // Percorre as Nfe's verificando os Status "cStat"
                            foreach (DataRow Dr2 in Ds2.Tables[0].Rows)
                            {
                                string[] ParamExcluiItens = {
                                                    "10",          
                                                    Dr2["ID"].ToString(),       
                                                    null,          
                                                    null,          
                                                    null,          
                                                    null,    
                                                    null,
                                                    null,
                                                    null,
                                                    null
                                                };
                                SqlHelper.ExecuteNonQuery(Conexao, "StpNfe", ParamExcluiItens);
                            }

                        }

                        Observacao = "StatusNFe: " + StatusNfe + "| cStat: " + cStat + "| MotivoNfe: " + MotivoNfe + "| nProt: " + nProt + "| dtRetorno: " + dtRetorno;

                        string[] ParamHistorico = {
                                                    "8",          
                                                    IDNota,       
                                                    StatusNfe,          
                                                    null,          
                                                    null,          
                                                    null,    
                                                    null,
                                                    null,
                                                    Dr["tpEmis"].ToString(),
                                                    Observacao
                                                };
                        SqlHelper.ExecuteNonQuery(Conexao, "StpNfe", ParamHistorico);

                    }
                    catch (Exception ex)
                    {
                        StreamWriter ArquivoErro = new StreamWriter(ConfigurationManager.AppSettings["PastaErro"].ToString() + IDNota + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".txt", false, Encoding.ASCII);
                        ArquivoErro.WriteLine("***************** RET RECEPCAO *****************");
                        ArquivoErro.WriteLine("Erro gerado " + DateTime.Now.ToString());
                        ArquivoErro.WriteLine("IDNota: " + IDNota);
                        ArquivoErro.WriteLine(ex.Message);
                        ArquivoErro.WriteLine(ex.ToString());
                        ArquivoErro.Flush();
                        ArquivoErro.Close();
                    }

                }
            }
            catch (Exception ex)
            {
                StreamWriter ArquivoErro = new StreamWriter(ConfigurationManager.AppSettings["PastaErro"].ToString() + "Instancias" + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".txt", false, Encoding.ASCII);
                ArquivoErro.WriteLine("***************** INSTANCIAS *****************");
                ArquivoErro.WriteLine("Erro gerado " + DateTime.Now.ToString());
                ArquivoErro.WriteLine(ex.Message);
                ArquivoErro.WriteLine(ex.ToString());
                ArquivoErro.Flush();
                ArquivoErro.Close();
            }

        }

        //public void FncConsultaCad()
        //{

        //    try
        //    {
        //        CL_NFE.ConsultaCad.CadConsultaCadastro2 objConsultaCad = new CL_NFE.ConsultaCad.CadConsultaCadastro2();
        //        NFE.Classes.NFE.Objetos.Ret_Recepcao.RetRecepcao objRetRecepcao = new NFE.Classes.NFE.Objetos.Ret_Recepcao.RetRecepcao();
        //        X509Certificate2 Cert = new X509Certificate2(objRetRecepcao.CaminhoCert, objRetRecepcao.SenhaCert);
        //        objConsultaCad.ClientCertificates.Add(Cert);
                
        //        StringBuilder XML = new StringBuilder();                
        //        XML.Append("<ConsCad versao='2.00' xmlns='http://www.portalfiscal.inf.br/nfe'>")
        //         .Append("<infCons>")
        //         .Append("<xServ>CONS-CAD</xServ>")
        //         //.Append("<UF>SP</UF>")
        //         //.Append("<IE>147125005111</IE>")
        //         .Append("<CNPJ>www</CNPJ>")
        //         .Append("</infCons>")
        //         .Append("</ConsCad>");;                

        //        XmlTextReader xmlReader = new XmlTextReader(new StringReader(XML.ToString()));
        //        XmlDocument xmlDocument = new XmlDocument();
        //        XmlNode nodeConsulta = xmlDocument.ReadNode(xmlReader);
        //        CL_NFE.ConsultaCad.nfeCabecMsg objConsulltaCadWSCab = new CL_NFE.ConsultaCad.nfeCabecMsg();
        //        XmlNode nodeRetCad;

        //        objConsulltaCadWSCab.versaoDados = "2.00";
        //        objConsulltaCadWSCab.cUF = "35";

        //        objConsultaCad.nfeCabecMsgValue = objConsulltaCadWSCab;
        //        nodeRetCad = objConsultaCad.consultaCadastro2(nodeConsulta);
        //    }
        //    catch (Exception ex)
        //    {
        //        string err = ex.Message;
        //    }
        //}
    }
}
