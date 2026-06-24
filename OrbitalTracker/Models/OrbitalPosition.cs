namespace OrbitalTracker.Models
{
    public class OrbitalPosition
    {
        public int SatelliteId { get; set; }
        public string SatelliteName { get; set; }
        public double Latitude { get; set; }    // Enlem (-90 ile 90 arası)
        public double Longitude { get; set; }   // Boylam (-180 ile 180 arası)
        public double AltitudeKm { get; set; }  // Yükseklik (km)
        public double SpeedKmS { get; set; }    // Hız (km/s)
        public DateTime Timestamp { get; set; } // Hesaplama zamanı
    }
}