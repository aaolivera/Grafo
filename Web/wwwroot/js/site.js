document.addEventListener('DOMContentLoaded', function () {
    var reordenar = false;
    document.getElementById("layoutButton").addEventListener("click", reord); 
    document.getElementById("resetButton").addEventListener("click", function () {
        writeConsole(`Reiniciando grafo...`);
        fetch('/home/ReiniciarGrafo', { mode: 'no-cors' }).then(x => { writeConsole(`Grafo ok`); setTimeout(function () { reordenar = true; }, 200); })
    });

    document.getElementById("tmButton").addEventListener("click", function () {
        writeConsole(`TendidoMinimoPrim...`);
        fetch('/home/TendidoMinimoPrim', { mode: 'no-cors' })
            .then(x => x.json().then(y => writeConsole(`Prim finalizado en ${y.time}`)));
    });
    document.getElementById("tmpButton").addEventListener("click", function () {
        writeConsole(`TendidoMinimoParalelPrim...`);
        fetch('/home/TendidoMinimoParalelPrim', { mode: 'no-cors' })
            .then(x => x.json().then(y => writeConsole(`Prim Paralel finalizado en ${y.time}`)));
    });
    document.getElementById("tmbButton").addEventListener("click", function () {
        writeConsole(`TendidoMinimoBoruvka...`);
        fetch('/home/TendidoMinimoBoruvka', { mode: 'no-cors' })
            .then(x => x.json().then(y => writeConsole(`Boruvka finalizado en ${y.time}`)));
    });
    document.getElementById("tmbpButton").addEventListener("click", function () {
        writeConsole(`TendidoMinimoParalelBoruvka...`);
        fetch('/home/TendidoMinimoParalelBoruvka', { mode: 'no-cors' })
            .then(x => x.json().then(y => writeConsole(`Boruvka finalizado en ${y.time}`)));
    });
    document.getElementById("generarButton").addEventListener("click", function () {
        
        var resp = window.prompt("Ingrese la cantidad de vertices:")
        writeConsole(`Generando grafo de ${resp} vertices...`);
        fetch('/home/GenerarGrafo?n=' + resp, { mode: 'no-cors' })
            .then(x => x.json().then(y => writeConsole(`generacion finalizada ${y.vertices} vertices, ${y.aristas} aristas`)))
    });
    async function writeConsole(text) {
        console.log(text);
        let dataLavel = document.getElementById("console");

        let today = new Date();
        let time = today.getHours() + ":" + today.getMinutes() + ":" + today.getSeconds();

        let node = document.createElement("li");
        node.className = "list-group-item";
        let textnode = document.createTextNode(`${time}> ${text}`);
        node.appendChild(textnode);

        dataLavel.appendChild(node);
        document.getElementById('console').removeChild(document.getElementById('console').getElementsByTagName('li')[0]);
    }

    function reord() {
        var layout = cy.layout({
            name: 'cola',
            infinite: true,
            fit: false
        });
        layout.run();
        reordenar = false;
    }

    async function load() {
        let response = await fetch('cy-style.json', { mode: 'no-cors' });
        let responsedata = await fetch('/home/getgrafo', { mode: 'no-cors' });
        writeConsole("Inicializando grafo...");
        let styles = await response.json();
        let data = await responsedata.json();
        writeConsole(`Cargando ${data.vertices} vertices y ${data.aristas} aristas...`);
        if (data.cantidad > 7000) {
            data.lista = [];
            writeConsole(`No es posible mostrar tantos elementos.`);
        }
            var cy = window.cy = cytoscape({
                container: document.getElementById('cy'),

                layout: {
                    name: 'cola',
                    infinite: true,
                    fit: false
                },

                style: styles,

                elements: data.lista,

                ready: function () {
                    writeConsole("Grafo cargado");
                }

            });
        
        async function reload() {
            let dataLavel = document.getElementById("dataLabel");
            let responsedata2 = await fetch('/home/getgrafo', { mode: 'no-cors' });
            let data2 = await responsedata2.json();
            if (data2.cantidad <= 7000) {
                cy.json({ elements: data2.lista });
                dataLavel.innerHTML = "nodos:" + cy.nodes().length + " aristas:" + cy.edges().length;
            } else {
                cy.json({ elements: [] });
                dataLavel.innerHTML = `No es posible mostrar ${data2.cantidad} elementos.`;
            }
            setTimeout(reload, 1000);
        }
        reload();
    }
    load();
});
