using lion_force_be.Models.Relations;

namespace lion_force_be.Models;

public class Invoice : BaseEntity
{
  public bool Paid { get; set; }
  public DateTime? PaymentDate { get; set; }
  public DateTime DueDate { get; set; }
  public int UserId { get; set; }
  public int ServiceId { get; set; }
  public UserService UserService { get; set; } = null!;
}