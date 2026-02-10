// Copyright (c) Nate McMaster & Archon Systems Inc.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

var builder = WebApplication.CreateBuilder(args);

// Configure LettuceEncrypt, adding required services to the DI container
builder.Services.AddLettuceEncrypt();
builder.WebHost.UseKestrel(k =>
{
    k.ListenAnyIP(443, o =>
    {
        // Configure Kestrel to use LettuceEncrypt for HTTPS for this endpoint
        o.UseLettuceEncrypt(k.ApplicationServices);

        // Optionally enable HTTP/3
        o.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2AndHttp3;
    });
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

