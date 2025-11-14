using Microsoft.AspNetCore.Mvc;
using tl2_tp8_2025_slackku.Models;
using tl2_tp8_2025_slackku.Repository;

namespace tl2_tp8_2025_slackku.Controllers
{
    public class PresupuestosController : Controller
    {
        private readonly ILogger<PresupuestosController> _logger;

        private PresupuestoRepository presupuestoRepository;
        public PresupuestosController(ILogger<PresupuestosController> logger)
        {
            _logger = logger;
            presupuestoRepository = new PresupuestoRepository();
        }

        [HttpGet]
        public IActionResult Crear([FromBody] Presupuesto content)
        {
            presupuestoRepository.Crear(content);
            return null;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<Presupuesto> presupuesto = presupuestoRepository.Listar();
            return View(presupuesto);
        }

        [HttpGet]
        public IActionResult Details()
        {
            return View();
        }

    }

}