using System;
using System.Collections.Generic;

using System.Text;


namespace NFE.Classes.NFE.Objetos.Recepcao.Det.Impostos.ICMs
{
    public class ICMS
    {

        ICMS00 _ICMs00 = new ICMS00();

        public ICMS00 ICMs00
        {
            get { return _ICMs00; }
            set { _ICMs00 = value; }
        }


        ICMS10 _ICMs10 = new ICMS10();
        public ICMS10 ICMs10
        {
            get { return _ICMs10; }
            set { _ICMs10 = value; }
        }


        ICMs20 _ICMs20 = new ICMs20();
        public ICMs20 ICMs20
        {
            get { return _ICMs20; }
            set { _ICMs20 = value; }
        }

        ICMs30 _ICMs30 = new ICMs30();
        public ICMs30 ICMs30
        {
            get { return _ICMs30; }
            set { _ICMs30 = value; }
        }

        ICMs40 _ICMs40 = new ICMs40();
        public ICMs40 ICMs40
        {
            get { return _ICMs40; }
            set { _ICMs40 = value; }
        }


        ICMs51 _ICMs51 = new ICMs51();
        public ICMs51 ICMs51
        {
            get { return _ICMs51; }
            set { _ICMs51 = value; }
        }

        ICMs60 _ICMs60 = new ICMs60();
        public ICMs60 ICMs60
        {
            get { return _ICMs60; }
            set { _ICMs60 = value; }
        }

        ICMs70 _ICMs70 = new ICMs70();
        public ICMs70 ICMs70
        {
            get { return _ICMs70; }
            set { _ICMs70 = value; }
        }

        ICMs90 _ICMs90 = new ICMs90();
        public ICMs90 ICMs90
        {
            get { return _ICMs90; }
            set { _ICMs90 = value; }
        }

       /* float _orig;
        public float orig
        {
            get { return _orig; }
            set { _orig = value; }
        }

        float _modBC;
        public float modBC
        {
            get { return _modBC; }
            set { _modBC = value; }
        }

        float _vBC;
        public float vBC
        {
            get { return _vBC; }
            set { _vBC = value; }
        }


      float _CST;
      public float CST
        {
            get { return _CST; }
            set { _CST = value; }
        }

        float _pICMS;
        public float pICMS
        {
            get { return _pICMS; }
            set { _pICMS = value; }
        }


        float _vICMS;
        public float vICMS
        {
            get { return _vICMS; }
            set { _vICMS = value; }
        }*/

    }
}
