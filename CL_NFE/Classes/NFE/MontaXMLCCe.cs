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
using System.Xml;
using System.IO;
using System.Globalization;
using System.Linq;

namespace NFE.Classes.NFE
{
    public class MontaXMLCCe : DB
    {
        public string Conexao;
        public string XmlAss;
        protected Assinatura.AssinaXML objAss = new Assinatura.AssinaXML();
        protected CCe objCCe = new CCe();

        //protected CL_NFE.HomologacaoCCe.RecepcaoEvento objHomologCCeWS = new CL_NFE.HomologacaoCCe.RecepcaoEvento();
        //protected CL_NFE.ProducaoEvento.RecepcaoEvento objProdCCeWS = new CL_NFE.ProducaoEvento.RecepcaoEvento();

        protected CL_NFE.HomoNFeRecepcaoEvento4.NFeRecepcaoEvento4 objHomologCCeWS = new CL_NFE.HomoNFeRecepcaoEvento4.NFeRecepcaoEvento4();
        protected CL_NFE.ProdNFeRecepcaoEvento4.NFeRecepcaoEvento4 objProdCCeWS = new CL_NFE.ProdNFeRecepcaoEvento4.NFeRecepcaoEvento4();

        protected Util.Utils objUtil = new Util.Utils();
        protected CL_NFE.Classes.NFE.Email objEmail = new CL_NFE.Classes.NFE.Email();
        protected Cabecalho objCabec = new Cabecalho();
        System.Gdr7.Util.NFE objNFe = new System.Gdr7.Util.NFE();
        public String IDCCe;
        public String IDNFe;
        public Int32 TpEmis;
        public String XmlEvento;
        public XmlNode nodeRetorno;
        public String RetornoXmlCCe;
        public int icStat;
        public String MotivoCCe;
        public Int32 IDStatusCCe;
        public String retorno;
        public String dhRegEvento;
        public String nProt;


        /// <summary>
        /// Validação do Status de Retorno
        /// </summary>
        /// <param name="cStat">Campor do XML de retorno</param>
        /// <returns></returns>
        public Boolean ValidacaoGeral(int cStat)
        {
            Boolean blnRetValidacaoGeral = true;

            List<int> lstValidacaoGeral = new List<int>();

            //Validação Inicial da Mensagem no Web Service
            lstValidacaoGeral.Add(108); // Verifica se o Servidor de Processamento está Paralisado Momentaneamente
            lstValidacaoGeral.Add(109); // Verifica se o Servidor de Processamento está Paralisado Momentaneamente
            lstValidacaoGeral.Add(214); // Tamanho do XML de Dados superior a 500 KB

            // Validação das informações de controle da chamada ao Web Service
            lstValidacaoGeral.Add(242); // Elemento nfeCabecMsg inexistente no SOAP Header
            lstValidacaoGeral.Add(409); // Campo cUF inexistente no elemento nfeCabecMsg do SOAP Header
            lstValidacaoGeral.Add(410); // Verificar se a UF informada no campo cUF é atendida pelo Web Service
            lstValidacaoGeral.Add(411); // Campo versaoDados inexistente no elemento nfeCabecMsg do SOAP Header
            lstValidacaoGeral.Add(238); // Versão dos Dados informada é superior à versão vigente
            lstValidacaoGeral.Add(239); // Versão dos Dados não suportada

            // Validação do Registro de Eventos – Regras de Negócios – parte Geral
            lstValidacaoGeral.Add(252); // Tipo do ambiente difere do ambiente do Web Service
            lstValidacaoGeral.Add(250); // Código do órgão de recepção do Evento da UF diverge da solicitada
            lstValidacaoGeral.Add(489); // CNPJ do autor do evento informado inválido (DV ou zeros)
            lstValidacaoGeral.Add(490); // CPF do autor do evento informado inválido (DV ou zeros)
            lstValidacaoGeral.Add(572); // Validar se atributo Id corresponde à concatenação dos campos evento (“ID” + tpEvento + chNFe + nSeqEvento)
            lstValidacaoGeral.Add(494); // Chave de Acesso inexistente para o tpEvento que exige a existência da NF-e
            lstValidacaoGeral.Add(573); // Verificar duplicidade do evento (tpEvento + chNFe + nSeqEvento)
            lstValidacaoGeral.Add(574); // Se evento do emissor verificar se CNPJ do Autor diferente do CNPJ base da chave de acesso da NF-e
            lstValidacaoGeral.Add(575); // Se evento do destinatário verificar se CNPJ do Autor diferente do CNPJ base do destinatário da NF-e
            lstValidacaoGeral.Add(576); // Se evento do Fisco/RFB/Outros órgãos, verificar se CNPJ do Autor consta da tabela de órgãos autorizados a gerar evento
            lstValidacaoGeral.Add(577); // Data do evento não pode ser menor que a data de emissão da NF-e, se existir
            lstValidacaoGeral.Add(578); // Data do evento não pode ser maior que a data de processamento
            lstValidacaoGeral.Add(579); // Data do evento não pode ser menor que a data de autorização para NF-e não emitida em contingência se a NF-e existir.

            // Validação da área de dados da mensagem
            lstValidacaoGeral.Add(225); // Verifica Schema XML da Área de Dados
            lstValidacaoGeral.Add(516); // Em caso de Falha de Schema, verificar se existe a tag raiz esperada para o lote
            lstValidacaoGeral.Add(517); // Em caso de Falha de Schema, verificar se existe o atributo versao para a tag raiz da mensagem
            lstValidacaoGeral.Add(545); // Em caso de Falha de Schema, verificar se o conteúdo do atributo versao difere do conteúdo da versaoDados informado no SOAPHeader
            lstValidacaoGeral.Add(587); // Verifica a existência de qualquer namespace diverso do namespace padrão da NF-e (http://www.portalfiscal.inf.br/nfe)
            lstValidacaoGeral.Add(588); // Verifica a existência de caracteres de edição no início ou fim da mensagem ou entre as tags
            lstValidacaoGeral.Add(404); // Verifica o uso de prefixo no namespace
            lstValidacaoGeral.Add(402); // XML utiliza codificação diferente de UTF-8

            // Validação do evento
            lstValidacaoGeral.Add(491); // Verifica se o tpEvento é válido
            lstValidacaoGeral.Add(492); // Verifica se o verEvento é válido
            lstValidacaoGeral.Add(493); // Verifica se o detEvento atende o respectivo schema XML

            // Validação do Registro de Eventos – Regras de Negócios específica
            lstValidacaoGeral.Add(580); // Verificar se a NF-e está autorizada (não pode estar cancelada nem denegada)
            lstValidacaoGeral.Add(501); // Verificar NF-e autorizada há mais de 30 dias (720) horas
            lstValidacaoGeral.Add(594); // Verificar o sequencial do evento (HP15 - nSeqEvento) é valor válido (1-20)


            blnRetValidacaoGeral = lstValidacaoGeral.Contains(cStat);

            return blnRetValidacaoGeral;
        }

        /// <summary>
        /// Tipo de Ambiente
        /// </summary>
        private enum tpAmb
        {
            eHomologacao = 2,
            eProducao = 1
        }

        /// <summary>
        /// Tipo de Emissão
        /// </summary>
        private enum tpEmis
        {
            eNormal = 1,
            eContingenciaSCAN = 3
        }

        /// <summary>
        /// Status do Retorno
        /// </summary>
        private enum cStat
        {
            eLoteEventoProcessado = 128,
            eEventoRegistradoeVinculadoNFe = 135,
            eEventoregistradoMasNãoVinculadoNFe = 136
        }

        public String ValidaSchemaXML(string sPathXml, string sPathSchema)
        {
            System.Gdr7.Util.NFE objNFe = new System.Gdr7.Util.NFE();

            objNFe.OnValidate += new System.Gdr7.Util.NFE.ValidationCallBack(objNFe_OnValidate);

            //Validação com Schema                   
            objNFe.ValidaNFE(sPathXml, sPathSchema);

            return retorno;
        }



        void objNFe_OnValidate(object sender, string erro)
        {
            retorno = erro;
        }
        /// <summary>
        /// Este metodo é responsável por montar o xml de evento e assina-lo.
        /// </summary>
        /// <param name="oCCe"></param>
        /// <returns></returns>
        private String MontaXMLEvento(CCe oCCe)
        {
            StringBuilder XML = new StringBuilder();
            XML.Append("<evento versao='1.00' xmlns='http://www.portalfiscal.inf.br/nfe'>");
            XML.Append(String.Format("<infEvento Id='{0}'>", oCCe.Id));
            XML.Append(String.Format("<cOrgao>{0}</cOrgao>", oCCe.cOrgao));
            XML.Append(String.Format("<tpAmb>{0}</tpAmb>", oCCe.tpAmb));

            if (oCCe.CNPJ_CPF.Length > 11)
                XML.Append(String.Format("<CNPJ>{0}</CNPJ>", oCCe.CNPJ_CPF));
            else
                XML.Append(String.Format("<CPF>{0}</CPF>", oCCe.CNPJ_CPF));

            XML.Append(String.Format("<chNFe>{0}</chNFe>", oCCe.chNFe));
            XML.Append(String.Format("<dhEvento>{0}</dhEvento>", oCCe.dhEvento));
            XML.Append(String.Format("<tpEvento>{0}</tpEvento>", oCCe.tpEvento));
            XML.Append(String.Format("<nSeqEvento>{0}</nSeqEvento>", oCCe.nSeqEvento));
            XML.Append(String.Format("<verEvento>{0}</verEvento>", oCCe.verEvento.ToString().Replace(",", ".")));
            XML.Append("<detEvento versao='1.00'>");
            XML.Append(String.Format("<descEvento>{0}</descEvento>", oCCe.descEvento));
            XML.Append(String.Format("<xCorrecao>{0}</xCorrecao>", objUtil.FUNC_CARACTER_ESPECIAL(oCCe.xCorrecao).TrimEnd().TrimStart()));
            XML.Append(String.Format("<xCondUso>{0}</xCondUso>", oCCe.xCondUso));
            XML.Append("</detEvento>");
            XML.Append("</infEvento>");
            XML.Append("</evento>");

            // Autentica o certificado eletronico.
            X509Certificate2 Cert = new X509Certificate2(objCCe.CaminhoCert, objCCe.SenhaCert);

            // Assina o xml
            XmlAss = objAss.FncAssinarXML(XML.ToString(), "infEvento", Cert);

            return XmlAss;
        }

        /// <summary>
        /// Monta o XML com o Arquivo de Envio e o de Retorno para armazenamento
        /// </summary>
        /// <param name="envEvento">XML de Envio</param>
        /// <param name="retEvento">XML de Retorno</param>
        /// <returns></returns>
        private String MontaXMLProcEvento(String envEvento, String retEvento)
        {
            StringBuilder XML = new StringBuilder();
            XML.Append("<procEventoNFe exmlns='http://www.portalfiscal.inf.br/nfe' versao='1.00'>");
            XML.Append(envEvento);
            XML.Append(retEvento);
            XML.Append("</procEventoNFe>");

            return XML.ToString();
        }

        private String MontaXML(Int64 iIDNF)
        {
            DataSet DSCCE = new DataSet();
            StringBuilder XML = new StringBuilder();
            Conexao = FncVerificaConexao();
            SqlConnection cnnMontaXML = new SqlConnection(Conexao);

            //infoLote.Append("<?xml version='1.0' encoding='" + ConfigurationManager.AppSettings["CodificacaoNFE"].ToString() + "' ?>");
            XML.Append("<envEvento versao='1.00'  xmlns='http://www.portalfiscal.inf.br/nfe'>");
            XML.Append(String.Format("<idLote>{0}</idLote>", iIDNF.ToString().PadLeft(15, '0')));

            // Recupera as CCe's a serem Enviadas    
            string[] ParamCCe = { "1", null, iIDNF.ToString(), null, null, null, null, null };
            DSCCE = SqlHelper.ExecuteDataset(cnnMontaXML, "StpCCe", ParamCCe);

            // Percorre as CCe's
            foreach (DataRow Dr in DSCCE.Tables[0].Rows)
            {
                #region ..:: Carrega propriedades ::..

                objCCe.Id = "ID" + objCCe.tpEvento + Dr["chNFe"].ToString() + Dr["nSeqEvento"].ToString().PadLeft(2, '0');
                objCCe.cOrgao = Convert.ToInt32(Dr["cOrgao"]);
                objCCe.CNPJ_CPF = Dr["CNPJ"].ToString();
                objCCe.chNFe = Dr["chNFe"].ToString();
                objCCe.nSeqEvento = Convert.ToInt32(Dr["nSeqEvento"]);
                objCCe.xCorrecao = Dr["TodasCorrecoes"].ToString();

                #endregion

                XML.Append(MontaXMLEvento(objCCe));
            }

            XML.Append("</envEvento>");
            DSCCE.Clear();
            DSCCE.Dispose();
            if (cnnMontaXML.State == ConnectionState.Open)
                cnnMontaXML.Close();

            return XML.ToString();
        }

        private void MontaCCe(Int64 iIDNF)
        {
            XmlDocument XmlDoc = new XmlDocument();
            Conexao = FncVerificaConexao();
            SqlConnection cnnMontaCCe = new SqlConnection(Conexao);

            try
            {
                #region ..:: Certificado ::...

                // Autentica o certificado eletronico.
                X509Certificate Cert = new X509Certificate(objCCe.CaminhoCert, objCCe.SenhaCert);

                // Adiciona o Certificado ao WebService CCe
                if (objCCe.tpAmb == (int)tpAmb.eHomologacao)
                {
                    objHomologCCeWS.ClientCertificates.Add(Cert);
                }
                else if (objCCe.tpAmb == (int)tpAmb.eProducao)
                {
                    objProdCCeWS.ClientCertificates.Add(Cert);
                }

                #endregion

                #region ..:: Monta Xml do Evento para enviar para WebService CCe ::..

                XmlEvento = MontaXML(iIDNF);

                // Grava arquivo de envio do Evento
                String NomeArquivo = iIDNF + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".xml";
                objUtil.FncGravaXML(XmlEvento, objCCe.PastaCCe + NomeArquivo);

                #endregion

                #region ..:: Validação com Schema ::..

                String RetValidacao = ValidaSchemaXML(objCCe.PastaCCe + NomeArquivo, ConfigurationManager.AppSettings["PathSchemaEvento"].ToString());

                if (!string.IsNullOrEmpty(RetValidacao))
                {
                    string[] ParamValidacaoCCe = { "4", null, iIDNF.ToString(), ((int)CCe.StatusCCe.eRejeitada).ToString(), null, RetValidacao, null, null };
                    SqlHelper.ExecuteNonQuery(cnnMontaCCe, "StpCCe", ParamValidacaoCCe);

                    return;
                }

                #endregion

                #region ..:: Convert a String em XML Node ::..

                XmlTextReader xmlReader = new XmlTextReader(new StringReader(XmlEvento));
                XmlDocument xmlDocument = new XmlDocument();
                XmlNode nodeEnvio = xmlDocument.ReadNode(xmlReader);

                #endregion

                #region ..:: Consome WS confrome Ambiente e manipula retorno ::..

                if (objCCe.tpAmb == (int)tpAmb.eHomologacao)
                {
                    //objHomologCCeWS.nfeCabecMsgValue = objCabec.FncRetornaCabecalhoCCeHomolog();
                    nodeRetorno = objHomologCCeWS.nfeRecepcaoEvento(nodeEnvio);

                    RetornoXmlCCe = nodeRetorno.OuterXml;
                }
                else
                {
                    //objProdCCeWS.nfeCabecMsgValue = objCabec.FncRetornaCabecalhoCCeProd();
                    nodeRetorno = objProdCCeWS.nfeRecepcaoEvento(nodeEnvio);

                    RetornoXmlCCe = nodeRetorno.OuterXml;
                }

                XmlDoc.LoadXml(RetornoXmlCCe);

                // Grava arquivo de retorno
                objUtil.FncGravaXML(RetornoXmlCCe, objCCe.PastaCCeRetorno + iIDNF + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".xml");

                // Resgata os valores no Node do Lote
                icStat = Convert.ToInt32(XmlDoc.GetElementsByTagName("cStat").Item(0).InnerXml.ToString());
                MotivoCCe = XmlDoc.GetElementsByTagName("xMotivo").Item(0).InnerXml.ToString();

                if (icStat == (int)cStat.eEventoRegistradoeVinculadoNFe)
                {
                    IDStatusCCe = (int)CCe.StatusCCe.eAprovada;
                }
                else if (icStat == (int)cStat.eLoteEventoProcessado)
                {
                    IDStatusCCe = (int)CCe.StatusCCe.eAprovada;
                }
                else if (icStat == (int)cStat.eEventoregistradoMasNãoVinculadoNFe)
                {
                    IDStatusCCe = (int)CCe.StatusCCe.eAprovada;
                }
                else
                {
                    IDStatusCCe = (int)CCe.StatusCCe.eRejeitada;
                }

                if (IDStatusCCe == (int)CCe.StatusCCe.eAprovada)
                {
                    // Resgata os valores no Node do Evento
                    icStat = Convert.ToInt32(XmlDoc.GetElementsByTagName("cStat").Item(1).InnerXml.ToString());
                    MotivoCCe = XmlDoc.GetElementsByTagName("xMotivo").Item(1).InnerXml.ToString();
                    try
                    {
                        nProt = XmlDoc.GetElementsByTagName("nProt").Item(0).InnerXml.ToString();
                    }
                    catch
                    {
                        nProt = "";
                    }
                    if (ValidacaoGeral(icStat))
                    {
                        IDStatusCCe = (int)CCe.StatusCCe.eRejeitada;
                    }
                }

                dhRegEvento = IDStatusCCe == (int)CCe.StatusCCe.eAprovada ? XmlDoc.GetElementsByTagName("dhRegEvento").Item(0).InnerXml.ToString() : "";

                string[] ParamAtualizaStatusCCe = { "4", null, iIDNF.ToString(), IDStatusCCe.ToString(), icStat.ToString(), MotivoCCe, dhRegEvento, nProt };
                SqlHelper.ExecuteNonQuery(cnnMontaCCe, "StpCCe", ParamAtualizaStatusCCe);

                #endregion

                #region ..:: Monta XML no modelo ProcEvento ::..

                if (IDStatusCCe == (int)CCe.StatusCCe.eAprovada)
                {
                    SqlDataReader DR;
                    String RetornoXmlProcEvento = this.MontaXMLProcEvento(XmlEvento, RetornoXmlCCe);

                    // Grava arquivo de retorno
                    objUtil.FncGravaXML(RetornoXmlProcEvento, objCCe.PastaCCeCliente + iIDNF + ".xml");

                    //DR = SqlHelper.ExecuteReader(cnnMontaCCe, CommandType.Text, String.Format("EXEC stpNotasFiscais @intOperacao=1, @intID={0}", iIDNF.ToString()));

                    //if (DR.Read())
                    //{
                    //    // Email Destinatário
                    //    if (DR["EmailNfe"].ToString() != "")
                    //        objEmail.EnviaEmailCCe(DR["EmailNfe"].ToString(), DR["RazaoSocial"].ToString(), DR["NumeroNF"].ToString(), DR["ChaveNFE"].ToString(), iIDNF.ToString());

                    //    // Email Transportadora
                    //    if (DR["EmailTransportadora"].ToString() != "")
                    //        objEmail.EnviaEmailCCe(DR["EmailTransportadora"].ToString(), DR["RazaoSocial"].ToString(), DR["NumeroNF"].ToString(), DR["ChaveNFE"].ToString(), iIDNF.ToString());

                    //}
                    //DR.Close();
                }

                #endregion
            }
            catch (Exception ex)
            {
                using (StreamWriter ArquivoErro = new StreamWriter(ConfigurationManager.AppSettings["PastaErro"].ToString() + IDCCe + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".txt", false, Encoding.ASCII))
                {
                    ArquivoErro.WriteLine("***************** CC-e *****************");
                    ArquivoErro.WriteLine("Erro gerado " + DateTime.Now.ToString());
                    ArquivoErro.WriteLine("IDNF: " + iIDNF.ToString());
                    ArquivoErro.WriteLine(ex.Message);
                    ArquivoErro.WriteLine(ex.ToString());
                }
            }
            finally
            {
                if (cnnMontaCCe.State == ConnectionState.Open)
                {
                    cnnMontaCCe.Close();
                    cnnMontaCCe.Dispose();
                }
            }
        }

        public void FncCCe()
        {
            DataSet DSNF = new DataSet();
            Conexao = FncVerificaConexao();
            SqlConnection cnnFncCCe = new SqlConnection(Conexao);

            // Recupera as NFs que tem CCe's a serem Enviadas    
            string[] ParamCCe = { "3", null, null, null, null, null, null, null };
            DSNF = SqlHelper.ExecuteDataset(cnnFncCCe, "StpCCe", ParamCCe);

            // Percorre as NFe's
            foreach (DataRow Dr in DSNF.Tables[0].Rows)
            {
                MontaCCe(Convert.ToInt64(Dr["IDNF"].ToString()));
            }
            DSNF.Clear();
            DSNF.Dispose();
            if (cnnFncCCe.State == ConnectionState.Open)
                cnnFncCCe.Close();
        }

    }
}
