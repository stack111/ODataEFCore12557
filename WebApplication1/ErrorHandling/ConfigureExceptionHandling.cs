
namespace WebApplication1
{
    using System;
    using System.Net;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    public static class ConfigureExceptionHandling
    {
        public static void UseErrorHandling(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.ContentType = MediaTypeConstants.JsonMediaType;
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    Exception ex = exceptionHandlerPathFeature?.Error;
                    context.Response.StatusCode = 500;
                    string requestPath = context.Request.Path;
                    Guid correlationId = Guid.NewGuid();
                    var logger = context.RequestServices.GetRequiredService<ILogger<IExceptionHandlerPathFeature>>();

                    if (ex != null && ex is ErrorResponseException errorResponseException)
                    {
                        context.Response.StatusCode = (int)errorResponseException.StatusCode;
                        string body = JsonConvert.SerializeObject(errorResponseException.ErrorResponse);
                        await context.Response.WriteAsync(body);
                        logger.LogWarning(errorResponseException, $"id:{correlationId} statusCode:{context.Response.StatusCode} body:{body}");
                    }
                    else if (ex == null)
                    {
                        var response = ErrorResponse.Builder()
                            .WithErrorCode(HttpStatusCode.InternalServerError.ToString())
                            .WithTarget(requestPath)
                            .Build();
                        logger.LogError(ex, $"id:{correlationId} - null exception");
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
                    }
                    else
                    {
                        var response = ErrorResponse.Builder()
                            .WithErrorCode(HttpStatusCode.InternalServerError.ToString())
                            .WithTarget(requestPath)
                            .Build();
                        var body = JsonConvert.SerializeObject(response);
                        await context.Response.WriteAsync(body);
                        logger.LogError(ex, $"id:{correlationId} statusCode:{context.Response.StatusCode} body:{body}");
                    }
                });
            });

            // handle status codes which are not returning response bodies. Status codes 4xx - 5xx
            app.UseStatusCodePages(async context =>
            {
                context.HttpContext.Response.ContentType = MediaTypeConstants.JsonMediaType;
                var exceptionHandlerPathFeature = context.HttpContext.Features.Get<IExceptionHandlerPathFeature>();
                string requestPath = context.HttpContext.Request.Path;
                Guid correlationId = Guid.NewGuid();
                var exception = exceptionHandlerPathFeature?.Error;
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<IExceptionHandlerPathFeature>>();
                if (exception != null && exception is ErrorResponseException errorException)
                {
                    string body = JsonConvert.SerializeObject(errorException.ErrorResponse);
                    await context.HttpContext.Response.WriteAsync(body);
                    logger.LogWarning(errorException, $"id:{correlationId} statusCode:{context.HttpContext.Response.StatusCode} body:{body}");
                    return;
                }

                HttpStatusCode parsedCode = (HttpStatusCode)context.HttpContext.Response.StatusCode;
                ErrorResponse errorResponse;
                if (parsedCode == HttpStatusCode.NotFound)
                {
                    errorResponse = ErrorResponse.Builder()
                           .WithErrorCode(parsedCode.ToString())
                           .WithTarget(requestPath)
                           .Build();
                }
                else if (parsedCode == HttpStatusCode.MethodNotAllowed)
                {
                    // convert to bad request
                    context.HttpContext.Response.StatusCode = 400;

                    errorResponse = ErrorResponse.Builder()
                           .WithErrorCode(HttpStatusCode.BadRequest.ToString())
                           .WithTarget(requestPath)
                           .Build();
                }
                else
                {
                    errorResponse = ErrorResponse.Builder()
                           .WithErrorCode(parsedCode.ToString())
                           .WithTarget(requestPath)
                           .Build();
                }

                string responseBody = JsonConvert.SerializeObject(errorResponse);
                await context.HttpContext.Response.WriteAsync(responseBody);
                logger.LogInformation($"id:{correlationId} statusCode:{context.HttpContext.Response.StatusCode} body:{responseBody}");
            });
        }
    }
}
