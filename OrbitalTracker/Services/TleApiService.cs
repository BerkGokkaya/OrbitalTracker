using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using OrbitalTracker.Models;

namespace OrbitalTracker.Services
{
    public class TleApiService
    {
        private readonly HttpClient _httpClient = new HttpClient();

        // Celestrak'tan TLE verisi çek
        private const string BaseUrl = "https://celestrak.org/SATCAT/";

        public async Task<List<TleData>> GetTleDataAsync(string category = "stations")
        {
            try
            {
                var url = $"https://celestrak.org/NORAD/elements/gp.php?GROUP={category}&FORMAT=tle";

                var response = await _httpClient.GetStringAsync(url);
                return ParseTle(response);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"API hatası: {ex.Message}",
                    "Hata", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return new List<TleData>();
            }
        }

        private List<TleData> ParseTle(string rawData)
        {
            var result = new List<TleData>();

            try
            {
                // TLE formatı: her 3 satır bir uyduyu temsil eder
                // Satır 0: İsim
                // Satır 1: TLE Line 1
                // Satır 2: TLE Line 2
                var lines = rawData.Split('\n');

                for (int i = 0; i + 2 < lines.Length; i += 3)
                {
                    var name = lines[i].Trim();
                    var line1 = lines[i + 1].Trim();
                    var line2 = lines[i + 2].Trim();

                    if (string.IsNullOrEmpty(name) ||
                        !line1.StartsWith("1") ||
                        !line2.StartsWith("2"))
                        continue;

                    result.Add(new TleData
                    {
                        Name = name,
                        Line1 = line1,
                        Line2 = line2
                    });
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"TLE ayrıştırma hatası: {ex.Message}",
                    "Hata", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }

            return result;
        }
    }
}