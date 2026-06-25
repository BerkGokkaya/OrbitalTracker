using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using OrbitalTracker.Helpers;
using OrbitalTracker.Models;
using OrbitalTracker.Services;

namespace OrbitalTracker.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly OrbitService _orbitService = new OrbitService();
        public OrbitService OrbitService => _orbitService;

        // --- Uydu listesi ---
        public ObservableCollection<OrbitalPosition> Positions { get; set; } = new();

        // --- Seçili uydu ---
        private OrbitalPosition? _selectedSatellite;
        public OrbitalPosition? SelectedSatellite
        {
            get => _selectedSatellite;
            set => SetProperty(ref _selectedSatellite, value);
        }

        // --- Simülasyon hızı ---
        private double _simulationSpeed = 1.0;
        public double SimulationSpeed
        {
            get => _simulationSpeed;
            set
            {
                SetProperty(ref _simulationSpeed, value);
                _orbitService.SimulationSpeed = value;
            }
        }

        // --- Durum mesajı ---
        private string _statusMessage = "Hazır";
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        // --- Loading ---
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // --- Uydu sayısı ---
        private int _satelliteCount;
        public int SatelliteCount
        {
            get => _satelliteCount;
            set => SetProperty(ref _satelliteCount, value);
        }

        // --- Komutlar ---
        public ICommand LoadDataCommand { get; }
        public ICommand StartTrackingCommand { get; }
        public ICommand StopTrackingCommand { get; }
        public ICommand SetSpeedCommand { get; }

        public MainViewModel()
        {
            LoadDataCommand = new RelayCommand(async _ => await LoadDataAsync());
            StartTrackingCommand = new RelayCommand(_ => StartTracking());
            StopTrackingCommand = new RelayCommand(_ => StopTracking());
            SetSpeedCommand = new RelayCommand(param =>
            {
                if (param is string speed && double.TryParse(speed, out var s))
                    SimulationSpeed = s;
            });

            // OrbitService'den konum güncellemelerini dinle
            _orbitService.PositionsUpdated += OnPositionsUpdated;
        }

        private async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Veri yükleniyor...";

                await _orbitService.LoadDataAsync("stations");

                SatelliteCount = Positions.Count;
                StatusMessage = "Veri yüklendi, takip başlatılabilir.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veri yükleme hatası: {ex.Message}",
                    "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Hata oluştu.";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void StartTracking()
        {
            _orbitService.StartTracking();
            StatusMessage = "Takip başladı...";
        }

        private void StopTracking()
        {
            _orbitService.StopTracking();
            StatusMessage = "Takip durduruldu.";
        }

        private void OnPositionsUpdated(List<OrbitalPosition> positions)
        {
            // UI thread'inde güncelle
            Application.Current.Dispatcher.Invoke(() =>
            {
                Positions.Clear();
                foreach (var pos in positions)
                    Positions.Add(pos);

                SatelliteCount = Positions.Count;
            });
        }
    }
}