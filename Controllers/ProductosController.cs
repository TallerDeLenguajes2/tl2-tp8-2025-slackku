using Microsoft.AspNetCore.Mvc;
using tl2_tp8_2025_slackku.Models;
using tl2_tp8_2025_slackku.Repository;

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

        [HttpPost]
        public IActionResult Create(ProductoDTO producto)
        {
            repository.Crear(producto);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Producto prod = repository.Obtener(id);
            if (prod != null)
                return View(prod);
            // TODO: show an error msg, not found element
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(Producto modif)
        {
            repository.Modificar(modif);
            // TODO: show a verification msg
            return RedirectToAction("Index");
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