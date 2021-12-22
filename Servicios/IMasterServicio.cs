using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using ModelosDto;

namespace Servicios
{
    public interface IMasterServicio
    {
       
        Task<List<string>> GetRoles();

        Task<bool> EliminaToken(string IdUser);

      

    
       
     
     
    
        Task<bool> EliminaUsuarioLogico(string UserId);

   
      

 }
}