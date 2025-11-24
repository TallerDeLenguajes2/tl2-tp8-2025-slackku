using Microsoft.AspNetCore.Mvc;
using tl2_tp8_2025_slackku.Interfaces;
using tl2_tp8_2025_slackku.Models;
using tl2_tp8_2025_slackku.Repository;
using tl2_tp8_2025_slackku.ViewModels;

namespace tl2_tp8_2025_slackku.Controllers
{
    public class ProductosController : Controller
    {
        private IProductoRepository repository;
        private IAuthenticationService _authService;
        public ProductosController(IProductoRepository prodRepo, IAuthenticationService authService)
        {
            _authService = authService;
            repository = prodRepo;
        }

        [HttpGet]
        public IActionResult Index() // Listar Todos
        {
            var securityCheck = CheckAdminPermissions();
            if (securityCheck != null) return securityCheck;

            List<Producto> productos = repository.Listar();
            return View(productos);
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
            return null; // Permiso concedido
        }

        [HttpGet]
        public IActionResult Create()
        {
            var securityCheck = CheckAdminPermissions();
            if (securityCheck != null) return securityCheck;

            return View();
        }

        [HttpPost]
        public IActionResult Create(ProductoViewModel productoVM)
        {
            var securityCheck = CheckAdminPermissions();
            if (securityCheck != null) return securityCheck;

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
            var securityCheck = CheckAdminPermissions();
            if (securityCheck != null) return securityCheck;

            Producto prod = repository.Obtener(id);
            if (prod != null)
                return View(prod);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(int id, ProductoViewModel productoVM)
        {
            var securityCheck = CheckAdminPermissions();
            if (securityCheck != null) return securityCheck;

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
            var securityCheck = CheckAdminPermissions();
            if (securityCheck != null) return securityCheck;

            Producto prod = repository.Obtener(id);
            if (prod != null)
                return View(prod);
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmation(int id)
        {
            var securityCheck = CheckAdminPermissions();
            if (securityCheck != null) return securityCheck;

            Producto prod = repository.Obtener(id);
            if (prod != null)
                repository.Eliminar(id);
            return RedirectToAction("Index");
        }

        public IActionResult AccesoDenegado()
        {
            return View();
        }

    }
}