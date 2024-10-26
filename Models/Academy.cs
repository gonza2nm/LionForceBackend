namespace lion_force_be.Models;

public class Academy : BaseEntity
{
  public required string Name { get; set; }
  public List<User> Users { get; set; } = new List<User>();
  public List<Service> Services { get; set; } = new List<Service>();
}