﻿using Microsoft.AspNetCore.Http;

namespace EC.SharedLibrary.Middleware;

public class ListenToOnlyApiGateway(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Extract specific header from the request
        var signedHeader = context.Response.Headers["Api-Gateway"];
        
        // Null means , the request is not coming from the Api Gateway
        // 503 StatusCode
        if (signedHeader.FirstOrDefault() is null)
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await context.Response.WriteAsync("Sorry , Service is unavailable");
            return;
        }
        else
        {
            await next(context);
        }
    }
}
