using System;
using System.Collections.Generic;
using System.Linq;
using OrbitalTracker.Models;

namespace OrbitalTracker.Services
{
    public class FilterService
    {
        public List<OrbitalPosition> Filter(
            IEnumerable<OrbitalPosition> positions,
            string? searchText = null,
            double minAltitude = 0,
            double maxAltitude = 50000,
            string category = "Tümü")
        {
            try
            {
                var filtered = positions.AsEnumerable();

                // İsim araması
                if (!string.IsNullOrWhiteSpace(searchText))
                    filtered = filtered.Where(p =>
                        p.SatelliteName.Contains(searchText, StringComparison.OrdinalIgnoreCase));

                // Yükseklik filtresi
                filtered = filtered.Where(p =>
                    p.AltitudeKm >= minAltitude && p.AltitudeKm <= maxAltitude);

                // Kategori filtresi
                if (!string.IsNullOrWhiteSpace(category) && category != "Tümü")
                    filtered = filtered.Where(p =>
                        p.SatelliteName.Contains(category, StringComparison.OrdinalIgnoreCase));

                return filtered.ToList();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Filtreleme hatası: {ex.Message}",
                    "Hata", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return new List<OrbitalPosition>();
            }
        }
    }
}