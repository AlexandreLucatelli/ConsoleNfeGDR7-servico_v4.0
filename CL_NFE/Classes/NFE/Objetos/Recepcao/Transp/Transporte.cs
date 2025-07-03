using System;
using System.Collections.Generic;

using System.Text;
using NFE.Classes.NFE.Objetos.Recepcao.Transp.Cobranca;

namespace NFE.Classes.NFE.Objetos.Recepcao.Transp
{
    public class Transporte
    {

        int _IdTransportadora;
        public int IdTransportadora
        {
            get { return _IdTransportadora; }
            set { _IdTransportadora = value; }
        }


        int _modFrete;
        public int modFrete
        {
            get { return _modFrete; }
            set { _modFrete = value; }
        }

        Transportadora _TransPortadora = new Transportadora();
        public Transportadora transportadora
        {
            get { return _TransPortadora; }
            set { _TransPortadora = value; }
        }

        retTransp _RetTransp = new retTransp();
        public retTransp RetTransp
        {
            get { return _RetTransp; }
            set { _RetTransp = value; }
        }


        Veictransp _VeicTransp = new Veictransp();
        public Veictransp VeicTransp
        {

            get { return _VeicTransp; }
            set { _VeicTransp = value; }
        }


        reboque _Reboque = new reboque();
        public reboque Reboque
        {

            get { return _Reboque; }
            set { _Reboque = value; }
        }

        vol _Vol = new vol();
        public vol Vol
        {

            get { return _Vol; }
            set { _Vol = value; }
        }

        

        cobr _Cobr = new cobr();
        public cobr Cobr
        {
            get { return _Cobr; }
            set { _Cobr = value; }             
        }

      
    }
}
