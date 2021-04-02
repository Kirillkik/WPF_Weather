using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherChecker.Models
{
    public class Event
    {
        public int Id { get; set; }
        public DateTime DateAndTime { get; set; }
        public int CityId { get; set; }
        public string Weather { get; set; }

        public City City { get; set; }
    }
}
