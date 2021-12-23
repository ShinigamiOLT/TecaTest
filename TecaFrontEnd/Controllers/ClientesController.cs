using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModelosDto;
using Servicios;
using System.Diagnostics;
using System.Threading.Tasks;
using TecaFrontEnd.Models;

namespace TecaFrontEnd.Controllers
{
    public class ClientesController : Controller
    {
        private readonly IClientesServicio _clientesServicio;
        private readonly ILogger<HomeController> _logger;

        public ClientesController(ILogger<HomeController> logger, IClientesServicio clientesServicio)
        {
            _clientesServicio = clientesServicio;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var clientes = await _clientesServicio.GetAll();
            return View(clientes);
        }
        public IActionResult Crear()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Crear(ClienteVm ClienteVm)
        {
            if(!ModelState.IsValid)
            {
               return View(ClienteVm);
            }

            var model = new ClienteCreateDto()
            {
                NumeroIdentificacion = ClienteVm.NumeroIdentificacion,
                Nombre = ClienteVm.Nombre,
                ApellidoPaterno = ClienteVm.ApellidoPaterno,
                ApellidoMaterno = ClienteVm.ApellidoMaterno
            };

            await _clientesServicio.Create(model);
            return RedirectToAction("Index");

        }


        public async Task<IActionResult> Editar(int Id)
        {
            var cliente = await _clientesServicio.GetById(Id);
            if (cliente == null)
            {
                return RedirectToAction("Index");
            }
            var clientevm = new ClienteVm()
            {
               NumeroIdentificacion = cliente.NumeroIdentificacion,
               Nombre = cliente.Nombre,
               ApellidoPaterno= cliente.ApellidoPaterno,
               ApellidoMaterno= cliente.ApellidoMaterno
            };
            return View(clientevm);
        }
        [HttpPost]
        public async Task<IActionResult> Editar(ClienteVm ClienteVm)
        {
            if (!ModelState.IsValid)
            {
                return View(ClienteVm);
            }

            var model = new ClienteCreateDto()
            {
                NumeroIdentificacion = ClienteVm.NumeroIdentificacion,
                Nombre = ClienteVm.Nombre,
                ApellidoPaterno = ClienteVm.ApellidoPaterno,
                ApellidoMaterno = ClienteVm.ApellidoMaterno
            };

            await _clientesServicio.Update(ClienteVm.Id, model);
            return RedirectToAction("Index");

        }

     

        public async Task<IActionResult> Borrar(ClienteVm ClienteVm)
        {


            await _clientesServicio.Remove(ClienteVm.Id);
            return RedirectToAction("Index");

        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
