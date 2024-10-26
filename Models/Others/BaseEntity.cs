using System.ComponentModel.DataAnnotations;

namespace lion_force_be.Models;

public abstract class BaseEntity
{
  [Key]
  public int Id { get; set; }
}