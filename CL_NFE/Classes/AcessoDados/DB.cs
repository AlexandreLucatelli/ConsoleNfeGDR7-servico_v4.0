using System;
using System.Collections.Generic;

using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.ApplicationBlocks.Data;


namespace NFE.Classes.AcessoDados
{
    public class DB
    { 

        public DB()
        {
           
        }      

        protected string FncVerificaConexao()
        {
            string Chave = ConfigurationManager.AppSettings["intConexao"].ToString();
            string Conexao = string.Empty;
            
            switch (Chave)
            {
                case "1" :
                    Conexao = ConfigurationManager.AppSettings["CNN_Desenv"].ToString();
                    //Conexao = @"String  de Conexão";
                    break;
                case "2" :
                    Conexao = ConfigurationManager.AppSettings["CNN_Homologacao"].ToString();
                    break;
                default :
                    Conexao = ConfigurationManager.AppSettings["CNN_Producao"].ToString();
                    break;
            }
            return Conexao;
        }
    }
}
