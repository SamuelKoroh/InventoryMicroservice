using InventoryService.BackgroundServices.Categories;
using InventoryService.BackgroundServices.Products;
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

            services.AddHostedService<GetCategories>();
            services.AddHostedService<GetCategoryById>();
            services.AddHostedService<GetProducts>();
            services.AddHostedService<GetProductsByCategoryId>();
            services.AddHostedService<CreateCategory>();
            services.AddHostedService<UpdateCategory>();
            services.AddHostedService<DeleteCategory>();

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
