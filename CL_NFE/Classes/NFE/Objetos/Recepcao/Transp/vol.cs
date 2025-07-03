using System;
using System.Collections.Generic;

using System.Text;

namespace NFE.Classes.NFE.Objetos.Recepcao.Transp
{
    public class vol
    {
        string _qVol;
        public string qVol
        {
            get { return _qVol; }
            set { _qVol = value; }
        }

        string _esp;
        public string esp
        {
            get { return _esp; }
            set { _esp = value; }
        }

        string _marca;
        public string marca
        {
            get { return _marca; }
            set { _marca = value; }
        }

        string _nVol=string.Empty;
        public string nVol
        {
            get { return _nVol; }
            set { _nVol = value; }
        }

        decimal _pesoL;
        public decimal pesoL
        {
            get { return _pesoL; }
            set { _pesoL = value; }
        }

        decimal _pesoB;
        public decimal pesoB
        {
            get { return _pesoB; }
            set { _pesoB = value; }
        }

        lacres _Lacres = new lacres();
        public lacres Lacres
        {

            get { return _Lacres; }
            set { _Lacres = value; }
        }
       
    }
}
