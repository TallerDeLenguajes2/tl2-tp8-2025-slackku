using Microsoft.AspNetCore.Mvc;
using tl2_tp8_2025_slackku.Models;
using tl2_tp8_2025_slackku.Repository;

namespace tl2_tp8_2025_slackku.Controllers
{
    public class PresupuestosController : Controller
    {
        private readonly ILogger<PresupuestosController> _logger;

        private PresupuestoRepository repository;
        public PresupuestosController(ILogger<PresupuestosController> logger)
        {
            _logger = logger;
            repository = new PresupuestoRepository();
        }

        [HttpGet]
        public IActionResult Create()
        {
            ProductoRepository productoRepository = new();
            var productos = productoRepository.Listar();
            ViewBag.Productos = productos;
            return View();
        }

        [HttpPost]
        public IActionResult Create(PresupuestoDTO presupuesto)
        {
            repository.Crear(presupuesto);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<Presupuesto> presupuesto = repository.Listar();
            return View(presupuesto);
        }

        [HttpGet]
        public IActionResult Details()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Presupuesto pres = repository.Obtener(id);
            if (pres != null)
                return View(pres);
            // TODO: show an error msg, not found element
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(Presupuesto modif)
        {
            repository.Modificar(modif);
            // TODO: show a verification msg
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            Presupuesto pres = repository.Obtener(id);
            if (pres != null)
                return View(pres);
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmation(int id)
        {
            Presupuesto pres = repository.Obtener(id);
            if (pres != null)
                repository.Eliminar(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult AddProd(int id)
        {
            var presupuesto = repository.Obtener(id);
            ProductoRepository productoRepo = new ProductoRepository();
            var todosLosProductos = productoRepo.Listar();

            var productosDisponibles = todosLosProductos
                .Where(p => !presupuesto.Detalle.Any(d => d.Producto.IdProducto == p.IdProducto))
                .ToList();

            ViewBag.Productos = productosDisponibles;

            return View(presupuesto);
        }

        [HttpPost]
        public IActionResult AddProd(int IdPresupuesto, int IdProducto, int Cantidad)
        {
            var prod = new Producto { IdProducto = IdProducto };
            repository.Agregar(IdPresupuesto, prod, Cantidad);
            return RedirectToAction("Index");
        }

    }

}