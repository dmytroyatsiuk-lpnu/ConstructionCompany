using AutoMapper;
using ConstructionCompany.Models;
using ConstructionCompany.Models.ModelsDTO;

namespace ConstructionCompany.AutoMapper
{
    public class EmployeeProfiler : Profile
    {
        public EmployeeProfiler()
        {
            CreateMap<Employee, EmployeeDTO>();
            CreateMap<EmployeeDTO, Employee>();
        }
    }
}
