using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherChecker.Models
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string QueryName { get; set; }

        public ICollection<Event> Events { get; set; }
    }
}
