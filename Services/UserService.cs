using lion_force_be.DBContext;
using lion_force_be.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace lion_force_be.Services;

public class UserService(DbContextLF dbContext)
{
  private readonly DbContextLF _dbContext = dbContext;
  private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
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
      Console.WriteLine(ex.Message);
      res.UpdateValues("500", "Internal Server Error", null, "Internal Server Error");
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