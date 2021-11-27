using AutoMapper;
using Data;
using AutoMapper.Extensions.ExpressionMapping;

namespace Data
{
    internal static class MapperService
    {
        internal static IMapper ConfigureAutoMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddExpressionMapping();

                cfg.CreateMap<Entities.Movie, Models.Movie>();
                cfg.CreateMap<Models.Movie, Entities.Movie>();
            });

            //configuration.AssertConfigurationIsValid();

            return configuration.CreateMapper();
        }
    }
}
