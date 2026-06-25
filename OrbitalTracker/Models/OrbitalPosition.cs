namespace OrbitalTracker.Models
{
    public class OrbitalPosition
    {
        public int SatelliteId { get; set; }
        public string SatelliteName { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double AltitudeKm { get; set; }
        public double SpeedKmS { get; set; }
        public string Type { get; set; } = string.Empty;        // LEO, MEO, GEO
        public DateTime Timestamp { get; set; }
    }
}