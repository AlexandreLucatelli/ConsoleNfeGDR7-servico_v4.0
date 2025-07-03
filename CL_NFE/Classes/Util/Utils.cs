using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Schema;
using System.Configuration;

using NFE.Classes.AcessoDados;
using System.Data;
using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;

namespace NFE.Classes.Util
{
   public class Utils : DB
    {
       public String retorno;

       public string FncVerificaXMLSchema(string CaminhoXML,string CaminhoSchema,string CaminhoNovoArquivo)
       { 
           
           string Resultado = string.Empty;
           XmlReader reader = null;
           try
           {
          
           XmlReaderSettings settings = new XmlReaderSettings();
           settings.Schemas.Add(ConfigurationSettings.AppSettings["NameSpaceNFE"].ToString(), XmlReader.Create(@CaminhoSchema));
           settings.ValidationType = ValidationType.Schema;
           
          
               reader = XmlReader.Create(@CaminhoXML, settings);
               XmlDocument document = new XmlDocument();
               document.Load(reader);
               ValidationEventHandler eventHandler = new ValidationEventHandler(ValidationEventHandler);
               // the following call to Validate succeeds.
               document.Validate(eventHandler);
               document.Save(@CaminhoNovoArquivo);
               reader.Close();
               Resultado = "XML Validado com sucesso";
           }
           catch (Exception Ex)
           {
               Resultado = Ex.Message;
               if (!(reader.ReadState == ReadState.Closed))
               {
                   reader.Close();
               }
           }
           return Resultado;
       }

       static void ValidationEventHandler(object sender, ValidationEventArgs e)
       {
           switch (e.Severity)
           {
               case XmlSeverityType.Error:
                   Console.WriteLine("Error: {0}", e.Message);
                   break;
               case XmlSeverityType.Warning:
                   Console.WriteLine("Warning {0}", e.Message);
                   break;
           }
       }
            
       public string FncRetornaDigitoNota(StringBuilder IdNota)
       {
           #region Variaveis de Valor
           int dig0 = 0,
               dig1 = 0,
               dig2 = 0,
               dig3 = 0,
               dig4 = 0,
               dig5 = 0,
               dig6 = 0,
               dig7 = 0,
               dig8 = 0,
               dig9 = 0,
               dig10 = 0,
               dig11 = 0,
               dig12 = 0,
               dig13 = 0,
               dig14 = 0,
               dig15 = 0,
               dig16 = 0,
               dig17 = 0,
               dig18 = 0,
               dig19 = 0,
               dig20 = 0,
               dig21 = 0,
               dig22 = 0,
               dig23 = 0,
               dig24 = 0,
               dig25 = 0,
               dig26 = 0,
               dig27 = 0,
               dig28 = 0,
               dig29 = 0,
               dig30 = 0,
               dig31 = 0,
               dig32 = 0,
               dig33 = 0,
               dig34 = 0,
               dig35 = 0,
               dig36 = 0,
               dig37 = 0,
               dig38 = 0,
               dig39 = 0,
               dig40 = 0,
               dig41 = 0,
               dig42 = 0,
               dig43 = 0;

           dig0 = int.Parse(IdNota[0].ToString()) * 4;
           dig1 = int.Parse(IdNota[1].ToString()) * 3;
           dig2 = int.Parse(IdNota[2].ToString()) * 2;
           dig3 = int.Parse(IdNota[3].ToString()) * 9;
           dig4 = int.Parse(IdNota[4].ToString()) * 8;
           dig5 = int.Parse(IdNota[5].ToString()) * 7;
           dig6 = int.Parse(IdNota[6].ToString()) * 6;
           dig7 = int.Parse(IdNota[7].ToString()) * 5;
           dig8 = int.Parse(IdNota[8].ToString()) * 4;
           dig9 = int.Parse(IdNota[9].ToString()) * 3;
           dig10 = int.Parse(IdNota[10].ToString()) * 2;
           dig11 = int.Parse(IdNota[11].ToString()) * 9;
           dig12 = int.Parse(IdNota[12].ToString()) * 8;
           dig13 = int.Parse(IdNota[13].ToString()) * 7;
           dig14 = int.Parse(IdNota[14].ToString()) * 6;
           dig15 = int.Parse(IdNota[15].ToString()) * 5;
           dig16 = int.Parse(IdNota[16].ToString()) * 4;
           dig17 = int.Parse(IdNota[17].ToString()) * 3;
           dig18 = int.Parse(IdNota[18].ToString()) * 2;
           dig19 = int.Parse(IdNota[19].ToString()) * 9;
           dig20 = int.Parse(IdNota[20].ToString()) * 8;
           dig21 = int.Parse(IdNota[21].ToString()) * 7;
           dig22 = int.Parse(IdNota[22].ToString()) * 6;
           dig23 = int.Parse(IdNota[23].ToString()) * 5;
           dig24 = int.Parse(IdNota[24].ToString()) * 4;
           dig25 = int.Parse(IdNota[25].ToString()) * 3;
           dig26 = int.Parse(IdNota[26].ToString()) * 2;
           dig27 = int.Parse(IdNota[27].ToString()) * 9;
           dig28 = int.Parse(IdNota[28].ToString()) * 8;
           dig29 = int.Parse(IdNota[29].ToString()) * 7;
           dig30 = int.Parse(IdNota[30].ToString()) * 6;
           dig31 = int.Parse(IdNota[31].ToString()) * 5;
           dig32 = int.Parse(IdNota[32].ToString()) * 4;
           dig33 = int.Parse(IdNota[33].ToString()) * 3;
           dig34 = int.Parse(IdNota[34].ToString()) * 2;
           dig35 = int.Parse(IdNota[35].ToString()) * 9;
           dig36 = int.Parse(IdNota[36].ToString()) * 8;
           dig37 = int.Parse(IdNota[37].ToString()) * 7;
           dig38 = int.Parse(IdNota[38].ToString()) * 6;
           dig39 = int.Parse(IdNota[39].ToString()) * 5;
           dig40 = int.Parse(IdNota[40].ToString()) * 4;
           dig41 = int.Parse(IdNota[41].ToString()) * 3;
           dig42 = int.Parse(IdNota[42].ToString()) * 2;
           #endregion

           int Total = (dig0 +
               dig1 +
               dig2 +
               dig3 +
               dig4 +
               dig5 +
               dig6 +
               dig7 +
               dig8 +
               dig9 +
               dig10 +
               dig11 +
               dig12 +
               dig13 +
               dig14 +
               dig15 +
               dig16 +
               dig17 +
               dig18 +
               dig19 +
               dig20 +
               dig21 +
               dig22 +
               dig23 +
               dig24 +
               dig25 +
               dig26 +
               dig27 +
               dig28 +
               dig29 +
               dig30 +
               dig31 +
               dig32 +
               dig33 +
               dig34 +
               dig35 +
               dig36 +
               dig37 +
               dig38 +
               dig39 +
               dig40 +
               dig41 +
               dig42) % 11;

           int Digito = 11-Total ;
           int DigitoReal = 0;

           if (Digito == 0 || Digito>9)
           {
               DigitoReal = 0;
           }
           else
           {
               DigitoReal = Digito;
           }

           return DigitoReal.ToString();
       }

       public void FncGravaXML(StringBuilder strXML, String StrCaminho)
       {
           try
           {
               StreamWriter ArquivoErro = new StreamWriter(StrCaminho, false, Encoding.ASCII);
               ArquivoErro.WriteLine(strXML.ToString());
               ArquivoErro.Flush();
               ArquivoErro.Close();

               //XmlDocument XMLDoc = new XmlDocument();

               // XMLDoc.LoadXml(StrXml);
               //XMLDoc.Save(StrCaminho);
           }
           catch
           {

           }
       }

       public void FncGravaXML(String strXML, String StrCaminho)
       {
           try
           {
               FileInfo fi = new FileInfo(StrCaminho);

               if (!fi.Directory.Exists)
                   fi.Directory.Create();

               StreamWriter ArquivoErro = new StreamWriter(StrCaminho, false, Encoding.ASCII);
               ArquivoErro.WriteLine(strXML.ToString());
               ArquivoErro.Flush();
               ArquivoErro.Close();

               //XmlDocument XMLDoc = new XmlDocument();
               // XMLDoc.LoadXml(StrXml);
               //XMLDoc.Save(StrCaminho);
           }
           catch
           {

           }
       }

       public String FUNC_CARACTER_ACENTO(String Str)
       {
           const string StrComAcentos = "ÄÅÁÂÀÃäáâàãÉÊËÈéêëèÍÎÏÌíîïìÖÓÔÒÕöóôòõÜÚÛüúûùÇç'";
           const string StrSemAcentos = "AAAAAAaaaaaEEEEeeeeIIIIiiiiOOOOOoooooUUUuuuuCc ";
           int i = 0;

           try
           {
               foreach (Char c in StrComAcentos)
               {
                   Str = Str.Replace(c.ToString().Trim(), StrSemAcentos[i].ToString().Trim());
                   i++;
               }

           }
           catch
           {
           }

           return Str;
       }

       public String FUNC_CARACTER_ESPECIAL(String Str)
       {
           const string StrComAcentos = "ÄÅÁÂÀÃäáâàãÉÊËÈéêëèÍÎÏÌíîïìÖÓÔÒÕöóôòõÜÚÛüúûùÇç'";
           const string StrSemAcentos = "AAAAAAaaaaaEEEEeeeeIIIIiiiiOOOOOoooooUUUuuuuCc ";
           int i = 0;

           try
           {
               foreach (Char c in StrComAcentos)
               {
                   Str = Str.Replace(c.ToString().Trim(), StrSemAcentos[i].ToString().Trim());
                   i++;
               }

               /*char[] trim = { '=', '\\', ';', '.', ':', ',', '+', '*','&','@','#','\'','\"','!','¨','ª','º','-','(',')','/','%','+',',',']','[','}','{','³'};
               int pos;
               while ((pos = Str.IndexOfAny(trim)) >= 0)
               {
                   Str = Str.Remove(pos, 1);
               }*/

               foreach (Char c in Str)
               {
                   // NEW 28/06/2010 - VALIDAÇÃO POR ASCII!! (INCREMENTADO DE CARACTERES ADICIONAIS!)
                   if ((c >= 1 && c <= 31) || (c >= 34 && c <= 43) || (c >= 45 && c <= 47) || (c >= 58 && c <= 60) || (c == 62) || (c == 92) ||
                        (c >= 94 && c <= 96) || (c >= 127 && c <= 129) || (c >= 131 && c <= 255) || (c >= 256))
                   {
                       Str = Str.Replace(c, '¬');
                   }
               }


               Str = Str.Replace("¬", "");

           }
           catch
           {
           }

           return Str;
       }

       public String FncRetiraCaracteresCampoTexto(String Str)
       {
           const string StrComAcentos = "ÄÅÁÂÀÃäáâàãÉÊËÈéêëèÍÎÏÌíîïìÖÓÔÒÕöóôòõÜÚÛüúûùÇç&'";
           const string StrSemAcentos = "AAAAAAaaaaaEEEEeeeeIIIIiiiiOOOOOoooooUUUuuuuCc  ";
           int i = 0;

           try
           {
               foreach (Char c in StrComAcentos)
               {
                   Str = Str.Replace(c.ToString().Trim(), StrSemAcentos[i].ToString().Trim());
                   i++;
               }

               foreach (Char c in Str)
               {
                   // NEW 28/06/2010 - VALIDAÇÃO POR ASCII!!
                   if ((c >= 1 && c <= 31) || (c == 34) || (c >= 38 && c <= 39) || (c == 60) || (c == 62) || (c >= 94 && c <= 96)
                        || (c >= 127 && c <= 129) || (c >= 131 && c <= 255) || (c >= 256))
                   {
                       Str = Str.Replace(c, '¬');
                   }
               }

               Str = Str.Replace("¬", "");
           }
           catch
           {
           }

           return Str;
       }


       public string FncVerificaTipoEmissao()
       {
           SqlDataReader Dr;
           string tpEmis = "1";

           string Conexao = FncVerificaConexao();

           Dr = SqlHelper.ExecuteReader(Conexao, "stpRetornaParametrosNFE", null);

           if (Dr.Read())
           {
               tpEmis = Dr["tpEmis"] is DBNull ? "1" : Dr["tpEmis"].ToString();
           }

            Dr.Close();
            Dr.Dispose();

           return tpEmis;

       }

       /// <summary>
       /// Validação do Schema
       /// </summary>
       /// <param name="sPathXml"></param>
       /// <param name="sPathSchema"></param>
       /// <returns></returns>
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
    }
}
