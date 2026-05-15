using SQLite;
using System.Text.Json.Serialization;


namespace ColetorDadosSpaceX.Models
{
    public class Rocket
    {
        [PrimaryKey]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("active")]
        public bool Active { get; set; }

        [JsonPropertyName("success_rate_pct")]
        public double SuccessRatePct { get; set; }
    }
}
