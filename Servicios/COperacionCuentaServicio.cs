using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Common;
using Microsoft.EntityFrameworkCore;
using Modelos.BaseApp;
using ModelosDto;
using Persistencia.DataBase;

namespace Servicios
{
    public class COperacionCuentaServicio : IOperacionCuentaServicio
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICuentaAhorroServicio _cuentaAhorroServicio;

        public COperacionCuentaServicio(ApplicationDbContext contexto,ICuentaAhorroServicio cuentaAhorroServicio, IMapper mapper)
        {
            _context = contexto;
            _mapper = mapper;
            _cuentaAhorroServicio = cuentaAhorroServicio;
        }
        /******************* Reglas de negocio************/
        async Task<RegistroOperacion> AplicaOperacionDeposito(int CuentaId, decimal MontoTransaccion, Enums.TipoOperacion tipoOperacion)
        {
            var respuesta = new RegistroOperacion() { Success = false,Message="Operacion Cancelada" };
            var cuenta = await _cuentaAhorroServicio.GetById(CuentaId);

            if (cuenta != null)
            {
                if (tipoOperacion == Enums.TipoOperacion.Deposito)
                {
                    respuesta.SaldoInicial = cuenta.SaldoActual;
                    respuesta.SaldoFinal = respuesta.SaldoInicial + MontoTransaccion;

                    await _cuentaAhorroServicio.Update(CuentaId, cuenta.ClienteId, new CuentaAhorroCreateDto
                    {
                        NumeroCuenta = cuenta.NumeroCuenta,
                        SaldoActual = respuesta.SaldoFinal
                    });

                    respuesta.Success = true;
                    respuesta.Message = "Deposito Aplicado";
                }
                else
                {
                    if (cuenta.SaldoActual - MontoTransaccion >= 0)
                    {
                        respuesta.SaldoInicial = cuenta.SaldoActual;
                        respuesta.SaldoFinal = respuesta.SaldoInicial - MontoTransaccion;

                        await _cuentaAhorroServicio.Update(CuentaId, cuenta.ClienteId, new CuentaAhorroCreateDto
                        {
                            NumeroCuenta = cuenta.NumeroCuenta,
                            SaldoActual = respuesta.SaldoFinal
                        });

                        respuesta.Success = true;
                        respuesta.Message = "Retiro Aplicado";
                    }
                    else
                    {
                        respuesta.Message = "Error, Monto de retiro supera al saldo actual.";
                    }
                }



            }
            return respuesta;

        }

        /********************* **********************/
        public async Task<Response<OperacionPorCuentaDto>> Create(int CuentaId, OperacionPorCuentaCreateDto modelo)
        {
            var operacion = await AplicaOperacionDeposito(CuentaId, modelo.MontoTransaccion,modelo.TipoOperacion);

            var respuesta = new Response< OperacionPorCuentaDto> ();

            if (operacion.Success)
            {
                var entri = new OperacionPorCuenta()
                {
                    TipoOperacion = modelo.TipoOperacion,
                    SaldoInicial = operacion.SaldoInicial,
                    MontoTransaccion = modelo.MontoTransaccion,
                    SaldoFinal = operacion.SaldoFinal,
                    CuentaId = CuentaId,
                    FechaOperacion = System.DateTime.Now

                };
                await _context.AddAsync(entri);
                await _context.SaveChangesAsync();

                respuesta.Data= _mapper.Map<OperacionPorCuentaDto>(entri);

                respuesta.Custom(operacion.Message);
                respuesta.Success = operacion.Success;
                return respuesta;
            }

       
            respuesta.Custom(operacion.Message);
            respuesta.Success = operacion.Success;
            return respuesta;
        }

        public async Task<List<OperacionPorCuentaDto>> GetAll()
        {
            var all = await _context.OperacionesPorCuentas.ToListAsync();
            return _mapper.Map<List<OperacionPorCuentaDto>>(all);
        }
        public async Task<List<OperacionPorCuentaDto>> GetAllForCuenta(int CuentaId)
        {
            var all = await _context.OperacionesPorCuentas.Where(x => x.CuentaId == CuentaId).ToListAsync();
            return _mapper.Map<List<OperacionPorCuentaDto>>(all);
        }
        public async Task<OperacionPorCuentaDto> GetById(int Id)
        {
            var Operacion = await _context.OperacionesPorCuentas.SingleOrDefaultAsync(x => x.Id == Id);
            return _mapper.Map<OperacionPorCuentaDto>(Operacion);
        }

        public async Task<bool> Remove(int CuentaId, int Id)
        {
            var Operacion = await _context.OperacionesPorCuentas.SingleOrDefaultAsync(x => x.Id == Id && x.CuentaId == CuentaId);
            if (Operacion != null)
            {
                _context.Entry(Operacion).State = EntityState.Deleted;
                return (await _context.SaveChangesAsync()) > 1;
            }
            return false;
        }

        public async Task<bool> Update(int Id, int ClienteId, OperacionPorCuentaCreateDto modelo)
        {
            var Operacion = await _context.OperacionesPorCuentas.SingleOrDefaultAsync(x => x.Id == Id && x.CuentaId == ClienteId);
            if (Operacion != null)
            {
                Operacion.TipoOperacion = modelo.TipoOperacion;
              //  Operacion. SaldoInicial = modelo.SaldoInicial;
                Operacion. MontoTransaccion = modelo.MontoTransaccion;
             //   Operacion.SaldoFinal = modelo.SaldoInicial - modelo.MontoTransaccion;
                Operacion.  FechaOperacion = System.DateTime.Now;
                _context.Entry(Operacion).State = EntityState.Modified;
                return (await _context.SaveChangesAsync()) > 1;
            }
            return false;
        }
    }

}