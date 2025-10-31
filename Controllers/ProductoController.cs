using Microsoft.AspNetCore.Mvc;
using tl2_tp8_2025_slackku.Models;
using tl2_tp8_2025_slackku.Repository;

namespace tl2_tp8_2025_slackku.Controllers
{
    public class ProductoController : Controller
    {
        private readonly ILogger<ProductoController> _logger;
        private ProductoRepository repository;
        public ProductoController(ILogger<ProductoController> logger)
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
        public IActionResult Create(Producto producto)
        {
            repository.Crear(producto);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

    }
}