using ColetorDadosSpaceX.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ColetorDadosSpaceX.Services
{
    public class EnviaDados
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://apispacex.runasp.net";

        public EnviaDados()
        {
            _httpClient = new HttpClient();
        }

        // Envia a lista de lançamentos para a API do Aluno 2
        public async Task<bool> LancamentosAsync(List<Launch> launches)
        {
            try
            {
                // Substitua "/api/Launches" pela rota certa do Swagger dele se for diferente
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/api/Launches", launches);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
                return false;
            }
        }

        // Envia a lista de foguetes para a API do Aluno 2
        public async Task<bool> FoguetesAsync(List<Rocket> rockets)
        {
            try
            {
                // Substitua "/api/Rockets" pela rota certa do Swagger dele se for diferente
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/api/Rockets", rockets);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
                return false;
            }
        }
    }
}