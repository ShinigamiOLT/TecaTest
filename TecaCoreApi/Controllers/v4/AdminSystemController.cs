using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Modelos.Identity;
using ModelosDto;
using Servicios;

namespace TecaCoreApi.Controllers.v4
{
    [Authorize] //(Roles = RoleHelper.Admin+","+RoleHelper.Especialista )]
    [ApiController]
    [Route("Administracion")]
    public class AdminSystemController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IMasterServicio _masterservicio;

        //   private readonly IManagerFileService _managerservice;
        private readonly ILogger<AdminSystemController> _logger;

        public AdminSystemController(
            UserManager<ApplicationUser> userManager,

            IMasterServicio masterservicio,
            //    IManagerFileService managerfiles,
            ILogger<AdminSystemController> logger
        )
        {
            _userManager = userManager;

            _masterservicio = masterservicio;
            //    _managerservice = managerfiles;
            _logger = logger;
        }

        /*Aqui es lo de asignar usuario*/

        /// <summary>
        /// Asignar un rol unico a  aun usuario
        /// </summary>
        /// <param name="editarRolDto"></param>
        /// <returns></returns>
        [HttpPost("AsignarUsuarioRol")]
        public async Task<ActionResult> AsignarRolUsuario(EditarRolDto editarRolDto)

        {
            var usuario = await _userManager.FindByIdAsync(editarRolDto.UserId);
            if (usuario == null)
                return NotFound();
            await _userManager.AddClaimAsync(usuario, new Claim(ClaimTypes.Role, editarRolDto.RoleName));
            await _userManager.AddToRoleAsync(usuario, editarRolDto.RoleName);
            return Ok();
        }

        /// <summary>
        ///     Asignar una lista de roles a un usuario, este reemplaza a los roles que tenia previamente
        /// </summary>
        /// <param name="editarRolDto"></param>
        /// <returns></returns>
        [HttpPost("AsignarUsuarioRoles")]
        public async Task<ActionResult> AsignarRolesUsuario(EditarRolesDto editarRolDto)

        {
            var usuario = await _userManager.FindByIdAsync(editarRolDto.UserId);
            if (usuario == null)
                return NotFound();
            //antes  de eso borremos      los que tiene
            var RolesTotal = await _userManager.GetRolesAsync(usuario);

            var RolSistema = await _masterservicio.GetRoles();
            //usuario.UserRoles.Select(x=>x.Role.Name).ToList();
            //

            foreach (var RoleName in RolesTotal)
            {
                await _userManager.RemoveClaimAsync(usuario, new Claim(ClaimTypes.Role, RoleName));
                await _userManager.RemoveFromRoleAsync(usuario, RoleName);
            }

            foreach (var RoleName in editarRolDto.RoleName)
            {
                if (RolSistema.Contains(RoleName))
                {
                    await _userManager.AddClaimAsync(usuario, new Claim(ClaimTypes.Role, RoleName));
                    await _userManager.AddToRoleAsync(usuario, RoleName);

                    _logger.LogInformation($"Info Set Rol: {RoleName} Usuario: {usuario.UserName}");
                }
            }

        

            return Ok();
        }

        /// <summary>
        /// Quita un rol en particular al usuario.
        /// </summary>
        /// <param name="editarRolDto"></param>
        /// <returns></returns>
        [HttpPost("RemoverUsuarioRol")]
        public async Task<ActionResult> RemoverRolUsuario(EditarRolDto editarRolDto)

        {
            var usuario = await _userManager.FindByIdAsync(editarRolDto.UserId);
            if (usuario == null)
                return NotFound();
            await _userManager.RemoveClaimAsync(usuario, new Claim(ClaimTypes.Role, editarRolDto.RoleName));
            await _userManager.RemoveFromRoleAsync(usuario, editarRolDto.RoleName);
            return Ok();
        }

        [HttpGet("UsuariosRoles")]
        public async Task<ActionResult> UsuariosRoles()
        {
            //cuales son los usuarios actuales

            var users = _userManager.Users.ToList();
            var RolesTotal = await _masterservicio.GetRoles();
            var roles = new List<UsuarioRolDto>();
            foreach (var user in users)
            {
                var actual = new UsuarioRolDto();

                //string str = "";
                actual.Nombre = user.Nombre;
                actual.Email = user.Email;
                actual.UserId = user.Id;
                foreach (var role in (await _userManager.GetRolesAsync(user)))
                {
                    //  str = (str == "") ? role.ToString() : str + " - " + role.ToString();

                    actual.Roles.Add(role);
                }
                //      roles.Add(str);

                roles.Add(actual);
            }

            return Ok(new { UsuariosRoles = roles, RolesSistema = RolesTotal });
        }

        /// <summary>
        /// Elimina un usuario en particular
        /// </summary>
        /// <param name="IdUser"></param>
        /// <returns></returns>
        [HttpPost("EliminarUsuario")]
        public async Task<ActionResult> EliminarUsuario(string IdUser)

        {
            var usuario = await _userManager.FindByIdAsync(IdUser);
            if (usuario == null)
                return NotFound();
            //ante de borrar veamos si tiene algun token valido
            try
            {
                //si es uno mismo pues no deberia de eliminarse
                var useractual = User.Claims.ToList()[0].Value; //Id
                if (useractual != IdUser)
                {
                    await _masterservicio.EliminaToken(IdUser);
                    await _masterservicio.EliminaUsuarioLogico(IdUser);
                    await _userManager.SetLockoutEnabledAsync(usuario, true);
                    await _userManager.SetLockoutEndDateAsync(usuario, DateTimeOffset.Now.AddYears(100));
                }

                return Ok("Eliminacion del usuario correcto");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            //  return Ok("Error al eliminar");
            return Ok("Eliminacion del usuario correcto logico");
        }



     

       
       

        /// <summary>
        /// Cambair la contraseña de un usuario
        /// </summary>
        /// <param name="modelo"></param>
        /// <returns></returns>
        [HttpPost("CambiarPassword")]
        public async Task<IActionResult> Cambiar(ChangeEmailPassword modelo)
        {
            Response<Vacio> respuesta = new Response<Vacio>();
            try
            {
                var myuser = await _userManager.FindByIdAsync(modelo.UserId);
                respuesta.Custom("Usuario invalido");
                respuesta.Success = false;
                respuesta.Code = 1;
                if (myuser == null)
                {
                    respuesta.Success = false;
                    respuesta.Custom("Usuario invalido");
                    //  respuesta.Code = 1;

                    return Ok(respuesta);
                }

                if (!string.IsNullOrEmpty(modelo.Email) && myuser.Email != modelo.Email)
                {
                    var resultado = await _userManager.SetUserNameAsync(myuser, modelo.Email);

                    if (!resultado.Succeeded)
                    {
                        _logger.LogError(resultado.Errors.ToList().Select(x => x.Description).ToString());
                        respuesta.Custom("Este correo ya esta registrado!!!");
                        respuesta.Success = false;
                        return Ok(respuesta);
                    }

                    if (resultado.Succeeded)
                    {
                        //  myuser.Nombre = "RosaCristina";
                        var tokenemail = await _userManager.GenerateChangeEmailTokenAsync(myuser, modelo.Email);
                        var rep = await _userManager.ChangeEmailAsync(myuser, modelo.Email, tokenemail);

                        //ahora los otros datos.
                    }
                    else
                    {
                        respuesta.Custom("Error al cambiar el correo");
                        respuesta.Success = false;
                        return Ok(respuesta);
                    }
                }

                
                if (!string.IsNullOrEmpty(modelo.NewPassword))
                {
                    var codigoReset = await _userManager.GeneratePasswordResetTokenAsync(myuser);
                    var result = await _userManager.ResetPasswordAsync(myuser, codigoReset, modelo.NewPassword);
                    if (result.Succeeded)
                    {
                        respuesta.Custom("Cambio de contraseña correctamente");
                        respuesta.Success = true;
                        return Ok(respuesta);
                    }
                    else
                    {
                        respuesta.Error("Error, la contraseña no cumple la seguridad minima");
                        respuesta.Success = false;
                        return Ok(respuesta);
                    }
                }

                respuesta.Custom("Información actualizada.");
                return Ok(respuesta);
            }
            catch (Exception error)
            {
                respuesta.Error(error.Message);
                return BadRequest(respuesta);
            }
        }
    }
}