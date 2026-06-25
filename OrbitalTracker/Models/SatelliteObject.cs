namespace OrbitalTracker.Models
{
    public class SatelliteObject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TleLine1 { get; set; } = string.Empty;
        public string TleLine2 { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;   // "Starlink", "ISS", "Debris" vb.
        public bool IsActive { get; set; }
    }
}