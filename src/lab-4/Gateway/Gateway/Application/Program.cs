using Gateway.Extensions;
using Gateway.Middlewares;
using Gateway.Services.Implementations;
using Gateway.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.Application;

internal abstract class Program
{
    private static Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.WebHost.UseUrls("http://localhost:5001");
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        builder.Services.AddGatewayServices(builder.Configuration);
        builder.Services.AddSingleton<IOrderGatewayService, OrderGatewayService>();

        WebApplication app = builder.Build();

        app.UseMiddleware<GrpcExceptionMiddleware>();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway lab-3 API v1");
        });
        app.UseHttpsRedirection();
        app.MapControllers();

        app.Run();
        return Task.CompletedTask;
    }
}