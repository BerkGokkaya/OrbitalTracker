using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OrbitalTracker.Models;
using SGPdotNET.CoordinateSystem;
using SGPdotNET.TLE;
using SGPdotNET.Propagation;
using SGPdotNET.Observation;

namespace OrbitalTracker.Services
{
    public class OrbitService
    {
        private readonly TleApiService _apiService = new TleApiService();
        private readonly Sgp4Calculator _calculator = new Sgp4Calculator();

        private List<TleData> _tleData = new List<TleData>();
        private readonly List<(string Name, Satellite Propagator)> _satellites = new();
        private CancellationTokenSource? _cts;

        public IReadOnlyList<(string Name, Satellite Propagator)> Satellites => _satellites;

        public double SimulationSpeed { get; set; } = 1.0; // 1x, 10x, 100x
        private DateTime _simulationTime;

        // UI'ya konum güncellemesi bildirmek için event
        public event Action<List<OrbitalPosition>>? PositionsUpdated;

        public async Task LoadDataAsync(string category = "stations")
        {
            _tleData = await _apiService.GetTleDataAsync(category);
            _satellites.Clear();
            foreach (var tle in _tleData)
            {
                try
                {
                    var tleObject = new SGPdotNET.TLE.Tle(tle.Name, tle.Line1, tle.Line2);
                    var sat = new Satellite(tleObject);
                    _satellites.Add((tle.Name, sat));
                }
                catch
                {
                    // Invalid TLE lines are skipped silently
                }
            }
            _simulationTime = DateTime.UtcNow;
        }

        public void StartTracking()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
            }
            _cts = new CancellationTokenSource();
            _ = RunTrackingLoopAsync(_cts.Token);
        }

        public void StopTracking()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }

        private async Task RunTrackingLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    _simulationTime = _simulationTime.AddSeconds(SimulationSpeed);

                    var positions = new List<OrbitalPosition>();

                    foreach (var sat in _satellites)
                    {
                        var pos = _calculator.Calculate(sat.Propagator, sat.Name, _simulationTime);
                        if (pos != null)
                            positions.Add(pos);
                    }

                    PositionsUpdated?.Invoke(positions);

                    // 1 saniyede bir güncelle
                    await Task.Delay(1000, token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch
                {
                    break;
                }
            }
        }
    }
}