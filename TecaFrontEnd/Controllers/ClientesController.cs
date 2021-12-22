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
        public async Task<IActionResult> Crear(ClienteVM ClienteVm)
        {
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
            return View(cliente);
        }
        [HttpPost]
        public async Task<IActionResult> Editar(ClienteDto clienteDto)
        {
            var model = new ClienteCreateDto()
            {
                NumeroIdentificacion = clienteDto.NumeroIdentificacion,
                Nombre = clienteDto.Nombre,
                ApellidoPaterno = clienteDto.ApellidoPaterno,
                ApellidoMaterno = clienteDto.ApellidoMaterno
            };

            await _clientesServicio.Update(clienteDto.Id, model);
            return RedirectToAction("Index");

        }

     

        public async Task<IActionResult> Borrar(ClienteVM clienteDto)
        {


            await _clientesServicio.Remove(clienteDto.Id);
            return RedirectToAction("Index");

        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
