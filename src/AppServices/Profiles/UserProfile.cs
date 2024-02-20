using AutoMapper;
using Domain.Models;

namespace AppServices.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserResult>();
            CreateMap<UpdateUserRequest, User>();
            CreateMap<CreateUserRequest, User>();
        }
    }
}
