using Grafo.Observer;
using System.Collections.Generic;
using System.Linq;

namespace Grafo.Grafo
{
    public class Vertice<K, T> : Sujeto
    {
        public Vertice(K clave, T valor)
        {
            this.Clave = clave;
            this.Valor = valor;
        }

        public int Id { get; } = idGenerator++;

        public K Clave { get; }
        public T Valor { get; }
        public bool Visitado { get; set; }
        public int VisitadoPor { get; set; }

        public List<Arista<K,T>> Aristas { get;} = new List<Arista<K, T>>();

        //Boruvka
        public Vertice<K, T> Parent { get; internal set; }
        public int Rank { get; internal set; }
        //
    }
}
