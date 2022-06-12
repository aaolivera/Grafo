using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grafo.Grafo
{
    public class Grafo<K,T> : IGrafo<K,T>
    {
        protected readonly ConcurrentDictionary<K, Vertice<K, T>> vertices = new();
        protected readonly List<Arista<K, T>> aristas = new();
        public void InsertarVertice(K clave, T valor)
        {
            vertices.TryAdd(clave, new Vertice<K, T>(clave, valor));
        }

        public void InsertarArista(K verticeA, K verticeB, int peso)
        {
            //if (!vertices.ContainsKey(verticeA) || !vertices.ContainsKey(verticeB)) return;
            //if (!aristas.ContainsKey(new Tuple<K, K>(verticeA, verticeB)) && !aristas.ContainsKey(new Tuple<K, K>(verticeB, verticeA)))
            //{
            if(peso > 0)
            {
                var arista = new Arista<K, T>(vertices[verticeA], vertices[verticeB], peso);
                var aristaOpuesta = new Arista<K, T>(vertices[verticeB], vertices[verticeA], peso);
                vertices[verticeA].Aristas.Add(arista);
                vertices[verticeB].Aristas.Add(aristaOpuesta);
                aristas.Add(arista);
                aristas.Add(aristaOpuesta);
            }
                
            //}
        }

        public IList<Arista<K, T>> TendidoMinimoPrim()
        {
            PriorityQueueExt<Arista<K, T>, int> aristasc = new();
            List<Arista<K, T>> resultado = new();
            Reset();
            var verticeInicial = vertices.Values.First();
            verticeInicial.Aristas.ForEach(x => aristasc.Enqueue(x, x.Peso));
            verticeInicial.Visitado = true;
            while (aristasc.Count > 0)
            {
                var aristaMinima = aristasc.Dequeue();
                if (!aristaMinima.VerticeB.Visitado)
                {
                    aristaMinima.VerticeB.Visitado = true;
                    aristaMinima.VerticeB.Aristas.ForEach(x => aristasc.Enqueue(x, x.Peso));
                    aristaMinima.EsMinima = true;
                    resultado.Add(aristaMinima);
                }
                aristaMinima.Visitado = true;
            }
            return resultado;
        }
        public IList<Arista<K, T>> TendidoMinimoParalelPrim()
        {
            List<Arista<K, T>> resultado = new();
            Reset();

            Random random = new();
            var tasks = new List<Task>();

            for (int i = 0; i < 5; i++)
            {
                int indice = random.Next(0, vertices.Values.Count);
                var verticeInicial = vertices.Values.Skip(indice).Take(1).First();
                tasks.Add(Task.Run(() =>
                {
                    PriorityQueueExt<Arista<K, T>, int> aristas = new();
                    verticeInicial.Aristas.ForEach(x => aristas.Enqueue(x, x.Peso));
                    while (aristas.Count > 0)
                    {
                        var aristaMinima = aristas.Dequeue();
                        if (!aristaMinima.VerticeB.Visitado)
                        {
                            aristaMinima.VerticeB.Visitado = true;
                            aristaMinima.VerticeB.VisitadoPor = Task.CurrentId.Value;
                            aristaMinima.VerticeB.Aristas.ForEach(x => aristas.Enqueue(x, x.Peso));
                            aristaMinima.EsMinima = true;
                            resultado.Add(aristaMinima);
                        }
                        aristaMinima.Visitado = true;
                    }
                }));
            }
            Task.WaitAll(tasks.ToArray());
            return resultado;
        }

        public IList<Arista<K, T>> TendidoMinimoBoruvka()
        {
            int selector = vertices.Count;
            List<Arista<K, T>> resultado = new();
            Reset();
            while (selector > 1)
            {
                ConcurrentDictionary<K, Arista<K, T>> cheapest = new();
                foreach (var vertice in vertices.Values)
                {
                    foreach (var arista in vertice.Aristas)
                    {
                        var set1 = this.Find(arista.VerticeA);
                        var set2 = this.Find(arista.VerticeB);
                        arista.Visitado = true;
                        if (set1 != set2)
                        {
                            if (!cheapest.ContainsKey(vertice.Clave) || cheapest[vertice.Clave].Peso > arista.Peso)
                            {
                                cheapest.TryAdd(vertice.Clave, arista);
                            }
                        }
                    }

                }
                foreach (var vertice in vertices.Values)
                {
                    if (cheapest.TryGetValue(vertice.Clave, out Arista<K, T> arista))
                    {
                        var set1 = this.Find(arista.VerticeA);
                        var set2 = this.Find(arista.VerticeB);

                        if (set1 != set2)
                        {
                            selector--;
                            this.FindUnion(arista.VerticeA, arista.VerticeB);
                            arista.Visitado = true;
                            arista.EsMinima = true;
                            arista.VerticeB.Visitado = true;
                            arista.VerticeA.Visitado = true;
                            resultado.Add(arista);
                        }
                    }
                }
            }
            return resultado;
        }
        public IList<Arista<K, T>> TendidoMinimoParalelBoruvka()
        {
            int selector = vertices.Count;
            List<Arista<K, T>> resultado = new();
            Reset();
            while (selector > 1)
            {
                ConcurrentDictionary<K, Arista<K, T>> cheapest = new();
                var tasks = new List<Task>();
                foreach (var vertice in vertices.Values)
                {
                    var v = vertice;
                    tasks.Add(Task.Run(() =>
                    {
                        foreach (var arista in v.Aristas)
                        {
                            var set1 = this.Find(arista.VerticeA);
                            var set2 = this.Find(arista.VerticeB);
                            arista.Visitado = true;
                            if (set1 != set2)
                            {
                                if (!cheapest.ContainsKey(v.Clave) || cheapest[v.Clave].Peso > arista.Peso)
                                {
                                    cheapest.TryAdd(v.Clave, arista);
                                }
                            }
                        }
                    }));
                }
                Task.WaitAll(tasks.ToArray());
                foreach (var vertice in vertices.Values)
                {
                        if (cheapest.TryGetValue(vertice.Clave, out Arista<K, T> arista))
                        {
                            var set1 = this.Find(arista.VerticeA);
                             var set2 = this.Find(arista.VerticeB);
                             if (set1 != set2)
                            {
                                selector--;
                                this.FindUnion(arista.VerticeA, arista.VerticeB);
                                arista.Visitado = true;
                                arista.EsMinima = true;
                                arista.VerticeB.Visitado = true;
                                arista.VerticeA.Visitado = true;
                                resultado.Add(arista);
                            }
                        }
                }
            }
            return resultado;
        }

        public ICollection<Vertice<K, T>> GetVertices()
        {
            return vertices.Values;
        }

        public ICollection<Arista<K, T>> GetAristas()
        {
            return aristas;
        }

        public virtual void Reset()
        {
            foreach (var a in aristas)
            {
                a.EsMinima = false;
                a.Visitado = false;
            }
            foreach (var v in vertices.Values)
            {
                v.Visitado = false;
                v.Rank = 0;
                v.Parent = v;
            }
        }
        public Vertice<K, T> Find(Vertice<K, T> i)
        {
            if (i.Parent != i)
            {
                i.Parent = this.Find(i.Parent);
            }
            return i.Parent;
        }

        private void FindUnion(Vertice<K, T> x, Vertice<K, T> y)
        {
            var a = this.Find(x);
            var b = this.Find(y);
            if (a.Rank < b.Rank)
            {
                a.Parent = b;
            }
            else if (a.Rank > b.Rank)
            {
                b.Parent = a;
            }
            else
            {
                b.Parent = a;
                a.Rank++;
            }
        }
    }
}
