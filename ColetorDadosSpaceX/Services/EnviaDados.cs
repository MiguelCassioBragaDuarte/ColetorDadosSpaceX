using ColetorDadosSpaceX.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace ColetorDadosSpaceX.Services
{
    public class EnviaDados
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://apispacex.runasp.net";
        private readonly JsonSerializerOptions _jsonOptions;

        public EnviaDados()
        {
            _httpClient = new HttpClient();
            _jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = null };
        }

        public async Task<bool> LancamentosAsync(List<Launch> launches)
        {
            try
            {
                var listaFormatada = new List<object>();
                foreach (var l in launches)
                {
                    listaFormatada.Add(new
                    {
                        Id = l.Id,
                        Name = l.Name ?? "",
                        Success = l.Success ?? false,
                        Details = l.Details ?? ""
                    });
                }

                // Faz a requisição e guarda a resposta completa do servidor
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/api/SpaceX/launches/batch", listaFormatada, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("[SUCESSO] Lote de Lançamentos enviado com sucesso para a API!");
                    return true;
                }
                else
                {
                    // Ponto importante da aula: Capturar o motivo exato do erro retornado pela API
                    string errorContent = await response.Content.ReadAsStringAsync();

                    Console.WriteLine("[ERRO DE API - LANÇAMENTOS]");
                    Console.WriteLine($"Status Code retornado: {(int)response.StatusCode} ({response.StatusCode})");
                    Console.WriteLine($"Detalhes do erro do servidor: {errorContent}");
                    Console.WriteLine("--------------------------------------------------");

                    return false;
                }
            }
            catch (Exception ex)
            {
                // Captura erros graves como: internet caída, servidor fora do ar ou URL errada
                Console.WriteLine("[EXCEÇÃO CRÍTICA - LANÇAMENTOS]");
                Console.WriteLine($"Mensagem do Erro: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> FoguetesAsync(List<Rocket> rockets)
        {
            try
            {
                var listaFormatada = new List<object>();
                foreach (var r in rockets)
                {
                    listaFormatada.Add(new
                    {
                        Id = r.Id,
                        Name = r.Name ?? "",
                        Description = r.Description ?? "",
                        Active = r.Active,
                        SuccessRatePct = r.SuccessRatePct
                    });
                }

                HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/api/SpaceX/rockets/batch", listaFormatada, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("[SUCESSO] Lote de Foguetes enviado com sucesso para a API!");
                    return true;
                }
                else
                {
                    // Ponto importante da aula: Capturar o motivo exato do erro retornado pela API
                    string errorContent = await response.Content.ReadAsStringAsync();

                    Console.WriteLine("[ERRO DE API - FOGUETES]");
                    Console.WriteLine($"Status Code retornado: {(int)response.StatusCode} ({response.StatusCode})");
                    Console.WriteLine($"Detalhes do erro do servidor: {errorContent}");
                    Console.WriteLine("--------------------------------------------------");

                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[EXCEÇÃO CRÍTICA - FOGUETES]");
                Console.WriteLine($"Mensagem do Erro: {ex.Message}");
                return false;
            }
        }
    }
}