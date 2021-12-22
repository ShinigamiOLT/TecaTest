using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Modelos.Identity;
using ModelosDto;
using Persistencia.DataBase;

namespace Servicios
{
    public class CMasterServicio : IMasterServicio
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CMasterServicio> _logger;
        private readonly IConfiguration _Configuration;

        public CMasterServicio(ApplicationDbContext contexto,

            IMapper mapper,
            IConfiguration Configuration,
            ILogger<CMasterServicio> logger
        )
        {
            _context = contexto;
            _mapper = mapper;
            _logger = logger;
            _Configuration = Configuration;
        }

        

        public async Task<List<string>> GetRoles()
        {
            var entri = await _context.Roles.Select(x => x.Name).ToListAsync();

            return entri;
        }

        /// <summary>
        /// Degvuleve la lista de nombre de roles del usuario marcado.
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public async Task<List<string>> GetRolesForUser(string UserId)
        {
            var ListaRoles = new List<string>();

            var entri = await _context.Users
                .Include(y => y.UserRoles)
                .ThenInclude(y => y.Role)
                .SingleOrDefaultAsync(x => x.Id == UserId);

            if (entri != null)
            {
                ListaRoles.AddRange(entri.UserRoles.Select(applicationUserRole => applicationUserRole.Role.Name));
            }

            return ListaRoles;
        }

        public async Task<List<string>> GetMenuPermisoForUser(string UserId)
        {
            var ListaRoles = new List<string>();

            var entri = await _context.Users
                .Include(y => y.UserRoles)
                .ThenInclude(y => y.Role)
                .SingleOrDefaultAsync(x => x.Id == UserId);

            if (entri != null)
            {
                foreach (var applicationUserRole in entri.UserRoles)
                {
                    ListaRoles.Add(applicationUserRole.Role.Name);
                }
            }

            return ListaRoles;
        }

        public async Task<bool> EliminaToken(string IdUser)
        {
            var entri = _context.MyToken.Where(x => x.UserId == IdUser).ToList();

            foreach (var myToken in entri)
            {
                _context.Entry(myToken).State = EntityState.Deleted;
            }

            await _context.SaveChangesAsync();
            return false;
        }

        public async Task<List<string>> GetRolesList()
        {
            var lista = new List<string>();
            try
            {
                var elem = await _context.Roles
                    .OrderBy(x => x.Id)
                    .AsQueryable()
                    .ToListAsync();
                foreach (var item in elem)
                {
                    var temp = item.Name;
                    lista.Add(temp);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return lista;
        }

      
    
     

        
        public async Task<bool> EliminaUsuarioLogico(string UserId)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Id == UserId);

            if (user != null)
            {
                user.EsBorrado = Enums.Borrado.Borrado;
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

   
       
     }
}