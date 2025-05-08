using AutoMapper;
using ConstructionCompany.Models;
using ConstructionCompany.Models.ModelsDTO;

namespace ConstructionCompany.AutoMapper
{
    public class BrigadeProfiler : Profile
    {
        public BrigadeProfiler()
        {
            CreateMap<Brigade, BrigadeDTO>();
            CreateMap<BrigadeDTO, Brigade>();
        }
    }
}
