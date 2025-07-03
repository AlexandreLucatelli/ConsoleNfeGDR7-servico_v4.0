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
using System.Security.Cryptography.Xml;

namespace NFE.Classes.NFE.MontaXMl
{
    public class MontaXMLNfe : DB
    {
        protected SqlDataReader Dr;
        public string Conexao;

        public void MontaXML(NFE.Objetos.NotaFiscalEletronica Nfe)
        {

            Conexao = FncVerificaConexao();

            StringBuilder XML = new StringBuilder();
            XML.Append("<?xml version='1.0' encoding='" + ConfigurationManager.AppSettings["CodificacaoNFE"].ToString() + "' ?>");
            //XML.Append("<enviNFe xmlns='" + ConfigurationManager.AppSettings["NameSpaceNFE"].ToString() + "' versao='" + ConfigurationManager.AppSettings["VersaoCabecalhoNFE"].ToString() + "'>");
           // XML.Append("<idLote>" + Nfe.IdLote.ToString() + "</idLote>");
            XML.Append("<NFe xmlns='" + ConfigurationManager.AppSettings["NameSpaceNFE"].ToString() + "'>");
            XML.Append("<infNFe Id='" + Nfe.infNfe + "' versao='1.10'>");

            XML.Append("<ide>");

                XML.Append("<cUF>" + Nfe.Ide.cUF.PadLeft(2,'0') + "</cUF>");
                XML.Append("<cNF>" + Nfe.Ide.cNF.PadLeft(9,'0') + "</cNF>");
                XML.Append("<natOp>" + (Nfe.Ide.natOp.Length > 60 ? Nfe.Ide.natOp.Substring(0, 60) : Nfe.Ide.natOp) + "</natOp>");
                XML.Append("<indPag>" + Nfe.Ide.indPag.ToString() + "</indPag>");
                XML.Append("<mod>" + Nfe.Ide.Mod.PadLeft(2, '0') + "</mod>");
                XML.Append("<serie>" + Nfe.Ide.Serie.PadLeft(3,'0') + "</serie>");
                XML.Append("<nNF>" + Nfe.Ide.nNF.PadLeft(9,'0') + "</nNF>");
                XML.Append("<dEmi>" + Nfe.Ide.dEmi + "</dEmi>");
                XML.Append("<dSaiEnt>" + Nfe.Ide.dSaiEnt + "</dSaiEnt>");
                XML.Append("<tpNF>" + Nfe.Ide.tpNF + "</tpNF>");
                XML.Append("<cMunFG>" + Nfe.Ide.cMunFG.ToString().PadLeft(7,'0') + "</cMunFG>");
                //XML.Append("<tpImp>" + Nfe.Ide.tpImp.ToString() + "</tpImp>");
                //XML.Append("<tpEmis>" + Nfe.Ide.tpEmis + "</tpEmis>");

                XML.Append("<NFref>");
                    XML.Append("<refNF>");
                        XML.Append("<cUF>" + Nfe.RefNf.cUF + "</cUF>");
                        XML.Append("<AAMM>" + Nfe.RefNf.AAMM + "</AAMM>");
                        XML.Append("<CNPJ>" + Nfe.RefNf.CNPJ + "</CNPJ>");
                        XML.Append("<mod>" + Nfe.RefNf.mod + "</mod>");                       
                        XML.Append("<serie>" + Nfe.RefNf.serie + "</serie>");
                        XML.Append("<nNF>" + Nfe.RefNf.nNF + "</nNF>");
                        XML.Append("<tpImp>" + Nfe.RefNf.nNF + "</tpImp>");
                        XML.Append("<cDV>" + Nfe.RefNf.cDV + "</cDV>");
                        XML.Append("<tpAmb>" + Nfe.RefNf.tpAmb + "</tpAmb>");
                        XML.Append("<finNFe>" + Nfe.RefNf.finNFe + "</finNFe>");
                        XML.Append("<verProc>" + Nfe.RefNf.verProc + "</verProc>");
                        XML.Append("</refNF>");
                    XML.Append("</NFref>");

            XML.Append("</ide>");

            XML.Append("<emit>");

                XML.Append("<CNPJ>" + Nfe.Emit.CNPJ.PadLeft(14,'0') + "</CNPJ>");
                XML.Append("<xNome>" + Nfe.Emit.xNome + "</xNome>");
                XML.Append("<xFant>" + Nfe.Emit.xFant + "</xFant>");

                    XML.Append("<enderEmit>");

                        XML.Append("<xLgr>" + Nfe.Emit.EnderEmit.xLgr + "</xLgr>");
                        XML.Append("<nro>" + Nfe.Emit.EnderEmit.nro + "</nro>");
                        if (!(Nfe.Emit.EnderEmit.xCpl == string.Empty))
                        {
                            XML.Append("<xCpl>" + Nfe.Emit.EnderEmit.xCpl + "</xCpl>");
                        }

                        XML.Append("<xBairro>" + Nfe.Emit.EnderEmit.xBairro + "</xBairro>");
                        if (!(Nfe.Emit.EnderEmit.cMun == string.Empty))
                        {
                            XML.Append("<cMun>" + Nfe.Emit.EnderEmit.cMun + "</cMun>");
                            XML.Append("<xMun>" + Nfe.Emit.EnderEmit.xMun + "</xMun>");
                        }
                        XML.Append("<UF>" + Nfe.Emit.EnderEmit.UF + "</UF>");
                        XML.Append("<CEP>" + Nfe.Emit.EnderEmit.CEP + "</CEP>");
                        XML.Append("<cPais>" + Nfe.Emit.EnderEmit.cPais + "</cPais>");
                        XML.Append("<xPais>" + Nfe.Emit.EnderEmit.xPais + "</xPais>");

                        if (!(Nfe.Emit.EnderEmit.fone == string.Empty))
                        {
                            XML.Append("<fone>" + Nfe.Emit.EnderEmit.fone + "</fone>");
                        }

                        XML.Append("<IE>" + Nfe.Emit.EnderEmit.IE + "</IE>");
                      /*  XML.Append("<IEST>" + Nfe.Emit.EnderEmit.IEST + "</IEST>");
                        XML.Append("<IM>" + Nfe.Emit.EnderEmit.IM + "</IM>");
                        XML.Append("<CNAE>" + Nfe.Emit.EnderEmit.CNAE + "</CNAE>");*/

                    XML.Append("</enderEmit>");
            
            XML.Append("</emit>");


            XML.Append("<dest>");

            XML.Append("<CNPJ>" + Nfe.Dest.CNPJ.PadLeft(14, '0') + "</CNPJ>");
                XML.Append("<xNome>" + Nfe.Dest.xNome + "</xNome>");

                    XML.Append("<enderDest>");
            
                        XML.Append("<xLgr>" + Nfe.Dest.EnderDest.xLgr + "</xLgr>");
                        XML.Append("<nro>" + Nfe.Dest.EnderDest.nro + "</nro>");
                        if (!(Nfe.Dest.EnderDest.xCpl == string.Empty))
                        {
                            XML.Append("<xCpl>" + Nfe.Dest.EnderDest.xCpl + "</xCpl>");
                        }

                        XML.Append("<xBairro>" + Nfe.Dest.EnderDest.xBairro + "</xBairro>");

                        if (!(Nfe.Dest.EnderDest.cMun == string.Empty))
                        {
                            XML.Append("<cMun>" + Nfe.Dest.EnderDest.cMun + "</cMun>");
                            XML.Append("<xMun>" + Nfe.Dest.EnderDest.xMun + "</xMun>");
                        }
                        XML.Append("<UF>" + Nfe.Dest.EnderDest.UF + "</UF>");
                        XML.Append("<CEP>" + Nfe.Dest.EnderDest.CEP + "</CEP>");
                        XML.Append("<cPais>" + Nfe.Dest.EnderDest.cPais + "</cPais>");
                        XML.Append("<xPais>" + Nfe.Dest.EnderDest.xPais + "</xPais>");
                        XML.Append("<fone>" + Nfe.Dest.EnderDest.fone + "</fone>");
                        XML.Append("<IE>" + Nfe.Dest.EnderDest.IE + "</IE>");
                      //XML.Append("<ISUF>" + Nfe.Dest.EnderDest.ISUF + "</ISUF>");

                    XML.Append("</enderDest>");

           XML.Append("</dest>");


           XML.Append("<entrega>");
                    XML.Append("<CNPJ>" + Nfe.Entregas.CNPJ + "</CNPJ>");
                    XML.Append("<xLgr>" + Nfe.Entregas.xLgr + "</xLgr>");
                    XML.Append("<nro>" + Nfe.Entregas.nro + "</nro>");
                    if (!(Nfe.Entregas.xLgr == string.Empty))
                    {
                        XML.Append("<xCpl>" + Nfe.Entregas.xLgr + "</xCpl>");
                    }
                    XML.Append("<xBairro>" + Nfe.Entregas.xBairro + "</xBairro>");
                    if (!(Nfe.Entregas.cMun == string.Empty))
                    {
                        XML.Append("<cMun>" + Nfe.Entregas.cMun + "</cMun>");
                        XML.Append("<xMun>" + Nfe.Entregas.xMun + "</xMun>");
                    }
                    XML.Append("<UF>" + Nfe.Entregas.UF + "</UF>");
           XML.Append("</entrega>");


             Dr = SqlHelper.ExecuteReader(Conexao, "stpRetornaTotalItensNf", Nfe.cNF);
             int Contator = 1;

             while (Dr.Read())
             {
                 Nfe.Det.Produto.cProd = Dr["IDProduto"] is DBNull ? "0" : Dr["IDProduto"].ToString();
                 Nfe.Det.Produto.xProd = Dr["Descricao"] is DBNull ? string.Empty : Dr["Descricao"].ToString();                 
                 Nfe.Det.Produto.NCM = Dr["IDNCM"] is DBNull ? string.Empty : Dr["IDNCM"].ToString().PadLeft(8,'0');
                 Nfe.Det.Produto.CFOP = Dr["CFop"] is DBNull ? string.Empty : Dr["CFop"].ToString().PadLeft(4, '0');
                 Nfe.Det.Produto.uCom = Dr["Unidade"] is DBNull ? string.Empty : Dr["Unidade"].ToString();
                 Nfe.Det.Produto.qCom = Dr["Qtde"] is DBNull ? 0 : decimal.Parse(Dr["Qtde"].ToString());
                 Nfe.Det.Produto.vUnCom = Dr["Vlunitario"] is DBNull ? 0 : decimal.Parse(Dr["Vlunitario"].ToString());
                 Nfe.Det.Produto.vProd = Dr["ValorTotal"] is DBNull ? 0 : decimal.Parse(Dr["ValorTotal"].ToString());
                 Nfe.Det.Produto.vFrete = Dr["frete"] is DBNull ? 0 : decimal.Parse(Dr["frete"].ToString());
                 Nfe.Det.Produto.vSeg = Dr["Seguro"] is DBNull ? 0 : decimal.Parse(Dr["Seguro"].ToString());

                 Nfe.Det.nItem = Contator.ToString();

                 XML.Append("<det nItem='" + Nfe.Det.nItem.PadLeft(3,'0') + "'>");

                     XML.Append("<prod>");
                             XML.Append("<cProd>" + Nfe.Det.Produto.cProd + "</cProd>");
                            // XML.Append("<cEAN>" + Nfe.Det.Produto.cEAN + "</cEAN>");
                             XML.Append("<xProd>" + Nfe.Det.Produto.xProd + "</xProd>");
                             XML.Append("<NCM>" + Nfe.Det.Produto.NCM + "</NCM>");
                            // XML.Append("<EXTIPI>" + Nfe.Det.Produto.EXTIPI + "</EXTIPI>");
                            // XML.Append("<genero>" + Nfe.Det.Produto.genero + "</genero>");
                             XML.Append("<CFOP>" + Nfe.Det.Produto.CFOP + "</CFOP>");
                             XML.Append("<uCom>" + Nfe.Det.Produto.uCom + "</uCom>");
                             XML.Append("<qCom>" + Nfe.Det.Produto.qCom + "</qCom>");
                             XML.Append("<vUnCom>" + Nfe.Det.Produto.vUnCom + "</vUnCom>");
                             XML.Append("<vProd>" + Nfe.Det.Produto.vProd + "</vProd>");
                            // XML.Append("<cEANTrib>" + Nfe.Det.Produto.cEANTrib + "</cEANTrib>");                             
                            // XML.Append("<uTrib>" + Nfe.Det.Produto.uTrib + "</uTrib>");
                            // XML.Append("<qTrib>" + Nfe.Det.Produto.qTrib + "</qTrib>");                            
                             XML.Append("<vFrete>" + Nfe.Det.Produto.vFrete + "</vFrete>");
                             XML.Append("<vSeg>" + Nfe.Det.Produto.vSeg + "</vSeg>");
                             XML.Append("<vDesc>" + Nfe.Det.Produto.vDesc + "</vDesc>");
                     XML.Append("</prod>");

                     XML.Append("<imposto>");

                         XML.Append("<ICMS>");

                            XML.Append("<ICMS00>");
                                 XML.Append("<orig>" + Nfe.Det.Ipost.Icms.ICMs00.orig+ "</orig>");
                                 XML.Append("<CST>" + Nfe.Det.Ipost.Icms.ICMs00.vBC + "</CST>");
                                 // XML.Append("<modBC>" + Nfe.Det.Ipost.ICMs00.modBC + "</modBC>");
                                 XML.Append("<vBC>" + Nfe.Det.Ipost.Icms.ICMs00.vBC + "</vBC>");
                                 XML.Append("<pICMS>" + Nfe.Det.Ipost.Icms.ICMs00.pICMS + "</pICMS>");
                                 XML.Append("<vICMS>" + Nfe.Det.Ipost.Icms.ICMs00.vICMS + "</vICMS>");                           
                            XML.Append("</ICMS00>");

                            XML.Append("<ICMS10>");
                                 XML.Append("<orig>" + Nfe.Det.Ipost.Icms.ICMs10.orig + "</orig>");
                               //  XML.Append("<CST>" + Nfe.Det.Ipost.Icms.ICMs10.CST + "</CST>");
                               //  XML.Append("<modBC>" + Nfe.Det.Ipost.ICMs10.modBC + "</modBC>");
                                 XML.Append("<vBC>" + Nfe.Det.Ipost.Icms.ICMs10.vBC + "</vBC>");
                                 XML.Append("<pICMS>" + Nfe.Det.Ipost.Icms.ICMs10.pICMS + "</pICMS>");
                                 XML.Append("<vICMS>" + Nfe.Det.Ipost.Icms.ICMs10.vICMS + "</vICMS>");
                               //  XML.Append("<modBCST>" + Nfe.Det.Ipost.Icms.ICMs10.modBCST + "</modBCST>");
                               //  XML.Append("<pMVAST>" + Nfe.Det.Ipost.Icms.ICMs10.pMVAST + "</pMVAST>");
                               //  XML.Append("<pRedBCST>" + Nfe.Det.Ipost.Icms.ICMs10.pRedBCST + "</pRedBCST>");
                               //  XML.Append("<vBCST>" + Nfe.Det.Ipost.Icms.ICMs10.vBCST + "</vBCST>");
                               //  XML.Append("<pICMSST>" + Nfe.Det.Ipost.Icms.ICMs10.pICMSST + "</pICMSST>");
                               //  XML.Append("<vICMSST>" + Nfe.Det.Ipost.Icms.ICMs10.vICMSST + "</vICMSST>");
                            XML.Append("</ICMS10>");


                            XML.Append("<ICMS20>");

                                   XML.Append("<orig>" + Nfe.Det.Ipost.Icms.ICMs20.orig + "</orig>");
                                   XML.Append("<CST>" + Nfe.Det.Ipost.Icms.ICMs20.CST + "</CST>");
                                   // XML.Append("<modBC>" + Nfe.Det.Ipost.Icms.ICMs20.modBCST + "</modBC>");
                                   // XML.Append("<pRedBC>" + Nfe.Det.Ipost.Icms.ICMs20.pRedBC + "</pRedBC>");
                                   XML.Append("<vBC>" + Nfe.Det.Ipost.Icms.ICMs20.vBC + "</vBC>");
                                   XML.Append("<pICMS>" + Nfe.Det.Ipost.Icms.ICMs20.pICMS + "</pICMS>");
                                   XML.Append("<vICMS>" + Nfe.Det.Ipost.Icms.ICMs20.vICMS + "</vICMS>");

                            XML.Append("</ICMS20>");
                                

                            XML.Append("<ICMS30>");
                                    XML.Append("<orig>" + Nfe.Det.Ipost.Icms.ICMs30.orig + "</orig>");
                                    XML.Append("<CST>" + Nfe.Det.Ipost.Icms.ICMs30.CST + "</CST>");
                                   // XML.Append("<modBCST>" + Nfe.Det.Ipost.Icms.ICMs30.modBCST + "</modBCST>");
                                   // XML.Append("<pMVAST>" + Nfe.Det.Ipost.Icms.ICMs30.pMVAST + "</pMVAST>");
                                   // XML.Append("<pRedBCST>" + Nfe.Det.Ipost.Icms.ICMs30.pRedBCST + "</pRedBCST>");
                                   // XML.Append("<vBCST>" + Nfe.Det.Ipost.Icms.ICMs30.vBCST + "</vBCST>");
                                   // XML.Append("<pICMSST>" + Nfe.Det.Ipost.Icms.ICMs30.pICMSST + "</pICMSST>");
                                   // XML.Append("<vICMSST>" + Nfe.Det.Ipost.Icms.ICMs30.vICMSST + "</vICMSST>");
                            XML.Append("</ICMS30>");

                            XML.Append("<ICMS40>");

                                    XML.Append("<orig>" + Nfe.Det.Ipost.Icms.ICMs40.orig + "</orig>");
                                    XML.Append("<CST>" + Nfe.Det.Ipost.Icms.ICMs40.CST + "</CST>");

                            XML.Append("</ICMS40>");

                            XML.Append("<ICMS51>");

                                    XML.Append("<orig>" + Nfe.Det.Ipost.Icms.ICMs51.orig + "</orig>");
                                    XML.Append("<CST>" + Nfe.Det.Ipost.Icms.ICMs51.CST + "</CST>");
                                    // XML.Append("<modBC>" + Nfe.Det.Ipost.Icms.ICMs51.modBCST + "</modBC>");
                                    // XML.Append("<pRedBC>" + Nfe.Det.Ipost.Icms.ICMs51.pRedBC + "</pRedBC>");
                                    XML.Append("<vBC>" + Nfe.Det.Ipost.Icms.ICMs51.vBC + "</vBC>");
                                    XML.Append("<pICMS>" + Nfe.Det.Ipost.Icms.ICMs51.pICMS + "</pICMS>");
                                    XML.Append("<vICMS>" + Nfe.Det.Ipost.Icms.ICMs51.vICMS + "</vICMS>");

                            XML.Append("</ICMS51>");


                            XML.Append("<ICMS60>");
                                    XML.Append("<orig>" + Nfe.Det.Ipost.Icms.ICMs60.orig + "</orig>");
                                    XML.Append("<CST>" + Nfe.Det.Ipost.Icms.ICMs60.CST + "</CST>");
                                  //  XML.Append("<vBCST>" + Nfe.Det.Ipost.Icms.ICMs60.vBCST + "</vBCST>");
                                  //  XML.Append("<vICMSST>" + Nfe.Det.Ipost.Icms.ICMs60.vICMSST + "</vICMSST>");
                            XML.Append("</ICMS60>");

                            XML.Append("<ICMS70>");
                                    XML.Append("<orig>" + Nfe.Det.Ipost.Icms.ICMs70.orig + "</orig>");
                                    XML.Append("<CST>" + Nfe.Det.Ipost.Icms.ICMs70.CST + "</CST>");
                                   // XML.Append("<modBC>" + Nfe.Det.Ipost.Icms.ICMs70.modBC + "</modBC>");
                                   // XML.Append("<pRedBC>" + Nfe.Det.Ipost.Icms.ICMs70.pRedBC + "</pRedBC>");
                                    XML.Append("<vBC>" + Nfe.Det.Ipost.Icms.ICMs70.vBC + "</vBC>");
                                    XML.Append("<pICMS>" + Nfe.Det.Ipost.Icms.ICMs70.pICMS + "</pICMS>");
                                    XML.Append("<vICMS>" + Nfe.Det.Ipost.Icms.ICMs70.vICMS + "</vICMS>");
                                    //XML.Append("<modBCST>" + Nfe.Det.Ipost.Icms.ICMs70.modBCST + "</modBCST>");
                                    //XML.Append("<pMVAST>" + Nfe.Det.Ipost.Icms.ICMs70.pMVAST + "</pMVAST>");
                                    //XML.Append("<pRedBCST>" + Nfe.Det.Ipost.Icms.ICMs70.pRedBCST + "</pRedBCST>");
                                    //XML.Append("<vBCST>" + Nfe.Det.Ipost.Icms.ICMs70.vBCST + "</vBCST>");
                                    //XML.Append("<pICMSST>" + Nfe.Det.Ipost.Icms.ICMs70.pICMSST + "</pICMSST>");
                                    //XML.Append("<vICMSST>" + Nfe.Det.Ipost.Icms.ICMs70.vICMSST + "</vICMSST>");
                            XML.Append("</ICMS70>");

                            XML.Append("<ICMS90>");
                           
                                    XML.Append("<orig>" + Nfe.Det.Ipost.Icms.ICMs90.orig + "</orig>");
                                    XML.Append("<CST>" + Nfe.Det.Ipost.Icms.ICMs90.CST + "</CST>");
                                    //XML.Append("<modBC>" + Nfe.Det.Ipost.Icms.ICMs90.modBC + "</modBC>");
                                    //XML.Append("<pRedBC>" + Nfe.Det.Ipost.Icms.ICMs90.pRedBC + "</pRedBC>");
                                    XML.Append("<vBC>" + Nfe.Det.Ipost.Icms.ICMs90.vBC + "</vBC>");
                                    XML.Append("<pICMS>" + Nfe.Det.Ipost.Icms.ICMs90.pICMS + "</pICMS>");
                                    XML.Append("<vICMS>" + Nfe.Det.Ipost.Icms.ICMs90.vICMS + "</vICMS>");
                                    //XML.Append("<modBCST>" + Nfe.Det.Ipost.Icms.ICMs90.modBCST + "</modBCST>");
                                    //XML.Append("<pMVAST>" + Nfe.Det.Ipost.Icms.ICMs90.pMVAST + "</pMVAST>");
                                    //XML.Append("<pRedBCST>" + Nfe.Det.Ipost.Icms.ICMs90.pRedBCST + "</pRedBCST>");
                                    //XML.Append("<vBCST>" + Nfe.Det.Ipost.Icms.ICMs90.vBCST + "</vBCST>");
                                    //XML.Append("<pICMSST>" + Nfe.Det.Ipost.Icms.ICMs90.pICMSST + "</pICMSST>");
                                    //XML.Append("<vICMSST>" + Nfe.Det.Ipost.Icms.ICMs90.vICMSST + "</vICMSST>");
                            
                            XML.Append("</ICMS90>");

                         XML.Append("</ICMS>");


                      /*   
                        
                       //Retirado pois não estava no Layout, este bloco foi de uma nota de exemplo tirada
                       //da web e não estava de acordo com o Layout
                       XML.Append("<ICMSST>");
                                 XML.Append("<modBC>" + Nfe.Det.Ipost.Icmsst.modBC + "</modBC>");
                                 XML.Append("<pMVA>" + Nfe.Det.Ipost.Icmsst.pMVA + "</pMVA>");
                                 XML.Append("<pRedBC>" + Nfe.Det.Ipost.Icmsst.pRedBC + "</pRedBC>");
                                 XML.Append("<vBC>" + Nfe.Det.Ipost.Icmsst.vBC + "</vBC>");
                                 XML.Append("<pICMS>" + Nfe.Det.Ipost.Icmsst.pICMS + "</pICMS>");
                                 XML.Append("<vICMS>" + Nfe.Det.Ipost.Icmsst.vICMS + "</vICMS>");
                         XML.Append("</ICMSST>");*/ 

                        
                         XML.Append("<IPI>");
                                //XML.Append("<clEnq>" + Nfe.Det.Ipost.Ipi.clEnq + "</clEnq>");
                                //XML.Append("<CNPJProd>" + Nfe.Det.Ipost.Ipi.CNPJProd + "</CNPJProd>");
                                //XML.Append("<cSelo>" + Nfe.Det.Ipost.Ipi.cSelo + "</cSelo>");
                                //XML.Append("<qSelo>" + Nfe.Det.Ipost.Ipi.qSelo + "</qSelo>");
                                //XML.Append("<cEnq>" + Nfe.Det.Ipost.Ipi.cEnq + "</cEnq>");
                                    XML.Append("<IPITrib>");
                                            //XML.Append("<CST>" + Nfe.Det.Ipost.Ipi.IPITrib.CST + "</CST>");
                                            XML.Append("<vBC>" + Nfe.Det.Ipost.Ipi.IPITrib.vBC + "</vBC>");
                                            //XML.Append("<qUnit>" + Nfe.Det.Ipost.Ipi.IPITrib.qUnid + "</qUnit>");
                                            //XML.Append("<vUnit>" + Nfe.Det.Ipost.Ipi.IPITrib.vUnid + "</vUnit>");
                                            XML.Append("<pIPI>" + Nfe.Det.Ipost.Ipi.IPITrib.pIPI + "</pIPI>");
                                            XML.Append("<vIPI>" + Nfe.Det.Ipost.Ipi.IPITrib.vIPI + "</vIPI>");
                                                    /*
                                                    XML.Append("<IPINT>");
                                                        XML.Append("<CST>" + Nfe.Det.Ipost.Ipi.IPITrib.IPINT.vBC + "</CST>");
                                                    XML.Append("</IPINT>");   
                                                    */
                                    XML.Append("</IPITrib>");
                         XML.Append("</IPI>");

                         //Os Blocos abaixo não Serão utilizados no momento, para utlizar basta descomentar
                         /*XML.Append("<II>");
                                 XML.Append("<vBC>" + Nfe.Det.Ipost.II.vBC + "</vBC>");
                                 XML.Append("<vDespAdu>" + Nfe.Det.Ipost.II.vDespAdu + "</vDespAdu>");
                                 XML.Append("<vII>" + Nfe.Det.Ipost.II.vII + "</vII>");
                                 XML.Append("<vIOF>" + Nfe.Det.Ipost.II.vIOF + "</vIOF>");
                         XML.Append("</II>");


                         XML.Append("<PIS>");
                                 XML.Append("<CST>" + Nfe.Det.Ipost.Pis.CST + "</CST>");
                                 XML.Append("<vBC>" + Nfe.Det.Ipost.Pis.vBC + "</vBC>");
                                 XML.Append("<pPIS>" + Nfe.Det.Ipost.Pis.pPIS + "</pPIS>");
                                 XML.Append("<vPis>" + Nfe.Det.Ipost.Pis.vPis + "</vPis>");
                         XML.Append("</PIS>");

                         XML.Append("<COFINS>");
                                 XML.Append("<CST>" + Nfe.Det.Ipost.Cofins.CST + "</CST>");
                                 XML.Append("<vBC>" + Nfe.Det.Ipost.Cofins.vBC + "</vBC>");
                                 XML.Append("<pCOFINS>" + Nfe.Det.Ipost.Cofins.pCOFINS + "</pCOFINS>");
                                 XML.Append("<vCOFINS>" + Nfe.Det.Ipost.Cofins.vCOFINS + "</vCOFINS>");
                         XML.Append("</COFINS>");*/

                     XML.Append("</imposto>");

                 XML.Append("</det>");

                 Contator++;

             }

           XML.Append("<total>");

                    XML.Append("<ICMSTot>");
                           XML.Append("<vBC>" + Nfe.IcmsTot.vBC + "</vBC>");
                           XML.Append("<vICMS>" + Nfe.IcmsTot.vICMS + "</vICMS>");
                           XML.Append("<vBCST>" + Nfe.IcmsTot.vBCST + "</vBCST>");
                           XML.Append("<vST>" + Nfe.IcmsTot.vST + "</vST>");
                           XML.Append("<vProd>" + Nfe.IcmsTot.vProd + "</vProd>");
                           XML.Append("<vFrete>" + Nfe.IcmsTot.vFrete + "</vFrete>");
                           XML.Append("<vSeg>" + Nfe.IcmsTot.vSeg + "</vSeg>");
                           XML.Append("<vDesc>" + Nfe.IcmsTot.vDesc + "</vDesc>");
                           XML.Append("<vII>" + Nfe.IcmsTot.vII + "</vII>");
                           XML.Append("<vIPI>" + Nfe.IcmsTot.vIPI + "</vIPI>");
                           XML.Append("<vPIS>" + Nfe.IcmsTot.vPIS + "</vPIS>");
                           XML.Append("<vCOFINS>" + Nfe.IcmsTot.vCOFINS + "</vCOFINS>");
                           XML.Append("<vOutro>" + Nfe.IcmsTot.vOutro + "</vOutro>");
                           XML.Append("<vNF>" + Nfe.IcmsTot.vNF + "</vNF>");
                                
                              /*  XML.Append("<ISSQNtot>");

                                    XML.Append("<vServ>" + Nfe.IcmsTot.ISSQNtot.vServ + "</vServ>");
                                    XML.Append("<vBC>" + Nfe.IcmsTot.ISSQNtot.vBC+ "</vBC>");
                                    XML.Append("<vISS>" + Nfe.IcmsTot.ISSQNtot.vISS + "</vISS>");
                                    XML.Append("<vPIS>" + Nfe.IcmsTot.ISSQNtot.vPIS + "</vPIS>");
                                    XML.Append("<vCOFINS>" + Nfe.IcmsTot.ISSQNtot.vCOFINS + "</vCOFINS>");

                                XML.Append("</ISSQNtot>");

                                XML.Append("<retTrib>");
                                    XML.Append("<vRetPIS>" + Nfe.IcmsTot.Retrib.vRetPIS + "</vRetPIS>");
                                    XML.Append("<vRetCOFINS>" + Nfe.IcmsTot.Retrib.vRetCOFINS + "</vRetCOFINS>");
                                    XML.Append("<vRetCSLL>" + Nfe.IcmsTot.Retrib.vRetCSLL + "</vRetCSLL>");
                                    XML.Append("<vBCIRRF>" + Nfe.IcmsTot.Retrib.vBCIRRF + "</vBCIRRF>");
                                    XML.Append("<vBCRetPrev>" + Nfe.IcmsTot.Retrib.vBCRetPrev + "</vBCRetPrev>");
                                    XML.Append("<vRetPrev>" + Nfe.IcmsTot.Retrib.vRetPrev + "</vRetPrev>");
                                XML.Append("</retTrib>");*/

                    XML.Append("</ICMSTot>");

           XML.Append("</total>");

           XML.Append("<transp>");
                        XML.Append("<modFrete>" + Nfe.TRansp.modFrete + "</modFrete>");

                            XML.Append("<transportadora>");
                                XML.Append("<CNPJ>" + Nfe.TRansp.transportadora.CNPJ + "</CNPJ>");
                                XML.Append("<xNome>" + Nfe.TRansp.transportadora.xNome + "</xNome>");
                                XML.Append("<IE>" + Nfe.TRansp.transportadora.IE + "</IE>");
                                XML.Append("<xEnder>" + Nfe.TRansp.transportadora.xEnder + "</xEnder>");
                                XML.Append("<xMun>" + Nfe.TRansp.transportadora.xMun + "</xMun>");
                                XML.Append("<UF>" + Nfe.TRansp.transportadora.UF + "</UF>");
                            XML.Append("</transportadora>");

                            XML.Append("<retTransp>");
                                XML.Append("<vServ>" + Nfe.TRansp.RetTransp.vServ + "</vServ>");
                                //XML.Append("<vBCRet>" + Nfe.TRansp.RetTransp.vBCRet + "</vBCRet>");
                                //XML.Append("<pICMSRet>" + Nfe.TRansp.RetTransp.pICMSRet + "</pICMSRet>");
                                //XML.Append("<vICMSRet>" + Nfe.TRansp.RetTransp.vICMSRet + "</vICMSRet>");
                                //XML.Append("<CFOP>" + Nfe.TRansp.RetTransp.CFOP + "</CFOP>");
                                //XML.Append("<cMunFG>" + Nfe.TRansp.RetTransp.cMunFG + "</cMunFG>");
                            XML.Append("</retTransp>");

                            XML.Append("<veicTransp>");
                            XML.Append("<placa>" + Nfe.TRansp.VeicTransp.placa.Replace(" ", "") + "</placa>");
                                XML.Append("<UF>" + Nfe.TRansp.VeicTransp.UF + "</UF>");
                                //XML.Append("<RNTC>" + Nfe.TRansp.VeicTransp.RNTC + "</RNTC>");
                            XML.Append("</veicTransp>");

                            /* Caso necessite utilizar o bloco reboque é só descomentar e preecher as propriedades
                            XML.Append("<reboque>");
                                XML.Append("<placa>" + Nfe.TRansp.Reboque.placa.Replace(" ", "") + "</placa>");
                                XML.Append("<UF>" + Nfe.TRansp.Reboque.UF + "</UF>");
                                XML.Append("<RNTC>" + Nfe.TRansp.Reboque.RNTC + "</RNTC>");
                            XML.Append("</reboque>");
                             */

                            XML.Append("<vol>");
                                XML.Append("<qVol>" + Nfe.TRansp.Vol.qVol + "</qVol>");
                                XML.Append("<esp>" + Nfe.TRansp.Vol.esp + "</esp>");
                                XML.Append("<marca>" + Nfe.TRansp.Vol.marca + "</marca>");
                                XML.Append("<nVol>" + Nfe.TRansp.Vol.nVol + "</nVol>");
                                XML.Append("<pesoL>" + Nfe.TRansp.Vol.pesoL + "</pesoL>");
                                XML.Append("<pesoB>" + Nfe.TRansp.Vol.pesoB + "</pesoB>");

                            /* Caso necessite utilizar o bloco lacres é só descomentar e preecher as propriedades
                                    XML.Append("<lacres>");
                                        XML.Append("<nLacre>" + Nfe.TRansp.Vol.pesoB + "</nLacre>");
                                    XML.Append("</lacres>");*/

                            XML.Append("</vol>");


                            XML.Append("<cobr>");
                                XML.Append("<fat>");
                                    XML.Append("<nFat>" + Nfe.TRansp.Cobr.Fat.nFat + "</nFat>");
                                    XML.Append("<vOrig>" + Nfe.TRansp.Cobr.Fat.vOrig + "</vOrig>");
                                    //XML.Append("<vDesc>" + Nfe.TRansp.Cobr.Fat.vDesc + "</vDesc>");
                                    //XML.Append("<vLiq>" + Nfe.TRansp.Cobr.Fat.vLiq + "</vLiq>");
                                XML.Append("</fat>");

                                XML.Append("<dup>");
                                    XML.Append("<nDup>" + Nfe.TRansp.Cobr.DUP.nDup + "</nDup>");
                                    XML.Append("<dVenc>" + Nfe.TRansp.Cobr.DUP.dVenc + "</dVenc>");
                                    XML.Append("<vDup>" + Nfe.TRansp.Cobr.DUP.dVenc + "</vDup>");
                                XML.Append("</dup>");
                            XML.Append("</cobr>");

           XML.Append("</transp>");

           XML.Append("</infNFe>");
           XML.Append("</NFe>");
          // XML.Append("</enviNFe>");



           Assinatura.AssinaXML ObjXml = new global::NFE.Classes.NFE.Assinatura.AssinaXML();

            X509Certificate2 Cert = new X509Certificate2("C:/Certificados/certificado_nfe.pfx", "inxp1008");      
            //oNFE.ClientCertificates.Add(Cert);
            string h = ObjXml.FncAssinarXML(XML.ToString(), "infNFe", Cert);
        
        }
    }
}
