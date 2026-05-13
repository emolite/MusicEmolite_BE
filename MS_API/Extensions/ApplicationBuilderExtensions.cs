// <copyright file=WebApplicationExtensions.cs company= AMF>
// Copyright (c) AMF. All rights reserved.
// </copyright>

namespace AMFS.API.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseApplicationPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Localhost")
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MUSIC API v1");
                c.RoutePrefix = "swagger";
            });
        }
        else
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors("AllowAll");

        app.UseStaticFiles();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}