using System.Threading.RateLimiting;
using GeracaoPropostaOrigo.Enums;
using GeracaoPropostaOrigo.Model;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using FluentValidation;
using GeracaoPropostaOrigo.PostValidator;
using GeracaoPropostaOrigo.DTOs;
using GeracaoPropostaOrigo.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using GeracaoPropostaOrigo.Logs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IValidator<Proposta>, PropostaValidator>();
builder.Services.AddScoped<GeracaoDeProposta>();

var Origins = "_origensComAcessoPermitido";

builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
{
    LogLevel = LogLevel.Information
}));

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: Origins, policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddRateLimiter(limiterOptions =>
{
    limiterOptions.AddFixedWindowLimiter("fixed", options =>
    {
        options.PermitLimit = 5;
        options.Window = TimeSpan.FromMinutes(1);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 1;
    });
    limiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erro inesperado ao processar a requisição. " + DateTime.Now);
        context.Response.StatusCode = 500;
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(Origins);

app.UseRateLimiter();

app.MapPost("/proposta", async ([FromBody] Proposta dadosBrutos, IValidator<Proposta> validator, GeracaoDeProposta geracao, ILogger<Proposta> logger) =>
{

    var validation = await validator.ValidateAsync(dadosBrutos);

    if (!validation.IsValid)
    {
        logger.LogError("Dados inválidos do cliente: " + dadosBrutos.RazaoSocial + " " + DateTime.Now);
        return Results.BadRequest();
    }
    var result = await geracao.GerarPdf(dadosBrutos);

    if (result != Results.Ok())
    {
        logger.LogError("Erro ao gerar proposta do cliente: " + dadosBrutos.RazaoSocial + " " + DateTime.Now);
        return Results.StatusCode(500);
    }
    string basePath = AppDomain.CurrentDomain.BaseDirectory;
    string resourcePath = Path.Combine(basePath, "Resources", "Proposta.pdf");

    var fileBytes = await File.ReadAllBytesAsync(resourcePath);

    logger.LogInformation("Sucesso ao gerar proposta do cliente: " + dadosBrutos.RazaoSocial + " " + DateTime.Now);
    return Results.File(fileBytes, "application/pdf", "Proposta.pdf");
});

app.Run();