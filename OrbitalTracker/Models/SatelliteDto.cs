using System.Text.Json.Serialization;

namespace OrbitalTracker.Models
{
    // Harun'un API'sinden gelecek JSON yapısını temsil eden sınıf
    public class SatelliteDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("altitude")]
        public double Altitude { get; set; }

        [JsonPropertyName("velocity")]
        public double Velocity { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; } // LEO, MEO, GEO
    }
}