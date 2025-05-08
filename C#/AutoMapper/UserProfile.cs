using AutoMapper;
using ConstructionCompany.Models;
using ConstructionCompany.Models.ModelsDTO;

namespace ConstructionCompany.AutoMapper
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDTO>();
            CreateMap<UserDTO, User>();
        }
    }
}
