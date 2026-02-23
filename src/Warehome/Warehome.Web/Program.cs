using Warehome.Application;
using Warehome.Infrastructure;
using Warehome.Web;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddWeb();

WebApplication app = builder.Build();

app.UseRouting();
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();

app.Run();