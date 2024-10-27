using lion_force_be.Models;

namespace lion_force_be.DTOs;

public class UserDTO
{
  public required string DNI { get; set; }
  public required string Name { get; set; }
  public required string LastName { get; set; }
  public required DateTime BirthDate { get; set; }
  public required string Password { get; set; }
  public int BeltId { get; set; }
  public int AcademyId { get; set; }
}
public class UserRequestDTO
{
  public required string DNI { get; set; }
  public required string Name { get; set; }
  public required string LastName { get; set; }
  public required DateTime BirthDate { get; set; }
  public required string Password { get; set; }
  public int BeltId { get; set; }
  public int AcademyId { get; set; }
  public int RoleId { get; set; }
}