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

        // Envia os lançamentos um por um para o servidor do colega
        public async Task<bool> LancamentosAsync(List<Launch> launches)
        {
            try
            {
                foreach (var launch in launches)
                {
                    // ATENÇÃO: Se no Swagger dele o caminho estiver no singular, mude para: $"{BaseUrl}/api/Launch"
                    var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/api/launches", launch);

                    if (!response.IsSuccessStatusCode)
                    {
                        // Se der erro 404 (Não Encontrado), significa que o nome da rota acima está errado
                        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                            return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar lançamentos: {ex.Message}");
                return false;
            }
        }

        // Envia os foguetes um por um para o servidor do colega
        public async Task<bool> FoguetesAsync(List<Rocket> rockets)
        {
            try
            {
                foreach (var rocket in rockets)
                {
                    // ATENÇÃO: Se no Swagger dele o caminho estiver no singular, mude para: $"{BaseUrl}/api/Rocket"
                    var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/api/Rockets", rocket);

                    if (!response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                            return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar foguetes: {ex.Message}");
                return false;
            }
        }
    }
}