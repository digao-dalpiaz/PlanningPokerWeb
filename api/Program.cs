using api;
using api.Exceptions;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR(options =>
{
    //options.EnableDetailedErrors = true;
    options.AddFilter<HubExceptionHandler>();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", p =>
    {
        p.AllowAnyMethod()
         .AllowAnyHeader()
         .AllowCredentials()
         .SetIsOriginAllowed(origin => builder.Environment.IsDevelopment() ? true : 
            origin.Equals(Global.URL_FRONTEND, StringComparison.InvariantCultureIgnoreCase));
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection(); -- não redirecionar HTTPS pois usamos dentro do container

app.UseAuthorization();

app.MapControllers();

app.UseCors("CorsPolicy");
app.MapHub<ChatHub>("/chatHub");

ApiExceptionHandler.ConfigurarExceptionHandler(app);

app.Run();
