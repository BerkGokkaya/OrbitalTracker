namespace OrbitalTracker.Models
{
    public class SatelliteObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TleLine1 { get; set; }
        public string TleLine2 { get; set; }
        public string Category { get; set; }   // "Starlink", "ISS", "Debris" vb.
        public bool IsActive { get; set; }
    }
}