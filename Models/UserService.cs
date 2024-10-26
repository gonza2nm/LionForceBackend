using System.Text.Json.Serialization;

namespace lion_force_be.Models.Relations;

public class UserService
{
  public int UserId { get; set; }
  public int ServiceId { get; set; }
  public DateTime PaymentDate { get; set; }
  [JsonIgnore]
  public User User { get; set; } = null!;
  [JsonIgnore]
  public Service Service { get; set; } = null!;

}