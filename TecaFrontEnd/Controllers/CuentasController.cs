using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModelosDto;
using Servicios;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TecaFrontEnd.Models;

namespace TecaFrontEnd.Controllers
{

    public class CuentasController : Controller
    {
        private readonly ICuentaAhorroServicio _cuentasServicio;
        private readonly IClientesServicio _clientesServicio;
        private readonly ILogger<HomeController> _logger;

        public CuentasController(ILogger<HomeController> logger, ICuentaAhorroServicio cuentasServicio, IClientesServicio clientesServicio)
        {
            _cuentasServicio = cuentasServicio;
            _clientesServicio = clientesServicio;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int ClienteId)
          
        {
            var Cuentas =await _cuentasServicio.GetAllForCliente(ClienteId);
            var Cliente = await _clientesServicio.GetById(ClienteId);
           
            List<CuentaAhorroVm> cuentasvm = new List<CuentaAhorroVm>();
            foreach (var vm in Cuentas)
            {
                cuentasvm.Add(new CuentaAhorroVm()
                {
                    Id = vm.Id,
                    NumeroCuenta = vm.NumeroCuenta,
                    SaldoActual = vm.SaldoActual,
                    FechaCreacion= vm.FechaCreacion,
                    ClienteId = vm.ClienteId
                });
            };
            ViewBag.DatoCliente = $"{Cliente.Nombre} {Cliente.Apellidos}";
            ViewBag.NumeroCliente = $"{Cliente.NumeroIdentificacion}";
            ViewBag.ClienteId = Cliente.Id;


            return View(cuentasvm);
        }
        public IActionResult Crear(int ClienteId)
        {

            ViewBag.ClienteId = ClienteId;
            return View();
        }
    [HttpPost]
        public async Task<IActionResult> Crear(CuentaAhorroCreateVm CuentaVm)
        {
            if (!ModelState.IsValid)
            {
             
                return View(CuentaVm);
            }
            var model = new CuentaAhorroCreateDto()
            {
                NumeroCuenta = CuentaVm.NumeroCuenta,
                SaldoActual = CuentaVm.SaldoActual
            };

            await _cuentasServicio.Create(CuentaVm.ClienteId, model);
            return RedirectToAction("Index",new { CuentaVm.ClienteId });

        }

       
        public async Task<IActionResult> Editar(int ClienteId,int Id)
        {
            var cuenta = await _cuentasServicio.GetById(Id);
            if(cuenta == null)
            {
                return RedirectToAction("Index", new { ClienteId });
            }

            var cuentavm = new CuentaAhorroVm()
            {
                Id = cuenta.Id,
                NumeroCuenta = cuenta.NumeroCuenta,
                SaldoActual = cuenta.SaldoActual,
                ClienteId = cuenta.ClienteId
            };
            return View(cuentavm);
        }
        [HttpPost]
        public async Task<IActionResult> Editar(CuentaAhorroVm CuentaVm)
        {
            if (!ModelState.IsValid)
            {

                return View(CuentaVm);
            }
            var model = new CuentaAhorroCreateDto() { 
                NumeroCuenta= CuentaVm.NumeroCuenta,
                SaldoActual= CuentaVm.SaldoActual
            };

            await _cuentasServicio.Update(CuentaVm.Id, CuentaVm.ClienteId, model);
            return RedirectToAction("Index", new { CuentaVm.ClienteId });

        }
       
        public async Task<IActionResult> Borrar(CuentaAhorroVm ClienteDto)
        {
          

            await _cuentasServicio.Remove(ClienteDto.ClienteId, ClienteDto.Id);
            return RedirectToAction("Index", new { ClienteDto.ClienteId });

        }
       
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
