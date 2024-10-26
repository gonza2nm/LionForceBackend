using lion_force_be.DBContext;
using lion_force_be.Models;
using Microsoft.EntityFrameworkCore;

namespace lion_force_be.Services;

public class UserService(DbContextLF dbContext)
{
  private readonly DbContextLF _dbContext = dbContext;

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
}