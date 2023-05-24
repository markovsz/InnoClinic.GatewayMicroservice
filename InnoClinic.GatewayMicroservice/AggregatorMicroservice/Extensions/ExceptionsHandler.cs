using AggregatorMicroservice.Exceptions;
using Newtonsoft.Json;
using System.Net;

namespace AggregatorMicroservice.Extensions;

public class ExceptionsHandler
{
    private readonly RequestDelegate _next;

    public ExceptionsHandler(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (BadHttpRequestException ex)
        {
            await HandleExceptionAsync(context, ex.Message, HttpStatusCode.BadRequest);
        }
        catch (UnauthorizedAccessException ex)
        {
            await HandleExceptionAsync(context, ex.Message, HttpStatusCode.Unauthorized);
        }
        catch (ForbiddenException ex)
        {
            await HandleExceptionAsync(context, ex.Message, HttpStatusCode.Forbidden);
        }
        catch (NotFoundException ex)
        {
            await HandleExceptionAsync(context, ex.Message, HttpStatusCode.NotFound);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, "internal server error", HttpStatusCode.InternalServerError);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, string message, HttpStatusCode code)
    {
        var response = context.Response;

        response.ContentType = "application/json";
        response.StatusCode = (int)code;

        await response.WriteAsync(JsonConvert.SerializeObject(new { Status = code, Message = message }));
    }
}
