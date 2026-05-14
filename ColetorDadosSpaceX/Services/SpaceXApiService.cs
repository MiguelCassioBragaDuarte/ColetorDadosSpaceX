using ColetorDadosSpaceX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ColetorDadosSpaceX.Services
{
    public class SpaceXApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://api.spacexdata.com/v4";

        public SpaceXApiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<Launch>> GetLaunchesAsync()
        {
            var response = await _httpClient.GetStringAsync($"{BaseUrl}/launches");
            return JsonSerializer.Deserialize<List<Launch>>(response) ?? new List<Launch>();
        }

        public async Task<List<Rocket>> GetRocketsAsync()
        {
            var response = await _httpClient.GetStringAsync($"{BaseUrl}/rockets");
            return JsonSerializer.Deserialize<List<Rocket>>(response) ?? new List<Rocket>();
        }
    }
}
