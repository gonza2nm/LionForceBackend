using lion_force_be.DBContext;
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
  public async Task<IActionResult> Login(User userBody)
  {
    if (!ModelState.IsValid || userBody.DNI == String.Empty || userBody.Password == String.Empty)
    {
      return StatusCode(StatusCodes.Status400BadRequest, "su solicitud fallo");
    }
    else
    {
      var res = await _userService.Auth(userBody.DNI, userBody.Password);
      if (res)
      {
        var user = await _dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.DNI == userBody.DNI && u.Password == userBody.Password);
        if (user != null)
        {
          var token = _authService.GenerateToken(user.DNI, user.Name, user.Role.Name);
          return StatusCode(StatusCodes.Status200OK, token);
        }
        return StatusCode(StatusCodes.Status404NotFound, "no se encontro el usuario");
      }
      else
      {
        return StatusCode(StatusCodes.Status404NotFound, "no se encontro el usuario");
      }
    }
  }

}