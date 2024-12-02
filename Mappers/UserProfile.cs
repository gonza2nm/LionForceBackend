using AutoMapper;
using lion_force_be.DTOs;
using lion_force_be.Models;

namespace lion_force_be.Mappers;

public class UserProfile : Profile
{
  public UserProfile()
  {
    CreateMap<User, UserDTO>().ReverseMap();
    CreateMap<User, UserRequestDTO>().ReverseMap();
    CreateMap<UserDTO, UserRequestDTO>().ReverseMap();
    CreateMap<UserDTOWithPassword, User>().ReverseMap();
    CreateMap<UserDTOWithPassword, UserDTO>().ReverseMap();

  }
}