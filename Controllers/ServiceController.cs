
using AutoMapper;
using lion_force_be.DTOs;
using lion_force_be.Models;
using lion_force_be.Services;
using Microsoft.AspNetCore.Authorization;
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
    var res = new ResponseOne<ServiceDTO> { Status = "", Message = "", Data = null };
    res = await _service.Add(serviceRDTO);
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

  /*
    [Authorize(Policy = "ControlRoles")]
    [HttpGet]
    public async Task<ActionResult<ResponseList<ServiceDTO>>> GetAll()
    {
      return await _service.GetAll();
    }

    [Authorize(Policy = "NotStudent")]
    [HttpGet("{academyid}")]
    public async Task<ActionResult<ResponseList<ServiceDTO>>> GetAllByAcademy()
    {
      return await _service.GetAllByAcademy();
    }

    [Authorize(Policy = "NotStudent")]
    [HttpPut("{id}")]
    public async Task<ActionResult<ResponseOne<ServiceDTO>>> Update()
    {
      return await _service.Update();
    }

    [Authorize(Policy = "NotStudent")]
    [HttpDelete("{id}")]
    public async Task<ActionResult<ResponseOne<ServiceDTO>>> Delete()
    {
      return await _service.Delete();
    }
  */
}
