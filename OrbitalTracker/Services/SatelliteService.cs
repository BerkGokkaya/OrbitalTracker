using OrbitalTracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrbitalTracker.Services
{
    public class SatelliteService
    {
        private readonly OrbitService _orbitService = new OrbitService();
        private List<OrbitalPosition> _lastPositions = new List<OrbitalPosition>();

        public event Action<List<OrbitalPosition>> PositionsUpdated;

        public async Task LoadAsync(string category = "stations")
        {
            await _orbitService.LoadDataAsync(category);

            _orbitService.PositionsUpdated += positions =>
            {
                _lastPositions = positions;
                PositionsUpdated?.Invoke(positions);
            };
        }

        public void StartTracking() => _orbitService.StartTracking();
        public void StopTracking() => _orbitService.StopTracking();

        public List<OrbitalPosition> GetLastPositions() => _lastPositions;
    }
}