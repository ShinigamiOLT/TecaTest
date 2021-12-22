using System.Collections.Generic;
using System.Threading.Tasks;
using ModelosDto;

namespace Servicios
{
    public interface IClientesServicio
    {
        Task<ClienteDto> Create(ClienteCreateDto modelo);
        Task<List<ClienteDto>> GetAll();
        Task<ClienteDto> GetById(int Id);
        Task<bool> Remove(int Id);
        Task<bool> Update(int Id, ClienteCreateDto modelo);
    }
}