using Grafo.Observer;
using System.Collections.Generic;
using System.Linq;

namespace Grafo.Grafo
{
    public class Arista<K, T> : Sujeto
    {
        public Vertice<K, T> VerticeA { get; set; }
        public Vertice<K, T> VerticeB { get; set; }
        public int Peso { get; set; }
        public int Id { get; } = idGenerator++;


        public bool Visitado { get; set; }
        public bool EsMinima { get; set; }

        public Arista(Vertice<K, T> a, Vertice<K, T> b, int peso)
        {
            this.VerticeA = a;
            this.VerticeB = b;
            this.Peso = peso;
        }

    }

    public class PriorityQueueExt<x, a> : PriorityQueue<x,a>
    {
        public PriorityQueueExt<x,a> Copy()
        {
            return (PriorityQueueExt<x, a>)this.MemberwiseClone();
        }
    }
}
