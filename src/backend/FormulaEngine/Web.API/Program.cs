using Application.API;
using Domain.Users;
using Infrastructure.AppSettings;
using Infrastructure.AppSettings.Models;
using Infrastructure.Persistence;
using NLog;
using NLog.Web;
using Web.API.Endpoints;
using Web.API.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var logger = LogManager.Setup()
    .LoadConfigurationFromAppSettings(nlogConfigSection: nameof(NLogSettings))
    .GetCurrentClassLogger();
try
{
    var configuration = builder.Configuration.AddJsonConfigurations();

    builder.UseNLog();

    builder.Services
        .AddExceptionHandler<GlobalExceptionHandler>()
        .AddProblemDetails()
        .AddSwagger()
        .AddEndpoints()
        .ConfigureSerializerOptions()
        .AddPersistence(configuration)
        .AddIdentity()
        .AddRequestHandlers()
        .AddMemoryCache(x =>
        {
            x.SizeLimit = 1000;
        });


    var app = builder.Build();
    app.UseAuthentication();
    app.UseAuthorization();
    

    app.MapGet("/api/ping", () => "pong");
    
    app.MapGroup("/api/users").MapIdentityApi<User>().WithTags(Tags.Account);
    app.MapEndpoints(app.MapGroup("api"));

    if (app.Environment.IsDevelopment())
    {
        await app.ApplyMigrations();
        app.MapOpenApi();
        app.UseSwagger();
    }

    app.UseHttpsRedirection();


    await app.RunAsync();
}
catch (Exception ex)
{
    logger.Error(ex, "Exception occured during startup {Message}.", ex.Message);
}
finally
{
    LogManager.Shutdown();
}
