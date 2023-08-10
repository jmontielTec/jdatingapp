using API.Helpers;
using API.Services;
using API.SignalR;
using Microsoft.OpenApi.Models;

namespace API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<PresenceTracker>();
        services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPhotoService, PhotoService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<LogUserActivity>();
        services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
       // services.AddDbContext<DataContext>(options =>
       //  {
        //     var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        //     if (env == "Development")
        //     {
        //         options.UseSqlServer(config.GetConnectionString("DefaultConnection"));
        //     }
        //     else{
                // Use connection string provided at runtime by Heroku.
        //         var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
        //         options.UseSqlServer(connUrl);
        //     }
       //  });

        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            var groupName = "v1";
            options.CustomSchemaIds(type => type.ToString());
            options.SwaggerDoc(groupName, new OpenApiInfo
            {
                Title = $"WebAPIv5 {groupName}",
                Version = groupName,
                Description = " API ",
                Contact = new OpenApiContact
                {
                    Name = "DatingAPP",
                    Email = string.Empty,
                    Url = new Uri("https://www.nothing.com")

                }
            });

            //***********************************************************************
            //Uncomment this if you want to used authentication local
            //***********************************************************************
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            //***********************************************************************

            //////Add Operation Specific Authorization///////
            options.OperationFilter<Filters.AuthOperationFilter>();
            ////////////////////////////////////////////////

            //options.IncludeXmlComments(Path.Combine(Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationBasePath, "SwaggerExample.xml"));

        });

        return services;
    }
}