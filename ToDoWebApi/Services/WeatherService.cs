using RestSharp;
using Newtonsoft.Json.Linq;
namespace ToDoWebApi.Services
{
    public class WeatherService
    {
        private readonly string _apiKey;
        public WeatherService(IConfiguration configuration)
        {
            _apiKey = configuration["WeatherApiKey"];
        }

        public async Task<(double temperature, string condition)> GetCurrentWeather(string location)
        {
            var client = new RestClient($"http://api.weatherapi.com/v1/current.json?key={_apiKey}&q={location}");
            var request = new RestRequest();
            var response = await client.GetAsync(request);

            var json = JObject.Parse(response.Content);
            var temperature = (double)json["current"]["temp_c"];
            var condition = (string)json["current"]["condition"]["text"];

            return (temperature, condition);
        }
    }
}
