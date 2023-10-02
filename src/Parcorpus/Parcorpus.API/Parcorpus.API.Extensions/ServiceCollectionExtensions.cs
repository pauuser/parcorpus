using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Parcorpus.API.Controllers;
using Parcorpus.Core.Configuration;
using Parcorpus.Core.Interfaces;
using Parcorpus.DataAccess.Repositories;
using Parcorpus.Services.AnnotationService;
using Parcorpus.Services.AuthService;
using Parcorpus.Services.JobService;
using Parcorpus.Services.LanguageService;
using Parcorpus.Services.QueueConsumerService;
using Parcorpus.Services.QueueProducerService;
using Parcorpus.Services.UserService;

namespace Parcorpus.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<ILanguageRepository, LanguageRepository>();
        serviceCollection.AddTransient<IUserRepository, UserRepository>();
        serviceCollection.AddTransient<ICredentialsRepository, CredentialsRepository>();
        serviceCollection.AddTransient<ISearchHistoryRepository, SearchHistoryRepository>();
        serviceCollection.AddTransient<IJobRepository, JobRepository>();

        return serviceCollection;
    }

    public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddOptions<WebAlignerConfiguration>()
            .BindConfiguration(WebAlignerConfiguration.ConfigurationSectionName);
        serviceCollection.AddOptions<LanguagesConfiguration>()
            .BindConfiguration(LanguagesConfiguration.ConfigurationSectionName);
        serviceCollection.AddOptions<WordAlignerConfiguration>()
            .BindConfiguration(WordAlignerConfiguration.ConfigurationSectionName);
        serviceCollection.AddOptions<TokenConfiguration>()
            .BindConfiguration(TokenConfiguration.ConfigurationSectionName);
        serviceCollection.AddOptions<QueueConfiguration>()
            .BindConfiguration(QueueConfiguration.ConfigurationSectionName);
        
        serviceCollection.AddTransient<IUserService, UserService>();
        serviceCollection.AddTransient<IAnnotationService, AnnotationService>();
        serviceCollection.AddTransient<ILanguageService, LanguageService>();
        serviceCollection.AddTransient<IAuthService, AuthService>();
        serviceCollection.AddSingleton<IWordAligner, Parcorpus.Services.AnnotationService.WordAlignerClient.WordAligner>();
        serviceCollection.AddSingleton<IQueueProducerService, QueueProducerService>();
        serviceCollection.AddTransient<IJobService, JobService>();
        serviceCollection.AddHostedService<QueueConsumerService>();
        
        return serviceCollection;
    }

    public static IServiceCollection AddJwtBearerAuthorization(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,

                ValidAudience = configuration["JWT:Audience"],
                ValidIssuer = configuration["JWT:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]))
            };
        });
        
        return serviceCollection;
    }

    public static IServiceCollection AddEndpointControllers(this IServiceCollection serviceCollection)
    {
        var mvcBuilder = MvcServiceCollectionExtensions.AddControllers(serviceCollection);
        mvcBuilder.PartManager.ApplicationParts.Add(new AssemblyPart(typeof(AuthController).Assembly));
        serviceCollection.AddEndpointsApiExplorer();
        
        return serviceCollection;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc("v1", new OpenApiInfo() { Title = "Parcorpus.API", Version = "v1" });
            opt.UseInlineDefinitionsForEnums();
            var securityDefinition = new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                BearerFormat = "JWT",
                Scheme = "Bearer",
                Description = "Specify the authorization token",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
            };
            opt.AddSecurityDefinition("Bearer", securityDefinition);
            
            var securityRequirement = new OpenApiSecurityRequirement();
            var secondSecurityDefinition = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };
            securityRequirement.Add(secondSecurityDefinition, new string[] { });
            opt.AddSecurityRequirement(securityRequirement);
            
            var xmlFiles = new[] 
            {
                "Parcorpus.API.Controllers.xml",
                "Parcorpus.API.Dto.xml",
            };
    
            foreach (var xmlFile in xmlFiles) 
            {
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath)) 
                {
                    opt.IncludeXmlComments(xmlPath);
                }
            }

        });

        return serviceCollection;
    }
}