using System.Text.Json.Serialization;
using lion_force_be.Models.Relations;

namespace lion_force_be.Models;
public class User : BaseEntity
{
  public required string DNI { get; set; }
  public required string Name { get; set; }
  public required string LastName { get; set; }
  public required DateTime BirthDate { get; set; }
  public int BeltId { get; set; }
  public int AcademyId { get; set; }
  public int RoleId { get; set; }
  public List<UserService> UserServices { get; set; } = new List<UserService>();

  [JsonIgnore]
  public Academy Academy { get; set; } = null!;
  [JsonIgnore]
  public Belt Belt { get; set; } = null!;
  [JsonIgnore]
  public Role Role { get; set; } = null!;
}