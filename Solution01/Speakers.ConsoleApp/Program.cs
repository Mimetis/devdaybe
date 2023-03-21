using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Speakers.ConsoleApp {
    internal class Program {
        private static IConfiguration configuration;

        public static string GetDevdayBeConnectionString() => configuration.GetConnectionString("SpeakersContext");

        static async Task Main(string[] args) {
            configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", false, true)
              .AddJsonFile("appsettings.local.json", true, true)
              .Build();

            await SynchronizeDevdayBeAsync();

        }

        private static async Task SynchronizeDevdayBeAsync() {
        }
    }
}