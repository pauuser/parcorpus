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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
