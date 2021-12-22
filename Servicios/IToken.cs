using System.Threading.Tasks;
using ModelosDto;

namespace Servicios
{
    public interface ITokenServicio
    {
        public Task<string> GuardaCode(string _UserId, string _Code, string Tipo);

        public Task<MyTokenDto> Get(string Key);

        public Task Remove(string key);

        Task<MyTokenDto> GetForUserConfirmar(string userId, string tipo);
    }
}