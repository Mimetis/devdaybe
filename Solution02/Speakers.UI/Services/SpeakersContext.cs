using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Speakers.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Speakers.UI.Services {
    public class SpeakersContext : DbContext {
        public DbSet<Speaker> Speakers { get; set; }

        
        public SpeakersContext(DbContextOptions<SpeakersContext> options) : base(options) {
            this.Database.EnsureCreated();
        }

    }
}
