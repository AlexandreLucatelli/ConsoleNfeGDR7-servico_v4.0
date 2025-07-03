using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.ApplicationBlocks.Data;
using NFE.Classes.AcessoDados;
using System.Configuration;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Data.SqlClient;

namespace NFE
{
    public class CancelamentoNovaVersao : DB
    {
        protected CL_NFE.HomologCancelamento2.NfeCancelamento2 objCancelamentoWS = new CL_NFE.HomologCancelamento2.NfeCancelamento2();
        protected CL_NFE.ProducaoCancelamento2.NfeCancelamento2 objCancelamentoProducaoWS = new CL_NFE.ProducaoCancelamento2.NfeCancelamento2();

        protected CL_NFE.SCANHomologCancelamento.NfeCancelamento2 objSCANCancelamentoWS = new CL_NFE.SCANHomologCancelamento.NfeCancelamento2();
        protected CL_NFE.SCANProducaoCancelamento.NfeCancelamento2 objSCANCancelamentoProducaoWS = new CL_NFE.SCANProducaoCancelamento.NfeCancelamento2();

        protected NFE.Classes.NFE.Objetos.Cancela.Cancelamento objCancelamento = new NFE.Classes.NFE.Objetos.Cancela.Cancelamento();
        protected NFE.Classes.NFE.MontaXMLNfeCancelamento objMontaCancelamento = new NFE.Classes.NFE.MontaXMLNfeCancelamento();
        protected NFE.Classes.NFE.Cabecalho objCabec = new NFE.Classes.NFE.Cabecalho();
        protected NFE.Classes.Util.Utils objUtil = new NFE.Classes.Util.Utils();
        protected CL_NFE.Classes.NFE.Email objEmail = new CL_NFE.Classes.NFE.Email(); 
        public String Conexao;
        public String IDNota;
        public Int32 TpEmis;
        public String XmlCanc;
        public String RetornoXmlCanc;
        public String cStat;
        public String MotivoNfe;
        public String StatusNfe;
        public String MsgError;
        public String Observacao;
        public String XmlCancDist; //NEW - SALVA ARQUIVO DE DISTRIBUIÇÃO!!
        public XmlNode nodeRetorno; //NEW - GET RETORNO DO WEB SERVICE!!

        public void FncCancelamento()
        {
            DataSet Ds = new DataSet();
            XmlDocument XmlDoc = new XmlDocument();
            Conexao = FncVerificaConexao();

            // Recupera Nfe's a serem canceladas    
            string[] ParamRetornoNfeCanc = { "3", null, null, null, null, null, null, null, null, null };
            Ds = SqlHelper.ExecuteDataset(Conexao, "StpNfe", ParamRetornoNfeCanc);

            // Percorre as Nfe's verificando os Status "cStat"
            foreach (DataRow Dr in Ds.Tables[0].Rows)
            {
                try
                {
                    IDNota = Dr["ID"].ToString();
                    TpEmis = Convert.ToInt32(Dr["TpEmis"]);

                    // Autentica o certificado eletronico.
                    X509Certificate Cert = new X509Certificate(objCancelamento.CaminhoCert, objCancelamento.SenhaCert);

                    // Adiciona o Certificado ao WebService Cancelamento
                    if (ConfigurationManager.AppSettings["Ambiente"].ToString() == "2")
                    {
                        if (TpEmis == 1) // 1-Normal
                        {
                            objCancelamentoWS.ClientCertificates.Add(Cert);
                        }
                        else if (TpEmis == 3) // 3 – Contingência SCAN
                        {
                            objSCANCancelamentoWS.ClientCertificates.Add(Cert);
                        }
                    }
                    else
                    {
                        if (TpEmis == 1) // 1-Normal
                        {
                            objCancelamentoProducaoWS.ClientCertificates.Add(Cert);
                        }
                        else if (TpEmis == 3) // 3 – Contingência SCAN
                        {
                            objSCANCancelamentoProducaoWS.ClientCertificates.Add(Cert);
                        }
                    }

                    // Carrega propriedades para montagem do Xml de Cancelamento
                    objCancelamento.Id = "ID" + Dr["chNFe"].ToString();
                    objCancelamento.xServ = "CANCELAR";
                    objCancelamento.chNFe = Dr["chNFe"].ToString();
                    objCancelamento.nProt = Dr["nProt"].ToString();
                    objCancelamento.xJust = objUtil.FUNC_CARACTER_ESPECIAL(Dr["MotivoCancelamento"].ToString()).TrimEnd().TrimStart();

                    // Retorno Xml para enviar para WebService Cancelamento
                    XmlCanc = objMontaCancelamento.MontaXMLCancelamentoNovo(objCancelamento.Id, objCancelamento.xServ, objCancelamento.chNFe, objCancelamento.nProt, objCancelamento.xJust);

                    // Grava arquivo de envio para cancelamento
                    objUtil.FncGravaXML(XmlCanc, objCancelamento.PastaCancelamento + IDNota + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".xml");


                    #region CONVERTE STRING EM XML NODE!!

                    XmlTextReader xmlReader = new XmlTextReader(new
                        StringReader(XmlCanc));

                    // if you already have an XmlDocument then use that, otherwise
                    // create one
                    XmlDocument xmlDocument = new XmlDocument();
                    XmlNode nodeEnvio = xmlDocument.ReadNode(xmlReader);

                    #endregion

                    if (ConfigurationManager.AppSettings["Ambiente"].ToString() == "2")
                    {
                        if (TpEmis == 1) // 1-Normal
                        {
                            objCancelamentoWS.nfeCabecMsgValue = objCabec.FncRetornaCabecalhoCancelamento2();
                            nodeRetorno = objCancelamentoWS.nfeCancelamentoNF2(nodeEnvio);

                            RetornoXmlCanc = nodeRetorno.OuterXml;
                        }
                        else if (TpEmis == 3) // 3 – Contingência SCAN
                        {
                            //RetornoXmlCanc = objSCANCancelamentoWS.nfeCancelamentoNF(objCabec.FncRetornaCabecalhoCancelamento(), XmlCanc);
                        }
                    }
                    else
                    {
                        if (TpEmis == 1) // 1-Normal
                        {
                            objCancelamentoProducaoWS.nfeCabecMsgValue = objCabec.FncRetornaCabecalhoCancelamentoProd2();
                            nodeRetorno = objCancelamentoProducaoWS.nfeCancelamentoNF2(nodeEnvio);

                            RetornoXmlCanc = nodeRetorno.OuterXml;
                        }
                        else if (TpEmis == 3) // 3 – Contingência SCAN
                        {

                            objSCANCancelamentoProducaoWS.nfeCabecMsgValue = objCabec.FncRetornaCabecalhoCancelamentoSCAN();
                            nodeRetorno = objSCANCancelamentoProducaoWS.nfeCancelamentoNF2(nodeEnvio);

                            RetornoXmlCanc = nodeRetorno.OuterXml;
                        }
                    }

                    XmlDoc.LoadXml(RetornoXmlCanc);

                    // Grava arquivo de retorno
                    objUtil.FncGravaXML(RetornoXmlCanc, objCancelamento.PastaCancelamentoRetorno + IDNota + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".xml");

                    // Resgata os valores no Node 0
                    cStat = XmlDoc.GetElementsByTagName("cStat").Item(0).InnerXml.ToString();
                    MotivoNfe = XmlDoc.GetElementsByTagName("xMotivo").Item(0).InnerXml.ToString();

                    if (cStat == "101")
                    {
                        StatusNfe = "7"; // Cancelado
                    }
                    else
                    {
                        StatusNfe = "8"; // Rejeição do Cancelamento
                    }

                    string[] ParamAtualizaNfe = { "4", IDNota, StatusNfe, cStat, MotivoNfe, null, null, null, null, null };
                    SqlHelper.ExecuteNonQuery(Conexao, "StpNfe", ParamAtualizaNfe);


                    Observacao = "StatusNFe: " + StatusNfe + ", cStat: " + cStat + ", MotivoNfe: " + MotivoNfe;

                    string[] ParamHistorico = {
                                                "8",          
                                                IDNota,       
                                                StatusNfe,          
                                                null,          
                                                null,          
                                                null,    
                                                null,
                                                null,
                                                TpEmis.ToString(),
                                                Observacao
                                            };
                    SqlHelper.ExecuteNonQuery(Conexao, "StpNfe", ParamHistorico);

                    #region ARQUIVO DISTRIBUIÇÃO - NEW MARÇO 2010

                    // CARREGA PROPRIEDADES DE MONTAGEM ADICIONAIS - INFORMAÇÕES VINDAS DO ARQUIVO DE RETORNO!
                    try
                    {
                        objCancelamento.IdRet = XmlDoc.GetElementsByTagName("Id").Item(0).InnerXml.ToString();
                    }
                    //NÃO É SEMPRE QUE VEM A TAG ID!
                    catch { objCancelamento.IdRet = "NOT TAG"; }

                    objCancelamento.verAplic = XmlDoc.GetElementsByTagName("verAplic").Item(0).InnerXml.ToString();
                    objCancelamento.cStat = cStat;
                    objCancelamento.xMotivo = MotivoNfe;
                    objCancelamento.cUF = XmlDoc.GetElementsByTagName("cUF").Item(0).InnerXml.ToString();

                    try
                    {
                        objCancelamento.dhRecbto = XmlDoc.GetElementsByTagName("dhRecbto").Item(0).InnerXml.ToString();
                    }
                    //NÃO É SEMPRE QUE VEM A TAG ID!
                    catch { objCancelamento.dhRecbto = "NOT TAG"; }

                    try
                    {
                        objCancelamento.nProtRet = XmlDoc.GetElementsByTagName("nProt").Item(0).InnerXml.ToString();
                    }
                    //NÃO É SEMPRE QUE VEM A TAG ID!
                    catch { objCancelamento.nProtRet = "NOT TAG"; }

                    //GERA STRING ARQUIVO DE DISTRIBUIÇÃO!!
                    XmlCancDist = objMontaCancelamento.MontaXMLCancelamentoDistribuicao(objCancelamento.Id, objCancelamento.xServ, objCancelamento.chNFe, objCancelamento.nProt, objCancelamento.xJust, objCancelamento.verAplic, objCancelamento.cStat, objCancelamento.xMotivo, objCancelamento.cUF, objCancelamento.dhRecbto, objCancelamento.IdRet, objCancelamento.nProtRet);


                    //GRAVA ARQUIVO DE DISTRIBUIÇÃO!!
                    //AQUI SOMENTE GUARDA O ÚLTIMO ARQUIVO ENVIADO!!

                    //RETIRAR CONDIÇÃO DO IF QUANDO SCAN FOR VERSÃO IGUAL NFe 4.01!
                    if (TpEmis == 1)
                        objUtil.FncGravaXML(XmlCancDist, objCancelamento.PastaCancelamentoCliente + IDNota + ".xml");
					else
					    objUtil.FncGravaXML(XmlCancDist.ToString().Replace("versao='2.00'", "versao='1.07'"), objCancelamento.PastaCancelamentoCliente + IDNota + ".xml");

                    #region ..:: Envio de Email ::..

                    SqlDataReader drEmail;
                    string[] ParamEmailNfe = { "5", IDNota, null, null, null, null, null, null, null, null };
                    drEmail = SqlHelper.ExecuteReader(Conexao, "StpNfe", ParamEmailNfe);

                    if (drEmail.Read())
                    {
                        if (drEmail["EmailNFe"].ToString().Trim() != "")
                        {
                            objEmail.EnviaEmailCancelamento(drEmail["EmailNFe"].ToString(), drEmail["RazaoSocial"].ToString(), drEmail["Numero"].ToString(), drEmail["chNFe"].ToString(), IDNota);
                        }
                    }

                    #endregion

                    #endregion
                }
                catch (Exception ex)
                {
                    StreamWriter ArquivoErro = new StreamWriter(ConfigurationManager.AppSettings["PastaErro"].ToString() + IDNota + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".txt", false, Encoding.ASCII);
                    ArquivoErro.WriteLine("***************** CANCELAMENTO *****************");
                    ArquivoErro.WriteLine("Erro gerado " + DateTime.Now.ToString());
                    ArquivoErro.WriteLine("IDNota: " + IDNota);
                    ArquivoErro.WriteLine(ex.Message);
                    ArquivoErro.WriteLine(ex.ToString());
                    ArquivoErro.Flush();
                    ArquivoErro.Close();
                }
            }
        }
    }
}
