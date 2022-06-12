using Grafo.Grafo;
using Grafo.Observer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grafo
{
    class Program
    {
        static void Main(string[] args)
        {
            var observador = new GrafoFactory();

            for(int i = 1000; i < 11000; i+=1000)
            {
                observador.GenerarGrafo(i);
                var grafo = observador.GetGrafo();

                var sw = Stopwatch.StartNew();
                grafo.TendidoMinimoParalelBoruvka();
                sw.Stop();

                var sw2 = Stopwatch.StartNew();
                grafo.TendidoMinimoBoruvka();
                sw2.Stop();
                Console.WriteLine($"{i}-{(decimal)sw.ElapsedMilliseconds/ 1000}-{(decimal)sw2.ElapsedMilliseconds/ 1000}");
            }
            Console.Read();
        }



        //static void Main(string[] args)
        //{
        //    //GenerarGrafo();
        //    var grafo = CargarGrafo();


        //    Console.WriteLine($"Iniciando TendidoMinimoBoruvka...");
        //    var sw = Stopwatch.StartNew();
        //    var tendidoMinimo = grafo.TendidoMinimoBoruvka();
        //    sw.Stop();
        //    Console.WriteLine($"TendidoMinimoBoruvka Tiempo: {sw.Elapsed.ToString("hh\\:mm\\:ss\\.fff")}");

        //    Console.WriteLine($"Iniciando TendidoMinimoPrim...");
        //    var sw2 = Stopwatch.StartNew();
        //    var tendidoMinimo2 = grafo.TendidoMinimoPrim();
        //    sw2.Stop();
        //    Console.WriteLine($"TendidoMinimoPrim Tiempo: {sw2.Elapsed.ToString("hh\\:mm\\:ss\\.fff")}");


        //    GuardarTendidoMinimo(tendidoMinimo, "TendidoMinimoBoruvka.csv");
        //    GuardarTendidoMinimo(tendidoMinimo2, "TendidoMinimoPrim.csv");
        //}

        static Grafo<int, int> CargarGrafo()
        {

            Console.WriteLine($"Cargando grafo...");
            var sw = Stopwatch.StartNew();
            Grafo<int, int> grafo = new();
            var data = File.ReadLines("matrizDeAdyacencias2.csv");
            var vertices = data.First().Split(',').Count();
            for (int i = 0; i < vertices; i++)
            {
                grafo.InsertarVertice(i, i);
            }
            foreach (var line in data.Select((value, i) => (value, i)))
            {
                var linea = line.value.Split(',');
                foreach (var vertex in linea.Select((value, i) => (value, i)))
                {
                    if (line.i == vertex.i) break;
                    if(line.i != vertex.i && int.Parse(vertex.value) > 0)
                    {
                        grafo.InsertarArista(line.i, vertex.i, int.Parse(vertex.value));
                    }
                }
            }
            sw.Stop();
            Console.WriteLine($"Grafo cargado {sw.Elapsed.ToString("hh\\:mm\\:ss\\.fff")}");
            return grafo;
        }

        static void GenerarGrafo()
        {
            Console.WriteLine($"Iniciando GenerarGrafoParalel...");
            var sw = Stopwatch.StartNew();
            System.Random random = new System.Random();
            var n = 400;
            int[,] array = new int[n, n];

            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n && j < i; ++j)
                {
                    var rand = random.Next(100);
                    array[i, j] = rand < 98 ? 0 : random.Next(200);
                    array[j, i] = array[i, j];
                }
            GuardarEnArchivo(n, array, "matrizDeAdyacencias2.csv");
            sw.Stop();
            Console.WriteLine($"GenerarGrafoParalel Tiempo: {sw.Elapsed.ToString("hh\\:mm\\:ss\\.fff")}");

        }

        static void GuardarTendidoMinimo(IList<Arista<int, int>> tendido, string nombre)
        {
            var n = 400;
            int[,] array = new int[n, n];

            foreach(var t in tendido)
            {
                array[t.VerticeA.Clave, t.VerticeB.Clave] = t.Peso;
                array[t.VerticeB.Clave, t.VerticeA.Clave] = t.Peso;
            }
            GuardarEnArchivo(n, array, nombre);
        }

        private static void GuardarEnArchivo(int n, int[,] array, string nombre)
        {
            var lines = new string[n];
            var tasks = new List<Task>();
            for (int i = 0; i < n; i++)
            {
                var ii = i;
                tasks.Add(Task.Run(() =>
                {
                    var sb = new StringBuilder();
                    for (int j = 0; j < n; ++j)
                    {
                        var fin = j < n - 1 ? "," : "";
                        sb.Append($"{array[ii, j]}{fin}");
                    }
                    lines[ii] = sb.ToString();
                }));
                if (i % 100 == 0)
                {
                    Task.WaitAll(tasks.ToArray());
                }
            }
            Task.WaitAll(tasks.ToArray());
            File.WriteAllLines(nombre, lines);
        }
    }
}
