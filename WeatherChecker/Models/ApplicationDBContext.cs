using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherChecker.Models
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext() : base("DefaultConnection")
        {

        }

        public DbSet<City> Cities { get; set; }
        public DbSet<Event> History { get; set; }
    }
}
