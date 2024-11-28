
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

  public async Task<ResponseOne<Service>> Update(ServiceUpdateDTO serviceToUpd, int id, bool isAdmin, string userDNI)
  {
    using var transaction = await _dbContext.Database.BeginTransactionAsync();
    var res = new ResponseOne<Service> { Status = "", Message = "", Data = null };
    Service? service;
    Price? lastPrice;
    try
    {
      service = await _dbContext.Services.FirstOrDefaultAsync(s => s.Id == id);
      if (service == null)
      {
        res.UpdateValues("404", "No se encontro ese servicio", null);
        return res;
      }
      lastPrice = await _dbContext.Prices.FirstOrDefaultAsync(p => p.ServiceId == service.Id && p.UntilDate == null);
      if (lastPrice == null)
      {
        res.UpdateValues("404", "No se encontro el ultimo precio de su servicio", null);
        return res;
      }
      if (!isAdmin)
      {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.DNI == userDNI);
        if (user == null)
        {
          res.UpdateValues("400", "Ocurrio un Problema al aplicar las logicas de negocio", null);
          return res;
        }
        if (service.AcademyId != user.AcademyId)
        {
          res.UpdateValues("400", "No puede modificar un servicio que corresponde a otra academia", null);
          return res;
        }
      }
      _mapper.Map<ServiceUpdateDTO, Service>(serviceToUpd, service);
      if (Math.Round(serviceToUpd.Value, 2) != lastPrice.Value)
      {
        var now = DateTime.Now;
        lastPrice.UntilDate = now;
        Price newPrice = new Price { ServiceId = service.Id, FromDate = now, Value = serviceToUpd.Value, UntilDate = null };
        await _dbContext.Prices.AddAsync(newPrice);
        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
      }
      res.UpdateValues("200", "Actualizado Exitosamente", null);
      return res;
    }
    catch (Exception ex)
    {
      await transaction.RollbackAsync();
      Console.WriteLine(ex.Message);
      res.UpdateValues("500", "Ocurrio un error al Actualizar los servicios de su academia", null);
      return res;
    }
  }
  /*
      public async Task<ResponseOne<Service>> Delete()
      {
        return await true;
      }
      */
}