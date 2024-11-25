﻿using FluentValidation;
using ProductService.Domain.Repository;
using ProductService.WebApi.Validators;
using ProductService.WebApi.Interceptors;
using ProductService.WebApi.Validators.Grpc;
using ProductService.WebApi.Validators.Asp;
using ProductService.WebApi.Middlewares;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace ProductService.WebApi;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[] { new CultureInfo("en-US") };
            options.DefaultRequestCulture = new RequestCulture("en-US");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
        });

        services.AddControllers();
        services.AddSwaggerGen(c => {
            c.EnableAnnotations();
        });

        services.AddGrpc(op => {
            op.Interceptors.Add<LoggerInterceptor>();
            op.Interceptors.Add<ExceptionInterceptor>();
        });

        services.AddGrpcReflection();

        services.AddSingleton<IProductRepository, DataAccess.ProductRepository>();

        services.AddValidatorsFromAssemblyContaining<CreateProductRequestValidator>(); 
        services.AddValidatorsFromAssemblyContaining<GetProductByIdRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<GetProductsRequestValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdateProductPriceRequestValidator>();

        services.AddValidatorsFromAssemblyContaining<DomainProductValidator>();
        services.AddValidatorsFromAssemblyContaining<FilterValidator>();
        services.AddValidatorsFromAssemblyContaining<ProductDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<UpdatePriceRequestValidator>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();

        app.UseMiddleware<LoggerMiddleware>();

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGrpcService<GrpcServices.ProductService>();
            endpoints.MapGrpcReflectionService();
        });
    }
}