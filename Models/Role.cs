namespace lion_force_be.Models;


public class Role : BaseEntity
{
  public required string Name { get; set; }
  public List<User> Users { get; set; } = new List<User>();
}