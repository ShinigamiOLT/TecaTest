using AutoMapper;
using Common;
using Modelos.BaseApp;
using ModelosDto;

namespace TecaFrontEnd
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Cliente, ClienteDto>();
            CreateMap<CuentaAhorro, CuentaAhorroDto>();
            CreateMap<OperacionPorCuenta, OperacionPorCuentaDto>().ForMember(dest => dest._TipoOperacion, opt => opt.MapFrom(src => EnumsToString.To(src.TipoOperacion)));



        }
    }
}
