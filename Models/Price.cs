using System.Text.Json.Serialization;

namespace lion_force_be.Models;

public class Price : BaseEntity
{
  public required decimal Value { get; set; }
  public required DateTime FromDate { get; set; }
  public DateTime? UntilDate { get; set; }
  public int ServiceId { get; set; }
  [JsonIgnore]
  public Service Service { get; set; } = null!;
}