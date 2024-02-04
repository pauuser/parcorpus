using Parcorpus.API.Extensions;
using Parcorpus.DataAccess.Context;
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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
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

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials());

app.Migrate<ParcorpusDbContext>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }