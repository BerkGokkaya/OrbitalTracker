using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using OrbitalTracker.Helpers;
using OrbitalTracker.Models;
using OrbitalTracker.Services;

namespace OrbitalTracker.ViewModels
{
    public class SatelliteListViewModel : BaseViewModel
    {
        private readonly MainViewModel _mainViewModel;
        private readonly FilterService _filterService = new FilterService();

        // --- Filtrelenmiş liste ---
        public ObservableCollection<OrbitalPosition> FilteredPositions { get; set; } = new();

        // --- Arama ---
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                ApplyFilter();
            }
        }

        // --- Yükseklik filtresi ---
        private double _minAltitude = 0;
        public double MinAltitude
        {
            get => _minAltitude;
            set
            {
                SetProperty(ref _minAltitude, value);
                ApplyFilter();
            }
        }

        private double _maxAltitude = 50000;
        public double MaxAltitude
        {
            get => _maxAltitude;
            set
            {
                SetProperty(ref _maxAltitude, value);
                ApplyFilter();
            }
        }

        // --- Kategori filtresi ---
        private string _selectedCategory = "Tümü";
        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                SetProperty(ref _selectedCategory, value);
                ApplyFilter();
            }
        }

        public ObservableCollection<string> Categories { get; set; } = new()
        {
            "Tümü", "Stations", "Starlink", "Debris"
        };

        // --- Komutlar ---
        public ICommand ClearFilterCommand { get; }

        public SatelliteListViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;

            ClearFilterCommand = new RelayCommand(_ => ClearFilter());

            // MainViewModel'deki Positions değişince filtreyi güncelle
            _mainViewModel.Positions.CollectionChanged += (s, e) => ApplyFilter();
        }


        private void ApplyFilter()
        {
            try
            {
                var filtered = _filterService.Filter(
                    _mainViewModel.Positions,
                    SearchText,
                    MinAltitude,
                    MaxAltitude,
                    SelectedCategory
                );

                FilteredPositions.Clear();
                foreach (var pos in filtered)
                    FilteredPositions.Add(pos);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Filtreleme hatası: {ex.Message}",
                    "Hata", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void ClearFilter()
        {
            SearchText = "";
            MinAltitude = 0;
            MaxAltitude = 50000;
            SelectedCategory = "Tümü";
        }
    }
}