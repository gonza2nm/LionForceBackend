using AutoMapper;
using lion_force_be.DBContext;
using lion_force_be.DTOs;
using lion_force_be.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace lion_force_be.Services;

public class UserService(DbContextLF dbContext, IMapper mapper)
{
  private readonly DbContextLF _dbContext = dbContext;
  private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
  private readonly IMapper _mapper = mapper;

  public async Task<ResponseOne<UserDTO>> Add(UserRequestDTO user, string userRequestRol)
  {
    var res = new ResponseOne<UserDTO> { Status = "", Message = "", Data = null, Error = null };
    using var transaction = await _dbContext.Database.BeginTransactionAsync();
    try
    {
      Console.WriteLine("entro al try catch");
      var userExist = await _dbContext.Users.AnyAsync(u => u.DNI == user.DNI);
      if (!userExist)
      {
        Console.WriteLine("No existe usuario con ese id");
        var userDB = _mapper.Map<User>(user);
        userDB.Password = EncryptPassword(userDB, user.Password);
        if (userRequestRol.Equals("Admin"))
        {
          Console.WriteLine("Rol de admin");
          await _dbContext.Users.AddAsync(userDB);
          await _dbContext.SaveChangesAsync();
          await transaction.CommitAsync();
          var userDTO = _mapper.Map<UserDTO>(user);
          res.UpdateValues("201", "User created succesfully", userDTO, null);
          return res;
        }
        else if (userRequestRol.Equals("Supervisor") || userRequestRol.Equals("Instructor"))
        {
          Console.WriteLine("Rol de supervisor o instructor");
          if (userDB.RoleId == 1 || userDB.RoleId == 2)
          {
            Console.WriteLine("intento crear un admin o supervisor sin permiso");
            await transaction.RollbackAsync();
            res.UpdateValues("403", "No tiene permisos para crear un usuario administrador o supervisor", null, "No tiene permisos suficientes");
            return res;
          }
          await _dbContext.Users.AddAsync(userDB);
          await _dbContext.SaveChangesAsync();
          await transaction.CommitAsync();
          var userDTO = _mapper.Map<UserDTO>(user);
          res.UpdateValues("201", "User created succesfully", userDTO, null);
          return res;
        }
        else
        {
          Console.WriteLine("Eror en los roles, ya que se envio de tipo usuario");
          res.UpdateValues("400", "Error con los roles", null, "No se identifico correctamente el rol a pesar de todos los filtros");
          return res;
        }
      }
      else
      {
        Console.WriteLine("El usuario existe");
        res.UpdateValues("409", "Ya existe un usuario con ese dni", null, "Conflict");
        return res;
      }
    }
    catch (Exception ex)
    {

      res.UpdateValues("500", "Ocurrio un Eror interno", null, ex.Message);
      return res;
    }


  }





  public async Task<User?> Auth(string dni, string password)
  {
    try
    {
      var user = await _dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.DNI == dni && u.Password == password);
      if (user != null)
      {
        return user;
      }
      return null;
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
      return null;
    }
  }

  //se envia dni del usuario y si el rol es instructor tambien el del instructor para solo consultar sobre sus alumnos
  public async Task<ResponseOne<User>> GetOne(string dni, string? instructorDNI)
  {
    var res = new ResponseOne<User> { Data = null, Error = null, Message = "", Status = "" };
    try
    {
      User? user = null;
      User? instructor = null;
      if (instructorDNI == null)
      {
        user = await _dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.DNI == dni);
      }
      else
      {
        instructor = await _dbContext.Users.FirstOrDefaultAsync(u => u.DNI == instructorDNI);
        if (instructor != null)
        {
          user = await _dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.DNI == dni && u.AcademyId == instructor.AcademyId);
        }
        else
        {
          res.UpdateValues("404", "No se encontro el instructor para hacer la consulta de su alumno", null, "Not Found");
          return res;
        }
      }
      if (user != null)
      {
        if (user.Role.Name.Equals("Admin") && user.Role.Name.Equals("Supervisor") && instructor != null)
        {
          res.UpdateValues("403", "Forbidden", null, "Forbidden");
        }
        res.UpdateValues("200", "Found User", user, null);
        return res;
      }
      res.UpdateValues("404", "User Not Found", null, "Not Found");
      return res;
    }
    catch (Exception ex)
    {
      res.UpdateValues("500", "Internal Server Error", null, ex.Message);
      return res;
    }
  }

  public string EncryptPassword(User user, string plainPassword)
  {
    return _passwordHasher.HashPassword(user, plainPassword);
  }

  public bool VerifyUserPassword(User user, string plainPassword)
  {
    var result = _passwordHasher.VerifyHashedPassword(user, user.Password, plainPassword);
    return result == PasswordVerificationResult.Success;
  }



}