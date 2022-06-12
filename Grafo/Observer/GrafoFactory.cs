using Grafo.Grafo;
using System.IO;
using System.Linq;

namespace Grafo.Observer
{
    public class GrafoFactory : IGrafoFactory
    {
        Grafo<int, string> grafo = null;

        public void GenerarGrafo(int n)
        {
            Grafo<int, string> grafotmp = new();
            System.Random random = new();
            for (int i = 0; i < n; i++)
            {
                grafotmp.InsertarVertice(i, i.ToString());
            }

            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n && j < i; ++j)
                {
                    if (i == j) break;
                    var rand = random.Next(100);
                    if(rand > 95 && i != j)
                    {
                        grafotmp.InsertarArista(i, j, random.Next(200));
                    }
                }
            grafo = grafotmp;
        }

        public IGrafo<int, string> GetGrafo(string file = "")
        {
            if(grafo == null)
            {
                grafo = new();
                var data = File.ReadLines(file);
                var vertices = data.First().Split(',').Length;
                for (int i = 0; i < vertices; i++)
                {
                    grafo.InsertarVertice(i, i.ToString());
                }

                foreach (var line in data.Select((value, i) => (value, i)))
                {
                    var linea = line.value.Split(',');
                    foreach (var vertex in linea.Select((value, i) => (value, i)))
                    {
                        if (line.i != vertex.i && int.Parse(vertex.value) > 0)
                        {
                            grafo.InsertarArista(line.i, vertex.i, int.Parse(vertex.value));
                        }
                    }
                }
            }
            
            return grafo;
        }
    }
}
