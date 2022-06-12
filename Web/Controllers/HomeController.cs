using Grafo.Observer;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IGrafoFactory observador;

        public HomeController(ILogger<HomeController> logger, IGrafoFactory observador)
        {
            _logger = logger;
            this.observador = observador;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetGrafo()
        {
            var resultado = new List<dynamic>();
            var grafo = observador.GetGrafo("matrizDeAdyacencias2.csv");
            //var grafo = observador.GetGrafo2("20220401.as-org2info.txt", "20220501.as-rel.txt");
            if(grafo.GetVertices().Count + grafo.GetAristas().Count < 7000)
            {
                foreach (var vertice in grafo.GetVertices())
                {
                    resultado.Add(new
                    {
                        data = new
                        {
                            id = vertice.Id.ToString(),
                            name = vertice.Valor,
                            score = vertice.Clave,
                            group = vertice.Visitado ? "visitado" : "coexp",
                        },
                        group = "nodes"
                    });
                }
                foreach (var arista in grafo.GetAristas().Where(x => !x.Visitado || x.EsMinima))
                {
                    resultado.Add(new
                    {
                        data = new
                        {
                            source = arista.VerticeA.Id.ToString(),
                            target = arista.VerticeB.Id.ToString(),
                            group = "coexp",
                            id = arista.Id.ToString(),
                        },
                        group = "edges"
                    });
                }
            }
            
            return Json(new { lista = resultado, vertices = grafo.GetVertices().Count, aristas = grafo.GetAristas().Count, cantidad = grafo.GetVertices().Count + grafo.GetAristas().Count });
        }

        [HttpGet]
        public IActionResult TendidoMinimoPrim()
        {
            var grafo = observador.GetGrafo();
            var sw = Stopwatch.StartNew();
            var aristas = grafo.TendidoMinimoPrim();
            sw.Stop();
            return Ok(new { time = sw.Elapsed.ToString("hh\\:mm\\:ss\\.fff") });
        }
        

        [HttpGet]
        public IActionResult TendidoMinimoParalelPrim()
        {
            var grafo = observador.GetGrafo(); 
            var sw = Stopwatch.StartNew();
            grafo.TendidoMinimoParalelPrim();
            sw.Stop();
            return Ok(new { time = sw.Elapsed.ToString("hh\\:mm\\:ss\\.fff") });
        }

        [HttpGet]
        public IActionResult TendidoMinimoParalelBoruvka()
        {
            var grafo = observador.GetGrafo();
            var sw = Stopwatch.StartNew();
            grafo.TendidoMinimoParalelBoruvka();
            sw.Stop();
            return Ok(new { time = sw.Elapsed.ToString("hh\\:mm\\:ss\\.fff") });
        }

        [HttpGet]
        public IActionResult TendidoMinimoBoruvka()
        {
            var grafo = observador.GetGrafo();
            var sw = Stopwatch.StartNew();
            grafo.TendidoMinimoBoruvka();
            sw.Stop();
            return Ok(new { time = sw.Elapsed.ToString("hh\\:mm\\:ss\\.fff") });
        }

        [HttpGet]
        public IActionResult ReiniciarGrafo()
        {
            var grafo = observador.GetGrafo();
            grafo.Reset();
            return Ok();
        }

        [HttpGet]
        public IActionResult GenerarGrafo(int n= 500)
        {
            observador.GenerarGrafo(n);
            var grafo = observador.GetGrafo();

            return Ok(new { vertices= grafo.GetVertices().Count, aristas = grafo.GetAristas().Count});
        }
    }
}