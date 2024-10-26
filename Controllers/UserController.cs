using lion_force_be.DBContext;
using lion_force_be.DTOs;
using lion_force_be.Models;
using lion_force_be.Services;
using lion_force_be.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lion_force_be.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
public class UserController(UserService userService, JwtTokenService authService, DbContextLF dbContext) : ControllerBase
{
  private readonly UserService _userService = userService;
  private readonly JwtTokenService _authService = authService;
  private readonly DbContextLF _dbContext = dbContext;

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
    var result = await _userService.Auth(userBody.DNI, userBody.Password);
    if (result)
    {
      Console.WriteLine("se encontro el usuario");
      var user = await _dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.DNI == userBody.DNI);
      var token = _authService.GenerateToken(user.DNI, user.Name, user.Role.Name);
      res.UpdateValues(token, null);
      return StatusCode(StatusCodes.Status200OK, res);
    }
    else
    {
      Console.WriteLine("NO se encontro el usuario");
      res.UpdateValues(null, "Usuario o contraseña incorrecta");
      return StatusCode(StatusCodes.Status404NotFound, res);
    }
  }

  [Authorize(Policy = "NotStudent")]
  [HttpGet("{dni}")]
  public async Task<ActionResult<ResponseOne<UserDTO>>> GetUser(string dni)
  {
    var res = await _userService.GetOne(dni);
    Console.WriteLine("estoy en el el getOne");
    switch (res.Status)
    {
      case "200":
        return StatusCode(StatusCodes.Status200OK, res);
      case "400":
        return StatusCode(StatusCodes.Status400BadRequest, res);
      case "404":
        return StatusCode(StatusCodes.Status404NotFound, res);
      case "500":
        return StatusCode(StatusCodes.Status500InternalServerError, res);
      default:
        return StatusCode(StatusCodes.Status503ServiceUnavailable, res);
    }
  }

}