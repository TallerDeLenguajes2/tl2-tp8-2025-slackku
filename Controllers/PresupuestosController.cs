using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using tl2_tp8_2025_slackku.Models;
using tl2_tp8_2025_slackku.Repository;
using tl2_tp8_2025_slackku.ViewModels;

namespace tl2_tp8_2025_slackku.Controllers
{
    public class PresupuestosController : Controller
    {
        private readonly ILogger<PresupuestosController> _logger;

        private PresupuestoRepository repository;
        // Necesitamos el repositorio de Productos para llenar el dropdown
        private readonly ProductoRepository _productoRepo = new ProductoRepository();
        public PresupuestosController(ILogger<PresupuestosController> logger)
        {
            _logger = logger;
            repository = new PresupuestoRepository();
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<Presupuesto> presupuesto = repository.Listar();
            return View(presupuesto);
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
        public IActionResult Create(PresupuestoViewModel presupuestoVM)
        {
            if (!ModelState.IsValid)
            {
                return View(presupuestoVM);
            }

            var presupuesto = new Presupuesto
            {
                NombreDestinatario = presupuestoVM.NombreDestinatario,
                FechaCreacion = DateTime.Now,
            };

            repository.Crear(presupuesto);
            return RedirectToAction(nameof(Index));
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
        public IActionResult Edit(int id, PresupuestoViewModel presupuestoVM, List<PresupuestoDetalle> detalle)
        {
            if (id != presupuestoVM.IdPresupuesto) return NotFound();

            var keys = ModelState.Keys.Where(k => k.StartsWith("Detalle") || k.StartsWith("detalle")).ToList();
            foreach (var key in keys)
            {
                ModelState.Remove(key);
            }
            if (!ModelState.IsValid)
            {
                // No puedes hacer 'return View(presupuestoVM)' porque la vista espera 'Presupuesto'.
                // Además, el VM no tiene los productos, así que la tabla saldría vacía.

                // Solución: Buscamos el presupuesto original (que trae los productos de la BD)
                var presupuestoOriginal = repository.Obtener(id);

                if (presupuestoOriginal != null)
                {
                    // Le pegamos los datos de texto que el usuario intentó escribir
                    // para que no tenga que escribirlos de nuevo.
                    presupuestoOriginal.NombreDestinatario = presupuestoVM.NombreDestinatario;
                    presupuestoOriginal.FechaCreacion = DateTime.Now;

                    // ENVIAMOS UN OBJETO TIPO 'Presupuesto' A LA VISTA
                    return View(presupuestoOriginal);
                }
                return RedirectToAction(nameof(Index));
            }

            var presupuestoAEditar = new Presupuesto
            {
                IdPresupuesto = presupuestoVM.IdPresupuesto,
                NombreDestinatario = presupuestoVM.NombreDestinatario,
                FechaCreacion = presupuestoVM.FechaCreacion,
                Detalle = detalle ?? new List<PresupuestoDetalle>()
            };

            repository.Modificar(presupuestoAEditar);
            return RedirectToAction(nameof(Index));
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
        public IActionResult AgregarProducto(int id)
        {
            Presupuesto presupuesto = repository.Obtener(id);
            List<Producto> productos = _productoRepo.Listar();

            var productosDisponibles = productos
                .Where(p => !presupuesto.Detalle.Any(d => d.Producto.IdProducto == p.IdProducto))
                .ToList();

            AgregarProductoViewModel model = new AgregarProductoViewModel
            {
                IdPresupuesto = id,
                ListaProductos = new SelectList(productosDisponibles, "IdProducto", "Descripcion")
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult AgregarProducto(AgregarProductoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // debemos recargar el SelectList porque se pierde en el POST.
                var productos = _productoRepo.Listar();
                model.ListaProductos = new SelectList(productos, "IdProducto", "Descripcion");
                return View(model);
            }
            repository.AgregarDetalle(model.IdPresupuesto, model.IdProducto, model.Cantidad);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult EliminarDetalle(int idPresupuesto, int idProducto)
        {
            var presupuesto = repository.Obtener(idPresupuesto);
            if (presupuesto == null) return RedirectToAction("Index");

            var detalle = presupuesto.Detalle.FirstOrDefault(d => d.Producto.IdProducto == idProducto);
            if (detalle == null) return RedirectToAction("Edit", new { id = idPresupuesto });

            // IMPORTANTE: Necesitamos pasar también el IdPresupuesto para poder volver, 
            // guardémoslo en ViewBag si tu modelo PresupuestoDetalle no tiene la prop IdPresupuesto.
            ViewBag.IdPresupuesto = idPresupuesto;
            return View(detalle);
        }


        [HttpPost, ActionName("EliminarDetalle")]
        public IActionResult EliminarDetalleConfirmado(int idPresupuesto, int idProducto)
        {
            repository.EliminarDetalle(idPresupuesto, idProducto);
            // Redirigir al Index para que el flujo de eliminación vuelva al listado principal.
            return RedirectToAction("Index");
        }

    }

}