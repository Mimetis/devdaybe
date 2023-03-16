using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Speakers.API.Models;

namespace Speakers.API.Data
{
    public class SpeakersContext : DbContext
    {
        public SpeakersContext (DbContextOptions<SpeakersContext> options)
            : base(options)
        {
        }

        public SpeakersContext(string sqlConnectionString) : base(GetOptions(sqlConnectionString))
        {
            
        }
        private static DbContextOptions GetOptions(string connectionString)
        {
            var options = SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connectionString).Options;
            return options;
        }


        public DbSet<Speaker> Speakers { get; set; } = default!;
    }
}
