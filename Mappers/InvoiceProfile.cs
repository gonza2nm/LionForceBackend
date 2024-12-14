using AutoMapper;
using lion_force_be.Models;

namespace lion_force_be.Mappers;

public class InvoiceProfile : Profile
{

  public InvoiceProfile()
  {
    CreateMap<Invoice, InvoiceDTO>().ReverseMap();
  }
}