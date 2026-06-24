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

namespace OrbitalTracker.Views
{
    public partial class MainWindow : Window
    {
        // Artık tek bir uydu değil, bir liste yönetiyoruz
        private List<SatelliteMarker> _satellites = new List<SatelliteMarker>();
        private DispatcherTimer _timer;
        private bool _isPanelOpen = false;
        private double _timeMultiplier = 1.0;

        // YENİ: Kameranın o an kilitlendiği uyduyu tutacak değişken
        private SatelliteMarker _trackedSatellite;
        public MainWindow()
        {
            InitializeComponent();
            SetupSatellites();

            // YENİ: Filtre paneli olayını dinle
            FilterPanel.OnFilterApplied += FilterPanel_OnFilterApplied;
        }
        private void FilterPanel_OnFilterApplied(object sender, FilterEventArgs e)
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
                    if (!MainViewport.Children.Contains(sat.Visual))
                    {
                        MainViewport.Children.Add(sat.Visual);
                        MainViewport.Children.Add(sat.Trail.Visual);
                    }
                }
                else
                {
                    if (MainViewport.Children.Contains(sat.Visual))
                    {
                        MainViewport.Children.Remove(sat.Visual);
                        MainViewport.Children.Remove(sat.Trail.Visual);
                    }
                }
            }
        }

        private void SetupSatellites()
        {
            // GERÇEKÇİ HIZ OPTİMİZASYONU: Taban hızları düşürdük ki 100x hızda 
            // uydular perane gibi dönüp gözü yormasın, yağ gibi aksın.
            var iss = new SatelliteMarker("ISS (ZARYA)", 40.15, 26.40, 420.0, Colors.Red, 0.4);
            var hubble = new SatelliteMarker("HUBBLE", 28.5, -80.5, 540.0, Colors.Blue, 0.3);
            var turksat = new SatelliteMarker("TURKSAT 4A", 0.0, 42.0, 35786.0, Colors.Gold, 0.08);

            _satellites.Add(iss);
            _satellites.Add(hubble);
            _satellites.Add(turksat);

            foreach (var sat in _satellites)
            {
                MainViewport.Children.Add(sat.Visual);
                MainViewport.Children.Add(sat.Trail.Visual);
            }

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(50); // Saniyede 20 kare güncelleme
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            foreach (var sat in _satellites)
            {
                sat.MoveForward(_timeMultiplier / 4.0);
            }

            // YENİ: KAMERA TAKİP (LOCK-ON) SİSTEMİ
            if (_trackedSatellite != null)
            {
                // 0 animasyon süresiyle anında uydunun merkezine bak
                MainViewport.LookAt(_trackedSatellite.Visual.Center, 0);
            }
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            if (_timer != null && _timer.IsEnabled)
            {
                _timer.Stop();
                BtnPause.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEF4444"));
                BtnPlay.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#22FFFFFF"));
            }
        }

        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (_timer != null && !_timer.IsEnabled)
            {
                _timer.Start();
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
                // YENİ: Kamerayı bu uyduya kilitle!
                _trackedSatellite = clickedSatellite;

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
                // YENİ: Uzay boşluğuna tıklandı, kamera kilidini serbest bırak!
                _trackedSatellite = null;

                if (_isPanelOpen)
                {
                    Storyboard slideOut = (Storyboard)FindResource("SlideOutAnimation");
                    slideOut.Begin(this);
                    _isPanelOpen = false;
                }
            }
        }
    }
}