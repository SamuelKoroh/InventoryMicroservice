using ApiGateway.GraphQLObj.GrahQLQueries;
using ApiGateway.GraphQLObj.GrahQLSchema;
using ApiGateway.GraphQLObj.GraphQL;
using GraphiQl;
using GraphQL;
using GraphQL.Types;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ApiGateway
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
            services.AddTransient<IDocumentExecuter, DocumentExecuter>();
            services.AddTransient<RootQuery>();
            //services.AddTransient<ProductType>();
            services.AddTransient<CategoryType>();
            var sp = services.BuildServiceProvider();
            services.AddSingleton<ISchema>(new AppSchema(new FuncDependencyResolver(type => sp.GetService(type))));
            services.AddMassTransit(x =>
            {
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.Host("rabbitmq://localhost");
                }));
            });
            services.AddMassTransitHostedService();
            services.AddControllers()
        .AddNewtonsoftJson(o => o.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseGraphiQl();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
