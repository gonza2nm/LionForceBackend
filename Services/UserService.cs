using lion_force_be.DBContext;
using lion_force_be.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace lion_force_be.Services;

public class UserService(DbContextLF dbContext)
{
  private readonly DbContextLF _dbContext = dbContext;
  private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
  public async Task<bool> Auth(string dni, string password)
  {
    using var transaction = await _dbContext.Database.BeginTransactionAsync();
    try
    {
      var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.DNI == dni && u.Password == password);
      if (user != null)
      {
        return true;
      }
      return false;
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
      return false;
    }
  }

  public async Task<ResponseOne<User>> GetOne(string dni)
  {
    var res = new ResponseOne<User> { Data = null, Error = null, Message = "", Status = "" };
    try
    {
      var user = await _dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.DNI == dni);
      if (user != null)
      {
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