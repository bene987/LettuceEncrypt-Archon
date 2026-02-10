// Copyright (c) Nate McMaster & Archon Systems Inc.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure LettuceEncrypt, adding required services to the DI container
builder.Services.AddLettuceEncrypt(o =>
{
    if (o.UseStagingServer)
    {
        o.AdditionalIssuers = Directory.GetFiles("Testing", "*.pem")
            .Select(File.ReadAllBytes)
            .Select(pem => Encoding.UTF8.GetString(pem))
            .ToArray();
    }
});

builder.WebHost.PreferHostingUrls(false);
builder.WebHost.UseKestrel(k =>
{
    // Configure Kestrel to use LettuceEncrypt for HTTPS for this endpoint
    k.ListenAnyIP(443, o => o.UseLettuceEncrypt(k.ApplicationServices));
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

// Minimal API endpoint
app.MapGet("/", () => "Hello World!");

app.Run();

