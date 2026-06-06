using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
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

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;

    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services
    .SetDBContext(builder.Configuration)
    .AddSwaggerConfiguration()
    .AddCorsConfiguration()
    .AddJwtAuthentication(builder.Configuration)
    .AddCloudinaryConfiguration(builder.Configuration)
    .AddLyricsApiConfiguration(builder.Configuration)
    .AddApplicationServices()
    .AddHttpContextAccessor();

var app = builder.Build();
app.UseForwardedHeaders();
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
