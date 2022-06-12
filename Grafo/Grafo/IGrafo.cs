using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Grafo.Grafo
{
    public interface IGrafo<K,T>
    {
        void InsertarVertice(K clave, T valor);
        void InsertarArista(K clave1, K clave2, int peso);
        IList<Arista<K, T>> TendidoMinimoPrim();
        IList<Arista<K, T>> TendidoMinimoParalelPrim();
        IList<Arista<K, T>> TendidoMinimoBoruvka();
        IList<Arista<K, T>> TendidoMinimoParalelBoruvka();

        ICollection<Vertice<K, T>> GetVertices();
        ICollection<Arista<K, T>> GetAristas();
        void Reset();
    }
}
