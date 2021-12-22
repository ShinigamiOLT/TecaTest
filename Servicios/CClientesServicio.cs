using System.Collections.Generic;
using System.Threading.Tasks;
using Modelos.BaseApp;
using ModelosDto;
using Persistencia.DataBase;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Servicios
{

    public class CClientesServicio : IClientesServicio
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CClientesServicio(ApplicationDbContext contexto, IMapper mapper)
        {
            _context = contexto;
            _mapper = mapper;
        }
        public async Task<ClienteDto> Create(ClienteCreateDto modelo)
        {
            var entri = new Cliente()
            {
             Nombre= modelo.Nombre,
             ApellidoPaterno=   modelo.ApellidoPaterno,
             ApellidoMaterno= modelo.ApellidoMaterno,
             NumeroIdentificacion= modelo.NumeroIdentificacion,
             FechaCreacion= System.DateTime.Now

            };
            await  _context.AddAsync(entri);
            await _context.SaveChangesAsync();

            return _mapper.Map<ClienteDto>(entri);
        }

        public async Task<List<ClienteDto>> GetAll()
        {
            var all = await _context.Clientes.Where(x=>x.EsBorrado == Common.Enums.Borrado.Existe).ToListAsync();
            return _mapper.Map<List<ClienteDto>>(all);
        }

        public async Task<ClienteDto> GetById(int Id)
        {
            var Cliente = await _context.Clientes.SingleOrDefaultAsync(x=> x.Id==Id);
            return _mapper.Map<ClienteDto>(Cliente);
        }

        public async Task<bool> Remove(int Id)
        {
            var Cliente = await _context.Clientes.Include(x=>x.Cuentas).SingleOrDefaultAsync(x => x.Id == Id);
            if(Cliente !=null)
            {     if(Cliente.Cuentas.Count>0)
                {
                    Cliente.EsBorrado = Common.Enums.Borrado.Borrado;
                    _context.Entry(Cliente).State = EntityState.Modified; 
                    return (await _context.SaveChangesAsync()) > 1;
                }
            else
                    _context.Entry(Cliente).State = EntityState.Deleted;
              return ( await _context.SaveChangesAsync())>1;
            }
            return false;
        }

        public async Task<bool> Update(int Id, ClienteCreateDto modelo)
        {
            var Cliente = await _context.Clientes.SingleOrDefaultAsync(x => x.Id == Id);
            if (Cliente != null)
            {
                Cliente.Nombre = modelo.Nombre;
                Cliente.ApellidoPaterno = modelo.ApellidoPaterno;
                Cliente.ApellidoMaterno = modelo.ApellidoMaterno;
                Cliente.NumeroIdentificacion = modelo.NumeroIdentificacion;
                _context.Entry(Cliente).State = EntityState.Modified;
                return (await _context.SaveChangesAsync()) > 1;
            }
            return false;
        }
    }
}