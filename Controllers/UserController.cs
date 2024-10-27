using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using lion_force_be.DBContext;
using lion_force_be.DTOs;
using lion_force_be.Models;
using lion_force_be.Services;
using lion_force_be.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lion_force_be.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
public class UserController(UserService userService, JwtTokenService authService) : ControllerBase
{
  private readonly UserService _userService = userService;
  private readonly JwtTokenService _authService = authService;

  [AllowAnonymous]
  [HttpPost]
  [Route("login")]
  public async Task<ActionResult<ResponseToken>> Login(UserRequestDTO userBody)
  {
    var res = new ResponseToken { Token = null, Error = null };
    if (!ModelState.IsValid || userBody.DNI == String.Empty || userBody.Password == String.Empty)
    {
      res.UpdateValues(null, "Solicitud mal armada");
      return StatusCode(StatusCodes.Status400BadRequest, res);
    }
    Console.WriteLine("estoy en el login");
    var user = await _userService.Auth(userBody.DNI, userBody.Password);
    if (user != null)
    {
      var token = _authService.GenerateToken(user.DNI, user.Name, user.Role.Name);
      res.UpdateValues(token, null);
      return StatusCode(StatusCodes.Status200OK, res);
    }
    else
    {
      res.UpdateValues(null, "Usuario o contraseña incorrecta");
      return StatusCode(StatusCodes.Status404NotFound, res);
    }
  }

  [Authorize(Policy = "NotStudent")]
  [HttpGet("{dni}")]
  public async Task<ActionResult<ResponseOne<UserDTO>>> GetUser(string dni)
  {
    var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
    var res = new ResponseOne<User> { Status = "", Message = "", Data = null, Error = null };
    if (userRole == null)
    {
      return BadRequest(res);
    }
    if (userRole.Equals("Supervisor") || userRole.Equals("Admin"))
    {
      res = await _userService.GetOne(dni, null);
    }
    else
    {
      var instructorDNI = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
      if (instructorDNI == null)
      {
        res.UpdateValues("400", "Ocurrio un error al identificar el rol", null, "Bad Request");
        return BadRequest(res);
      }
      res = await _userService.GetOne(dni, instructorDNI);
    }
    switch (res.Status)
    {
      case "200":
        return StatusCode(StatusCodes.Status200OK, res);
      case "400":
        return StatusCode(StatusCodes.Status400BadRequest, res);
      case "403":
        return StatusCode(StatusCodes.Status403Forbidden, res);
      case "404":
        return StatusCode(StatusCodes.Status404NotFound, res);
      case "500":
        return StatusCode(StatusCodes.Status500InternalServerError, res);
      default:
        return StatusCode(StatusCodes.Status503ServiceUnavailable, res);
    }
  }

}