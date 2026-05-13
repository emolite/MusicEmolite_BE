using Microsoft.AspNetCore.Http.Features;
using MS_API.Extensions;
using MS_Infrastructure.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 524288000;
});

builder.Services.AddControllers();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 524288000;
    options.MultipartHeadersLengthLimit = 524288000;
});

builder.Services
    .SetDBContext(builder.Configuration)
    .AddSwaggerConfiguration()
    .AddCorsConfiguration()
    .AddJwtAuthentication(builder.Configuration)
    .AddCloudinaryConfiguration(builder.Configuration)
    .AddApplicationServices();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
