using AutoMapper;
using CS.Security.DTO;
using CS.Security.Models;

namespace CS.Security.Servises;

public class Mapper : Profile
{
    public Mapper()
    {
        CreateMap<UserDto, User>();
        CreateMap<User, UserDto>();
        
        CreateMap<UserLogInDto, User>();
        CreateMap<User, UserLogInDto>();
        
        CreateMap<UserSignUpDto, User>();
        CreateMap<User, UserSignUpDto>();
    }
}