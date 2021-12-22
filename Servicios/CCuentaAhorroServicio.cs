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
    public class CCuentaAhorroServicio : ICuentaAhorroServicio
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CCuentaAhorroServicio(ApplicationDbContext contexto, IMapper mapper)
        {
            _context = contexto;
            _mapper = mapper;
        }
        public async Task<CuentaAhorroDto> Create(int ClienteId,CuentaAhorroCreateDto modelo)
        {
            var entri = new CuentaAhorro()
            {
                NumeroCuenta = modelo.NumeroCuenta,
                SaldoActual = modelo.SaldoActual,
                ClienteId = ClienteId,              
                FechaCreacion = System.DateTime.Now

            };
            await _context.AddAsync(entri);
            await _context.SaveChangesAsync();

            return _mapper.Map<CuentaAhorroDto>(entri);
        }

        public async Task<List<CuentaAhorroDto>> GetAll()
        {
            var all = await _context.CuentasAhorros.Where(x=>x.EsBorrado == Common.Enums.Borrado.Existe).ToListAsync();
            return _mapper.Map<List<CuentaAhorroDto>>(all);
        }
        public async Task<List<CuentaAhorroDto>> GetAllForCliente(int ClienteId)
        {
            var all = await _context.CuentasAhorros.Where(x => x.EsBorrado == Common.Enums.Borrado.Existe && x.ClienteId== ClienteId).ToListAsync();
            return _mapper.Map<List<CuentaAhorroDto>>(all);
        }
        public async Task<CuentaAhorroDto> GetById(int Id)
        {
            var cuenta = await _context.CuentasAhorros.SingleOrDefaultAsync(x => x.Id == Id);
            return _mapper.Map<CuentaAhorroDto>(cuenta);
        }

        public async Task<bool> Remove(int ClienteId, int Id)
        {
            var cuenta = await _context.CuentasAhorros.Include(x=>x.Operaciones).SingleOrDefaultAsync(x => x.Id == Id && x.ClienteId == ClienteId);
            if (cuenta != null)
            {   if(cuenta.Operaciones.Count>0)
                {
                    cuenta.EsBorrado = Common.Enums.Borrado.Borrado;
                    _context.Entry(cuenta).State = EntityState.Modified; 
                    return (await _context.SaveChangesAsync()) > 1;
                }
            else
                
                _context.Entry(cuenta).State = EntityState.Deleted;
                return (await _context.SaveChangesAsync()) > 1;
            }
            return false;
        }

        public async Task<bool> Update(int Id, int ClienteId, CuentaAhorroCreateDto modelo)
        {
            var cuenta = await _context.CuentasAhorros.SingleOrDefaultAsync(x => x.Id == Id && x.ClienteId == ClienteId);
            if (cuenta != null)
            {
                cuenta.NumeroCuenta = modelo.NumeroCuenta;
                cuenta.SaldoActual = modelo.SaldoActual;              
                _context.Entry(cuenta).State = EntityState.Modified;
                return (await _context.SaveChangesAsync()) > 1;
            }
            return false;
        }
    }
}