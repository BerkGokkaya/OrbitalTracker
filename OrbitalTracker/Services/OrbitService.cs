using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OrbitalTracker.Models;

namespace OrbitalTracker.Services
{
    public class OrbitService
    {
        private readonly TleApiService _apiService = new TleApiService();
        private readonly Sgp4Calculator _calculator = new Sgp4Calculator();

        private List<TleData> _tleData = new List<TleData>();
        private CancellationTokenSource _cts;

        public double SimulationSpeed { get; set; } = 1.0; // 1x, 10x, 100x
        private DateTime _simulationTime;

        // UI'ya konum güncellemesi bildirmek için event
        public event Action<List<OrbitalPosition>> PositionsUpdated;

        public async Task LoadDataAsync(string category = "stations")
        {
            _tleData = await _apiService.GetTleDataAsync(category);
            _simulationTime = DateTime.UtcNow;
        }

        public void StartTracking()
        {
            _cts = new CancellationTokenSource();
            _ = RunTrackingLoopAsync(_cts.Token);
        }

        public void StopTracking()
        {
            _cts?.Cancel();
        }

        private async Task RunTrackingLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    _simulationTime = _simulationTime.AddSeconds(SimulationSpeed);

                    var positions = new List<OrbitalPosition>();

                    foreach (var tle in _tleData)
                    {
                        var pos = _calculator.Calculate(tle, _simulationTime);
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
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Takip hatası: {ex.Message}",
                        "Hata", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    break;
                }
            }
        }
    }
}