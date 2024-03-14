using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using MauiProjectBWeather.Models;
using MauiProjectBWeather.Services;

namespace MauiProjectBWeather.Views
{
    // En klass som representerar en grupperad väderprognos, innehållande stadens namn och en samling av prognoser grupperade efter datum.
    public class GroupedForecast
    {
        // Egenskap för att lagra stadsnamnet.
        public string City { get; set; }

        // Egenskap för att lagra en samling av grupperade prognoser (grupperade efter datum).
        public IEnumerable<IGrouping<DateTime, ForecastItem>> Items { get; set; }
    }

    // En delvis klass som representerar en sida för väderprognos.
    public partial class ForecastPage : ContentPage
    {
        // En instans av OpenWeatherService för att hämta väderprognoser.
        OpenWeatherService service;

        // En instans av GroupedForecast för att lagra och visa de grupperade väderprognoserna.
        GroupedForecast groupedforecast;

        // En instans av CityPicture för att lagra information om staden.
        CityPicture city;

        // Konstruktorn för ForecastPage som tar emot en instans av CityPicture.
        public ForecastPage(CityPicture city)
        {
            // Initialiserar komponenter för sidan.
            InitializeComponent();

            // Tilldelar city instansen som skickades till konstruktorn.
            this.city = city;

            // Skapar en instans av OpenWeatherService.
            service = new OpenWeatherService();

            // Skapar en instans av GroupedForecast.
            groupedforecast = new GroupedForecast();
        }

        // Metod som körs när sidan visas.
        protected override void OnAppearing()
        {
            // Kör basklassen OnAppearing-metod.
            base.OnAppearing();

            // Sätter titeln på sidan till "Forecast for {city.Name}".
            Title = $"Forecast for {city.Name}";

            // Kör LoadForecast-metoden på huvudtråden.
            MainThread.BeginInvokeOnMainThread(async () => { await LoadForecast(); });
        }

        // Händelsehanterare för knappklick.
        private async void Button_Clicked(object sender, EventArgs e)
        {
            // Laddar väderprognos.
            await LoadForecast();
        }

        // Metod för att ladda väderprognos.
        private async Task LoadForecast()
        {
            // Hämtar väderprognos för staden från OpenWeatherService.
            Forecast forecast = await service.GetForecastAsync(city.Name);

            // Skapar en instans av GroupedForecast med stadsnamnet och grupperade väderprognoser.
            groupedforecast = new GroupedForecast() { City = city.Name, Items = forecast.Items.GroupBy(x => x.DateTime.Date) };

            // Tilldelar ItemsSource för GroupedForecast-elementet på sidan till de grupperade väderprognoserna.
            GroupedForecast.ItemsSource = groupedforecast.Items;
        }
    }
}