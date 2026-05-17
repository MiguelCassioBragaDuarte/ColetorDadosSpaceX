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
            // Garante que o C# respeite as letras maiúsculas das propriedades anônimas que criamos abaixo
            _jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = null };
        }

        public async Task<bool> LancamentosAsync(List<Launch> launches)
        {
            try
            {
                var listaFormatada = new List<object>();

                foreach (var l in launches)
                {
                    // Monta o JSON exatamente como o Swagger dele pediu: Id, Name, Success, Details
                    listaFormatada.Add(new
                    {
                        Id = l.Id,
                        Name = l.Name ?? "",
                        Success = l.Success ?? false,
                        Details = l.Details ?? ""
                    });
                }

                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/api/SpaceX/launches/batch", listaFormatada, _jsonOptions);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar lançamentos: {ex.Message}");
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
                    // Monta o JSON mapeando para "SuccessRatePct" exigido pelo Swagger do Aluno 2
                    listaFormatada.Add(new
                    {
                        Id = r.Id,
                        Name = r.Name ?? "",
                        Description = r.Description ?? "",
                        Active = r.Active,
                        SuccessRatePct = r.SuccessRatePct // Aqui faz a correspondência perfeita!
                    });
                }

                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/api/SpaceX/rockets/batch", listaFormatada, _jsonOptions);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar foguetes: {ex.Message}");
                return false;
            }
        }
    }
}