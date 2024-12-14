using AutoMapper;
using lion_force_be.DBContext;
using lion_force_be.Models;
using Microsoft.EntityFrameworkCore;

namespace lion_force_be.Services;

public class InvoiceService(DbContextLF dbContext, IMapper mapper)
{
  public readonly IMapper _mapper = mapper;
  public readonly DbContextLF _dbContext = dbContext;

  public async Task<ResponseList<Invoice>> GetAll()
  {
    var res = new ResponseList<Invoice> { Data = [], Message = "", Status = "" };
    try
    {
      var invoices = await _dbContext.Invoices.ToListAsync();
      res.UpdateValues("200", "Invoices: ", invoices);
      return res;
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
      res.UpdateValues("500", "Ocurriio un error al listar las facturas", []);
      return res;
    }
  }

  public async Task<ResponseList<Invoice>> GetAllByAcademy(int AcademyId)
  {
    var res = new ResponseList<Invoice> { Data = [], Message = "", Status = "" };
    try
    {
      var invoices = await _dbContext.Invoices.ToListAsync();
      res.UpdateValues("200", "Invoices: ", invoices);
      return res;
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
      res.UpdateValues("500", "Ocurriio un error al listar las facturas", []);
      return res;
    }
  }

}