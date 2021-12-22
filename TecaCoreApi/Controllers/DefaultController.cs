using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Modelos.BaseApp;
using Serilog.Context;
using Servicios;

namespace TecaCoreApi.Controllers
{
    [ApiController]
    [Route("/")]
    public class DefaultController : ControllerBase
    {
        private readonly ILogger<DefaultController> _logger;
      

        /// <summary>
        /// Punto de entrada
        /// </summary>
        /// <param name="Logger"></param>
        
        public DefaultController(ILogger<DefaultController> Logger)
        {
            _logger = Logger;
          
        }

        /// <summary>
        /// Mensaje de Bienvenida  API
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<string> Running()
        {
            _logger.LogInformation("Corriendo ...");
            
            return Ok("Running... success");
        }
    
        
    }
}