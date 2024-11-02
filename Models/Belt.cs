namespace lion_force_be.Models;

public class Belt : BaseEntity
{
  public required string BeltRank { get; set; }
  public string? Degree { get; set; }
  public bool Decided { get; set; }
  public List<User> Users { get; set; } = new List<User>();

}