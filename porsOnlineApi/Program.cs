using Microsoft.AspNetCore.Mvc;
using porsOnlineApi.Extensions;
using porsOnlineApi.Models.ViewModels;
using porsOnlineApi.Services.Api;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApiConfig>(builder.Configuration.GetSection("ApiConfig"));
builder.Services.Configure<AutoSyncOptions>(builder.Configuration.GetSection("AutoSync"));
builder.Services.AddDb_InjectionnServices(builder.Configuration);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
})
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .Select(e => new
                {
                    Field = e.Key,
                    Errors = e.Value?.Errors.Select(x => x.ErrorMessage)
                });

            return new BadRequestObjectResult(errors);
        };
    });
// Add Swagger services
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "PorsLine API", Version = "v1" });
});

builder.Services.AddOpenApi();
builder.Services.AddHttpClient();
builder.Services.AddHostedService<AutoSyncService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PorsLine API V1");
        c.RoutePrefix = string.Empty; // Swagger at app root
    });
}
app.UseMiddleware<DatabaseCleanupMiddleware>();

app.MapControllers();

app.Run();
