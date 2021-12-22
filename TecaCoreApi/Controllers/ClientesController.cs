using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModelosDto;
using Servicios;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TecaCoreApi.Controllers
{

    [ApiController]
    [Route("Clientes")]
    public class ClientesController : ControllerBase
    {
        private readonly IClientesServicio _clienteservicio;
        private readonly ILogger<DefaultController> _logger;


       

        public ClientesController(IClientesServicio clienteservicio, ILogger<DefaultController> Logger)
        {
            _clienteservicio = clienteservicio;
            _logger = Logger;


        }
        [HttpPost]
        public async Task<ActionResult<ClienteDto>> Create( ClienteCreateDto modelo)
        {
        
            return      await _clienteservicio.Create( modelo);
        }

        [HttpGet]
        public async Task<ActionResult<List<ClienteDto>>> GetAll()
        {
            return await _clienteservicio.GetAll();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDto>> GetById(int id)
        {
            return await _clienteservicio.GetById(id);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, ClienteCreateDto modelo)
        {
            await _clienteservicio.Update(id,modelo);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Remove(int id)
        {
            await _clienteservicio.Remove(id);
            return NoContent();
        }
    }
}