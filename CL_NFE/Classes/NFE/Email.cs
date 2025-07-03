using System;
using System.Net;
using System.Web;
using System.Web.Mail;
using System.IO;
using System.Configuration;
using System.Text;
using System.Net.Mime;

namespace CL_NFE.Classes.NFE
{
    public class Email
    {
        //Metodo usando System.Web.Mail
        public void EnviaEmail(string EmailNFe, string NmCliente, string NrNfe, string ChAcesso, string ID)
        {
            try
            {
                String CaminhoDistribuicao = "";
          
                if (ConfigurationManager.AppSettings["Ambiente"].ToString() == "2")
                {
                    CaminhoDistribuicao = ConfigurationManager.AppSettings["PastaXMLRetRecepcaoClienteHomologacao"].ToString();
                }
                else
                {
                    CaminhoDistribuicao = ConfigurationManager.AppSettings["PastaXMLRetRecepcaoClienteProducao"].ToString();
                }

                string Corpo = @"<table>
                                    <tr>
                                        <td style='font-family:Verdana; font-size:12pt;'>
                                            São Paulo, {0}
                                            <p />
                                            <b>À</b>
                                            <p />
                                            <b>{1}</b>
                                            <p />
                                            <b style='text-decoration:underline;'>REF. NOTA FISCAL ELETRÔNICA – Nº {2}</b>
                                            <p />
                                            Informamos que nesta data foi emitida a Nota Fiscal Eletrônica acima. 
                                            <br />
                                            Conforme determina a legislação, encaminhamos em anexo o arquivo xml com todas as informações sobre a NF-e.
                                            <p />
                                            A mesma poderá ser consultada em nos endereços abaixo através de chave de acesso:
                                            <p />
                                            <b>{3}</b>
                                            <p />
                                            <b><a href='http://www.fazenda.sp.gov.br/nfe'>www.fazenda.sp.gov.br/nfe</a> (Secretaria da Fazenda de São Paulo)</b>
                                            <br /><br />
                                            <b><a href='http://www.nfe.fazenda.gov.br'>www.nfe.fazenda.gov.br</a> (Ambiente Nacional da Nota Fiscal Eletrônica)</b>
                                            <br /><br /><br /><br /><br />
                                            Ficamos à disposição par quaisquer esclarecimentos.
                                            <br /><br /><br /><br /><br />
                                            Atenciosamente.
                                            <br /><br /><br /><br /><br />
                                            <b>Armazém 7</b>
                                            <br/>
                                            <a href='http://www.armazem7.com.br'>www.armazem7.com.br</a>
                                        </td>
                                    </tr>
                                </table>";

                Corpo = string.Format(  Corpo,
                                        string.Format("{0:D}", DateTime.Now),
                                        NmCliente,
                                        NrNfe,
                                        ChAcesso);
                                              
                MailMessage Email = new MailMessage();
                Email.From = "nfe@inoxplasma.com.br";
                Email.To = EmailNFe;
                Email.Subject = "Nota Fiscal Eletrônica";
                Email.BodyFormat = MailFormat.Html;
                Email.Body = Corpo;
                Email.Attachments.Add(new MailAttachment(CaminhoDistribuicao + ID + ".xml"));
                SmtpMail.Send(Email);                
            }
            catch (Exception ex)
            {
                StreamWriter ArquivoErro = new StreamWriter(ConfigurationManager.AppSettings["PastaErro"].ToString() + NrNfe + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".txt", false, Encoding.ASCII);
                ArquivoErro.WriteLine("***************** ENVIO DE EMAIL *****************");
                ArquivoErro.WriteLine("Erro gerado " + DateTime.Now.ToString());
                ArquivoErro.WriteLine("Numero da Nota: " + NrNfe);
                ArquivoErro.WriteLine(ex.Message);
                ArquivoErro.WriteLine(ex.ToString());
                ArquivoErro.Flush();
                ArquivoErro.Close();
            }
        }

        //Metodo usando System.Net.Mail
        public void EnviaEmail2(string EmailNFe, string NmCliente, string NrNfe, string ChAcesso, string ID)
        {
            try
            {
                string CaminhoDistribuicao = "";

                if (ConfigurationManager.AppSettings["Ambiente"].ToString() == "2")
                {
                    CaminhoDistribuicao = ConfigurationManager.AppSettings["PastaXMLRetRecepcaoClienteHomologacao"].ToString();
                }
                else
                {
                    CaminhoDistribuicao = ConfigurationManager.AppSettings["PastaXMLRetRecepcaoClienteProducao"].ToString();
                }

                string Corpo = @"<table>
                                    <tr>
                                        <td style='font-family:Verdana; font-size:12pt;'>
                                            São Paulo, {0}
                                            <p />
                                            <b>À</b>
                                            <p />
                                            <b>{1}</b>
                                            <p />
                                            <b style='text-decoration:underline;'>REF. NOTA FISCAL ELETRÔNICA – Nº {2}</b>
                                            <p />
                                            Informamos que nesta data foi emitida a Nota Fiscal Eletrônica acima. 
                                            <br />
                                            Conforme determina a legislação, encaminhamos em anexo o arquivo xml e uma cópia da DANFE com todas as informações sobre a NF-e.
                                            <p />
                                            A mesma poderá ser consultada em nos endereços abaixo através de chave de acesso:
                                            <p />
                                            <b>{3}</b>
                                            <p />
                                            <b><a href='http://www.fazenda.sp.gov.br/nfe'>www.fazenda.sp.gov.br/nfe</a> (Secretaria da Fazenda de São Paulo)</b>
                                            <br /><br />
                                            <b><a href='http://www.nfe.fazenda.gov.br'>www.nfe.fazenda.gov.br</a> (Ambiente Nacional da Nota Fiscal Eletrônica)</b>
                                            <br /><br /><br /><br /><br />
                                            Ficamos à disposição para quaisquer esclarecimentos.
                                            <br /><br /><br /><br /><br />
                                            Atenciosamente.
                                            <br /><br /><br /><br /><br />
                                            <b>Armazém 7</b>
                                            <br/>
                                            <a href='http://www.armazem7.com.br'>www.armazem7.com.br</a>
                                        </td>
                                    </tr>
                                </table>";

                Corpo = string.Format(Corpo,
                                        string.Format("{0:D}", DateTime.Now),
                                        NmCliente,
                                        NrNfe,
                                        ChAcesso);

                System.Net.Mail.MailMessage Email = new System.Net.Mail.MailMessage();
                System.IO.MemoryStream strMem = new System.IO.MemoryStream();

               
                Email.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["EmailFrom"].ToString());
                Email.To.Add(EmailNFe);
                Email.Subject = "Nota Fiscal Eletrônica";
                Email.IsBodyHtml = true;
                Email.Body = Corpo;

                System.Net.Mail.Attachment attachFile = new System.Net.Mail.Attachment(CaminhoDistribuicao + ID + ".xml");
                Email.Attachments.Add(attachFile);

                if (File.Exists(ConfigurationManager.AppSettings["PastaPDF"] + ID + ".pdf"))
                {
                    System.Net.Mail.Attachment attachFilePDF = new System.Net.Mail.Attachment(ConfigurationManager.AppSettings["PastaPDF"] + ID + ".pdf");
                    Email.Attachments.Add(attachFilePDF);
                }

                System.Net.Mail.SmtpClient objSmtp = new System.Net.Mail.SmtpClient(ConfigurationManager.AppSettings["SmtpServer"].ToString());
                objSmtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["EmailFrom"].ToString(), ConfigurationManager.AppSettings["EmailSenha"].ToString());
                objSmtp.Send(Email);

                if (File.Exists(ConfigurationManager.AppSettings["PastaPDF"] + ID + ".pdf"))
                    File.Delete(ConfigurationManager.AppSettings["PastaPDF"] + ID + ".pdf");
            }
            catch (Exception ex)
            {
                StreamWriter ArquivoErro = new StreamWriter(ConfigurationManager.AppSettings["PastaErro"].ToString() + NrNfe + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".txt", false, Encoding.ASCII);
                ArquivoErro.WriteLine("***************** ENVIO DE EMAIL *****************");
                ArquivoErro.WriteLine("Erro gerado " + DateTime.Now.ToString());
                ArquivoErro.WriteLine("Numero da Nota: " + NrNfe);
                ArquivoErro.WriteLine(ex.Message);
                ArquivoErro.WriteLine(ex.ToString());
                ArquivoErro.Flush();
                ArquivoErro.Close();
            }

        }

        /// <summary>
        /// Método responsável por enviar o arquivo da CC-e para o cliente.
        /// </summary>
        /// <param name="EmailNFe">Email do Destinatario</param>
        /// <param name="RazaoSocialCliente">Razão Social do Cliente</param>
        /// <param name="NrNfe">Número da Nota Fiscal</param>
        /// <param name="ChAcesso">Chave de Acesso da NF-e</param>
        /// <param name="IDNF">ID da Nota Fiscal</param>
        public void EnviaEmailCCe(string EmailNFe, string RazaoSocialCliente, string NrNfe, string ChAcesso, string IDNF)
        {
            try
            {
                string CaminhoDistribuicao = "";

                if (ConfigurationManager.AppSettings["Ambiente"].ToString() == "2")
                    CaminhoDistribuicao = ConfigurationManager.AppSettings["PastaXMLCCeHomologacaoCliente"].ToString();
                else
                    CaminhoDistribuicao = ConfigurationManager.AppSettings["PastaXMLCCeProducaoCliente"].ToString();

                string Corpo = @"<table>
                                    <tr>
                                        <td style='font-family:Verdana; font-size:12pt;'>
                                            São Paulo, {0}
                                            <p />
                                            <b>À</b>
                                            <p />
                                            <b>{1}</b>
                                            <p />
                                            <b style='text-decoration:underline;'>REF. NOTA FISCAL ELETRÔNICA – Nº {2}</b>
                                            <p />
                                            Informamos que nesta data foi emitida a <span style='color:#cc0000;'>Carta de Correção Eletrônica (CC-e)</span> acima. 
                                            <br />
                                            Conforme determina a legislação, encaminhamos em anexo o arquivo xml com todas as informações sobre a CC-e.
                                            <p />
                                            A mesma poderá ser consultada em nos endereços abaixo através de chave de acesso:
                                            <p />
                                            <b>{3}</b>
                                            <p />
                                            <b><a href='http://www.fazenda.sp.gov.br/nfe'>www.fazenda.sp.gov.br/nfe</a> (Secretaria da Fazenda de São Paulo)</b>
                                            <br /><br />
                                            <b><a href='http://www.nfe.fazenda.gov.br'>www.nfe.fazenda.gov.br</a> (Ambiente Nacional da Nota Fiscal Eletrônica)</b>
                                            <br /><br /><br /><br /><br />
                                            Ficamos à disposição para quaisquer esclarecimentos.
                                            <br /><br /><br /><br /><br />
                                            Atenciosamente.
                                            <br /><br /><br /><br /><br />
                                            <b>Armazém 7</b>
                                            <br/>
                                            <a href='http://www.armazem7.com.br'>www.armazem7.com.br</a>
                                        </td>
                                    </tr>
                                </table>";

                Corpo = string.Format(Corpo,
                                        string.Format("{0:D}", DateTime.Now),
                                        RazaoSocialCliente,
                                        NrNfe,
                                        ChAcesso);

                System.Net.Mail.MailMessage Email = new System.Net.Mail.MailMessage();
                System.IO.MemoryStream strMem = new System.IO.MemoryStream();

                System.Net.Mail.Attachment attachFile = new System.Net.Mail.Attachment(CaminhoDistribuicao + IDNF + ".xml");
                attachFile.Name = IDNF + ".xml";

                Email.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["EmailFrom"].ToString());
                Email.To.Add(EmailNFe);
                Email.Subject = "Carta de Correção Eletrônica";
                Email.IsBodyHtml = true;
                Email.Body = Corpo;
                Email.Attachments.Add(attachFile);

                System.Net.Mail.SmtpClient objSmtp = new System.Net.Mail.SmtpClient(ConfigurationManager.AppSettings["SmtpServer"].ToString());
                objSmtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["EmailFrom"].ToString(), ConfigurationManager.AppSettings["EmailSenha"].ToString());
                objSmtp.Send(Email);

            }
            catch (Exception ex)
            {
                StreamWriter ArquivoErro = new StreamWriter(ConfigurationManager.AppSettings["PastaErro"].ToString() + NrNfe + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".txt", false, Encoding.ASCII);
                ArquivoErro.WriteLine("***************** ENVIO DE EMAIL CC-e *****************");
                ArquivoErro.WriteLine("Erro gerado " + DateTime.Now.ToString());
                ArquivoErro.WriteLine("Numero da Nota: " + NrNfe);
                ArquivoErro.WriteLine(ex.Message);
                ArquivoErro.WriteLine(ex.ToString());
                ArquivoErro.Flush();
                ArquivoErro.Close();
            }

        }

        //Metodo usando System.Net.Mail
        public void EnviaEmailCancelamento(string EmailNFe, string NmCliente, string NrNfe, string ChAcesso, string ID)
        {
            try
            {
                string CaminhoDistribuicao = "";

                if (ConfigurationManager.AppSettings["Ambiente"].ToString() == "2")
                {
                    CaminhoDistribuicao = ConfigurationManager.AppSettings["PastaXMLCancelamentoHomologacaoCliente"].ToString();
                }
                else
                {
                    CaminhoDistribuicao = ConfigurationManager.AppSettings["PastaXMLCancelamentoProducaoCliente"].ToString();
                }

                string Corpo = @"<table>
                                    <tr>
                                        <td style='font-family:Verdana; font-size:12pt;'>
                                            São Paulo, {0}
                                            <p />
                                            <b>À</b>
                                            <p />
                                            <b>{1}</b>
                                            <p />
                                            <b style='text-decoration:underline;'>REF. NOTA FISCAL ELETRÔNICA – Nº {2}</b>
                                            <p />
                                            Informamos que nesta data foi <span style='color:#cc0000;'>CANCELADA</span> a Nota Fiscal Eletrônica acima. 
                                            <br />
                                            Conforme determina a legislação, encaminhamos em anexo o arquivo xml com as informações deste cancelamento.
                                            <p />
                                            A mesma poderá ser consultada em nos endereços abaixo através de chave de acesso:
                                            <p />
                                            <b>{3}</b>
                                            <p />
                                            <b><a href='http://www.fazenda.sp.gov.br/nfe'>www.fazenda.sp.gov.br/nfe</a> (Secretaria da Fazenda de São Paulo)</b>
                                            <br /><br />
                                            <b><a href='http://www.nfe.fazenda.gov.br'>www.nfe.fazenda.gov.br</a> (Ambiente Nacional da Nota Fiscal Eletrônica)</b>
                                            <br /><br /><br /><br /><br />
                                            Ficamos à disposição para quaisquer esclarecimentos.
                                            <br /><br /><br /><br /><br />
                                            Atenciosamente.
                                            <br /><br /><br /><br /><br />
                                            <b>Armazém 7</b>
                                            <br/>
                                            <a href='http://www.armazem7.com.br'>www.armazem7.com.br</a>
                                        </td>
                                    </tr>
                                </table>";

                Corpo = string.Format(Corpo,
                                        string.Format("{0:D}", DateTime.Now),
                                        NmCliente,
                                        NrNfe,
                                        ChAcesso);

                System.Net.Mail.MailMessage Email = new System.Net.Mail.MailMessage();
                System.IO.MemoryStream strMem = new System.IO.MemoryStream();

                System.Net.Mail.Attachment attachFile = new System.Net.Mail.Attachment(CaminhoDistribuicao + ID + ".xml");
                Email.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["EmailFrom"].ToString());
                Email.To.Add(EmailNFe);
                Email.Subject = "Nota Fiscal Eletrônica - Cancelamento";
                Email.IsBodyHtml = true;
                Email.Body = Corpo;
                Email.Attachments.Add(attachFile);

                System.Net.Mail.SmtpClient objSmtp = new System.Net.Mail.SmtpClient(ConfigurationManager.AppSettings["SmtpServer"].ToString());
                objSmtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["EmailFrom"].ToString(), ConfigurationManager.AppSettings["EmailSenha"].ToString());
                objSmtp.Send(Email);

            }
            catch (Exception ex)
            {
                StreamWriter ArquivoErro = new StreamWriter(ConfigurationManager.AppSettings["PastaErro"].ToString() + NrNfe + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".txt", false, Encoding.ASCII);
                ArquivoErro.WriteLine("***************** ENVIO DE EMAIL *****************");
                ArquivoErro.WriteLine("Erro gerado " + DateTime.Now.ToString());
                ArquivoErro.WriteLine("Numero da Nota: " + NrNfe);
                ArquivoErro.WriteLine(ex.Message);
                ArquivoErro.WriteLine(ex.ToString());
                ArquivoErro.Flush();
                ArquivoErro.Close();
            }

        }

        
        public void EnviaEmailNFe(string[] EmailNFe, string NmCliente, string NrNfe, string ChAcesso, string ID)
        {
            try
            {
                string CaminhoDistribuicao = "";

                if (ConfigurationManager.AppSettings["Ambiente"].ToString() == "2")
                {
                    CaminhoDistribuicao = ConfigurationManager.AppSettings["PastaXMLRetRecepcaoClienteHomologacao"].ToString();
                }
                else
                {
                    CaminhoDistribuicao = ConfigurationManager.AppSettings["PastaXMLRetRecepcaoClienteProducao"].ToString();
                }

                string Corpo = @"<table>
                                    <tr>
                                        <td style='font-family:Verdana; font-size:12pt;'>
                                            São Paulo, {0}
                                            <p />
                                            <b>À</b>
                                            <p />
                                            <b>{1}</b>
                                            <p />
                                            <b style='text-decoration:underline;'>REF. NOTA FISCAL ELETRÔNICA – Nº {2}</b>
                                            <p />
                                            Informamos que nesta data foi emitida a Nota Fiscal Eletrônica acima. 
                                            <br />
                                            Conforme determina a legislação, encaminhamos em anexo o arquivo xml e uma cópia da DANFE com todas as informações sobre a NF-e.
                                            <p />
                                            A mesma poderá ser consultada em nos endereços abaixo através de chave de acesso:
                                            <p />
                                            <b>{3}</b>
                                            <p />
                                            <b><a href='http://www.fazenda.sp.gov.br/nfe'>www.fazenda.sp.gov.br/nfe</a> (Secretaria da Fazenda de São Paulo)</b>
                                            <br /><br />
                                            <b><a href='http://www.nfe.fazenda.gov.br'>www.nfe.fazenda.gov.br</a> (Ambiente Nacional da Nota Fiscal Eletrônica)</b>
                                            <br /><br /><br /><br /><br />
                                            Ficamos à disposição para quaisquer esclarecimentos.
                                            <br /><br /><br /><br /><br />
                                            Atenciosamente.
                                            <br /><br /><br /><br /><br />
                                            <b>Armazém 7</b>
                                            <br/>
                                            <a href='http://www.armazem7.com.br'>www.armazem7.com.br</a>
                                        </td>
                                    </tr>
                                </table>";

                Corpo = string.Format(Corpo,
                                        string.Format("{0:D}", DateTime.Now),
                                        NmCliente,
                                        NrNfe,
                                        ChAcesso);

                System.Net.Mail.MailMessage Email = new System.Net.Mail.MailMessage();
                System.IO.MemoryStream strMem = new System.IO.MemoryStream();


                Email.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["EmailFrom"].ToString());

                // Destinatário
                Email.To.Add(EmailNFe[0].ToString());

                Email.Subject = "Nota Fiscal Eletrônica";
                Email.IsBodyHtml = true;
                Email.Body = Corpo;

                System.Net.Mail.Attachment attachFile = new System.Net.Mail.Attachment(CaminhoDistribuicao + ID + ".xml");
                attachFile.Name = "67423111000181_" + NrNfe + ".xml";
                Email.Attachments.Add(attachFile);

                if (File.Exists(ConfigurationManager.AppSettings["PastaPDF"] + ID + ".pdf"))
                {
                    System.Net.Mail.Attachment attachFilePDF = new System.Net.Mail.Attachment(ConfigurationManager.AppSettings["PastaPDF"] + ID + ".pdf");
                    attachFilePDF.Name = "67423111000181_" + NrNfe + ".pdf";              
                    Email.Attachments.Add(attachFilePDF);
                }

                System.Net.Mail.SmtpClient objSmtp = new System.Net.Mail.SmtpClient(ConfigurationManager.AppSettings["SmtpServer"].ToString());
                objSmtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["EmailFrom"].ToString(), ConfigurationManager.AppSettings["EmailSenha"].ToString());
                objSmtp.Send(Email);

                try
                {
                    if (File.Exists(ConfigurationManager.AppSettings["PastaPDF"] + ID + ".pdf"))
                        File.Delete(ConfigurationManager.AppSettings["PastaPDF"] + ID + ".pdf");
                }
                catch (Exception)
                {
                }
            }
            catch (Exception ex)
            {
                StreamWriter ArquivoErro = new StreamWriter(ConfigurationManager.AppSettings["PastaErro"].ToString() + NrNfe + "_" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".txt", false, Encoding.ASCII);
                ArquivoErro.WriteLine("***************** ENVIO DE EMAIL *****************");
                ArquivoErro.WriteLine("Erro gerado " + DateTime.Now.ToString());
                ArquivoErro.WriteLine("Numero da Nota: " + NrNfe);
                ArquivoErro.WriteLine(ex.Message);
                ArquivoErro.WriteLine(ex.ToString());
                ArquivoErro.Flush();
                ArquivoErro.Close();
            }

        }
    }
}
