using AutoMapper;
namespace lion_force_be.Mappers;

public class ResponseProfile : Profile
{
  public ResponseProfile()
  {
    CreateMap(typeof(ServiceResponseList<>), typeof(ResponseList<>));
    CreateMap(typeof(ServiceResponseOne<>), typeof(ResponseOne<>));
  }
}