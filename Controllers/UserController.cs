using lion_force_be.DBContext;
using lion_force_be.DTOs;
using lion_force_be.Models;
using lion_force_be.Services;
using lion_force_be.Services.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lion_force_be.Controllers;

[ApiController]
[Route("/api/users")]
public class UserController(UserService userService, JwtTokenService authService, DbContextLF dbContext) : ControllerBase
{
  private readonly UserService _userService = userService;
  private readonly JwtTokenService _authService = authService;
  private readonly DbContextLF _dbContext = dbContext;


  [HttpPost]
  public async Task<ActionResult<ResponseToken>> Login(UserRequestDTO userBody)
  {
    var res = new ResponseToken { Token = null, Error = null };
    if (!ModelState.IsValid || userBody.DNI == String.Empty || userBody.Password == String.Empty)
    {
      res.UpdateValues(null, "Solicitud mal armada");
      return StatusCode(StatusCodes.Status400BadRequest, res);
    }

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

}