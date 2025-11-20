using Microsoft.AspNetCore.Mvc;
using tl2_tp8_2025_slackku.Models;
using tl2_tp8_2025_slackku.Repository;
using tl2_tp8_2025_slackku.ViewModels;

namespace tl2_tp8_2025_slackku.Controllers
{
    public class ProductosController : Controller
    {
        private readonly ILogger<ProductosController> _logger;
        private ProductoRepository repository;
        public ProductosController(ILogger<ProductosController> logger)
        {
            _logger = logger;
            repository = new ProductoRepository();
        }

        [HttpGet]
        public IActionResult Index() // Listar Todos
        {
            List<Producto> productos = repository.Listar();
            return View(productos);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ProductoViewModel productoVM)
        {
            if (!ModelState.IsValid)
            {
                return View(productoVM);
            }

            var nuevoProducto = new Producto
            {
                Descripcion = productoVM.Descripcion,
                Precio = productoVM.Precio
            };

            repository.Crear(nuevoProducto);
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public IActionResult Edit(int id)
        {
            Producto prod = repository.Obtener(id);
            if (prod != null)
                return View(prod);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(int id, ProductoViewModel productoVM)
        {
            if (id != productoVM.IdProducto) return NotFound();

            if (!ModelState.IsValid)
                return View(productoVM);

            var productoAEditar = new Producto
            {
                IdProducto = productoVM.IdProducto, // Necesario para el UPDATE
                Descripcion = productoVM.Descripcion,
                Precio = productoVM.Precio
            };

            repository.Modificar(productoAEditar);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            Producto prod = repository.Obtener(id);
            if (prod != null)
                return View(prod);
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmation(int id)
        {
            Producto prod = repository.Obtener(id);
            if (prod != null)
                repository.Eliminar(id);
            return RedirectToAction("Index");
        }

    }
}