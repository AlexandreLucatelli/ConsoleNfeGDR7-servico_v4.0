using System;
using System.Collections.Generic;

using System.Text;
using NFE.Classes.NFE.Objetos.Recepcao.Det.Produto;
using NFE.Classes.NFE.Objetos.Recepcao.Det.Impostos;


namespace NFE.Classes.NFE.Objetos.Recepcao.Det
{
  public class det
    {

      string _nItem;
      string _modelo;
      string _nECF;
      string _nCOO;

      public string nItem
      {
          get { return _nItem; }
          set { _nItem = value; }
      }
      
      Prod _Produto = new Prod();
      public Prod Produto
      {
          get {return _Produto; }
          set { _Produto = value; }
      }

      Imposto _Impostos = new Imposto();

      public Imposto Ipost
      {
          get { return _Impostos; }
          set { _Impostos = value; }
      }

      Ii _II = new Ii();

      public Ii II
      {
          get { return _II; }
          set { _II = value; }
      }

      public string modelo
      {
          get { return _modelo; }
          set { _modelo = value; }
      }

      public string nECF
      {
          get { return _nECF; }
          set { _nECF = value; }
      }

      public string nCOO
      {
          get { return _nCOO; }
          set { _nCOO = value; }
      }
    }
}
