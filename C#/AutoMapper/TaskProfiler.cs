using AutoMapper;
using ConstructionCompany.Models;
using ConstructionCompany.Models.ModelsDTO;

namespace ConstructionCompany.AutoMapper
{
    public class TaskProfiler : Profile
    {
        public TaskProfiler()
        {
            CreateMap<ConstructionCompany.Models.Task, TaskDTO>();
            CreateMap<TaskDTO, ConstructionCompany.Models.Task>();
        }
    }
}
