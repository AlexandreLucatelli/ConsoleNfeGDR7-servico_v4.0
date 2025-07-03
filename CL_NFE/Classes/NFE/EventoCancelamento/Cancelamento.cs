using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Data.SqlClient;
using NFE.Classes.NFE.Assinatura;
using System.IO;
using NFE.Classes.Util;
using NFE.Classes.AcessoDados;
using System.Configuration;

namespace CL_NFE.Classes.EventoCancelamento.EventoCancelamento
{
    public class Cancelamento : DB
    {
        #region ..:: Padronização ::..

        System.Gdr7.DBClass.Classes.DB Db = null;
        System.Gdr7.DBClass.Classes.Parameter clParameter;
               
        public Cancelamento(string sConnectionString)
        {
            this.Db = new System.Gdr7.DBClass.Classes.DB(sConnectionString);
            this.clParameter = new System.Gdr7.DBClass.Classes.Parameter();
        }

        public Cancelamento()
        {
            this.Db = new System.Gdr7.DBClass.Classes.DB(FncVerificaConexao());
            this.clParameter = new System.Gdr7.DBClass.Classes.Parameter();
        }
        
        #endregion

        #region ..:: Cabeçalhos do Evento ::..

        private HomologacaoEvento.nfeCabecMsg RetornaCabecalhoEventoHomolog()
        {
            CL_NFE.HomologacaoEvento.nfeCabecMsg objHomologEventoCab = new CL_NFE.HomologacaoEvento.nfeCabecMsg();
            objHomologEventoCab.versaoDados = "1.00";
            objHomologEventoCab.cUF = "35";

            return objHomologEventoCab;
        }

        private ProducaoEvento.nfeCabecMsg RetornaCabecalhoEventoProducao()
        {
            CL_NFE.ProducaoEvento.nfeCabecMsg objProducaoEventoCab = new CL_NFE.ProducaoEvento.nfeCabecMsg();

            objProducaoEventoCab.versaoDados = "1.00";
            objProducaoEventoCab.cUF = "35";

            return objProducaoEventoCab;
        }

        #endregion
        
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
            lstValidacaoGeral.Add(109); // Verifica se o Servidor de Processamento está Paralisado sem Previsão
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
            lstValidacaoGeral.Add(236); // Chave de Acesso com dígito verificador inválido
            lstValidacaoGeral.Add(614); // Chave de Acesso inválida (Código UF inválido)
            lstValidacaoGeral.Add(615); // Chave de Acesso inválida (Ano < 05 ou Ano maior que Ano corrente)
            lstValidacaoGeral.Add(616); // Chave de Acesso inválida (Mês = 0 ou Mês > 12)
            lstValidacaoGeral.Add(617); // Chave de Acesso inválida (CNPJ zerado ou dígito inválido)
            lstValidacaoGeral.Add(618); // Chave de Acesso inválida (modelo diferente de 55)
            lstValidacaoGeral.Add(619); // Chave de Acesso inválida (número NF = 0)
            lstValidacaoGeral.Add(572); // Validar se atributo Id corresponde à concatenação dos campos evento (“ID” + tpEvento + chNFe + nSeqEvento)
            lstValidacaoGeral.Add(494); // Acesso BD NFE (Chave: CNPJ Emitente, Modelo, Série e Nro):
                                        //- Chave Acesso inexistente para o tpEvento que exige a existência da NF-e
                                        //Obs.: Para o evento de cancelamento da NF-e (tpEvento=110111)
                                        //concatenar a Chave de Acesso na descrição do erro, caso o CNPJ-Base
            lstValidacaoGeral.Add(573); // Acesso BD de Eventos: - Verificar duplicidade do evento (tpEvento + chNFe + nSeqEvento)
            lstValidacaoGeral.Add(574); // Se evento do emissor verificar se CNPJ do Autor diferente do CNPJ base da Chave de Acesso da NF-e
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
            lstValidacaoGeral.Add(266); // Campo serie – na autorização pela SEFAZ Autorizadora: não aceitar série diferente de 0-899
            lstValidacaoGeral.Add(503); // Campo serie – na autorização pelo SCAN: não aceitar série diferente de 900-999
            lstValidacaoGeral.Add(203); // Acesso Cadastro Contribuinte: - Verificar Emitente não autorizado a emitir NF-e
            lstValidacaoGeral.Add(240); // - Verificar Situação Fiscal irregular do Emitente
            lstValidacaoGeral.Add(580); // Verificar se a NF-e está autorizada (não pode estar cancelada nem denegada)
            lstValidacaoGeral.Add(501); // Verificar NF-e autorizada há mais de 30 dias (720) horas
            lstValidacaoGeral.Add(594); // Verificar o sequencial do evento (HP15 - nSeqEvento) é valor válido (1-20)
            lstValidacaoGeral.Add(222); // Verificar se o número Protocolo informado difere do nro. Protocolo da NF-e
            lstValidacaoGeral.Add(221); // Verificar recebimento da NF-e pelo Destinatário (implementação futura)
            lstValidacaoGeral.Add(219); // Acesso Registro de Passagem: - Verificar registro de Circulação de Mercadoria
            lstValidacaoGeral.Add(642); // - Falha na consulta do Registro de Passagem

            lstValidacaoGeral.Add(213); // - CNPJ-Base do Autor da mensagem difere do CNPJ-Base do Certificado Digital

            blnRetValidacaoGeral = lstValidacaoGeral.Contains(cStat);

            return blnRetValidacaoGeral;
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

        /// <summary>
        /// Método responsável por listar todas as NFs com Status Aguardando Cancelamento
        /// </summary>
        /// <returns>DataSet</returns>
        private System.Data.DataSet ListaNFAguardandoCancelamento()
        {
            this.clParameter = new System.Gdr7.DBClass.Classes.Parameter();

            String sSQL = "StpEventoCancelamento";
            this.clParameter.Add("@Operacao", 1, typeof(Int64));
			//string[] ParamRetornoNfeCanc = { "3", null, null, null, null, null, null, null, null, null };
			//Ds = SqlHelper.ExecuteDataset(Conexao, "StpNfe", ParamRetornoNfeCanc);

            return this.Db.ExecuteProcedureWithReturn(sSQL, this.clParameter, CommandType.StoredProcedure);
        }

        /// <summary>
        /// Método responsável por montar o XML do Evento de Cancelamento
        /// </summary>
        /// <param name="Entity">Entidade com propriedades do Evento de Cancelamento</param>
        /// <returns>Retorna o XML no Tipo String</returns>
        private String MontaXMLEvento(EntityEventoCancelamento Entity)
        {
            StringBuilder XML = new StringBuilder();
            XML.Append("<evento versao='1.00' xmlns='http://www.portalfiscal.inf.br/nfe'>");
            XML.Append(String.Format("<infEvento Id='{0}'>", Entity.Id));
            XML.Append(String.Format("<cOrgao>{0}</cOrgao>", Entity.cOrgao));
            XML.Append(String.Format("<tpAmb>{0}</tpAmb>", Entity.tpAmb));

            if (Entity.CNPJ_CPF.Length > 11)
                XML.Append(String.Format("<CNPJ>{0}</CNPJ>", Entity.CNPJ_CPF));
            else
                XML.Append(String.Format("<CPF>{0}</CPF>", Entity.CNPJ_CPF));

            XML.Append(String.Format("<chNFe>{0}</chNFe>", Entity.chNFe));
            XML.Append(String.Format("<dhEvento>{0}</dhEvento>", Entity.dhEvento));
            XML.Append(String.Format("<tpEvento>{0}</tpEvento>", Entity.tpEvento));
            XML.Append(String.Format("<nSeqEvento>{0}</nSeqEvento>", Entity.nSeqEvento));
            XML.Append(String.Format("<verEvento>{0}</verEvento>", Entity.verEvento.ToString().Replace(",", ".")));
            XML.Append("<detEvento versao='1.00'>");

            XML.Append(String.Format("<descEvento>{0}</descEvento>", Entity.descEvento));
            XML.Append(String.Format("<nProt>{0}</nProt>", Entity.nProt));
            XML.Append(String.Format("<xJust>{0}</xJust>", Entity.xJust));

            XML.Append("</detEvento>");
            XML.Append("</infEvento>");
            XML.Append("</evento>");

            // Autentica o certificado eletronico.
            X509Certificate2 Cert = new X509Certificate2(Entity.CaminhoCert, Entity.SenhaCert);

            // Assina o xml
            AssinaXML objAss = new AssinaXML();
            String XmlAss = objAss.FncAssinarXML(XML.ToString(), "infEvento", Cert);

            StringBuilder XMLenvEvento = new StringBuilder();
            XMLenvEvento.Append("<envEvento versao='1.00'  xmlns='http://www.portalfiscal.inf.br/nfe'>");
            XMLenvEvento.Append(String.Format("<idLote>{0}</idLote>", Entity.IDNF.ToString().PadLeft(15, '0')));

            XMLenvEvento.Append(XmlAss);

            XMLenvEvento.Append("</envEvento>");

            return XMLenvEvento.ToString();
        }

        /// <summary>
        /// Método responsável por efetuar a comunicação com webservice
        /// </summary>
        /// <param name="Entity">Entidade com propriedades do Evento de Cancelamento</param>
        private void EnviaWebServiceEvento(EntityEventoCancelamento Entity)
        {
            X509Certificate Cert = new X509Certificate(Entity.CaminhoCert, Entity.SenhaCert);

            //HomologacaoEvento.RecepcaoEvento objHomologEventoWS = new HomologacaoEvento.RecepcaoEvento();
            //ProducaoEvento.RecepcaoEvento objProdEventoWS = new ProducaoEvento.RecepcaoEvento();

            HomoNFeRecepcaoEvento4.NFeRecepcaoEvento4 objHomologEventoWS = new HomoNFeRecepcaoEvento4.NFeRecepcaoEvento4();
            ProdNFeRecepcaoEvento4.NFeRecepcaoEvento4 objProdEventoWS = new ProdNFeRecepcaoEvento4.NFeRecepcaoEvento4(); 

            Utils objUtil = new Utils();
            XmlNode nodeRetorno;
            String RetornoXmlEvento = "";
            String XMLEvento = "";
            XmlDocument XmlDoc = new XmlDocument();

            #region ..:: Certificado ::...  

            // Adiciona o Certificado ao WebService
            if (Entity.tpAmb == (int)EntityEventoCancelamento.eTipoAmbiente.eHomologacao)
            {
                objHomologEventoWS.ClientCertificates.Add(Cert);
            }
            else if (Entity.tpAmb == (int)EntityEventoCancelamento.eTipoAmbiente.eProducao)
            {
                objProdEventoWS.ClientCertificates.Add(Cert);
            }

            #endregion

            #region ..:: Monta Xml do Evento para enviar para WebService ::..

            XMLEvento = MontaXMLEvento(Entity);

            #endregion
                      
            XmlTextReader xmlReader = new XmlTextReader(new StringReader(XMLEvento));
            XmlDocument xmlDocument = new XmlDocument();
            XmlNode nodeEnvio = xmlDocument.ReadNode(xmlReader);
                        
            if (Entity.tpAmb == (int)EntityEventoCancelamento.eTipoAmbiente.eHomologacao)
            {
                //objHomologEventoWS.nfeCabecMsgValue = RetornaCabecalhoEventoHomolog();
                nodeRetorno = objHomologEventoWS.nfeRecepcaoEvento(nodeEnvio);

                RetornoXmlEvento = nodeRetorno.OuterXml;
            }
            else
            {
                //objProdEventoWS.nfeCabecMsgValue = RetornaCabecalhoEventoProducao();
                nodeRetorno = objProdEventoWS.nfeRecepcaoEvento(nodeEnvio);

                RetornoXmlEvento = nodeRetorno.OuterXml;
            }

            XmlDoc.LoadXml(RetornoXmlEvento);

            // Grava arquivo de retorno
            objUtil.FncGravaXML(RetornoXmlEvento, Entity.PastaCancelamentoRetorno + Entity.IDNF.ToString() + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".xml");

            // Resgata os valores no Node do Lote
            Entity.cStat = Convert.ToInt32(XmlDoc.GetElementsByTagName("cStat").Item(0).InnerXml.ToString());
            Entity.xMotivo = XmlDoc.GetElementsByTagName("xMotivo").Item(0).InnerXml.ToString();

            int IDStatusEvento;

            // Existem 2 etapas de validação:
            // 1° Efetua a validação a partir do campo "cStat" do PRIMEIRO NODE DO XML, caso esteja OK iremos efetuar a 2° validação.
            // 2° Efetua a validação a partir do campo "cStat" do SEGUNDO NODE DO XML.
            // 
            // OBS: Estas validações definem o campo IDStatusEvento.

            #region ..:: 1° Validação ::..
            if (Entity.cStat == (int)cStat.eEventoRegistradoeVinculadoNFe)
            {
                IDStatusEvento = (int)EntityEventoCancelamento.eStatusNFe.eCancelada;
            }
            else if (Entity.cStat == (int)cStat.eLoteEventoProcessado)
            {
                IDStatusEvento = (int)EntityEventoCancelamento.eStatusNFe.eCancelada;
            }
            else if (Entity.cStat == (int)cStat.eEventoregistradoMasNãoVinculadoNFe)
            {
                IDStatusEvento = (int)EntityEventoCancelamento.eStatusNFe.eCancelada;
            }
            else
            {
                IDStatusEvento = (int)EntityEventoCancelamento.eStatusNFe.eRejeicaoCancelamento;
            }
            #endregion

            #region ..:: 2° Validação ::..
            if (IDStatusEvento == (int)EntityEventoCancelamento.eStatusNFe.eCancelada)
            {
                // Resgata os valores no Node do Lote
                Entity.cStat = Convert.ToInt32(XmlDoc.GetElementsByTagName("cStat").Item(1).InnerXml.ToString());
                Entity.xMotivo = XmlDoc.GetElementsByTagName("xMotivo").Item(1).InnerXml.ToString();

                if (ValidacaoGeral(Entity.cStat))
                {
                    IDStatusEvento = (int)EntityEventoCancelamento.eStatusNFe.eRejeicaoCancelamento;
                }
            }
            #endregion

            if (IDStatusEvento == (int)EntityEventoCancelamento.eStatusNFe.eCancelada)
            {
                this.Cancelar(XMLEvento, RetornoXmlEvento, Entity);
            }
            else
            {
                this.RejeitaCancelamento(Entity);
            }

        }

        /// <summary>
        /// Método responsável por alterar o Status da NF-e para Cancelado
        /// </summary>
        /// <param name="IDNF">ID da NF-e</param>
        /// <param name="MotivoNfe">Campo xMotivo do XML de retorno</param>
        private void AlteraStatusCancelado(Int64 IDNF, String MotivoNfe, Int32 cStat)
        {
            this.clParameter = new System.Gdr7.DBClass.Classes.Parameter();

            String sSQL = "StpEventoCancelamento";
            this.clParameter.Add("@Operacao", 3, typeof(Int64));
            this.clParameter.Add("@ID", IDNF, typeof(Int64));
            this.clParameter.Add("@cStat", cStat, typeof(Int32));
            this.clParameter.Add("@MotivoNfe", MotivoNfe, typeof(String));

            this.Db.ExecuteProcedure(sSQL, this.clParameter);
        }

        /// <summary>
        /// Método responsável por alterar o Status da NF-e para REJEIÇÃO DE CANCELAMENTO
        /// </summary>
        /// <param name="IDNF">ID da NF-e</param>
        /// <param name="MotivoNfe">Campo xMotivo do XML de retorno</param>
        private void AlteraStatusRejeicaoCancelamento(Int64 IDNF, String MotivoNfe, Int32 cStat)
        {
            this.clParameter = new System.Gdr7.DBClass.Classes.Parameter();

            String sSQL = "StpEventoCancelamento";
            this.clParameter.Add("@Operacao", 2, typeof(Int64));
            this.clParameter.Add("@ID", IDNF, typeof(Int64));
            this.clParameter.Add("@cStat", cStat, typeof(Int32));
            this.clParameter.Add("@MotivoNfe", MotivoNfe, typeof(String));

            this.Db.ExecuteProcedure(sSQL, this.clParameter);
        }

        /// <summary>
        /// Método responsável por Alterar a NF-e caso o envio do Webservice esteja retorne CANCELADO.
        /// Altera Status para CANCELADO
        /// Grava Histórico da NF-e
        /// Monta o XML de Distribuição
        /// Envia Email para o Destinatário
        /// </summary>
        /// <param name="envEvento">XML de Envio</param>
        /// <param name="retEvento">XML de Retorno</param>
        /// <param name="Entity">Entidade com propriedades do Evento de Cancelamento</param>
        private void Cancelar(String envEvento, String retEvento, EntityEventoCancelamento Entity)
        {       
            this.AlteraStatusCancelado(Entity.IDNF, Entity.xMotivo, Entity.cStat);

            this.GeraEstoqueCancelamento(Entity.IDNF);
            
            this.GravaHistoricoCancelamentoNFe(Entity, (int)EntityEventoCancelamento.eStatusNFe.eCancelada);

            this.MontaXMLProcEvento(envEvento, retEvento, Entity);

            this.EnviaEmails(Entity);
        }

        /// <summary>
        /// Métood responsável por enviar os emails para o Destinatário e Contador
        /// </summary>
        /// <param name="Entity">Entidade com propriedades do Evento de Cancelamento</param>
        private void EnviaEmails(EntityEventoCancelamento Entity)
        {
			//DataBase.Entities.tblParametros tblParametros = new DataBase.Entities.tblParametros();
			//CL_NFE.Classes.NFE.Email objEmail = new CL_NFE.Classes.NFE.Email();

			//objEmail.EnviaEmailCancelamento(Entity.emailDest, Entity.RazaoDest, Entity.NumeroNF, Entity.chNFe, Entity.IDNF.ToString());
                        
			//BLL.Outras.Parametros objParametros = new BLL.Outras.Parametros();
			//tblParametros = objParametros.Pesquisar();

			//if (tblParametros.EmailContador != "")
			//{
			//    objEmail.EnviaEmailCancelamento(tblParametros.EmailContador, tblParametros.EmailContador, Entity.NumeroNF, Entity.chNFe, Entity.IDNF.ToString());
			//}

        }

        /// <summary>
        /// Método responsável por estornar a movimentação em estoque
        /// </summary>
        /// <param name="IDNF">ID da Nota Fiscal</param>
        private void GeraEstoqueCancelamento(Int32 IDNF)
        {
            try
            {
				//BLL.Faturamento.NotasFiscais.NotasFiscais NotaFiscaisBusiness = new BLL.Faturamento.NotasFiscais.NotasFiscais(FncVerificaConexao());
				//NotaFiscaisBusiness.GeraEstoqueCancelamento(IDNF);
            }
            catch (Exception)
            {
            }

           
        }

        /// <summary>
        /// Método responsável por Alterar a NF-e caso o envio do Webservice esteja retorne REJEIÇÃO DE CANCELAMENTO.
        /// Altera Status para REJEIÇÃO DE CANCELAMENTO
        /// </summary>
        /// <param name="Entity">Entidade com propriedades do Evento de Cancelamento</param>
        private void RejeitaCancelamento(EntityEventoCancelamento Entity)
        {
            try
            {
                this.AlteraStatusRejeicaoCancelamento(Entity.IDNF, Entity.xMotivo, Entity.cStat);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Método responsável por Gravar o histórico da NF-e
        /// </summary>
        /// <param name="Entity">Entidade com propriedades do Evento de Cancelamento</param>
        /// <param name="StatusNfe">Status da NF-e</param>
        private void GravaHistoricoCancelamentoNFe(EntityEventoCancelamento Entity, int StatusNfe)
        {
            this.clParameter = new System.Gdr7.DBClass.Classes.Parameter();
          
            String sSQL = "StpNfe";
            this.clParameter.Add("@Operacao", 8, typeof(Int64));
            this.clParameter.Add("@ID", Entity.IDNF, typeof(Int64));
            this.clParameter.Add("@StatusNfe", StatusNfe, typeof(Int64));
            this.clParameter.Add("@tpEmis", Entity.tpEmis, typeof(Int64));

            this.Db.ExecuteProcedure(sSQL, this.clParameter);
        }

        /// <summary>
        /// Método responsável por Montar o XML de Distribuição
        /// </summary>
        /// <param name="envEvento">XML de envio</param>
        /// <param name="retEvento">XML de retorno</param>
        /// <param name="Entity">Entidade com propriedades do Evento de Cancelamento</param>
        /// <returns>Retorna o XML de distribuição no tipo String</returns>
        private String MontaXMLProcEvento(String envEvento, String retEvento, EntityEventoCancelamento Entity)
        {
            Utils objUtil = new Utils();            
            StringBuilder XML = new StringBuilder();

            XML.Append("<procEventoNFe exmlns='http://www.portalfiscal.inf.br/nfe' versao='1.00'>");
            XML.Append(envEvento);
            XML.Append(retEvento);
            XML.Append("</procEventoNFe>");

            // Grava arquivo de retorno
            objUtil.FncGravaXML(XML.ToString(), Entity.PastaCancelamentoCliente + Entity.IDNF.ToString() + ".xml");

            return XML.ToString();
        }

        /// <summary>
        /// Método responsável por iniciar o processo de Cancelamento da NF-e.
        /// Envio para o SEFAZ
        /// Alteração de Status da NF-e, Cancelado ou Rejeitado Cancelamento.
        /// Gravar o histórico da NF-e
        /// </summary>
        public void IniciaProcessoCancelamento()
        {
            EntityEventoCancelamento EntityEvento = new EntityEventoCancelamento();

            try
            {
                DataSet DREntity = this.ListaNFAguardandoCancelamento();

                foreach (DataRow Dr in DREntity.Tables[0].Rows)
                {
					EntityEvento.Id = "ID" + EntityEvento.tpEvento + Dr["chNFe"].ToString() + EntityEvento.nSeqEvento.ToString().PadLeft(2, '0');
                    EntityEvento.IDNF = Convert.ToInt32(Dr["ID"]);
                    EntityEvento.cOrgao = Convert.ToInt32(Dr["cOrgao"]);
                    EntityEvento.CNPJ_CPF = Dr["CNPJ"].ToString();
                    EntityEvento.chNFe = Dr["ChaveNFE"].ToString();
                    EntityEvento.nProt = Dr["nProt"].ToString();
					EntityEvento.xJust = Dr["MotivoCancelamento"].ToString().PadRight(15, '_');
                    EntityEvento.tpEmis = Convert.ToInt32(Dr["TipoEmissaoNFE"]);
                    EntityEvento.RazaoDest = Dr["RazaoDest"].ToString();
                    EntityEvento.NumeroNF = Dr["Numero"].ToString();
                    EntityEvento.emailDest = Dr["EmailNFe"].ToString();

                    EnviaWebServiceEvento(EntityEvento);
                }
            }
            catch (Exception ex)
            {
                StreamWriter ArquivoErro = new StreamWriter(ConfigurationManager.AppSettings["PastaErro"].ToString() + EntityEvento.IDNF.ToString() + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".txt", false, Encoding.ASCII);
                ArquivoErro.WriteLine("***************** IniciaProcessoCancelamento *****************");
                ArquivoErro.WriteLine("Erro gerado " + DateTime.Now.ToString());
                ArquivoErro.WriteLine("IDNota: " + EntityEvento.IDNF.ToString());
                ArquivoErro.WriteLine(ex.Message);
                ArquivoErro.WriteLine(ex.ToString());
                ArquivoErro.Flush();
                ArquivoErro.Close();
            }

           
        }

    }
}
