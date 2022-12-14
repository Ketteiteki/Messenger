using Messenger.BusinessLogic.Hubs;
using Messenger.Domain.Constants;
using Messenger.Infrastructure.DependencyInjection;
using Messenger.Infrastructure.Middlewares;
using Messenger.Services;
using Microsoft.EntityFrameworkCore;

namespace Messenger.WebApi;

public class Startup
{
    private const string CorsPolicyName = "DefaultCors";
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddControllers();

        var databaseConnectionString = _configuration[AppSettingConstants.DatabaseConnectionString];
        var signKey = _configuration[AppSettingConstants.MessengerJwtSettingsSecretAccessTokenKey];
        var allowOrigins = _configuration[AppSettingConstants.AllowedHosts];

        serviceCollection.AddDatabaseServices(databaseConnectionString);

        serviceCollection.AddInfrastructureServices(signKey);

        serviceCollection.AddMessengerServices();
        
        serviceCollection.ConfigureCors(CorsPolicyName, allowOrigins);

        serviceCollection.AddSwagger();

        var databaseContext = serviceCollection.BuildServiceProvider().GetService<DatabaseContext>();
        
        databaseContext?.Database.Migrate();
    }

    public void Configure(IApplicationBuilder applicationBuilder, IHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            applicationBuilder.UseSwagger();
            applicationBuilder.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Messenger Api v1");
                options.RoutePrefix = "";
            });
        }
        
        applicationBuilder.UseStaticFiles();

        applicationBuilder.UseHttpsRedirection();

        applicationBuilder.UseRouting();

        applicationBuilder.UseCors(CorsPolicyName);
        
        applicationBuilder.UseAuthentication();
        applicationBuilder.UseAuthorization();
        
        applicationBuilder.UseMiddleware<ValidationMiddleware>();
        
        applicationBuilder.UseEndpoints(options =>
        {
            options.MapHub<ChatHub>("/notification").RequireCors(CorsPolicyName);
            options.MapControllers();
        });
    }
}