using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WeatherChecker.Models;

namespace WeatherChecker.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        ApplicationDBContext db;
        public string[] Cities { get; }
        public string SelectedCity { get; set; }
        private string weather;
        public string Weather
        {
            get { return weather; }
            set
            {
                weather = value;
                RaisePropertyChanged(() => Weather);
            }
        }
        public DateTime HistoryFrom { get; set; } = DateTime.Now.Date.AddDays(-1);
        public DateTime HistoryTo { get; set; } = DateTime.Now.Date.AddDays(1);
        public string SelectedCityForHistory { get; set; }
        private EventHistoryModel[] history;
        public EventHistoryModel[] History
        {
            get { return history; }
            set
            {
                history = value;
                RaisePropertyChanged(() => History);
            }
        }



        public MainViewModel()
        {
            try
            {
                db = new ApplicationDBContext();
                Cities = db.Cities.Select(city => city.Name).ToArray();
            }
            catch
            {
                ThrowException("Ошибка подключения к базе данных");
            }
        }

        public ICommand GetWeather
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    Weather = GetWeatherForCity(SelectedCity);
                });
            }
        }

        public ICommand GetHistory
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    History = GetEventsHistoryForCity(SelectedCityForHistory, HistoryFrom, HistoryTo);
                });
            }
        }

        private string GetWeatherForCity(string cityName)
        {
            if (string.IsNullOrWhiteSpace(cityName))
            {
                ThrowException("Название города не может быть пустым");
                return "";
            }
                
            var city = db.Cities.FirstOrDefault(_city => _city.Name == cityName);

            if (city == null)
            {
                ThrowException("Заданный город не найден в базе");
                return "";
            }

            string weather;

            using (WebClient client = new WebClient())
            {
                string htmlCode = client.DownloadString("https://yandex.ru/pogoda/" + city.QueryName);

                var regex = new Regex("div class=\"temp fact__temp fact__temp_size_s\" role=\"text\">.*?</div>");
                var match = regex.Match(htmlCode);

                regex = new Regex("<span class=\"temp__value temp__value_with-unit\">.*?</span>");
                match = regex.Match(match.Value);

                weather = match.Value.Replace("<span class=\"temp__value temp__value_with-unit\">", string.Empty);
                weather = weather.Replace("</span>", string.Empty);
            }


            var NewEvent = new Event()
            {
                DateAndTime = DateTime.Now,
                CityId = city.Id,
                Weather = weather
            };

            db.History.Add(NewEvent);
            db.SaveChanges();

            return weather;
        }

        private EventHistoryModel[] GetEventsHistoryForCity(string cityName, DateTime dateFrom, DateTime dateTo)
        {
            if (string.IsNullOrWhiteSpace(cityName))
            {
                ThrowException("Название города не может быть пустым");
                return new EventHistoryModel[0];
            }
            
            if(dateFrom > dateTo)
            {
                ThrowException("Начальная дата не может быть больше даты окончания");
                return new EventHistoryModel[0];
            }

            EventHistoryModel[] history = db.History.Join(db.Cities,
                                              hist => hist.CityId,
                                              city => city.Id,
                                              (hist, city) => new EventHistoryModel()
                                              {
                                                  Id = hist.Id,
                                                  DateAndTime = hist.DateAndTime,
                                                  CityName = city.Name,
                                                  Weather = hist.Weather
                                              }).Where(_event => _event.CityName == cityName &&
                                              _event.DateAndTime >= dateFrom &&
                                              _event.DateAndTime <= dateTo).ToArray();
            return history;
        }

        private void ThrowException(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}