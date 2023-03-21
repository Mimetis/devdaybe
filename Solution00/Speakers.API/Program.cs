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

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.MapControllers();

            app.Run();
        }
    }
}