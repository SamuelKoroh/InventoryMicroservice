using ApiGateway.GraphQLObj.GrahQLMutations;
using ApiGateway.GraphQLObj.GrahQLQueries;
using ApiGateway.GraphQLObj.GrahQLSchema;
using ApiGateway.GraphQLObj.GraphQL;
using ApiGateway.GraphQLObj.GraphQLTypes;
using ApiGateway.RedisPubSub;
using ApiGateway.Utils;
using GraphiQl;
using GraphQL;
using GraphQL.Validation;
using GraphQL.Server.Authorization.AspNetCore;
using GraphQL.Types;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

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
            services.AddScoped<IDocumentExecuter, DocumentExecuter>();
            services.AddTransient<RootQuery>();
            services.AddTransient<RootMutation>();
            services.AddTransient<ProductType>();
            services.AddTransient<RequestType>();
            services.AddTransient<CategoryType>();
            services.AddTransient<ProductInputType>();
            services.AddTransient<CategoryInputType>();
            services.AddTransient<RegisterInputType>();
            services.AddTransient<LoginInputType>();
            services.AddTransient<RequestInputType>();
            services.AddTransient<RequestApprovalInputType>();
            services.AddTransient<IRedisPubSub, RedisPubSubHandler>();

            services.AddScoped<IDependencyResolver>(type => new FuncDependencyResolver(type.GetService));
            services.AddScoped<ISchema, AppSchema>();

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                            Configuration.GetSection("JwtSetting:SecretKey").Value))
                };
            });

            services.AddHttpContextAccessor();
            services.AddTransient<IValidationRule, AuthorizationValidationRule>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.InventoryKeeper, _ => _.RequireClaim(ClaimTypes.Role, Policies.InventoryKeeper));
                options.AddPolicy(Policies.Approver, _ => _.RequireClaim(ClaimTypes.Role, Policies.Approver));
                options.AddPolicy(Policies.Requester, _ => _.RequireClaim(ClaimTypes.Role, Policies.Requester));
            });

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
            app.UseGraphiQl();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
