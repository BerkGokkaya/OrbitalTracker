using HelixToolkit.Wpf;
using OrbitalTracker.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using OrbitalTracker.Services;
using OrbitalTracker.Models;
using System.Threading.Tasks;

namespace OrbitalTracker.Views
{
    public partial class MainWindow : Window
    {
        public OrbitalTracker.ViewModels.MainViewModel ViewModel { get; }
        private readonly Sgp4Calculator _calculator = new Sgp4Calculator();
        private DateTime _simulationTime = DateTime.UtcNow;

        private readonly List<SatelliteMarker> _satellites = new List<SatelliteMarker>();
        private DispatcherTimer? _timer;
        private bool _isPanelOpen = false;
        private double _timeMultiplier = 1.0;

        // YENİ: Kameranın o an kilitlendiği uyduyu tutacak değişken
        private SatelliteMarker? _trackedSatellite;

        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new OrbitalTracker.ViewModels.MainViewModel();
            DataContext = ViewModel;

            SetupSatellitesAsync();

            FilterPanel.OnFilterApplied += FilterPanel_OnFilterApplied;
        }

        private void FilterPanel_OnFilterApplied(object? sender, FilterEventArgs e)
        {
            foreach (var sat in _satellites)
            {
                bool isVisible = true;

                // 1. İsim Taraması
                if (!string.IsNullOrWhiteSpace(e.SearchText))
                {
                    if (!sat.Name.ToLower().Contains(e.SearchText))
                        isVisible = false;
                }

                // 2. Yükseklik Sınırı Taraması
                if (sat.CurrentAlt > e.MaxAltitude)
                {
                    isVisible = false;
                }

                // 3. Yörünge Tipi Taraması (LEO < 2000km, MEO < 35000km, GEO > 35000km kabaca)
                if (sat.CurrentAlt <= 2000 && !e.ShowLEO) isVisible = false;
                else if (sat.CurrentAlt > 2000 && sat.CurrentAlt <= 35000 && !e.ShowMEO) isVisible = false;
                else if (sat.CurrentAlt > 35000 && !e.ShowGEO) isVisible = false;

                // Kararı Uygula: Uyduyu ve Kuyruğunu 3D dünyada göster veya gizle
                if (isVisible)
                {
                    if (!sat.IsVisible)
                    {
                        MainViewport.Children.Add(sat.Visual);
                        MainViewport.Children.Add(sat.Trail.Visual);
                        sat.IsVisible = true;
                    }
                }
                else
                {
                    if (sat.IsVisible)
                    {
                        MainViewport.Children.Remove(sat.Visual);
                        MainViewport.Children.Remove(sat.Trail.Visual);
                        if (MainViewport.Children.Contains(sat.CoverageCone))
                        {
                            MainViewport.Children.Remove(sat.CoverageCone);
                        }
                        sat.IsVisible = false;
                    }
                }
            }
        }

        private void SetupSatellitesAsync()
        {
            LoadingOverlay.Visibility = Visibility.Visible;

            var orbitService = ViewModel.OrbitService;

            // Start loading via ViewModel command
            ViewModel.LoadDataCommand.Execute(null);

            orbitService.PositionsUpdated += async updatedPositions =>
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    foreach (var pos in updatedPositions)
                    {
                        // Daha önce eklenmişse atlıyoruz
                        if (_satellites.Any(s => s.Name == pos.SatelliteName)) continue;

                        var satInfo = orbitService.Satellites.FirstOrDefault(s => s.Name == pos.SatelliteName);
                        if (satInfo.Propagator == null) continue;

                        var color = pos.AltitudeKm > 35000 ? Colors.Gold :
                                    pos.SatelliteName.Contains("ISS") ? Colors.Red : Colors.Blue;

                        var satMarker = new SatelliteMarker(
                            pos.SatelliteName,
                            satInfo.Propagator,
                            pos.Latitude,
                            pos.Longitude,
                            pos.AltitudeKm,
                            color,
                            pos.SpeedKmS
                        );

                        _satellites.Add(satMarker);
                        MainViewport.Children.Add(satMarker.Visual);
                        MainViewport.Children.Add(satMarker.Trail.Visual);
                    }

                    if (_timer == null)
                    {
                        _timer = new DispatcherTimer();
                        _timer.Interval = TimeSpan.FromMilliseconds(50);
                        _timer.Tick += Timer_Tick;
                        _timer.Start();
                        _simulationTime = DateTime.UtcNow;
                    }

                    LoadingOverlay.Visibility = Visibility.Collapsed;
                });
            };

            ViewModel.StartTrackingCommand.Execute(null);
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            // 1. Tüm uyduları kendi hızlarında hareket ettir (Physics-based SGP4)
            double deltaTime = 0.05; // 50ms interval
            _simulationTime = _simulationTime.AddSeconds(deltaTime * _timeMultiplier);

            foreach (var sat in _satellites)
            {
                sat.UpdatePositionByTime(_simulationTime, _calculator);
            }

            // KAMERA TAKİP (LOCK-ON) VE UI GÜNCELLEME SİSTEMİ
            if (_trackedSatellite != null)
            {
                // 2. Kamerayı zorla uydunun merkezine bakmaya odakla (Drone takibi)
                MainViewport.LookAt(_trackedSatellite.Visual.Center, 0);

                // --- YENİ: ANLIK UI TELEMETRİ GÜNCELLEMESİ ---
                // Eğer bir uydu takip ediliyorsa, sol paneldeki verileri (Enlem, Boylam vb.) 
                // saniyede 20 kez, tıkır tıkır güncelle!
                SatelliteDetailPanel.UpdateDetails(
                    _trackedSatellite.Name,
                    _trackedSatellite.CurrentAlt,
                    _trackedSatellite.Speed,
                    _trackedSatellite.CurrentLat,
                    _trackedSatellite.CurrentLon
                );
            }
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            if (_timer != null && _timer.IsEnabled)
            {
                _timer.Stop();
                ViewModel.StopTrackingCommand.Execute(null);
                BtnPause.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEF4444"));
                BtnPlay.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#22FFFFFF"));
            }
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (_timer != null && !_timer.IsEnabled)
            {
                _timer.Start();
                ViewModel.StartTrackingCommand.Execute(null);
                BtnPlay.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF10B981"));
                BtnPause.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#22FFFFFF"));
            }
        }

        private void SpeedRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.Tag != null)
            {
                if (double.TryParse(rb.Tag.ToString(), out double newSpeed))
                {
                    _timeMultiplier = newSpeed;

                    if (ViewModel != null)
                    {
                        ViewModel.SimulationSpeed = newSpeed;
                    }

                    if (_satellites != null)
                    {
                        foreach (var sat in _satellites)
                        {
                            // Kuyruğu tamamen silmek yerine hıza göre akıllıca oranlıyoruz
                            int baseLimit = sat.CurrentAlt > 10000 ? 500 : 120;

                            if (newSpeed == 1) sat.Trail.MaxTrailLength = baseLimit;
                            else if (newSpeed == 10) sat.Trail.MaxTrailLength = baseLimit / 2;
                            // 100x hızda Türksat hala 100 noktalık devasa ve pürüzsüz bir kuyruğa sahip olacak!
                            else if (newSpeed == 100) sat.Trail.MaxTrailLength = (int)Math.Max(20, baseLimit / 5);
                        }
                    }
                }
            }
        }

        private void MainViewport_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePos = e.GetPosition(MainViewport);
            var hits = MainViewport.Viewport.FindHits(mousePos);
            var clickedVisual = hits?.FirstOrDefault()?.Visual;
            var clickedSatellite = _satellites.FirstOrDefault(s => s.Visual == clickedVisual);

            if (clickedSatellite != null)
            {
                // YENİ: Başka bir uydu seçiliyse önce onun konisini sahneden temizle
                if (_trackedSatellite != null)
                {
                    _trackedSatellite.RemoveHighlight();
                    if (MainViewport.Children.Contains(_trackedSatellite.CoverageCone))
                    {
                        MainViewport.Children.Remove(_trackedSatellite.CoverageCone);
                    }
                }

                _trackedSatellite = clickedSatellite;
                _trackedSatellite.Highlight();

                // YENİ: Tıklanan uydunun kapsama konisini 3D sahneye ekle!
                if (!MainViewport.Children.Contains(_trackedSatellite.CoverageCone))
                {
                    MainViewport.Children.Add(_trackedSatellite.CoverageCone);
                }

                SatelliteDetailPanel.UpdateDetails(
                    clickedSatellite.Name,
                    clickedSatellite.CurrentAlt,
                    clickedSatellite.Speed,
                    clickedSatellite.CurrentLat,
                    clickedSatellite.CurrentLon
                );

                if (!_isPanelOpen)
                {
                    Storyboard slideIn = (Storyboard)FindResource("SlideInAnimation");
                    slideIn.Begin(this);
                    _isPanelOpen = true;
                }
            }
            else
            {
                // Uzay boşluğuna tıklandığında temizlik
                if (_isPanelOpen)
                {
                    if (_trackedSatellite != null)
                    {
                        _trackedSatellite.RemoveHighlight();

                        // YENİ: Kamera kilidi kırılınca koniyi de sahneden kaldır
                        if (MainViewport.Children.Contains(_trackedSatellite.CoverageCone))
                        {
                            MainViewport.Children.Remove(_trackedSatellite.CoverageCone);
                        }

                        _trackedSatellite = null;
                    }

                    Storyboard slideOut = (Storyboard)FindResource("SlideOutAnimation");
                    slideOut.Begin(this);
                    _isPanelOpen = false;
                }
            }
        }
    }
}