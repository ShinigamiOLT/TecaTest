using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using ModelosDto;

namespace Servicios
{
    public interface IOperacionCuentaServicio
    {
        Task<Response<OperacionPorCuentaDto>> Create(int CuentaId, OperacionPorCuentaCreateDto modelo);
        Task<List<OperacionPorCuentaDto>> GetAll();
        Task<List<OperacionPorCuentaDto>> GetAllForCuenta(int CuentaId);
        Task<OperacionPorCuentaDto> GetById(int Id);
        Task<bool> Remove(int CuentaId, int Id);
        Task<bool> Update(int Id, int CuentaId, OperacionPorCuentaCreateDto modelo);
    }

}