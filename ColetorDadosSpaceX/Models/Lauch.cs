using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ColetorDadosSpaceX.Models
{
    public class Launch
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("date_utc")]
        public DateTime DateUtc { get; set; }

        // O sucesso pode ser nulo na API da SpaceX se o voo ainda não aconteceu ou foi cancelado antes
        [JsonPropertyName("success")]
        public bool? Success { get; set; }

        [JsonPropertyName("details")]
        public string Details { get; set; }
    }
}
