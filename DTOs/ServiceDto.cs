using lion_force_be.Models;

namespace lion_force_be.DTOs;

public class ServiceDTO
{
  public required string Name { get; set; }
  public required string Details { get; set; }
  public required int AcademyId { get; set; }
  public decimal Value { get; set; }
}

public class ServiceRequestDTO
{
  public required string Name { get; set; }
  public required string Details { get; set; }
  public required int AcademyId { get; set; }
  public required decimal Value { get; set; }
}
public class ServiceUpdateDTO
{
  public required string Name { get; set; }
  public required string Details { get; set; }
  public required decimal Value { get; set; }
}