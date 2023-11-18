using Microsoft.OpenApi.Models;
using Parcorpus.API.Extensions;
using Parcorpus.DataAccess.Context.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddLogging();
builder.Services.AddEndpointControllers();

builder.Services.AddSwagger();

builder.Services.ConnectToDatabase(builder.Configuration.GetConnectionString("DbConnectionString")!);
builder.Services.AddRepositories();

builder.Services.AddServices();

builder.Services.AddHttpClient();

builder.Services.AddJwtBearerAuthorization(builder.Configuration);

builder.Services.AddCors();

var _corsPolicy = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(_corsPolicy, builder =>
    {
        builder.WithOrigins("http://localhost:5173",
                            "http://127.0.0.1:5173",
                            "http://localhost",
                            "http://127.0.0.1",
                            "http://0.0.0.0:5173",
                            "http://185.31.160.57:5550/")
            .AllowCredentials()
            .AllowAnyHeader()
            .SetIsOriginAllowed((host) => true)
            .SetPreflightMaxAge(TimeSpan.FromSeconds(86400));
    });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(_corsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
