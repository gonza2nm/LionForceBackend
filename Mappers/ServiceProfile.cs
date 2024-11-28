using AutoMapper;
using lion_force_be.DTOs;
using lion_force_be.Models;

namespace lion_force_be.Mappers;

public class ServiceProfile : Profile
{
  public ServiceProfile()
  {
    CreateMap<Service, ServiceDTO>().ReverseMap();
    CreateMap<ServiceDTO, ServiceRequestDTO>().ReverseMap();
    CreateMap<Service, ServiceRequestDTO>().ReverseMap();
  }
}