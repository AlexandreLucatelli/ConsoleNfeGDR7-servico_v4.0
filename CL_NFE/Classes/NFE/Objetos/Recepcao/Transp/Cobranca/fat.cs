using System;
using System.Collections.Generic;

using System.Text;

namespace NFE.Classes.NFE.Objetos.Recepcao.Transp.Cobranca
{

    public class fat
    {
        string _nFat;
        public string nFat
        {
            get { return _nFat; }
            set { _nFat = value; }
        }

        float _vOrig;
        public float vOrig
        {
            get { return _vOrig; }
            set { _vOrig = value; }
        }
        /// <summary>
        /// Valor do Desconto
        /// </summary>
        float _vDesc;
        public float vDesc
        {
            get { return _vDesc; }
            set { _vDesc = value; }
        }
        /// <summary>
        /// Valor Liquido da Fatura
        /// </summary>
        float _vLiq;
        public float vLiq
        {
            get { return _vLiq; }
            set { _vLiq = value; }
        }
    }
}

