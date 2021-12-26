using System.Net;
using FirstApp.Consts;
using Microsoft.AspNetCore.Server.IISIntegration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAuthentication(IISDefaults.AuthenticationScheme);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed(_ => true)
            .AllowCredentials());
});
builder.Services.AddHttpClient(ServiceConsts.ImpersonateClientName)
    .ConfigurePrimaryHttpMessageHandler(_ =>
        new SocketsHttpHandler()
        {
            UseProxy = false,
            Credentials = CredentialCache.DefaultCredentials,
            PreAuthenticate = false,
            //PooledConnectionLifetime = TimeSpan.Zero
        });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers().RequireCors("AllowSpecificOrigin").RequireAuthorization();

app.Run();
