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

}