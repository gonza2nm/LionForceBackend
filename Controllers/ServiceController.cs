
using System.Security.Claims;
using AutoMapper;
using lion_force_be.DTOs;
using lion_force_be.Models;
using lion_force_be.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace lion_force_be.Controllers;

[Authorize]
[ApiController]
[Route("api/services")]
public class ServiceController(ServiceService service, IMapper mapper) : ControllerBase
{
  public readonly IMapper _mapper = mapper;
  public readonly ServiceService _service = service;

  [Authorize(Policy = "NotStudent")]
  [HttpPost]
  public async Task<ActionResult<ResponseOne<ServiceDTO>>> Add(ServiceRequestDTO serviceRDTO)
  {
    var res = await _service.Add(serviceRDTO);
    switch (res.Status)
    {
      case "201":
        return StatusCode(StatusCodes.Status201Created, res);
      case "400":
        return BadRequest(res);
      default:
        return StatusCode(StatusCodes.Status500InternalServerError, res);
    }
  }


  [Authorize(Policy = "ControlRoles")]
  [HttpGet]
  public async Task<ActionResult<ResponseList<ServiceDTO>>> GetAll()
  {
    ResponseList<Service> serviceRes = await _service.GetAll();
    var servicesDTO = _mapper.Map<List<ServiceDTO>>(serviceRes);
    var res = new ResponseList<ServiceDTO> { Status = serviceRes.Status, Message = serviceRes.Message, Data = servicesDTO };
    switch (res.Status)
    {
      case "200": return Ok(res);
      default: return StatusCode(StatusCodes.Status500InternalServerError, res);
    }
  }

  [Authorize(Policy = "NotStudent")]
  [HttpGet("{academyid}")]
  public async Task<ActionResult<ResponseList<ServiceDTO>>> GetAllByAcademy(int academyid)
  {
    bool ControlRole = false;
    var res = new ResponseList<ServiceDTO> { Status = "", Message = "", Data = [] };
    var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
    if (userRole == null)
    {
      res.UpdateValues("400", "No se identifico su authorizacion", []);
      return BadRequest(res);
    }
    var userDNI = User.FindFirst("dni")?.Value;
    if (userDNI == null)
    {
      res.UpdateValues("400", "No se pudo identificar al usuario", []);
      return BadRequest(res);
    }
    if (userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase) || userRole.Equals("Supervisor", StringComparison.OrdinalIgnoreCase))
    {
      ControlRole = true;
    }
    var serviceRes = await _service.GetAllByAcademy(userDNI, academyid, ControlRole);
    var servicesDTO = _mapper.Map<List<ServiceDTO>>(serviceRes);
    res.UpdateValues(serviceRes.Status, serviceRes.Message, servicesDTO);
    switch (res.Status)
    {
      case "200": return Ok(res);
      case "400": return BadRequest(res);
      default: return StatusCode(StatusCodes.Status500InternalServerError, res);
    }
  }

  [Authorize(Policy = "NotStudent")]
  [HttpPut("{id}")]
  public async Task<ActionResult<ResponseOne<ServiceDTO>>> Update(int id, ServiceUpdateDTO serviceToUpd)
  {
    bool isAdmin = false;
    var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
    var userDNI = User.FindFirst("dni")?.Value;
    var res = new ResponseOne<ServiceDTO> { Status = "", Message = "", Data = null };
    if (userRole == null || userDNI == null)
    {
      res.UpdateValues("400", "Hubo un Problema al identificar el usuario", null);
      return BadRequest(res);
    }
    if (userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase))
    {
      isAdmin = true;
    }
    var serviceRes = await _service.Update(serviceToUpd, id, isAdmin, userDNI);
    res.UpdateValues(serviceRes.Status, serviceRes.Message, null);
    switch (res.Status)
    {
      case "200": return Ok(res);
      case "400": return BadRequest(res);
      case "404": return NotFound(res);
      default: return StatusCode(StatusCodes.Status500InternalServerError, res);
    }
  }

  /*
          [Authorize(Policy = "NotStudent")]
          [HttpDelete("{id}")]
          public async Task<ActionResult<ResponseOne<ServiceDTO>>> Delete()
          {
            return await _service.Delete();
          }
        */
}
