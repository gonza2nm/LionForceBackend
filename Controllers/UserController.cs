using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
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
public class UserController(UserService userService, JwtTokenService authService, IMapper mapper) : ControllerBase
{
  private readonly UserService _userService = userService;
  private readonly JwtTokenService _authService = authService;
  private readonly IMapper _mapper = mapper;

  [Authorize(Policy = "NotStudent")]
  [HttpPost]
  [Route("signup")]
  public async Task<ActionResult<ResponseOne<UserDTO>>> SignUp(UserRequestDTO userBody)
  {
    var res = new ResponseOne<UserDTO> { Status = "", Message = "", Data = null };
    var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
    if (userRole == null)
    {
      res.UpdateValues("400", "Bad Request", null);
      return BadRequest(res);
    }
    if (userBody.Password.Length < 6)
    {
      res.UpdateValues("400", "La contraseña debe ser mayor a 6 caracteres", null);
    }
    else
    {
      var serviceRes = await _userService.Add(userBody, userRole);
      var userDTO = _mapper.Map<UserDTO>(serviceRes.Data);
      res.UpdateValues(serviceRes.Status, serviceRes.Message, userDTO);
    }
    switch (res.Status)
    {
      case "201":
        return StatusCode(StatusCodes.Status201Created, res);
      case "400":
        return BadRequest(res);
      case "403":
        return StatusCode(StatusCodes.Status403Forbidden, res);
      case "409":
        return StatusCode(StatusCodes.Status409Conflict, res);
      default:
        return StatusCode(StatusCodes.Status500InternalServerError, res);
    }

  }

  [AllowAnonymous]
  [HttpPost]
  [Route("login")]
  public async Task<ActionResult<ResponseOne<UserDTO>>> Login(UserLoginDTO userBody)
  {
    var res = new ResponseOne<UserDTO> { Data = null, Status = "", Message = "" };
    var user = await _userService.Auth(userBody.DNI);
    if (user == null || !_userService.VerifyUserPassword(user, userBody.Password))
    {
      return NotFound();
    }
    if (user.Role.Name.Equals("Student", StringComparison.OrdinalIgnoreCase))
    {
      res.UpdateValues("403", "No tienes permiso para acceder a este login", null);
      return Unauthorized(res);
    }
    var token = _authService.GenerateToken(user.DNI, user.Name, user.Role.Name);
    var cookieOptions = new CookieOptions
    {
      HttpOnly = true,
      Secure = false,
      SameSite = SameSiteMode.Lax,
      Expires = DateTimeOffset.UtcNow.AddHours(8)
    };
    Response.Cookies.Append("authToken", token, cookieOptions);
    var userDTO = _mapper.Map<UserDTO>(user);
    res.UpdateValues("200", "Usuario logeado correctamente", userDTO);
    return Ok(res);
  }

  [AllowAnonymous]
  [HttpPost]
  [Route("students/login/")]
  public async Task<ActionResult> LoginStudents(UserLoginDTO userBody)
  {
    var res = new ResponseOne<UserDTO> { Data = null, Status = "", Message = "" };
    var user = await _userService.Auth(userBody.DNI);
    if (user == null || !_userService.VerifyUserPassword(user, userBody.Password))
    {
      return NotFound();
    }
    if (user.Role.Name.Equals("Student", StringComparison.OrdinalIgnoreCase))
    {
      var token = _authService.GenerateToken(user.DNI, user.Name, user.Role.Name);
      var cookieOptions = new CookieOptions
      {
        HttpOnly = true,
        Secure = false,
        SameSite = SameSiteMode.Lax,
        Expires = DateTimeOffset.UtcNow.AddHours(8)
      };
      Response.Cookies.Append("authToken", token, cookieOptions);
      var userDTO = _mapper.Map<UserDTO>(user);
      res.UpdateValues("200", "Usuario logeado correctamente", userDTO);
      return Ok();
    }
    else
    {
      res.UpdateValues("403", "No tienes permiso para acceder tu rol", null);
      return Unauthorized(res);
    }
  }

  [AllowAnonymous]
  [HttpPost("logout")]
  public ActionResult Logout()
  {
    try
    {
      var cookieOptions = new CookieOptions
      {
        Expires = DateTimeOffset.UtcNow.AddDays(-1),
        HttpOnly = true,
        Secure = false,
        SameSite = SameSiteMode.Lax
      };

      Response.Cookies.Append("authToken", "", cookieOptions);
      return Ok();
    }
    catch
    {
      return StatusCode(500);
    }
  }


  [Authorize(Policy = "ControlRoles")]
  [HttpGet]
  public async Task<ActionResult<ResponseList<UserDTO>>> GetUsers()
  {
    var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
    var res = new ResponseList<UserDTO> { Status = "", Message = "", Data = [] };
    if (userRole == null)
    {
      return BadRequest();
    }
    var serviceRes = await _userService.GetUsers(null, false);
    var usersDTO = _mapper.Map<List<UserDTO>>(serviceRes.Data);
    res.UpdateValues(serviceRes.Status, serviceRes.Message, usersDTO);
    return Ok(res);
  }

  [Authorize(Policy = "NotStudent")]
  [HttpGet("myacademy/users")]
  public async Task<ActionResult<ResponseList<UserDTO>>> GetUsersByAcademy()
  {
    var userDNI = User.FindFirst("dni")?.Value;
    var res = new ResponseList<UserDTO> { Status = "", Message = "", Data = [] };
    if (userDNI == null)
    {
      res.UpdateValues("400", "No se identifico el usuario", []);
      return BadRequest(res);
    }
    var resInstructor = await _userService.GetMyData(userDNI);
    if (resInstructor.Data == null)
    {
      return BadRequest();
    }
    var serviceRes = await _userService.GetUsers(resInstructor.Data.AcademyId, false);
    var usersDTO = _mapper.Map<List<UserDTO>>(serviceRes.Data);
    res.UpdateValues(serviceRes.Status, serviceRes.Message, usersDTO);
    return Ok(res);
  }

  [Authorize(Policy = "NotStudent")]
  [HttpGet("myacademy/students")]
  public async Task<ActionResult<ResponseList<UserDTO>>> GetStudentsByAcademy()
  {
    var userDNI = User.FindFirst("dni")?.Value;
    var res = new ResponseList<UserDTO> { Status = "", Message = "", Data = [] };
    if (string.IsNullOrEmpty(userDNI))
    {
      res.UpdateValues("400", $"No se identifico el usuario dni:{userDNI}", []);
      return BadRequest(res);
    }
    var resInstructor = await _userService.GetMyData(userDNI);
    if (resInstructor.Data == null)
    {
      return BadRequest();
    }
    var serviceRes = await _userService.GetUsers(resInstructor.Data.AcademyId, true);
    var usersDTO = _mapper.Map<List<UserDTO>>(serviceRes.Data);
    res.UpdateValues(serviceRes.Status, serviceRes.Message, usersDTO);
    return Ok(res);
  }


  [Authorize(Policy = "NotStudent")]
  [HttpGet("{dni}")]
  public async Task<ActionResult<ResponseOne<UserDTO>>> GetUser(string dni)
  {
    var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
    var userDNI = User.FindFirst("dni")?.Value;
    var res = new ResponseOne<UserDTO> { Status = "", Message = "", Data = null };
    ResponseOne<User> serviceRes;
    if (userRole == null || userDNI == null)
    {
      return BadRequest();
    }
    if (userDNI.Equals(dni))
    {
      serviceRes = await _userService.GetMyData(dni);
    }
    else if (userRole.Equals("Supervisor") || userRole.Equals("Admin"))
    {
      serviceRes = await _userService.GetOne(dni, null);
    }
    else
    {
      var instructorDNI = User.FindFirst("dni")?.Value;
      if (instructorDNI == null)
      {
        res.UpdateValues("400", "Ocurrio un error al identificar el rol", null);
        return BadRequest(res);
      }
      serviceRes = await _userService.GetOne(dni, instructorDNI);
    }
    var userDTO = _mapper.Map<UserDTO>(serviceRes.Data);
    res.UpdateValues(serviceRes.Status, serviceRes.Message, userDTO);
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
      default:
        return StatusCode(StatusCodes.Status500InternalServerError, res);
    }
  }

  [Authorize(Policy = "NotStudent")]
  [HttpPut("{dni}")]
  public async Task<ActionResult<ResponseOne<UserDTO>>> Update(string dni, UserDTO userDTO)
  {
    var res = new ResponseOne<UserDTO> { Status = "", Message = "", Data = null };
    ResponseOne<User> serviceRes;
    var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
    var userDNI = User.FindFirst("dni")?.Value;
    if (userRole == null || userDNI == null)
    {
      return BadRequest();
    }
    if (userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase))
    {
      serviceRes = await _userService.Update(dni, userDTO, userDNI, false);
    }
    else
    {
      serviceRes = await _userService.Update(dni, userDTO, userDNI, true);
    }
    var userRes = _mapper.Map<UserDTO>(serviceRes.Data);
    res.UpdateValues(serviceRes.Status, serviceRes.Message, userRes);
    switch (res.Status)
    {
      case "200":
        return Ok(res);
      case "400":
        return BadRequest(res);
      case "404":
        return NotFound(res);
      default:
        return StatusCode(StatusCodes.Status500InternalServerError, res);
    }
  }

  [Authorize(Policy = "NotStudent")]
  [HttpPut("update-password/{dni}")]
  public async Task<ActionResult<ResponseOne<UserDTO>>> UpdateWithPassword(string dni, UserDTOWithPassword userDTO)
  {
    var res = new ResponseOne<UserDTO> { Status = "", Message = "", Data = null };
    ResponseOne<User> serviceRes;
    var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
    var userDNI = User.FindFirst("dni")?.Value;
    if (userRole == null || userDNI == null)
    {
      return BadRequest();
    }
    if (userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase))
    {
      serviceRes = await _userService.UpdateWithPassword(dni, userDTO, userDNI, false);
    }
    else
    {
      serviceRes = await _userService.UpdateWithPassword(dni, userDTO, userDNI, true);
    }
    var userRes = _mapper.Map<UserDTO>(serviceRes.Data);
    res.UpdateValues(serviceRes.Status, serviceRes.Message, userRes);
    switch (res.Status)
    {
      case "200":
        return Ok(res);
      case "400":
        return BadRequest(res);
      case "404":
        return NotFound(res);
      default:
        return StatusCode(StatusCodes.Status500InternalServerError, res);
    }
  }

  [Authorize(Policy = "NotStudent")]
  [HttpDelete("{dni}")]
  public async Task<ActionResult> Delete(string dni)
  {
    var res = await _userService.Delete(dni);
    switch (res.Status)
    {
      case "200": return Ok(res);
      case "404": return NotFound(res);
      default: return StatusCode(StatusCodes.Status500InternalServerError, res);
    }
  }
}