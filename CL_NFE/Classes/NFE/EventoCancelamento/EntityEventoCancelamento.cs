using System;
using System.Collections.Generic;

using System.Text;
using NFE.Classes.Util;
using System.Configuration;

namespace CL_NFE.Classes.EventoCancelamento.EventoCancelamento
	{
	public class EntityEventoCancelamento
		{
		/// <summary>
		/// Status da NF-e
		/// </summary>
		public enum eStatusNFe
			{
			eCancelada = 7,
			eRejeicaoCancelamento = 8
			}

		/// <summary>
		/// Tipo de Ambiente
		/// </summary>
		public enum eTipoAmbiente
			{
			eHomologacao = 2,
			eProducao = 1
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
			get
				{
				return _IdLote;
				}
			set
				{
				_IdLote = value;
				}
			}

		/// <summary>
		/// ID da NF
		/// </summary>
		Int32 _IDNF;
		public Int32 IDNF
			{
			get
				{
				return _IDNF;
				}
			set
				{
				_IDNF = value;
				}
			}

		/// <summary>
		/// Número da NF
		/// </summary>
		string _NumeroNF;
		public string NumeroNF
			{
			get
				{
				return _NumeroNF;
				}
			set
				{
				_NumeroNF = value;
				}
			}

		/// <summary>
		/// Identificador da TAG a ser assinada, a regra de formação do Id é:
		/// "ID" + tpEvento + chave da NF-e + nSeqEvento
		/// </summary>
		string _Id;
		public string Id
			{
			get
				{
				return _Id;
				}
			set
				{
				_Id = value;
				}
			}

		/// <summary>
		/// Código do órgão de recepção do Evento. Utilizar a Tabela do IBGE.
		/// </summary>
		int _cOrgao;
		public int cOrgao
			{
			get
				{
				return _cOrgao;
				}
			set
				{
				_cOrgao = value;
				}
			}

		/// <summary>
		/// Identificação do Ambiente:
		/// 1 - Produção
		/// 2 – Homologação
		/// </summary>
		int _tpAmb = int.Parse(ConfigurationManager.AppSettings["Ambiente"].ToString());
		public int tpAmb
			{
			get
				{
				return _tpAmb;
				}
			set
				{
				_tpAmb = value;
				}
			}

		int _tpEmis;
		public int tpEmis
			{
			get
				{
				return _tpEmis;
				}
			set
				{
				_tpEmis = value;
				}
			}

		int _cStat;
		public int cStat
			{
			get
				{
				return _cStat;
				}
			set
				{
				_cStat = value;
				}
			}

		string _xMotivo;
		public string xMotivo
			{
			get
				{
				return _xMotivo;
				}
			set
				{
				_xMotivo = value;
				}
			}


		/// <summary>
		/// Informar o CNPJ ou o CPF do autor do Evento
		/// </summary>
		string _CNPJ_CPF;
		public string CNPJ_CPF
			{
			get
				{
				return _CNPJ_CPF;
				}
			set
				{
				_CNPJ_CPF = value;
				}
			}

		/// <summary>
		/// Chave de Acesso da NF-e vinculada ao Evento
		/// </summary>
		string _chNFe;
		public string chNFe
			{
			get
				{
				return _chNFe;
				}
			set
				{
				_chNFe = value;
				}
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
			get
				{
				return _dhEvento;
				}
			set
				{
				_dhEvento = value;
				}
			}

		/// <summary>
		/// Código do de evento = 110110
		/// </summary>
		string _tpEvento = "110111";
		public string tpEvento
			{
			get
				{
				return _tpEvento;
				}
			set
				{
				_tpEvento = value;
				}
			}

		/// <summary>
		/// Sequencial do evento para o mesmo tipo de evento. Para maioria
		/// dos eventos nSeqEvento=1, nos casos em que possa existir mais
		/// de um evento, como é o caso da Carta de Correção, o autor do
		/// evento deve numerar de forma sequencial.
		/// </summary>
		int _nSeqEvento = 1;
		public int nSeqEvento
			{
			get
				{
				return _nSeqEvento;
				}
			set
				{
				_nSeqEvento = value;
				}
			}

		/// <summary>
		/// Versão do evento
		/// </summary>
		decimal _verEvento = 1.00M;
		public decimal verEvento
			{
			get
				{
				return _verEvento;
				}
			set
				{
				_verEvento = value;
				}
			}

		/// <summary>
		/// “Cancelamento”
		/// </summary>
		string _descEvento = "Cancelamento";
		public string descEvento
			{
			get
				{
				return _descEvento;
				}
			set
				{
				_descEvento = value;
				}
			}

		/// <summary>
		/// Informar o número do Protocolo de Autorização da NF-e a ser Cancelada.
		/// </summary>
		string _nProt;
		public string nProt
			{
			get
				{
				return _nProt;
				}
			set
				{
				_nProt = value;
				}
			}

		/// <summary>
		/// Informar a justificativa do cancelamento
		/// </summary>
		string _xJust;
		public string xJust
			{
			get
				{
				return _xJust;
				}
			set
				{
				_xJust = value;
				}
			}

		/// <summary>
		/// e-mail do destinatário informado na NF-e.
		/// </summary>
		string _emailDest;
		public string emailDest
			{
			get
				{
				return _emailDest;
				}
			set
				{
				_emailDest = value;
				}
			}

		/// <summary>
		/// Razão Social do Destinatário
		/// </summary>
		string _RazaoDest;
		public string RazaoDest
			{
			get
				{
				return _RazaoDest;
				}
			set
				{
				_RazaoDest = value;
				}
			}


		/// <summary>
		/// Caminho do Certificado Digital  
		/// </summary>
		string _CaminhoCert = ConfigurationManager.AppSettings["CaminhoCertificado"].ToString();
		public string CaminhoCert
			{
			get
				{
				return _CaminhoCert;
				}
			set
				{
				_CaminhoCert = value;
				}
			}

		/// <summary>
		/// Senha do Certificado Digital
		/// </summary>
		string _SenhaCert = ConfigurationManager.AppSettings["SenhaCertificado"].ToString();
		public string SenhaCert
			{
			get
				{
				return _SenhaCert;
				}
			set
				{
				_SenhaCert = value;
				}
			}

		string _PastaCancelamento = ConfigurationManager.AppSettings["Ambiente"].ToString() == "2" ?
								 ConfigurationManager.AppSettings["PastaXMLCancelamentoHomologacao"].ToString() :
								 ConfigurationManager.AppSettings["PastaXMLCancelamentoProducao"].ToString();
		public string PastaCancelamento
			{
			get
				{
				return _PastaCancelamento;
				}
			set
				{
				_PastaCancelamento = value;
				}
			}

		string _PastaCancelamentoRetorno = ConfigurationManager.AppSettings["Ambiente"].ToString() == "2" ?
								  ConfigurationManager.AppSettings["PastaXMLCancelamentoHomologacaoRetorno"].ToString() :
								  ConfigurationManager.AppSettings["PastaXMLCancelamentoProducaoRetorno"].ToString();
		public string PastaCancelamentoRetorno
			{
			get
				{
				return _PastaCancelamentoRetorno;
				}
			set
				{
				_PastaCancelamentoRetorno = value;
				}
			}


		string _PastaCancelamentoCliente = ConfigurationManager.AppSettings["Ambiente"].ToString() == "2" ?
					  ConfigurationManager.AppSettings["PastaXMLCancelamentoHomologacaoCliente"].ToString() :
					  ConfigurationManager.AppSettings["PastaXMLCancelamentoProducaoCliente"].ToString();
		public string PastaCancelamentoCliente
			{
			get
				{
				return _PastaCancelamentoCliente;
				}
			set
				{
				_PastaCancelamentoCliente = value;
				}
			}

		}
	}
