using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Speakers.API.Data;
using Dotmim.Sync;
using Dotmim.Sync.SqlServer;
using Microsoft.OpenApi.Models;

namespace Speakers.API {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddJsonFile("appsettings.local.json", true);

            var sql = builder.Configuration.GetConnectionString("SpeakersContext");

            builder.Services.AddDbContext<SpeakersContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("SpeakersContext")
                    ?? throw new InvalidOperationException("Connection string 'SpeakersContext' not found.")));


            // Add services to the container.
            builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration, "AzureAd")
                        .EnableTokenAcquisitionToCallDownstreamApi()
                            .AddInMemoryTokenCaches();

            builder.Services.AddControllers();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options => options.IdleTimeout = TimeSpan.FromMinutes(30));

            // [Required]: Get a connection string to your server data source
            var connectionString = builder.Configuration.GetConnectionString("SpeakersContext");

            var tables = new string[] { "Speakers" };

            builder.Services.AddSyncServer<SqlSyncChangeTrackingProvider>(connectionString, tables);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            // For Swagger Access
            var instance = builder.Configuration["SwaggerAD:Instance"];
            instance = instance.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase) ? instance : $"https://{instance}";
            instance = instance.EndsWith("/", StringComparison.InvariantCultureIgnoreCase) ? instance.TrimEnd('/') : instance;

            var authorizationUrl = new Uri($"{instance}/{builder.Configuration["AzureAd:TenantId"]}/oauth2/v2.0/authorize");
            var tokenUrl = new Uri($"{instance}/{builder.Configuration["AzureAd:TenantId"]}/oauth2/v2.0/token");
            var clientId = builder.Configuration["SwaggerAD:ClientId"];
            var scopes = builder.Configuration["SwaggerAD:Scopes"];

            builder.Services.AddSwaggerGen(c => {

                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Speakers Web API", Version = "v1" });

                var oauth2Scheme = new OpenApiSecurityScheme {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows {
                        AuthorizationCode = new OpenApiOAuthFlow {
                            AuthorizationUrl = authorizationUrl,
                            TokenUrl = tokenUrl,
                            Scopes = new Dictionary<string, string>
                              {
                               { scopes , scopes }
                          },
                        }
                    },
                    Description = "OpenId Security Scheme"
                };

                // This filter add a new line in swagger to indicates the type of response we get if we are not authorized
                c.AddSecurityDefinition("oauth2", oauth2Scheme);

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                  {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                        },
                        Array.Empty<string>()
                    }
              });
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI(c => {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "webapi v1");
                    c.OAuthClientId(clientId);
                    c.OAuthAppName("API");
                    c.OAuthScopeSeparator(" ");
                    c.OAuthUsePkce();
                    c.OAuthScopes(scopes);
                });
            }

            app.UseHttpsRedirection();
            app.UseSession();
            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}