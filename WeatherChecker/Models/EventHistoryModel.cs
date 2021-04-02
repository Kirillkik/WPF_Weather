using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherChecker.Models
{
    public class EventHistoryModel
    {
        public int Id { get; set; }
        public DateTime DateAndTime { get; set; }
        public string CityName { get; set; }
        public string Weather { get; set; }

        public override string ToString()
        {
            return $"Дата: {DateAndTime:d2}.{DateAndTime.Month:d2}.{DateAndTime.Year}, Время: {DateAndTime.Hour:d2}:{DateAndTime.Minute:d2}, Город: {CityName}, Погода: {Weather}";
        }
    }
}
