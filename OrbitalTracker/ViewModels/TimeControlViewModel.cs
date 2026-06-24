using System.Windows.Input;
using OrbitalTracker.Helpers;
using OrbitalTracker.ViewModels;

namespace OrbitalTracker.ViewModels
{
    public class TimeControlViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;

        // --- Simülasyon hızı ---
        private double _simulationSpeed = 1.0;
        public double SimulationSpeed
        {
            get => _simulationSpeed;
            set
            {
                SetProperty(ref _simulationSpeed, value);
                _mainViewModel.SimulationSpeed = value;
                UpdateSpeedLabel();
            }
        }

        private string _speedLabel = "1x";
        public string SpeedLabel
        {
            get => _speedLabel;
            set => SetProperty(ref _speedLabel, value);
        }

        // --- Simülasyon durumu ---
        private bool _isRunning;
        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                SetProperty(ref _isRunning, value);
                OnPropertyChanged(nameof(PlayPauseLabel));
            }
        }

        public string PlayPauseLabel => IsRunning ? "⏸ Durdur" : "▶ Başlat";

        // --- Komutlar ---
        public ICommand PlayPauseCommand { get; }
        public ICommand Speed1xCommand { get; }
        public ICommand Speed10xCommand { get; }
        public ICommand Speed100xCommand { get; }

        public TimeControlViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;

            PlayPauseCommand = new RelayCommand(_ => TogglePlayPause());
            Speed1xCommand = new RelayCommand(_ => SimulationSpeed = 1.0);
            Speed10xCommand = new RelayCommand(_ => SimulationSpeed = 10.0);
            Speed100xCommand = new RelayCommand(_ => SimulationSpeed = 100.0);
        }

        private void TogglePlayPause()
        {
            if (IsRunning)
            {
                _mainViewModel.StopTrackingCommand.Execute(null);
                IsRunning = false;
            }
            else
            {
                _mainViewModel.StartTrackingCommand.Execute(null);
                IsRunning = true;
            }
        }

        private void UpdateSpeedLabel()
        {
            SpeedLabel = SimulationSpeed switch
            {
                1.0 => "1x",
                10.0 => "10x",
                100.0 => "100x",
                _ => $"{SimulationSpeed}x"
            };
        }
    }
}