﻿using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Backend;

public class ConfigureSwaggerOptions: IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;
    
    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => _provider = provider;

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(
                description.GroupName,
                new OpenApiInfo
                {
                    Title = $"Chess-online API {description.ApiVersion}",
                    Version = description.ApiVersion.ToString(),
                    Description = "ASP.NET Basic web API for chess",
                    Contact = new OpenApiContact
                    {
                        Name = "Telegram Contact",
                        Url = new Uri("https://t.me/ProgMaksim")
                    }
                });
        }
    }
}