using lion_force_be.Models;

public class ResponseToken
{
  public string? Token { get; set; }
}

public class ServiceResponseOne<T>
{
  public required string StatusCode { get; set; }
  public required string Message { get; set; }
  public required T? Data { get; set; }

  public void UpdateValues(string status, string message, T? data)
  {
    StatusCode = status;
    Message = message;
    Data = data;
  }
}

public class ServiceResponseList<T>
{
  public required string StatusCode { get; set; }
  public required string Message { get; set; }
  public required List<T> Data { get; set; }

  public void UpdateValues(string status, string message, List<T> data)
  {
    StatusCode = status;
    Message = message;
    Data = data;
  }
}

public class ResponseOne<T>
{
  public required string Message { get; set; }
  public required T? Data { get; set; }

  public void UpdateValues(string message, T? data)
  {
    Message = message;
    Data = data;
  }

}
public class ResponseList<T>
{
  public required string Message { get; set; }
  public required List<T> Data { get; set; }

  public void UpdateValues(string message, List<T> data)
  {
    Message = message;
    Data = data;
  }

}