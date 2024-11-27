using lion_force_be.Models;

public class ResponseToken
{
  public string? Token { get; set; }
}
public class ResponseOne<T>
{
  public required string Status { get; set; }
  public required string Message { get; set; }
  public required T? Data { get; set; }

  public void UpdateValues(string status, string message, T? data)
  {
    Status = status;
    Message = message;
    Data = data;
  }

}
public class ResponseList<T>
{
  public required string Status { get; set; }
  public required string Message { get; set; }
  public required List<T> Data { get; set; }

  public void UpdateValues(string status, string message, List<T> data)
  {
    Status = status;
    Message = message;
    Data = data;
  }

}