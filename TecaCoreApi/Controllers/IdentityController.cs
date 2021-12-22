using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Modelos.Identity;
using NETCore.MailKit.Core;
using Servicios;

namespace TecaCoreApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("Auth")]
    public class IdentityController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuracion;
        private readonly IEmailService _emailService;
        private readonly ITokenServicio _tokenservicio;
       
        private readonly IMasterServicio _masterservicio;
        private readonly ILogger<IdentityController> _logger;

        public IdentityController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuracion,
            IEmailService emailService,
            ITokenServicio tokenservicio,
         
            IMasterServicio masterservicio,
            ILogger<IdentityController> logger

        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuracion = configuracion;
            _emailService = emailService;
            _tokenservicio = tokenservicio;
   
            _masterservicio = masterservicio;
            _logger = logger;
        }

        private static string GeneraPass()
        {
            string code = "Tab_";

            // code += DateTime.Now.Second.ToString("D2");
            code += DateTime.Now.Millisecond.ToString("D2");

            return code;
        }

        /// <summary>
        /// Registro de usuario
        /// </summary>
        /// <param name="model">Datos de registro</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("Signup")]
        public async Task<ActionResult<Response<Vacio>>> Create(ApplicationUserRegisterDto model)
        {
            //if (model.Password == "Admin2.0")
            //{
            //    model.Password = "Oim_2021";
            //}
            model.Apellido = model.Apellido.Replace("  ", " ").TrimEnd().TrimStart();
            model.Nombre = model.Nombre.Replace("  ", " ").TrimEnd().TrimStart();
            model.Telefono = model.Telefono.Replace("  ", " ").TrimEnd().TrimStart();
            model.Password = GeneraPass();
            _logger.LogError($"New User: {model.Email} pass: {model.Password}");

            Response<Vacio> respuesta = new Response<Vacio>();
            try
            {
                var user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                    Nombre = model.Nombre,
                    Apellido = model.Apellido,
                    PhoneNumber = model.Telefono,
                    Avatar = "https://res.cloudinary.com/ovidiolt/image/upload/v1631557562/Default/Profile_hcsgbl.jpg",
                    LockoutEnabled = false
                };
                //Antes de crear  veamos si existe

                var usuarioresgitsrado = await _userManager.FindByEmailAsync(model.Email);

                if (usuarioresgitsrado != null)
                {
                    if (usuarioresgitsrado.EsBorrado == Enums.Borrado.Borrado)
                    {
                        usuarioresgitsrado.EsBorrado = Enums.Borrado.Existe;
                        await _userManager.UpdateAsync(usuarioresgitsrado);
                        respuesta.Error("Este correo se activo nuevamente y se recupero los registros anteriores!!!");
                        return Ok(respuesta);
                    }
                    respuesta.Error("Este correo electronico ya se encuentra resgistrado!!!");

                    return Ok(respuesta);
                }
                var result = await _userManager.CreateAsync(user, model.Password);

                await _userManager.AddToRoleAsync(user, RoleHelper.Usuario);

                await _userManager.SetLockoutEnabledAsync(user, false);//no bloquear

                if (!result.Succeeded)
                {
                    respuesta.Success = false;
                    respuesta.Code = 4;

                    return Ok(respuesta);
                }
                //despues de crear se debe de mandar el correo de confirmacion

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                //ok ahora veamos si podemos enviarlo para confirmarlo
                code = await _tokenservicio.GuardaCode(user.Id, code, "Confirmar");
            
                //model.Plantilla.Template =
                //    model.Plantilla.Template.Replace("One Integral Management</h1>", $"One Integral Management</h1><h2>Tu password temporal es: {model.Password}</h2><h5>* Te recomendamos cambiar tu contraseña para obtener la más alta seguridad en nuestro sitio.</h5>");

                await EnviarMsj(user, new ActivacionUserHelper() { Code = code, Asunto = "Validación de cuenta" }, model.Plantilla);

                // await EnviarMSJ(user, code, model.Plantilla);
                respuesta.Success = false;
                respuesta.Code = 2;
            }
            catch (Exception error)
            {
                respuesta.Error(error.Message);

                return Ok(respuesta);
            }

            respuesta.Success = true;
            respuesta.Code = 0;
            //validar el correo

            return respuesta;
        }

        /// <summary>
        /// Login al web api
        /// </summary>
        /// <param name="model">Informacion de login</param>
        /// <returns></returns>
      
        [HttpPost("Signin")]
        [AllowAnonymous]
        public async Task<ActionResult<Response<DatosUsuario>>> Login(ApplicationUserLoginDto model)
        {
            Response<DatosUsuario> respuesta = new Response<DatosUsuario>();

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                respuesta.Success = false;
                respuesta.Code = 1;

                return Ok(respuesta);
            }

            if (user.LockoutEnabled || user.EsBorrado == Enums.Borrado.Borrado)
            {
                respuesta.Success = false;
                respuesta.Error("Cuenta bloqueada / inexistente");
                return respuesta;
            }

            var valida = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (valida.Succeeded)
            {
                //como el usuario si pudo iniciar secion vamos si esta en master user

                respuesta.Success = true;
                respuesta.Code = 0;
                respuesta.Data.Token = await GenerateToken(user);
                respuesta.Data.Avatar = user.Avatar;
                respuesta.Data.Nombre = user.Nombre;
                respuesta.Data.Apellidos = user.Apellido;
                respuesta.Data.User = user.Email;
                respuesta.Data.IdUser = user.Id;
                respuesta.Data.Telefono = user.PhoneNumber;

                respuesta.Data.Roles = (await _userManager.GetRolesAsync(user)).ToList();

              
                if (!string.IsNullOrWhiteSpace(respuesta.Data.Foto))
                {
                    respuesta.Data.Avatar = respuesta.Data.Foto;
                }
                _logger.LogWarning("Sesion Iniciado " + user.UserName, user.UserName);
                return Ok(respuesta);
            }

            if (!user.EmailConfirmed)// if (valida.IsNotAllowed)
            {
                //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                respuesta.Success = false;
                respuesta.Code = 2;

                try
                {
                    var codigoexistente = await _tokenservicio.GetForUserConfirmar(user.Id, "Confirmar");

                    var code = "";
                    if (codigoexistente == null || codigoexistente.DateExpired < DateTime.Now)
                    {
                        code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = await _tokenservicio.GuardaCode(user.Id, code, "Confirmar");
                    }
                    else
                    {
                        code = codigoexistente.MyKey;
                    }

                    await EnviarMsj(user, new ActivacionUserHelper() { Code = code, Asunto = "Activación de Cuenta" }, model.Plantilla);
                }
                catch (Exception error)
                {
                    respuesta.Success = false;
                    respuesta.Error(error.Message);
                }

                return Ok(respuesta);
            }

            respuesta.Success = false;
            respuesta.Code = 1;
            _logger.LogWarning("Inicio de sesion No Valido " + user.UserName, user.UserName);
            return Ok(respuesta);
        }

        [HttpPost]
        public async Task<IActionResult> VerifyMail(string Key)
        {
            Response<Vacio> respuesta = new Response<Vacio>();
            try
            {
                var MyTokenDto = await _tokenservicio.Get(Key);
                if (MyTokenDto != null)
                {
                    var user = await _userManager.FindByIdAsync(MyTokenDto.UserId);
                    var result = await _userManager.ConfirmEmailAsync(user, MyTokenDto.Code);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"Se verifico para : {user.UserName} Codigo Valido: {Key}");
                        respuesta.Success = true;
                        respuesta.Code = 0;
                        await _tokenservicio.Remove(Key);
                        return Ok(respuesta);
                    }

                    foreach (var error in result.Errors)
                    {
                        _logger.LogError($"Error al validar el codigo {error.Description} {error.Code}");
                    }
                }

                respuesta.Code = 6;
                respuesta.Success = false;
            }
            catch (Exception error)
            {
                respuesta.Error(error.Message);
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        private async Task<string> GenerateToken(ApplicationUser user)
        {
            var cadena = _configuracion.GetValue<string>("SecretKey");
            var key = Encoding.ASCII.GetBytes(cadena);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Nombre),
                new Claim(ClaimTypes.Surname, user.Apellido),
            };
            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenhander = new JwtSecurityTokenHandler();

            var createtoke = tokenhander.CreateToken(tokenDescriptor);

            return tokenhander.WriteToken(createtoke);
        }

        /// <summary>
        /// Datos de la cuenta
        /// </summary>

        /// <returns></returns>

        [Authorize]
        [HttpGet("Profile")]
        public async Task<IActionResult> Profile()
        {
            Response<DatosUsuario> respuesta = new Response<DatosUsuario>();
            var IdUser = User.Claims.ToList()[0].Value; //Id

            var user = await _userManager.FindByIdAsync(IdUser);

            if (user == null)
            {
                return BadRequest(respuesta);
            }
            else
            {
                respuesta.Success = true;
                respuesta.Code = 0;
                //  respuesta.Data.Token = await GenerateToken(user);
                respuesta.Data.Avatar = user.Avatar;
                respuesta.Data.Nombre = user.Nombre;
                respuesta.Data.Apellidos = user.Apellido;
                respuesta.Data.User = user.Email;
                respuesta.Data.Telefono = user.PhoneNumber;
                respuesta.Data.IdUser = user.Id;
                return Ok(respuesta);
            }
        }

        /// <summary>
        /// Endpoint para Solicitar restablecer password
        /// </summary>
        /// <param name="modelo">Email</param>
        /// <returns></returns>
        [HttpPost("Reset")]
        public async Task<IActionResult> ReestablecerPassword(ResetPasswordDto modelo)
        {
            Response<Vacio> respuesta = new Response<Vacio>();

            var user = await _userManager.FindByEmailAsync(modelo.Email);
            respuesta.Success = false;
            respuesta.Code = 1;
            if (user == null)
            {
                respuesta.Success = false;
                respuesta.Code = 1;

                return Ok(respuesta);
            }
            else
            {
                var confirmado = await _userManager.IsEmailConfirmedAsync(user);
                if (!confirmado)
                {
                    respuesta.Success = false;
                    respuesta.Code = 2;

                    return Ok(respuesta);
                }

                try
                {
                    var codigoexistente = await _tokenservicio.GetForUserConfirmar(user.Id, "Reset");

                    var code = "";
                    if (codigoexistente == null || codigoexistente.DateExpired < DateTime.Now)
                    {
                        var result = await _userManager.GeneratePasswordResetTokenAsync(user);
                        //ok ahora veamos si podemos enviarlo para confirmarlo
                        code = await _tokenservicio.GuardaCode(user.Id, result, "Reset");
                    }
                    else
                    {
                        code = codigoexistente.MyKey;
                    }

                    await EnviarMsj(user, new ActivacionUserHelper() { Code = code, Asunto = "Restablecer Contraseña" },
                        modelo.Plantilla);

                    //  await EnviarMSJ(user, result, modelo.Plantilla);
                    ; // EnviarRecuperacion(user, result, modelo.Plantilla);
                    respuesta.Success = true;
                    respuesta.Code = 8;
                }
                catch (Exception ex)
                {
                    respuesta.Error(ex.Message);
                }

                //  respuesta.Data.User = user.Email;
                return Ok(respuesta);
            }
        }

        /// <summary>
        /// EndPoint Para establecer contraseña
        /// </summary>
        /// <param name="modelo">key y Nuevo password</param>
        /// <returns></returns>
        [HttpPost("SetPassword")]
        public async Task<IActionResult> SetPassword(SetPasswordDto modelo)
        {
            Response<Vacio> respuesta = new Response<Vacio>();
            try
            {
                var MyTokenDto = await _tokenservicio.Get(modelo.Key);
                if (MyTokenDto == null)
                {
                    respuesta.Code = 6;
                    respuesta.Success = false;
                    return Ok(respuesta);
                }
                var user = await _userManager.FindByIdAsync(MyTokenDto.UserId);
                var result = await _userManager.ResetPasswordAsync(user, MyTokenDto.Code, modelo.NewPassword);

                if (result.Succeeded)
                {
                    respuesta.Success = true;
                    respuesta.Code = 7;
                    await _tokenservicio.Remove(modelo.Key);
                    return Ok(respuesta);
                }

                respuesta.Code = 6;
                respuesta.Success = false;
            }
            catch (Exception error)
            {
                respuesta.Error(error.Message);
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        private async Task<bool> EnviarMsj(ApplicationUser user, ActivacionUserHelper code, Plantilla plantilla)
        {
            //veamos si podemos guardar
            try
            {
                // _logger.LogError( "Esto esta llegando "+plantilla.Url + plantilla.Template + " code " + code);

                var callbackUrl2 = Url.Action(nameof(VerifyMail), "Identity",
                    new { Key = code }, protocol: HttpContext.Request.Scheme,
                    HttpContext.Request.Host.ToString());

                var callbackUrl =
                    plantilla.Url;// + code; //+ user.Id + "/" + code; //+ "?UserId="+user.Id+"&code=" + code;   dch1bee13

                if (string.IsNullOrEmpty(plantilla.Url) || plantilla.Url == "string")
                {
                    callbackUrl = callbackUrl2;
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Error de correo: " + e.Message);
            }
            return true;
        }

        /// <summary>
        /// EndPoint para cambio de contraseña
        /// </summary>
        /// <param name="modelo">Password anterior y nuevo</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto modelo)
        {
            Response<Vacio> respuesta = new Response<Vacio>();
            try
            {
                var IdUser = User.Claims.ToList()[0].Value; //Id
                var user = await _userManager.FindByIdAsync(IdUser);

                var escorrecto = await _signInManager.CheckPasswordSignInAsync(user, modelo.OldPassword, false);

                if (escorrecto.Succeeded)
                {
                    var result = await _userManager.ChangePasswordAsync(user, modelo.OldPassword, modelo.NewPassword);

                    if (result.Succeeded)
                    {
                        respuesta.Success = true;
                        respuesta.Code = 7;

                        return Ok(respuesta);
                    }
                    else
                    {
                        respuesta.Success = false;
                        respuesta.Error("La contraseña nueva no cumple con la seguridad minima");
                        _logger.LogError("Change Password: " + result.ToString());
                    }
                }
                else
                {
                    respuesta.Code = 6;
                    respuesta.Success = false;
                    respuesta.Error("Contraseña actual incorrecta");
                    _logger.LogError("Change Password: " + escorrecto.ToString());
                }
            }
            catch (Exception error)
            {
                respuesta.Error(error.Message);
                // return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        /// <summary>
        /// Endpoint para cambiar los datos del usuario
        /// </summary>
        /// <param name="modelo">nuevo datos</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("Profile")]
        public async Task<IActionResult> UpdateProfile(DatosUsuarioUpdateDto modelo)
        {
            Response<DatosUsuario> respuesta = new Response<DatosUsuario>();
            var IdUser = User.Claims.ToList()[0].Value; //Id

            var user = await _userManager.FindByIdAsync(IdUser);

            if (user == null)
            {
                return BadRequest(respuesta);
            }
            else
            {
                user.Nombre = modelo.Nombre;
                user.Apellido = modelo.Apellidos;
                user.PhoneNumber = modelo.Telefono;

                var resultado = await _userManager.UpdateAsync(user);

                if (resultado.Succeeded)
                {
                    respuesta.Success = true;
                    respuesta.Code = 0;
                    //  respuesta.Data.Token = await GenerateToken(user);
                    respuesta.Data.Avatar = user.Avatar;
                    respuesta.Data.Nombre = user.Nombre;
                    respuesta.Data.Apellidos = user.Apellido;
                    respuesta.Data.User = user.Email;
                    respuesta.Data.Telefono = user.PhoneNumber;
                }
                else
                    respuesta.Error("Error al actualizar");

                return Ok(respuesta);
            }
        }

       
    }
}