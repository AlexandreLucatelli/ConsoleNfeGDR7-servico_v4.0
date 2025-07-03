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

namespace NFE.Classes.NFE.Objetos
{
    public class CCe : Util.Utils
    {
        /// <summary>
        /// Status da CCe
        /// </summary>
        public enum StatusCCe
        {
            eEmAberto = 1,
            eFinalizada = 2,
            eEmProcessamento = 3,
            eAprovada = 4,
            eRejeitada = 5
        }

        /// <summary>
        /// Identificador de controle do Lote de envio do Evento.
        /// Número seqüencial autoincremental único para
        /// identificação do Lote. A responsabilidade de gerar e
        /// controlar é exclusiva do autor do evento. O Web Service
        /// não faz qualquer uso deste identificador.
        /// </summary>
        string _IdLote;
        public string IdLote
        {
            get { return _IdLote; }
            set {_IdLote=value; }
        }
                
        /// <summary>
        /// Identificador da TAG a ser assinada, a regra de formação do Id é:
        /// "ID" + tpEvento + chave da NF-e + nSeqEvento
        /// </summary>
        string _Id;
        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        /// <summary>
        /// Código do órgão de recepção do Evento. Utilizar a Tabela do IBGE.
        /// </summary>
        int _cOrgao;
        public int cOrgao
        {
            get { return _cOrgao; }
            set { _cOrgao = value; }
        }

        /// <summary>
        /// Identificação do Ambiente:
        /// 1 - Produção
        /// 2 – Homologação
        /// </summary>
        int _tpAmb = int.Parse(ConfigurationManager.AppSettings["Ambiente"].ToString());
        public int tpAmb
        {
            get { return _tpAmb; }
            set { _tpAmb = value; }
        }

        /// <summary>
        /// Informar o CNPJ ou o CPF do autor do Evento
        /// </summary>
        string _CNPJ_CPF;
        public string CNPJ_CPF
        {
            get { return _CNPJ_CPF; }
            set { _CNPJ_CPF = value; }
        }

        /// <summary>
        /// Chave de Acesso da NF-e vinculada ao Evento
        /// </summary>
        string _chNFe;
        public string chNFe
        {
            get { return _chNFe; }
            set { _chNFe = value; }
        }

        /// <summary>
        /// Data e hora do evento no formato AAAA-MM-DDThh: mm:ssTZD 
        /// (UTC - Universal Coordinated Time, onde TZD pode ser -02:00 
        /// (Fernando de Noronha), -03:00 
        /// (Brasília) ou -04:00 
        /// (Manaus), no horário de verão serão - 01:00, -02:00 e -03:00. 
        /// Ex.: 2010-08-19T13:00:15-03:00.
        /// </summary>
        string _dhEvento = string.Format("{0:yyyy-MM-ddTHH:mm:ss}{1}", DateTime.Now.AddMinutes(-3), TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).ToString().Substring(0, 6));
        public string dhEvento
        {
            get { return _dhEvento; }
            set { _dhEvento = value; }
        }

        /// <summary>
        /// Código do de evento = 110110
        /// </summary>
        string _tpEvento = "110110";
        public string tpEvento
        {
            get { return _tpEvento; }
            set { _tpEvento = value; }
        }

        /// <summary>
        /// Seqüencial do evento para o mesmo tipo de evento. 
        /// Para maioria dos eventos será 1, nos casos em que possa 
        /// existir mais de um evento, como é o caso da carta de 
        /// correção, o autor do evento deve numerar de forma seqüencial.
        /// </summary>
        int _nSeqEvento;
        public int nSeqEvento
        {
            get { return _nSeqEvento; }
            set { _nSeqEvento = value; }
        }

        /// <summary>
        /// Versão do evento
        /// </summary>
        decimal _verEvento = 1.00M;
        public decimal verEvento
        {
            get { return _verEvento; }
            set { _verEvento = value; }
        }

        /// <summary>
        /// “Carta de Correção” ou “Carta de Correcao”
        /// </summary>
        string _descEvento = "Carta de Correcao";
        public string descEvento
        {
            get { return _descEvento; }
            set { _descEvento = value; }
        }

        /// <summary>
        /// Correção a ser considerada, texto livre. A correção mais recente substitui as anteriores.
        /// </summary>
        string _xCorrecao;
        public string xCorrecao
        {
            get { return _xCorrecao; }
            set { _xCorrecao = value; }
        }

        /// <summary>
        /// Condições de uso da Carta de Correção, informar a literal :
        /// “A Carta de Correção é disciplinada pelo § 1º-A do art. 7º do Convênio S/N, de 15 de dezembro de 1970 e pode ser
        /// utilizada para regularização de erro ocorrido na emissão de documento fiscal, desde que o erro não esteja relacionado com: 
        /// I - as variáveis que determinam o valor do imposto tais como: base de cálculo, alíquota, diferença de preço, quantidade, valor da operação ou da prestação; 
        /// II - a correção de dados cadastrais que implique mudança do remetente ou do destinatário; 
        /// III - a data de emissão ou de saída.” 
        /// (texto com acentuação) ou
        /// “A Carta de Correcao e disciplinada pelo paragrafo 1o-A do art. 7o do Convenio S/N, de 15 de dezembro de 1970 e
        /// pode ser utilizada para regularizacao de erro ocorrido na emissao de documento fiscal, desde que o erro nao esteja
        /// relacionado com: 
        /// I - as variaveis que determinam o valor do imposto tais como: base de calculo, aliquota, diferenca de preco, quantidade, valor da operacao ou da prestacao; 
        /// II - a correcao de dados cadastrais que implique mudanca do remetente ou do destinatario; 
        /// III - a data de emissao ou de saida.” (texto sem acentuação)
        /// </summary>
        string _xCondUso = "A Carta de Correcao e disciplinada pelo paragrafo 1o-A do art. 7o do Convenio S/N, de 15 de dezembro de 1970 e pode ser utilizada para regularizacao de erro ocorrido na emissao de documento fiscal, desde que o erro nao esteja relacionado com: I - as variaveis que determinam o valor do imposto tais como: base de calculo, aliquota, diferenca de preco, quantidade, valor da operacao ou da prestacao; II - a correcao de dados cadastrais que implique mudanca do remetente ou do destinatario; III - a data de emissao ou de saida.";
        public string xCondUso
        {
            get { return _xCondUso; }
            set { _xCondUso = value; }
        }

        /// <summary>
        /// Caminho do Certificado Digital  
        /// </summary>
        string _CaminhoCert = ConfigurationManager.AppSettings["CaminhoCertificado"].ToString();
        public string CaminhoCert
        {
            get { return _CaminhoCert; }
            set { _CaminhoCert = value; }
        }

        /// <summary>
        /// Senha do Certificado Digital
        /// </summary>
        string _SenhaCert = ConfigurationManager.AppSettings["SenhaCertificado"].ToString();
        public string SenhaCert
        {
            get { return _SenhaCert; }
            set { _SenhaCert = value; }
        }

        string _PastaCCe = ConfigurationManager.AppSettings["Ambiente"].ToString() == "2" ?
                                  ConfigurationManager.AppSettings["PastaXMLCCeHomologacao"].ToString() :
                                  ConfigurationManager.AppSettings["PastaXMLCCeProducao"].ToString();
        public string PastaCCe
        {
            get { return _PastaCCe; }
            set { _PastaCCe = value; }
        }

        string _PastaCCeRetorno = ConfigurationManager.AppSettings["Ambiente"].ToString() == "2" ?
                                  ConfigurationManager.AppSettings["PastaXMLCCeHomologacaoRetorno"].ToString() :
                                  ConfigurationManager.AppSettings["PastaXMLCCeProducaoRetorno"].ToString();
        public string PastaCCeRetorno
        {
            get { return _PastaCCeRetorno; }
            set { _PastaCCeRetorno = value; }
        }


        string _PastaCCeCliente = ConfigurationManager.AppSettings["Ambiente"].ToString() == "2" ?
                      ConfigurationManager.AppSettings["PastaXMLCCeHomologacaoCliente"].ToString() :
                      ConfigurationManager.AppSettings["PastaXMLCCeProducaoCliente"].ToString();
        public string PastaCCeCliente
        {
            get { return _PastaCCeCliente; }
            set { _PastaCCeCliente = value; }
        }

        string _nProt;
        public string nProt
        {
            get { return _nProt; }
            set { _nProt = value; }
        }
        
    }
}
