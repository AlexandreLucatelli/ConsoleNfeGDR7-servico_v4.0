using System;
using System.Collections.Generic;

using System.Text;
using Microsoft.ApplicationBlocks.Data;
using NFE.Classes.AcessoDados;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using NFE.Classes.Util;

namespace NFE.Classes.NFE.Objetos
{

 
   public class CarregaObjNfe : DB
    {
       
       public string Conexao ;
       protected SqlDataReader Dr;
       protected Utils oUtil = new Utils();
       protected StringBuilder IdNota = new StringBuilder();

       public NotaFiscalEletronica FncCarregaObjNfe(NotaFiscalEletronica oNfe)
       {
           Conexao =FncVerificaConexao();

           StringBuilder XML = new StringBuilder();


           int IDCLIENTE = 0;
           try
           {

               oNfe.IdLote = "001";

               #region Parte Responsável por formar o ID do cabeçalho da Nota e acrescentar o digito no mesmo

                       Dr = SqlHelper.ExecuteReader(Conexao, "stpRetornaParametrosNFE", null);

                       if (Dr.Read())
                       {
                           oNfe.cUf = Dr["Cuf"] is DBNull ? "0" : Dr["Cuf"].ToString();
                           oNfe.CNPJ = Dr["cnpj"] is DBNull ? "0".PadLeft(14,'0') : Dr["cnpj"].ToString();
                           oNfe.nNF = Dr["nNf"] is DBNull ? "1" : Dr["nNf"].ToString();
                           oNfe.mod = Dr["mod"] is DBNull ? "0" : Dr["mod"].ToString();

                           oNfe.Det.Ipost.Pis.CST = Dr["PisCST"] is DBNull ? "01" : Dr["PisCST"].ToString();
                           oNfe.Det.Ipost.Cofins.CST = Dr["CofinsCST"] is DBNull ? "01" : Dr["CofinsCST"].ToString();


                           /*     O Campo ICMS 00 tem os valroes abaixo
                                  0 – Nacional;
                                  1 – Estrangeira – Importação direta;
                                  2 – Estrangeira – Adquirida no
                                  mercado interno.*/

                           oNfe.Det.Ipost.Icms.ICMs00.orig = Dr["ICMSOrg"] is DBNull ? "0" : Dr["ICMSOrg"].ToString();                                  
                           oNfe.Det.Ipost.Icms.ICMs00.CST = Dr["ICMSCst"] is DBNull ? "00" : Dr["ICMSCst"].ToString();
                           oNfe.Det.Ipost.Icms.ICMs00.modBC = Dr["ModBC"] is DBNull ? "0" : Dr["ModBC"].ToString();

                   
                           oNfe.RefNf.cUF = Dr["ICMSorg"] is DBNull ? "0" : Dr["ICMSorg"].ToString();
                           oNfe.RefNf.CNPJ = Dr["cnpj"] is DBNull ? string.Empty : Dr["cnpj"].ToString();
                           oNfe.RefNf.tpImp = Dr["tpImp"] is DBNull ? "1" : Dr["tpImp"].ToString();
                           oNfe.RefNf.tpAmb = ConfigurationManager.AppSettings["Ambiente"].ToString();
                           oNfe.RefNf.finNFe = Dr["finNfe"] is DBNull ? "1" : Dr["finNfe"].ToString();
                           oNfe.RefNf.procEmi = Dr["procEmiss"] is DBNull ? "0" : Dr["procEmiss"].ToString();
                           oNfe.RefNf.verProc = "Inoxplasma NFe";
                           oNfe.RefNf.AAMM = DateTime.Now.ToString("yyyy-MM-dd");

                           
                           oNfe.Ide.Mod = Dr["mod"] is DBNull ? "55" : Dr["mod"].ToString();
                           oNfe.Ide.tpImp = Dr["tpImp"] is DBNull ? 1 : int.Parse(Dr["tpImp"].ToString());
                           oNfe.Ide.tpEmis = Dr["tpEmis"] is DBNull ? "1" : Dr["tpEmis"].ToString();
                           oNfe.Ide.tpAmb = ConfigurationManager.AppSettings["Ambiente"].ToString();
                           oNfe.Ide.finNFe= Dr["finNfe"] is DBNull ? "1" : Dr["finNfe"].ToString();
                           oNfe.Ide.procEmi = Dr["procEmiss"] is DBNull ? "0" : Dr["procEmiss"].ToString();
                           oNfe.Ide.dSaiEnt = DateTime.Now.ToString("yyyy-MM-dd");
                           oNfe.Ide.verProc = "NF-eletronica.com";
                           oNfe.Ide.cUF = Dr["Cuf"] is DBNull ? "0" : Dr["Cuf"].ToString();
                           oNfe.Ide.tpNF = Dr["tpNf"] is DBNull ? 0 : int.Parse(Dr["tpNf"].ToString());
                           oNfe.Ide.cMunFG = Dr["cMunFg"] is DBNull ? 0 : int.Parse(Dr["cMunFg"].ToString());
                           oNfe.Ide.tpImp = Dr["tpImp"] is DBNull ? 1 : int.Parse(Dr["tpImp"].ToString());
                           oNfe.Ide.tpEmis = Dr["tpEmis"] is DBNull ? "1" : Dr["tpEmis"].ToString();
                           oNfe.Ide.indPag = Dr["IndPag"] is DBNull ? 0 : int.Parse(Dr["IndPag"].ToString());
                           oNfe.Ide.nNF = Dr["nNf"] is DBNull ? "1" : Dr["nNf"].ToString();
                           oNfe.Ide.verProc= "Inoxplasma NFe";
                          
                             
                           oNfe.Emit.EnderEmit.cMun = Dr["Cuf"] is DBNull ? "0" : Dr["Cuf"].ToString();                           
                           oNfe.Emit.EnderEmit.xPais = Dr["DescPais"] is DBNull ? string.Empty : Dr["DescPais"].ToString();
                           oNfe.Emit.EnderEmit.cPais = Dr["CodPais"] is DBNull ? string.Empty : Dr["CodPais"].ToString();
                           
                           oNfe.Dest.EnderDest.xPais = Dr["DescPais"] is DBNull ? string.Empty : Dr["DescPais"].ToString();
                           oNfe.Dest.EnderDest.cPais = Dr["CodPais"] is DBNull ? string.Empty : Dr["CodPais"].ToString();                                                    
                       }

                       Dr.Close();

                       oNfe.AAMM = DateTime.Now.ToString("yyMM");                         
         
                       Dr = SqlHelper.ExecuteReader(Conexao, "stpRetornaNotaFiscal", null);                              

                       if(Dr.Read())
                       {
                           // oNfe.nNF = Dr["Numero"] is DBNull ? "0" : Dr["Numero"].ToString();
                           oNfe.cNF = Dr["IdNota"] is DBNull ? "0" : Dr["IdNota"].ToString();
                           oNfe.serie = Dr["serie"] is DBNull ? "0" : Dr["serie"].ToString();
                           //

                           oNfe.RefNf.mod = Dr["modelo"] is DBNull ? string.Empty : Dr["modelo"].ToString();
                           oNfe.RefNf.serie = Dr["serie"] is DBNull ? string.Empty : Dr["serie"].ToString();
                           oNfe.RefNf.nNF = Dr["Numero"] is DBNull ? "0" : Dr["Numero"].ToString().PadLeft(9,'0');


                           oNfe.TRansp.modFrete = Dr["modFrete"] is DBNull ? 0 : int.Parse(Dr["modFrete"].ToString());
                           oNfe.TRansp.IdTransportadora = Dr["idTransportadora"] is DBNull ? 0 : int.Parse(Dr["idTransportadora"].ToString());
                           oNfe.TRansp.VeicTransp.placa = Dr["PlacaVeic"] is DBNull ? "0" : Dr["PlacaVeic"].ToString();
                           oNfe.TRansp.VeicTransp.UF = Dr["UfVeiculo"] is DBNull ? "0" : Dr["UfVeiculo"].ToString();

                           
                           oNfe.TRansp.Vol.qVol = Dr["Volumes"] is DBNull ? "0" : Dr["Volumes"].ToString();
                           oNfe.TRansp.Vol.esp = Dr["Especie"] is DBNull ? "Volumes" : Dr["Especie"].ToString();
                           oNfe.TRansp.Vol.marca = Dr["Marca"] is DBNull ? string.Empty : Dr["Marca"].ToString();
                           oNfe.TRansp.Vol.pesoL = Dr["PesoLiquido"] is DBNull ? 0 : decimal.Parse(Dr["PesoLiquido"].ToString());
                           oNfe.TRansp.Vol.pesoB = Dr["PesoBruto"] is DBNull ? 0 : decimal.Parse(Dr["PesoBruto"].ToString());
                        

                           IDCLIENTE = Dr["IdCliente"] is DBNull ? 0 : int.Parse(Dr["IdCliente"].ToString());

                                                  
                         //  oNfe.Ide.nNF = Dr["Numero"] is DBNull ? "0" : Dr["Numero"].ToString();
                           oNfe.Ide.natOp = Dr["DescNaturezaOP"] is DBNull ? "0" : Dr["DescNaturezaOP"].ToString();                                                      
                           oNfe.Ide.Serie = Dr["serie"] is DBNull ? "0" : Dr["serie"].ToString();
                           oNfe.Ide.dEmi = DateTime.Now.ToString("yyyy-MM-dd");
                           oNfe.Ide.cNF = Dr["IdNota"] is DBNull ? "0" : Dr["IdNota"].ToString();  

                       }

                      IdNota.Append(
                                            oNfe.cUf.PadLeft(2, '0')    +
                                            oNfe.AAMM +
                                            oNfe.CNPJ.PadLeft(14, '0')  +
                                            oNfe.serie.PadLeft(3, '0')  +
                                            oNfe.mod.PadLeft(2,   '0')    +
                                            oNfe.nNF.PadLeft(9,   '0')    +
                                            oNfe.cNF.PadLeft(9,   '0')                                               
                                            );

                           // Para teste Numero do Manual IdNota.Append("5206043300991100250655012000000780026730161");
                           //IdNota.Append("5206043300991100250655012000000780026730161");
                           // IdNota.Append("3509036742311100018100201000065471000000014");


                           IdNota.Append(oUtil.FncRetornaDigitoNota(IdNota));
                           oNfe.infNfe ="NFe" + IdNota.ToString();
                           oNfe.RefNf.cDV = oNfe.infNfe.Substring(oNfe.infNfe.Length - 1, 1);

                           oNfe.Ide.cDV = oNfe.infNfe.Substring(oNfe.infNfe.Length - 1, 1);
                          
                       Dr.Close();


                     //  oNfe.Det.nItem = "001";



               #endregion


               #region Parte Responsável selecionar os dados da empresa Emissora

                       Dr = SqlHelper.ExecuteReader(Conexao, "stpRetornaDadosEmpresa", null);

                       if (Dr.Read())
                       {
                           oNfe.Emit.CNPJ = Dr["CNPJ"] is DBNull ? "0" : Dr["CNPJ"].ToString().Replace(".","").Replace("/","").PadLeft(14,'0').Replace("-","");                           
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
                           oNfe.Emit.EnderEmit.fone = Dr["Telefone"] is DBNull ? string.Empty : Dr["Telefone"].ToString().Replace(".", "").Replace("/", "").Replace("(", "").Replace(")", "").Replace(" ","").Trim();
                       }
                       Dr.Close();

               #endregion



               #region Parte Responsável selecionar os dados da empresa recebedora

                       Dr = SqlHelper.ExecuteReader(Conexao, "stpRetornaDadoCliente", IDCLIENTE);

                  if (Dr.Read())
                  {
                      oNfe.Dest.CNPJ = Dr["CNPJ"] is DBNull ? "0" : Dr["CNPJ"].ToString().Replace(".", "").Replace("/", "").PadLeft(14, '0').Replace("-", "");
                      oNfe.Dest.xNome = Dr["Empresa"] is DBNull ? string.Empty : Dr["Empresa"].ToString();
                      oNfe.Dest.EnderDest.IE = Dr["IE"] is DBNull ? string.Empty : Dr["IE"].ToString();
                      oNfe.Dest.EnderDest.xLgr = Dr["Flogradouro"] is DBNull ? string.Empty : (Dr["Flogradouro"].ToString().Length > 60 ? Dr["Flogradouro"].ToString().Substring(0, 60) : Dr["Flogradouro"].ToString().Trim());
                      oNfe.Dest.EnderDest.nro = Dr["FNumero"] is DBNull ? string.Empty : Dr["FNumero"].ToString();
                      oNfe.Dest.EnderDest.xBairro = Dr["FBairro"] is DBNull ? string.Empty : (Dr["FBairro"].ToString().Length > 60 ? Dr["FBairro"].ToString().Substring(0, 60) : Dr["FBairro"].ToString().Trim());
                      oNfe.Dest.EnderDest.cMun = Dr["IDMunicipioF"] is DBNull ? "0" : Dr["IDMunicipioF"].ToString().PadLeft(7, '0');
                      oNfe.Dest.EnderDest.xMun = Dr["FMunicipio"] is DBNull ? string.Empty : (Dr["FMunicipio"].ToString().Length > 60 ? Dr["FMunicipio"].ToString().Substring(0, 60) : Dr["FMunicipio"].ToString().Trim());
                      oNfe.Dest.EnderDest.UF = Dr["FUF"] is DBNull ? string.Empty : Dr["FUF"].ToString().Length > 2 ? Dr["FUF"].ToString().Substring(0, 2) : Dr["FUF"].ToString().Trim();
                      oNfe.Dest.EnderDest.CEP = Dr["FCEP"] is DBNull ? string.Empty : Dr["FCEP"].ToString().Replace("-", "").Length > 8 ? Dr["FCEP"].ToString().Replace("-", "").Substring(0, 8) : Dr["FCEP"].ToString().Replace("-", "").Trim();
                      oNfe.Dest.EnderDest.xCpl = Dr["FComplemento"] is DBNull ? string.Empty : (Dr["FComplemento"].ToString().Length > 60 ? Dr["FComplemento"].ToString().Trim().Substring(0, 60) : Dr["FComplemento"].ToString());
                      oNfe.Dest.EnderDest.fone = Dr["Telefone"] is DBNull ? string.Empty : Dr["Telefone"].ToString().Replace(".", "").Replace("/", "").Replace("(", "").Replace(")", "").Replace("/", "").Replace("-", "").Replace(" ", "").Trim();

                      oNfe.Entregas.CNPJ =Dr["CNPJ"] is DBNull ? "0" : Dr["CNPJ"].ToString().Replace(".", "").Replace("/", "").PadLeft(14, '0').Replace("-", "");
                      oNfe.Entregas.xLgr = Dr["ELogradouro"] is DBNull ? string.Empty : (Dr["ELogradouro"].ToString().Length > 60 ? Dr["ELogradouro"].ToString().Substring(0, 60) : Dr["ELogradouro"].ToString().Trim());
                      oNfe.Entregas.nro = Dr["ENumero"] is DBNull ? string.Empty : Dr["ENumero"].ToString();
                      oNfe.Entregas.xBairro = Dr["EBairro"] is DBNull ? string.Empty : (Dr["EBairro"].ToString().Length > 60 ? Dr["EBairro"].ToString().Substring(0, 60) : Dr["EBairro"].ToString().Trim());
                      oNfe.Entregas.cMun = Dr["IDMunicipioE"] is DBNull ? string.Empty : Dr["IDMunicipioE"].ToString().PadLeft(7, '0');
                      oNfe.Entregas.xMun = Dr["EMunicipio"] is DBNull ? string.Empty : (Dr["EMunicipio"].ToString().Length > 60 ? Dr["EMunicipio"].ToString().Substring(0, 60) : Dr["EMunicipio"].ToString().Trim());
                      oNfe.Entregas.UF = Dr["EUF"] is DBNull ? string.Empty : Dr["EUF"].ToString().Length > 2 ? Dr["EUF"].ToString().Substring(0, 2) : Dr["EUF"].ToString().Trim();
                      
                  }
                  Dr.Close();
               #endregion
           }
           catch { }

           return oNfe;
       }
    }
}
