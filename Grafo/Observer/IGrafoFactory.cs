using Grafo.Grafo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grafo.Observer
{
    public interface IGrafoFactory
    {
        public void GenerarGrafo(int n);
        public IGrafo<int, string> GetGrafo(string file = "");
    }
}
