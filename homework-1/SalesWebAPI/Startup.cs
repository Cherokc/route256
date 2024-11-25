using Domain.Interfaces;
using DataAccess;
using DataAccess.Commands;

namespace SalesWebAPI
{
    internal class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IADSCommand, ADSCommand>();
            services.AddScoped<ISalesPredictionCommand, SalesPredictionCommand>();
            services.AddScoped<IDemandCommand, DemandCommand>();

            services.AddScoped<IProductRepository, ProductRepository>(s =>
            {
                var configuration = s.GetRequiredService<IConfiguration>();
                var dataPath = configuration["ProductRepository:DataPath"];
                return new ProductRepository(dataPath);
            });

            services.AddScoped<ISalesService, SalesService>(s =>
            {
                var adsCommand = s.GetRequiredService<IADSCommand>();
                var predictionCommand = s.GetRequiredService<ISalesPredictionCommand>();
                var demandCommand = s.GetRequiredService<IDemandCommand>();
                var repository = s.GetRequiredService<IProductRepository>();

                return new SalesService(adsCommand, predictionCommand, demandCommand, repository);
            });

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
