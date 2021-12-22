using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Persistencia.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modelos.Identity;
using ModelosDto;

namespace Servicios
{
    public class TokenServicio : ITokenServicio
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TokenServicio> _logger;
        private readonly IMapper _mapper;

        public TokenServicio(ApplicationDbContext _contexto, IMapper mapper, ILogger<TokenServicio> logger)
        {
            _context = _contexto;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<string> GuardaCode(string _UserId, string _Code, string tipo)
        {
            try
            {
                //antes de eso eliminemos los otros codigos

                var elemen = _context.MyToken.Where(x => x.UserId == _UserId).Select(x => x.MyKey).ToList();

                foreach (var claves in elemen)
                {
                    await Remove(claves);
                }

                // var subcode = Guid.NewGuid().ToString("N").Substring(0, 10);
                byte[] buffer = Guid.NewGuid().ToByteArray();
                var FormNumber = BitConverter.ToUInt32(buffer, 0) ^ BitConverter.ToUInt32(buffer, 4) ^ BitConverter.ToUInt32(buffer, 8) ^ BitConverter.ToUInt32(buffer, 12);
                var subcode = FormNumber.ToString("D").Substring(0, 6);
                var entry = new MyToken
                {
                    UserId = _UserId,
                    Code = _Code,
                    MyKey = subcode,
                    DateExpired = DateTime.Now.AddHours(6),
                    Tipo = tipo
                };
                await _context.AddAsync(entry);
                var result = await _context.SaveChangesAsync();

                return subcode;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return "";
        }

        public async Task<MyTokenDto> Get(string Key)
        {
            try
            {
                var elemento = await _context.MyToken.FirstOrDefaultAsync(x => x.MyKey == Key);
                if (elemento == null)
                {
                    _logger.LogError("Codigo no valido: " + Key);
                    return null;
                }
                var mitoken = new MyTokenDto
                {
                    UserId = elemento.UserId,
                    Code = elemento.Code
                };

                return mitoken;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return null;
        }

        public async Task Remove(string Key)
        {
            var elemento = await _context.MyToken.FirstOrDefaultAsync(x => x.MyKey == Key);
            if (elemento == null)
            {
                _logger.LogError("Codigo no valido: " + Key);
            }
            else
            {
                _context.Entry(elemento).State = EntityState.Deleted;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<MyTokenDto> GetForUserConfirmar(string UserId, string tipo)
        {
            //antes de eso eliminemos los otros codigos

            var elemen = await _context.MyToken.Where(x => x.UserId == UserId && x.Tipo == tipo).ToListAsync();

            if (elemen.Any())
            {
                return _mapper.Map<MyTokenDto>(elemen.First());
            }

            return null;
        }
    }
}