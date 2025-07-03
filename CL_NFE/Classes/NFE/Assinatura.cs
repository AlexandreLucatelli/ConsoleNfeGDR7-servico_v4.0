using System;
using System.Collections.Generic;

using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Security.Permissions;
using System.Xml;

namespace NFE.Classes.NFE.Assinatura 
{

    public class AssinaXML 
    {
       
        private XmlDocument XMLDoc;

        public XmlDocument XMLDocAssinado
        {
            get { return XMLDoc; }
        }

        public string XMLStringAssinado
        {
            get { return XMLDoc.OuterXml; }
        }
        
        public string FncAssinarXML(string XML, string RefUri, X509Certificate2 X509Cert)
        {

            /*
            *     Entradas:
            *         XMLString: string XML a ser assinada
            *         RefUri   : Referência da URI a ser assinada (Ex. infNFe
            *         X509Cert : certificado digital a ser utilizado na assinatura digital
            * 
            *     Retornos:
            *         Assinar : 0 - Assinatura realizada com sucesso
            *                   1 - Erro: Problema ao acessar o certificado digital - %exceção%
            *                   2 - Problemas no certificado digital
            *                   3 - XML mal formado + exceção
            *                   4 - A tag de assinatura %RefUri% inexiste
            *                   5 - A tag de assinatura %RefUri% não é unica
            *                   6 - Erro Ao assinar o documento - ID deve ser string %RefUri(Atributo)%
            *                   7 - Erro: Ao assinar o documento - %exceção%
            * 
            *         XMLStringAssinado : string XML assinada
            * 
            *         XMLDocAssinado    : XMLDocument do XML assinado
            */


            int Resultado = 0;
            string MSG = string.Empty;

            try
            {
                string _xnome = "";
                if (X509Cert != null)
                {
                    _xnome = X509Cert.Subject.ToString();
                }

                string x;
                x = X509Cert.GetKeyAlgorithm().ToString();
                XmlDocument Doc = new XmlDocument();

                //Formata o Documento e Ignora os espaços
                Doc.PreserveWhitespace = true;

                try
                {
                    //Carrega o XML Passado usando este nome
                    Doc.LoadXml(XML);

                    // Verifica se a tag a ser assinada existe é única
                    int qtdeRefUri = Doc.GetElementsByTagName(RefUri).Count;

                    if (qtdeRefUri == 0)
                    {
                        //  a URI indicada não existe
                        Resultado = 4;
                        MSG = "A tag de assinatura " + RefUri.Trim() + " inexiste";
                    }

                    else
                    {
                        if (qtdeRefUri > 1)
                        {
                            // existe mais de uma URI indicada
                            Resultado = 5;
                            MSG = "A tag de assinatura " + RefUri.Trim() + " não é unica";
                        }

                        else
                        {
                            try
                            {
                                // Create a SignedXml object.
                                SignedXml signedXml = new SignedXml(Doc);                                

                                // Add the key to the SignedXml document 
                                //  signedXml.SigningKey = _X509Cert.PrivateKey;
                                signedXml.SigningKey = X509Cert.PrivateKey;

                                // Create a reference to be signed
                                Reference reference = new Reference();
                                // pega o uri que deve ser assinada
                                XmlAttributeCollection _Uri = Doc.GetElementsByTagName(RefUri).Item(0).Attributes;
                                foreach (XmlAttribute _atributo in _Uri)
                                {
                                    if (_atributo.Name == "Id")
                                    {
                                        reference.Uri = "#" + _atributo.InnerText;
                                    }
                                }

                                // Add an enveloped transformation to the reference.
                                XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
                                reference.AddTransform(env);

                                XmlDsigC14NTransform c14 = new XmlDsigC14NTransform();
                                reference.AddTransform(c14);

                                // Add the reference to the SignedXml object.
                                signedXml.AddReference(reference);

                                // Create a new KeyInfo object
                                KeyInfo keyInfo = new KeyInfo();

                                // Load the certificate into a KeyInfoX509Data object
                                // and add it to the KeyInfo object.
                                //keyInfo.AddClause(new KeyInfoX509Data(_X509Cert));
                                keyInfo.AddClause(new KeyInfoX509Data(X509Cert));

                                // Add the KeyInfo object to the SignedXml object.
                                signedXml.KeyInfo = keyInfo;

                                signedXml.ComputeSignature();

                                // Get the XML representation of the signature and save
                                // it to an XmlElement object.
                                XmlElement xmlDigitalSignature = signedXml.GetXml();

                                // Append the element to the XML document.
                              
                                Doc.DocumentElement.AppendChild(Doc.ImportNode(xmlDigitalSignature, true));
                                XMLDoc = new XmlDocument();
                                XMLDoc.PreserveWhitespace = false;
                                XMLDoc = Doc;
                                //XMLDoc.Save(@"C:/Certificados/NFETeste.xml");
                            }
                            catch (Exception caught)
                            {
                                Resultado = 7;
                                MSG = "Erro: Ao assinar o documento - " + caught.Message;
                            }
                        }
                    }
                }
                catch (Exception caught)
                {
                    Resultado = 3;
                    MSG = "Erro: XML mal formado - " + caught.Message;
                }
                //  }
            }
            catch (Exception caught)
            {
                Resultado = 1;
                MSG = "Erro: Problema ao acessar o certificado digital" + caught.Message;
            }

            //return Resultado;
            return XMLDoc.InnerXml;
        }
    }
}

