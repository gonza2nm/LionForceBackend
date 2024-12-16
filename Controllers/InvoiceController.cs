using lion_force_be.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lion_force_be.Controllers;

[Authorize]
[ApiController]
[Route("api/invoices")]
public class InvoiceController(InvoiceService service) : ControllerBase
{
  public readonly InvoiceService _service = service;


  [Authorize(Policy = "NotStudent")]
  [HttpPost]
  public async Task<ActionResult<ResponseOne<InvoiceDTO>>> Add(InvoiceDTO invoiceToAdd)
  {
    var res = new ResponseOne<InvoiceDTO>() { Status = "", Message = "", Data = null };

    //eliminar
    await _service.GetAll();

    return res;
  }

  [Authorize(Policy = "ControlRoles")]
  [HttpGet]
  public async Task<ActionResult<ResponseList<InvoiceDTO>>> GetAll()
  {
    var res = new ResponseList<InvoiceDTO>() { Status = "", Message = "", Data = [] };
    //eliminar
    await _service.GetAll();

    return res;

  }

  [Authorize(Policy = "NotStudent")]
  [HttpGet("academy/{academyid}")]
  public async Task<ActionResult<ResponseList<InvoiceDTO>>> GetAllByAcademy(int academyid)
  {
    var res = new ResponseList<InvoiceDTO>() { Status = "", Message = "", Data = [] };

    //eliminar
    await _service.GetAll();

    return res;

  }

  [Authorize(Policy = "NotStudent")]
  [HttpGet("{id}")]
  public async Task<ActionResult<ResponseList<InvoiceDTO>>> GetOne(int id)
  {
    var res = new ResponseList<InvoiceDTO>() { Status = "", Message = "", Data = [] };

    //eliminar
    await _service.GetAll();

    return res;

  }

  [Authorize(Policy = "NotStudent")]
  [HttpPut("{id}")]
  public async Task<ActionResult<ResponseOne<InvoiceDTO>>> Update(InvoiceDTO invoice)
  {
    var res = new ResponseOne<InvoiceDTO>() { Status = "", Message = "", Data = null };

    //eliminar
    await _service.GetAll();

    return res;

  }

  [Authorize(Policy = "NotStudent")]
  [HttpDelete("{id}")]
  public async Task<ActionResult<ResponseOne<InvoiceDTO>>> Delete(InvoiceDTO invoice)
  {
    var res = new ResponseOne<InvoiceDTO>() { Status = "", Message = "", Data = null };

    //eliminar
    await _service.GetAll();

    return res;
  }

}