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

  public async Task<ResponseOne<User>> Add(UserRequestDTO user, string userRequestRol)
  {
    var res = new ResponseOne<User> { Status = "", Message = "", Data = null };
    using var transaction = await _dbContext.Database.BeginTransactionAsync();
    try
    {
      var userExist = await _dbContext.Users.AnyAsync(u => u.DNI == user.DNI);
      if (!userExist)
      {
        var userDB = _mapper.Map<User>(user);
        userDB.Password = EncryptPassword(userDB, user.Password);
        if (userRequestRol.Equals("Admin", StringComparison.OrdinalIgnoreCase))
        {
          await _dbContext.Users.AddAsync(userDB);
          await _dbContext.SaveChangesAsync();
          await transaction.CommitAsync();
          res.UpdateValues("201", "User created succesfully", userDB);
          return res;
        }
        else if (userRequestRol.Equals("Supervisor", StringComparison.OrdinalIgnoreCase) || userRequestRol.Equals("Instructor", StringComparison.OrdinalIgnoreCase))
        {
          if (userDB.RoleId == 1 || userDB.RoleId == 2)
          {
            await transaction.RollbackAsync();
            res.UpdateValues("403", "No tiene permisos para crear un usuario administrador o supervisor", null);
            return res;
          }
          await _dbContext.Users.AddAsync(userDB);
          await _dbContext.SaveChangesAsync();
          await transaction.CommitAsync();
          res.UpdateValues("201", "User created succesfully", userDB);
          return res;
        }
        else
        {
          res.UpdateValues("400", "Error con los roles", null);
          return res;
        }
      }
      else
      {
        res.UpdateValues("409", "Ya existe un usuario con ese dni", null);
        return res;
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
      res.UpdateValues("500", "Ocurrio un Eror interno", null);
      return res;
    }
  }

  public async Task<User?> Auth(string dni)
  {
    try
    {
      var user = await _dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.DNI == dni);
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


  public async Task<ResponseOne<User>> GetMyData(string dni)
  {
    var res = new ResponseOne<User> { Data = null, Message = "", Status = "" };
    try
    {
      var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.DNI == dni);
      if (user == null)
      {
        res.UpdateValues("404", "No se encontro el usuario", null);
        return res;
      }
      res.UpdateValues("200", "Se encontro el usuario exitosamente", user);
      return res;
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
      res.UpdateValues("500", "Ocurrio un error al hacer la consulta", null);
      return res;
    }
  }


  //se envia dni del usuario y si el rol es instructor tambien el del instructor para solo consultar sobre sus alumnos
  public async Task<ResponseOne<User>> GetOne(string dni, string? instructorDNI)
  {
    var res = new ResponseOne<User> { Data = null, Message = "", Status = "" };
    try
    {
      User? User = null;
      User? instructor = null;
      if (instructorDNI == null)
      {
        User = await _dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.DNI == dni);
      }
      else
      {
        instructor = await _dbContext.Users.FirstOrDefaultAsync(u => u.DNI == instructorDNI);
        if (instructor != null)
        {
          User = await _dbContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.DNI == dni);
          if (User != null && User.AcademyId != instructor.AcademyId)
          {
            res.UpdateValues("403", "No puede pedir datos de alumnos que no sean de su academia", null);
            return res;
          }
        }
        else
        {
          res.UpdateValues("404", "No se encontro el instructor para hacer la consulta de su alumno", null);
          return res;
        }
      }
      if (User == null)
      {
        res.UpdateValues("404", "No se encontro un usuario con ese dni", null);
        return res;
      }
      else
      {
        res.UpdateValues("200", "Found User", User);
        return res;
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
      res.UpdateValues("500", "Internal Server Error", null);
      return res;
    }
  }

  public async Task<ResponseList<User>> GetUsers(int? academyId, bool onlyStudents)
  {
    var res = new ResponseList<User> { Status = "", Message = "", Data = [] };
    try
    {
      List<User>? users;
      if (academyId == null)
      {
        users = await _dbContext.Users.ToListAsync();
        res.UpdateValues("200", "Usuarios encontrados", users);
        return res;
      }
      if (onlyStudents)
      {
        users = await _dbContext.Users.Include(u => u.Role).Where(u => u.AcademyId == academyId && u.Role.Name == "Student").ToListAsync();
      }
      else
      {
        users = await _dbContext.Users.Where(u => u.AcademyId == academyId).ToListAsync();
      }
      res.UpdateValues("200", "Usuarios encontrados", users);
      return res;
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.Message);
      res.UpdateValues("500", "Hubo un error al ejecutar al hacer la consulta a la base de datos", []);
      return res;
    }
  }

  public async Task<ResponseOne<User>> Update(string dni, UserDTO userDTO, string instructorDNI, bool onlyYourAcademy)
  {
    User? Instructor;
    User? User;
    var res = new ResponseOne<User> { Status = "", Message = "", Data = null };
    using var transaction = await _dbContext.Database.BeginTransactionAsync();
    try
    {
      User = await _dbContext.Users.FirstOrDefaultAsync(u => u.DNI == dni);
      if (User == null)
      {
        res.UpdateValues("404", "User not Found", null);
        return res;
      }
      if (onlyYourAcademy)
      {
        Instructor = await _dbContext.Users.FirstOrDefaultAsync(u => u.DNI == instructorDNI);
        if (Instructor != null)
        {
          if (User.AcademyId != userDTO.AcademyId)
          {
            res.UpdateValues("400", "No puede cambiar el alumno de academia, para hacerlo debe tener permisos de administrador", null);
            return res;
          }
          if (User.AcademyId == Instructor.AcademyId)
          {
            _mapper.Map<UserDTO, User>(userDTO, User);
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            res.UpdateValues("200", "User Updated successfully", User);
            return res;
          }
          else
          {
            res.UpdateValues("400", "No tienes permiso para editar ese usuario", null);
            return res;
          }
        }
        else
        {
          res.UpdateValues("400", "Instructor not found", null);
          return res;
        }
      }
      else
      {
        _mapper.Map<UserDTO, User>(userDTO, User);
        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        res.UpdateValues("200", "User Updated successfully", User);
        return res;
      }
    }
    catch (Exception ex)
    {
      await transaction.RollbackAsync();
      Console.WriteLine(ex.Message);
      res.UpdateValues("500", "Ocurrio un Error inesperado", null);
      return res;
    }
  }

  public async Task<ResponseOne<User>> UpdateWithPassword(string dni, UserDTOWithPassword userDTO, string instructorDNI, bool onlyYourAcademy)
  {
    User? Instructor;
    User? User;
    var res = new ResponseOne<User> { Status = "", Message = "", Data = null };
    using var transaction = await _dbContext.Database.BeginTransactionAsync();
    try
    {
      User = await _dbContext.Users.FirstOrDefaultAsync(u => u.DNI == userDTO.DNI);
      if (User == null)
      {
        res.UpdateValues("404", "User not Found", null);
        return res;
      }
      if (onlyYourAcademy)
      {
        Instructor = await _dbContext.Users.FirstOrDefaultAsync(u => u.DNI == instructorDNI);
        if (Instructor != null)
        {
          if (User.AcademyId != userDTO.AcademyId)
          {
            res.UpdateValues("400", "No puede cambiar el alumno de academia, para hacerlo debe tener permisos de administrador", null);
            return res;
          }
          if (User.AcademyId == Instructor.AcademyId)
          {
            _mapper.Map<UserDTOWithPassword, User>(userDTO, User);
            User.Password = EncryptPassword(User, userDTO.Password);
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            res.UpdateValues("200", "User Updated successfully", User);
            return res;
          }
          else
          {
            res.UpdateValues("400", "No tienes permiso para editar ese usuario", null);
            return res;
          }
        }
        else
        {
          res.UpdateValues("400", "Instructor not found", null);
          return res;
        }
      }
      else
      {
        _mapper.Map<UserDTOWithPassword, User>(userDTO, User);
        User.Password = EncryptPassword(User, userDTO.Password);
        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        res.UpdateValues("200", "User Updated successfully", User);
        return res;
      }
    }
    catch (Exception ex)
    {
      await transaction.RollbackAsync();
      Console.WriteLine(ex.Message);
      res.UpdateValues("500", "Ocurrio un Error inesperado", null);
      return res;
    }
  }

  public async Task<ResponseOne<User>> Delete(string dni)
  {
    var res = new ResponseOne<User> { Status = "", Message = "", Data = null };
    using var transaction = await _dbContext.Database.BeginTransactionAsync();
    try
    {
      var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.DNI == dni);
      if (user != null)
      {
        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        res.UpdateValues("200", "Eliminado Exitosamente", null);
        return res;
      }
      else
      {
        res.UpdateValues("404", "No se encontro el usuario que desea eliminar", null);
        return res;
      }
    }
    catch (Exception ex)
    {
      await transaction.RollbackAsync();
      Console.WriteLine(ex.Message);
      res.UpdateValues("500", "Ocurrio un error al eliminar el usuario", null);
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