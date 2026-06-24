using OrbitalTracker.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace OrbitalTracker.Services
{
    public class SatelliteService
    {
        private readonly HttpClient _httpClient;

        public SatelliteService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<SatelliteDto>> GetSatellitesAsync(string apiUrl)
        {

            /*
            var response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<SatelliteDto>>(json);
            }
            return new List<SatelliteDto>();
            */


            
            await Task.Delay(2000); // 2 saniyelik ağ indirme gecikmesi simülasyonu

            return new List<SatelliteDto>
            {
                new SatelliteDto { Id = 25544, Name = "ISS (ZARYA)", Latitude = 40.15, Longitude = 26.40, Altitude = 420.0, Velocity = 0.4, Type = "LEO" },
                new SatelliteDto { Id = 20580, Name = "HUBBLE", Latitude = 28.5, Longitude = -80.5, Altitude = 540.0, Velocity = 0.3, Type = "LEO" },
                new SatelliteDto { Id = 39534, Name = "TURKSAT 4A", Latitude = 0.0, Longitude = 42.0, Altitude = 35786.0, Velocity = 0.08, Type = "GEO" }
            };
        }
    }
}