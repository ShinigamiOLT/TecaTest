using System.Collections.Generic;
using System.Threading.Tasks;
using ModelosDto;

namespace Servicios
{
    public interface ICuentaAhorroServicio
    {
        Task<CuentaAhorroDto> Create(int ClienteId, CuentaAhorroCreateDto modelo);
        Task<List<CuentaAhorroDto>> GetAll();
        Task<List<CuentaAhorroDto>> GetAllForCliente(int ClienteId);
        Task<CuentaAhorroDto> GetById(int Id);
        Task<bool> Remove(int ClienteId, int Id);
        Task<bool> Update(int Id, int ClienteId, CuentaAhorroCreateDto modelo);
    }
}