using System;
using System.Configuration;
using System.IO;
using System.Text;

namespace ConsoleNfeGDR7
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.Write(DateTime.Now.ToString() + ": Iniciado.");
            Engine();
        }

        static public void Engine()
        {
            try
            {

                #region INSTANCIA VERSÃO ANTIGA!
                //NFE.Classes.NFE.MontaXMl.MontaXMLNfe2 oxml = new NFE.Classes.NFE.MontaXMl.MontaXMLNfe2();
                //NFE.RetRecepcao objRetRecepcao = new NFE.RetRecepcao();
                //NFE.Cancelamento objCancelamento = new NFE.Cancelamento();
                #endregion

                #region INSTANCIA NOVA VERSÃO!
                NFE.Classes.NFE.MontaXMLNfeNovaVersao oxmlNova = new NFE.Classes.NFE.MontaXMLNfeNovaVersao();
                NFE.RetRecepcaoNovaVersao objRetRecepcaoNova = new NFE.RetRecepcaoNovaVersao();
                NFE.CancelamentoNovaVersao objCancelamentoNova = new NFE.CancelamentoNovaVersao();
                NFE.Classes.NFE.MontaXMLCCe objCCe = new NFE.Classes.NFE.MontaXMLCCe();
                CL_NFE.Classes.EventoCancelamento.EventoCancelamento.Cancelamento objEventoCancelamento = new CL_NFE.Classes.EventoCancelamento.EventoCancelamento.Cancelamento();
                #endregion

                NFE.Classes.Util.Utils objUtil = new NFE.Classes.Util.Utils();

                #region GET VERSÃO E TIPO DE EMISSÃO!
                string VersaoNfe = ConfigurationManager.AppSettings["VersaoNFe"].ToString();
                string tpEmis = objUtil.FncVerificaTipoEmissao();
                #endregion

                NFE.Classes.NFE.MontaXMLNfeNovaVersao objNFe = new NFE.Classes.NFE.MontaXMLNfeNovaVersao();

                // RetRecepcao
                //objRetRecepcaoNova.FncRetRecepcao();

                // Cancelamento
                //objCancelamentoNova.FncCancelamento();

                objEventoCancelamento.IniciaProcessoCancelamento();

                // Recepcao    
                oxmlNova.MontaXML();

                // Recepcao Contingencia
                //oxmlNova.MontaXMLContingencia();

                //oxmlNova.XmlValidacao(486);

                // Carta de Correção
                objCCe.FncCCe();
                //}
            }
            catch (Exception ex)
            {
                StreamWriter ArquivoErro = new StreamWriter(ConfigurationManager.AppSettings["PastaErro"].ToString() + "Inicializacao_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".txt", false, Encoding.ASCII);
                ArquivoErro.WriteLine("***************** INICIALIZAÇÃO *****************");
                ArquivoErro.WriteLine("Erro gerado " + DateTime.Now.ToString());
                ArquivoErro.WriteLine(ex.Message);
                ArquivoErro.WriteLine(ex.ToString());
                ArquivoErro.Flush();
                ArquivoErro.Close();
            }

        }
    }
}
