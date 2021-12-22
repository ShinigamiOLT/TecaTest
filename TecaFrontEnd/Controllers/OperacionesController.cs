using Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using ModelosDto;
using Servicios;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TecaFrontEnd.Models;

namespace TecaFrontEnd.Controllers
{
    public class OperacionesController : Controller
    {
        private readonly ICuentaAhorroServicio _cuentasServicio;
        private readonly IOperacionCuentaServicio _clientesOperacion;
        private readonly ILogger<HomeController> _logger;

        public OperacionesController(ILogger<HomeController> logger, ICuentaAhorroServicio cuentasServicio, IOperacionCuentaServicio clientesOperacion)
        {
            _cuentasServicio = cuentasServicio;
            _clientesOperacion = clientesOperacion;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int CuentaId)

        {
            ViewBag.CuentaId = CuentaId;
            var Operaciones = await _clientesOperacion.GetAllForCuenta(CuentaId);
            var Cuenta = await _cuentasServicio.GetById(CuentaId);
            ViewBag.SaldoCuenta = Cuenta.SaldoActual.ToString("C");
            ViewBag.NumeroCuenta = $"{Cuenta.NumeroCuenta}";
            ViewBag.ClienteId = Cuenta.ClienteId;
            List<OperacionPorCuentaVm> cuentasvm = new List<OperacionPorCuentaVm>();
            foreach (var vm in Operaciones)
            {
                cuentasvm.Add(new OperacionPorCuentaVm()
                {
                    Id = vm.Id,
                    FechaOperacion = vm.FechaOperacion,
                    _TipoOperacion = vm._TipoOperacion,
                    MontoTransaccion = vm.MontoTransaccion,
                    SaldoFinal= vm.SaldoFinal
                });
            };


            return View(cuentasvm);
        }
        public IActionResult Crear(int CuentaId)
        {

            ViewBag.CuentaId = CuentaId;
            ViewBag.Success = true;


            ViewBag.Tipos = ToSelectList(new List<Enums.TipoOperacion>() { Enums.TipoOperacion.Deposito, Enums.TipoOperacion.Retiro}, "Id", "Text");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Crear(OperacionPorCuentaVm operacionVm)
        {
            var model = new OperacionPorCuentaCreateDto()
            {
                TipoOperacion =(Enums.TipoOperacion) operacionVm.TipoOperacion ,
                MontoTransaccion = operacionVm.MontoTransaccion
            };

          var respuesta=  await _clientesOperacion.Create(operacionVm.CuentaId, model);
            ViewBag.Success = respuesta.Success;
            ViewBag.Msg = respuesta.Msg;

            ViewBag.Tipos = ToSelectList(new List<Enums.TipoOperacion>() { Enums.TipoOperacion.Deposito, Enums.TipoOperacion.Retiro }, "Id", "Text");
            if (!respuesta.Success)
            {
                return View(operacionVm);
            }
            return RedirectToAction("Index", new { operacionVm.CuentaId });

        }
        [NonAction]
        public SelectList ToSelectList(List<Enums.TipoOperacion> lista, string valueField, string textField)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            foreach (var item in lista)
            {
                list.Add(new SelectListItem()
                {
                    Text = EnumsToString.To(item),
                    Value = ((int)item).ToString()
                }); ;
            }

            return new SelectList(list, "Value", "Text");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
