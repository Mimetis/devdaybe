using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Speakers.API.Data;
using Dotmim.Sync;
using Dotmim.Sync.SqlServer;

namespace Speakers.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddJsonFile("appsettings.local.json", true);

            var sql = builder.Configuration.GetConnectionString("SpeakersContext");
            
            builder.Services.AddDbContext<SpeakersContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("SpeakersContext") 
                    ?? throw new InvalidOperationException("Connection string 'SpeakersContext' not found.")));

            // Add services to the container.
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

            builder.Services.AddControllers();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options => options.IdleTimeout = TimeSpan.FromMinutes(30));

            // [Required]: Get a connection string to your server data source
            var connectionString = builder.Configuration.GetConnectionString("SpeakersContext");

            var tables = new string[] { "Speakers" };

            builder.Services.AddSyncServer<SqlSyncChangeTrackingProvider>(connectionString, tables);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
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