using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModelosDto;
using Servicios;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TecaCoreApi.Controllers
{

    [ApiController]
    [Route("Clientes/{ClienteId}/CuentasAhorros")]
    public class CuentasAhorrosController : ControllerBase
    {
        private readonly ICuentaAhorroServicio _clienteservicio;
        private readonly ILogger<DefaultController> _logger;
        public CuentasAhorrosController(ICuentaAhorroServicio clienteservicio, ILogger<DefaultController> Logger)
        {
            _clienteservicio = clienteservicio;
            _logger = Logger;


        }
        [HttpPost]
        public async Task<ActionResult<CuentaAhorroDto>> Create(int ClienteId, CuentaAhorroCreateDto modelo)
        {

            return await _clienteservicio.Create(ClienteId, modelo);
        }

        [HttpGet]
        public async Task<ActionResult<List<CuentaAhorroDto>>> GetAll(int ClienteId)
        {
            return await _clienteservicio.GetAllForCliente(ClienteId);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<CuentaAhorroDto>> GetById(int id)
        {
            return await _clienteservicio.GetById(id);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int ClienteId,int id, CuentaAhorroCreateDto modelo)
        {
            await _clienteservicio.Update( id,ClienteId, modelo);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Remove(int id, int ClienteId)
        {
            await _clienteservicio.Remove(id, ClienteId);
            return NoContent();
        }
    }
}