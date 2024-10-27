using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using lion_force_be.DTOs;
using lion_force_be.Models;
using lion_force_be.Services;
using lion_force_be.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace lion_force_be.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
public class UserController(UserService userService, JwtTokenService authService) : ControllerBase
{
  private readonly UserService _userService = userService;
  private readonly JwtTokenService _authService = authService;

  [HttpPost]
  [Route("signup")]
  [Authorize(Policy = "NotStudent")]
  public async Task<ActionResult<ResponseOne<UserDTO>>> SignUp(UserRequestDTO userBody)
  {
    var res = new ResponseOne<UserDTO> { Status = "", Message = "", Data = null, Error = null };
    if (!ModelState.IsValid || userBody.DNI == string.Empty || userBody.Password == string.Empty)
    {
      res.UpdateValues("400", "Solicitud mal armada", null, "Bad Request");
      return BadRequest(res);
    }
    var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
    if (userRole == null || userRole == "Student")
    {
      res.UpdateValues("403", "Forbidden", null, "Forbidden");
      return StatusCode(StatusCodes.Status403Forbidden, res);
    }
    res = await _userService.Add(userBody, userRole);
    switch (res.Status)
    {
      case "201":
        return StatusCode(StatusCodes.Status201Created, res);
      case "400":
        return BadRequest(res);
      case "403":
        return StatusCode(StatusCodes.Status403Forbidden, res);
      case "404":
        return NotFound(res);
      case "500":
        return StatusCode(StatusCodes.Status500InternalServerError, res);
      default:
        return StatusCode(StatusCodes.Status503ServiceUnavailable, res);
    }

  }

  [AllowAnonymous]
  [HttpPost]
  [Route("login")]
  public async Task<ActionResult<ResponseToken>> Login(UserRequestDTO userBody)
  {
    var res = new ResponseToken { Token = null, Error = null };
    if (!ModelState.IsValid || userBody.DNI == string.Empty || userBody.Password == string.Empty)
    {
      res.UpdateValues(null, "Solicitud mal armada");
      return BadRequest(res);
    }
    var user = await _userService.Auth(userBody.DNI, userBody.Password);
    if (user != null)
    {
      var token = _authService.GenerateToken(user.DNI, user.Name, user.Role.Name);
      res.UpdateValues(token, null);
      return Ok(res);
    }
    else
    {
      res.UpdateValues(null, "Usuario o contraseña incorrecta");
      return NotFound(res);
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
        return Ok(res);
      case "400":
        return BadRequest(res);
      case "403":
        return StatusCode(StatusCodes.Status403Forbidden, res);
      case "404":
        return NotFound(res);
      case "500":
        return StatusCode(StatusCodes.Status500InternalServerError, res);
      default:
        return StatusCode(StatusCodes.Status503ServiceUnavailable, res);
    }
  }

}