using Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModelosDto;
using Servicios;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TecaCoreApi.Controllers
{
    [ApiController]
    [Route("CuentasAhorros/{CuentaId}/OperacionesPorCuentas")]
    public class OperacionesPorCuentasController : ControllerBase
    {
        private readonly IOperacionCuentaServicio _operacioneservicio;
        private readonly ILogger<DefaultController> _logger;
        public OperacionesPorCuentasController(IOperacionCuentaServicio operacioneservicio, ILogger<DefaultController> Logger)
        {
           _operacioneservicio = operacioneservicio;
            _logger = Logger;


        }
        [HttpPost]
        public async Task<ActionResult<Response< OperacionPorCuentaDto>>> Create(int CuentaId, OperacionPorCuentaCreateDto modelo)
        {

            return await _operacioneservicio.Create(CuentaId, modelo);
        }

        [HttpGet]
        public async Task<ActionResult<List<OperacionPorCuentaDto>>> GetAll(int CuentaId)
        {
            return await _operacioneservicio.GetAllForCuenta(CuentaId);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<OperacionPorCuentaDto>> GetById(int id)
        {
            return await _operacioneservicio.GetById(id);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int CuentaId, int id, OperacionPorCuentaCreateDto modelo)
        {
            await _operacioneservicio.Update(id, CuentaId, modelo);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Remove(int id, int CuentaId)
        {
            await _operacioneservicio.Remove(id, CuentaId);
            return NoContent();
        }
    }
}