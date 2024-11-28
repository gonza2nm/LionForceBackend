
using System.Transactions;
using AutoMapper;
using lion_force_be.DBContext;
using lion_force_be.DTOs;
using lion_force_be.Models;
using Microsoft.EntityFrameworkCore;

namespace lion_force_be.Services;

public class ServiceService(DbContextLF dbContext, IMapper mapper)
{
  public readonly DbContextLF _dbContext = dbContext;
  public readonly IMapper _mapper = mapper;

  public async Task<ResponseOne<ServiceDTO>> Add(ServiceRequestDTO serviceRDTO)
  {
    var res = new ResponseOne<ServiceDTO> { Status = "", Message = "", Data = null };
    using var transaction = await _dbContext.Database.BeginTransactionAsync();
    try
    {
      var ServiceDB = _mapper.Map<Service>(serviceRDTO);
      await _dbContext.Services.AddAsync(ServiceDB);
      await _dbContext.SaveChangesAsync();
      var Price = new Price { FromDate = DateTime.Now, ServiceId = ServiceDB.Id, UntilDate = null, Value = serviceRDTO.Value };
      await _dbContext.Prices.AddAsync(Price);
      await _dbContext.SaveChangesAsync();
      await transaction.CommitAsync();
      var ServiceDTO = _mapper.Map<ServiceDTO>(ServiceDB);
      ServiceDTO.Value = serviceRDTO.Value;
      res.UpdateValues("200", "Servicio agregado correctamente", ServiceDTO);
      return res;
    }
    catch (Exception ex)
    {
      await transaction.RollbackAsync();
      Console.WriteLine(ex.Message);
      res.UpdateValues("500", "Ocurrio un error al agregar el servicio", null);
      return res;
    }
  }

  public async Task<ResponseList<Service>> GetAll()
  {
    var res = new ResponseList<Service> { Status = "", Message = "", Data = [] };
    try
    {
      var services = await _dbContext.Services.ToListAsync();
      res.UpdateValues("200", "Listado de Servicios", services);
      return res;
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
      res.UpdateValues("500", "Ocurrio un error al listar todos los servicios", []);
      return res;
    }
  }

  public async Task<ResponseList<Service>> GetAllByAcademy(string instructorDNI, int academyid, bool ControleRole)
  {
    var res = new ResponseList<Service> { Status = "", Message = "", Data = [] };
    List<Service> services;
    try
    {
      if (ControleRole)
      {
        services = await _dbContext.Services.Where(s => s.AcademyId == academyid).ToListAsync();
      }
      else
      {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.DNI == instructorDNI && u.AcademyId == academyid);
        if (user == null)
        {
          res.UpdateValues("400", "No puede pedir servicios que sean de otra academia", []);
          return res;
        }
        services = await _dbContext.Services.Where(s => s.AcademyId == academyid).ToListAsync();
      }
      res.UpdateValues("200", "Listado de servicios de su academia", services);
      return res;
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
      res.UpdateValues("500", "Ocurrio un error al buscar los servicios de su academia", []);
      return res;
    }
  }
  /*
    public async Task<ResponseOne<Service>> Update()
    {
      return await true;
    }

    public async Task<ResponseOne<Service>> Delete()
    {
      return await true;
    }
    */
}