using System.Text.Json.Serialization;
using lion_force_be.Models.Relations;

namespace lion_force_be.Models;


public class Service : BaseEntity
{
  public required string Name { get; set; }
  public required string Details { get; set; }
  public int AcademyId { get; set; }
  public List<UserService> UserServices { get; set; } = new List<UserService>();
  public List<Price> Prices { get; set; } = new List<Price>();

  [JsonIgnore]
  public Academy Academy { get; set; } = null!;
}