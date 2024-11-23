namespace lion_force_be.Middlewares;

public class CookieToAuthorizationHeaderMiddleware(RequestDelegate next)
{
  private readonly RequestDelegate _next = next;

  public async Task Invoke(HttpContext context)
  {
    if (context.Request.Cookies.ContainsKey("authToken"))
    {
      var token = context.Request.Cookies["authToken"];
      if (!string.IsNullOrEmpty(token))
      {
        context.Request.Headers["Authorization"] = $"Bearer {token}";
      }
    }
    await _next(context);
  }
}