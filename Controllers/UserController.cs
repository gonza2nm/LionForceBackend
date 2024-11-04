using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
    if (userBody.Password.Length < 6)
    {
      res.UpdateValues("400", "La contraseña debe ser mayor a 6 caracteres", null, "Bad Request");
    }
    else
    {
      res = await _userService.Add(userBody, userRole);
    }
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
  public async Task<ActionResult<ResponseToken>> Login(UserLoginDTO userBody)
  {
    var res = new ResponseToken { Token = null };
    if (!ModelState.IsValid)
    {
      return BadRequest(res);
    }
    var user = await _userService.Auth(userBody.DNI);
    if (user == null || !_userService.VerifyUserPassword(user, userBody.Password))
    {
      return NotFound(res);
    }
    if (user.Role.Name.Equals("Student", StringComparison.OrdinalIgnoreCase))
    {
      return Unauthorized("No tienes permiso para acceder a este login.");
    }
    var token = _authService.GenerateToken(user.DNI, user.Name, user.Role.Name);
    res.Token = token;
    return Ok(res);
  }

  [AllowAnonymous]
  [HttpPost]
  [Route("students/login/")]
  public async Task<ActionResult<ResponseToken>> LoginStudents(UserLoginDTO userBody)
  {
    var res = new ResponseToken { Token = null };
    if (!ModelState.IsValid)
    {
      return BadRequest(res);
    }
    var user = await _userService.Auth(userBody.DNI);
    if (user == null || !_userService.VerifyUserPassword(user, userBody.Password))
    {
      return NotFound(res);
    }
    if (user.Role.Name.Equals("Student", StringComparison.OrdinalIgnoreCase))
    {
      var token = _authService.GenerateToken(user.DNI, user.Name, user.Role.Name);
      res.Token = token;
      return Ok(res);
    }
    else
    {
      return Unauthorized("Este login no es el indicado para tu rol");
    }
  }


  [Authorize(Policy = "ControlRoles")]
  [HttpGet]
  public async Task<ActionResult<ResponseList<UserDTO>>> GetUsers()
  {
    var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
    var res = new ResponseList<UserDTO> { Status = "", Message = "", Data = [], Error = null };
    if (userRole == null)
    {
      res.UpdateValues("400", "No se identifico el usuario", [], "Error al identificar el usuario");
      return BadRequest(res);
    }
    res = await _userService.GetUsers(null);
    return Ok(res);
  }







  //corregir esto










  /*
    [Authorize(Policy = "NotStudent")]
    [HttpGet("myacademy")]
    public async Task<ActionResult<ResponseList<UserDTO>>> GetUsersByAcademy()
    {
      var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
      var userDNI = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
      var res = new ResponseList<UserDTO> { Status = "", Message = "", Data = [], Error = null };
      if (userRole == null || userDNI == null)
      {
        res.UpdateValues("400", "No se identifico el usuario", [], "Error al identificar el usuario");
        return BadRequest(res);
      }
      if (userRole.Equals("Instructor"))
      {
        var resInstructor = await _userService.GetMyData(userDNI);
        if (resInstructor.Data == null)
        {
          return BadRequest();
        }
        res = await _userService.GetUsers(resInstructor.Data.AcademyId);
        return Ok(res);
      }
      else
      {
        if (myacademy == null)
        {
          res = await _userService.GetUsers(null);
          return Ok(res);
        }
        var resUser = await _userService.GetMyData(userDNI);
        if (resUser.Data == null)
        {
          return BadRequest();
        }
        res = await _userService.GetUsers(resUser.Data.AcademyId);
        return Ok(res);
      }
    }
  */

  [Authorize(Policy = "NotStudent")]
  [HttpGet("{dni}")]
  public async Task<ActionResult<ResponseOne<UserDTO>>> GetUser(string dni)
  {
    var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
    var userDNI = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
    var res = new ResponseOne<UserDTO> { Status = "", Message = "", Data = null, Error = null };
    if (userRole == null || userDNI == null)
    {
      return BadRequest();
    }
    if (userDNI.Equals(dni))
    {
      res = await _userService.GetMyData(dni);
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