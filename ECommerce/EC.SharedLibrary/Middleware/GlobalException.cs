using System.Net;
using System.Text.Json;
using EC.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EC.SharedLibrary.Middleware;

public  class GlobalException(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Declare variables
        string message = "sorry , internal server error";
        int statusCode = (int)HttpStatusCode.InternalServerError;
        string title = "Internal Server Error";

        try
        {
            await next(context);
            
            // check if Response is too many request => 429 StatusCode
            if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
            {
                message = "Too Many Requests";
                statusCode = (int)HttpStatusCode.TooManyRequests;
                title = "Warning";
                
                // ModifyHeader
                await ModifyHeader(context , message , statusCode , title);
            }
            
            // If Response is UnAuthorized => 401 StatusCode
            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                title = "Alert";
                message = "You are not authorized to access this resource";
                
                // ModifyHeader
                await ModifyHeader(context , message , statusCode , title);
            }
            
            // If Response is Forbidden => 403 StatusCode
            if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                title = "Out of Access To Server";
                message = "You are not allowed/required to access.";
                statusCode = (int)HttpStatusCode.Forbidden;
                
                // ModifyHeader
                await ModifyHeader(context , message , statusCode , title);
            }
        }
        catch (Exception e)
        {
           // Log Original Exception => Console | File | Debug
           LogException.LogExceptions(e);
           
           //check if Exception is Timeout => 408 StatusCode
           if (e is TaskCanceledException or TimeoutException)
           {
               title = "Out of Time Server";
               message = "Time Out... Try Again";
               statusCode = StatusCodes.Status408RequestTimeout;
           }
           
           // If none of the exceptions then do the default
           await ModifyHeader(context , message , statusCode , title);
        }
    }
    private static async Task ModifyHeader(HttpContext context, string message, int statusCode, string title)
    {
       // display scary-free message to client
       context.Response.ContentType = "application/json";
       
       await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
       {
           Status = statusCode,
           Title = title,
           Detail = message
           
       }) , CancellationToken.None);
       
       return;
    }
}
