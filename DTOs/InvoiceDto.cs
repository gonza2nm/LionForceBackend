public class InvoiceDTO
{
  public required bool Paid { get; set; }
  public required DateTime? PaymentDate { get; set; }
  public required DateTime DueDate { get; set; }
  public required int UserId { get; set; }
  public required int ServiceId { get; set; }
}