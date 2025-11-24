using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using tl2_tp8_2025_slackku.Interfaces;
using tl2_tp8_2025_slackku.Models;
using tl2_tp8_2025_slackku.Repository;
using tl2_tp8_2025_slackku.ViewModels;

namespace tl2_tp8_2025_slackku.Controllers
{
    public class PresupuestosController : Controller
    {
        private IPresupuestoRepository repository;
        private IAuthenticationService _authService;
        // Necesitamos el repositorio de Productos para llenar el dropdown
        private readonly ProductoRepository _productoRepo = new ProductoRepository();
        public PresupuestosController(IPresupuestoRepository presRepo, IAuthenticationService authService)
        {
            _authService = authService;
            repository = presRepo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (!_authService.IsAuthenticated())
                return RedirectToAction("Index", "Login");

            // Verifica Nivel de acceso que necesite validar
            if (_authService.HasAccessLevel("Administrador") || _authService.HasAccessLevel("Cliente"))
            {
                //si es es valido entra sino vuelve a login
                List<Presupuesto> presupuestos = repository.Listar();
                return View(presupuestos);
            }
            else
                return RedirectToAction("Index", "Login");
        }

        private IActionResult CheckAdminPermissions()
        {
            // 1. No logueado? -> vuelve al login
            if (!_authService.IsAuthenticated())
            {
                return RedirectToAction("Index", "Login");
            }
            // 2. No es Administrador? -> Da Error
            if (!_authService.HasAccessLevel("Administrador"))
            {
                // Llamamos a AccesoDenegado (llama a la vista correspondiente de Productos)
                return RedirectToAction(nameof(AccesoDenegado));
            }
            return View(new PresupuestoViewModel()); // Permiso concedido
        }

        [HttpGet]
        public IActionResult Create()
        {
            var securityCheck = CheckAdminPermissions();
            if (securityCheck != null) return securityCheck;

            ProductoRepository productoRepository = new();
            var productos = productoRepository.Listar();
            ViewBag.Productos = productos;
            return View();
        }

        [HttpPost]
        public IActionResult Create(PresupuestoViewModel presupuestoVM)
        {
            var securityCheck = CheckAdminPermissions();
            if (securityCheck != null) return securityCheck;

            ModelState.Remove("IdPresupuesto");
            ModelState.Remove("FechaCreacion");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("No valido!");
                return View(presupuestoVM);
            }

            var presupuesto = new Presupuesto
            {
                NombreDestinatario = presupuestoVM.NombreDestinatario,
                FechaCreacion = DateTime.Now,
            };


            if (!repository.Crear(presupuesto))
                Console.WriteLine("Error al crear");
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
            var securityCheck = CheckAdminPermissions();
            if (securityCheck != null) return securityCheck;

            Presupuesto pres = repository.Obtener(id);
            if (pres != null)
                return View(pres);
            // TODO: show an error msg, not found element
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(int id, PresupuestoViewModel presupuestoVM, List<PresupuestoDetalle> detalle)
        {
            var securityCheck = CheckAdminPermissions();
            if (securityCheck != null) return securityCheck;

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
            var securityCheck = CheckAdminPermissions();
            if (securityCheck != null) return securityCheck;

            Presupuesto pres = repository.Obtener(id);
            if (pres != null)
                return View(pres);
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmation(int id)
        {
            var securityCheck = CheckAdminPermissions();
            if (securityCheck != null) return securityCheck;

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

        public IActionResult AccesoDenegado()
        {
            return View();
        }
    }
}