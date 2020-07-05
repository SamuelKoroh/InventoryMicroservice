using InventoryService.BackgroundServices.RabbitMQ.Categories;
using InventoryService.BackgroundServices.RabbitMQ.Products;
using InventoryService.BackgroundServices.RabbitMQ.Requests;
using InventoryService.Domain;
using InventoryService.Domain.Services;
using InventoryService.Persistence;
using InventoryService.Persistence.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace InventoryService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<InventoryDbContext>(config => 
                config.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IRequestService, RequestService>();

            services.AddHostedService<RabGetAllCategoryService>();
            services.AddHostedService<RadGetCategoryByIdService>();
            services.AddHostedService<RabGetAllAvailableProductService>();
            services.AddHostedService<RabGetAllProductService>();
            services.AddHostedService<RabGetProductByIdService>();
            services.AddHostedService<RabGetProductsForCategoryService>();
            services.AddHostedService<RabGetAllRequestService>();
            services.AddHostedService<RabGetAllRequestByRequesterService>();
            services.AddHostedService<RabCreateCategoryService>();
            services.AddHostedService<RabCreateProductService>();
            services.AddHostedService<RabUpdateCategoryService>();
            services.AddHostedService<RabUpdateProductService>();
            services.AddHostedService<RabDeleteCategoryService>();
            services.AddHostedService<RabDeleteProductService>();
            services.AddHostedService<RabRequestApprovalService>();
            services.AddHostedService<RabPlaceRequestService>();


            services.AddControllers()
                    .AddNewtonsoftJson(o => 
                        o.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
