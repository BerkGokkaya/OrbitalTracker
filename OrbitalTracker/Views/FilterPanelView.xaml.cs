using System;
using System.Windows;
using System.Windows.Controls;

namespace OrbitalTracker.Views
{
    // Filtre kriterlerini taşıyacak küçük bir veri paketi
    public class FilterEventArgs : EventArgs
    {
        public string SearchText { get; set; }
        public bool ShowGEO { get; set; }
        public bool ShowMEO { get; set; }
        public bool ShowLEO { get; set; }
        public double MaxAltitude { get; set; }
    }

    public partial class FilterPanelView : UserControl
    {
        // MainWindow'un dinleyeceği olay
        public event EventHandler<FilterEventArgs> OnFilterApplied;

        public FilterPanelView()
        {
            InitializeComponent();
        }

        // Slider kaydırıldıkça üstündeki sayıyı günceller
        private void SldAltitude_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (TxtAltValue != null)
            {
                TxtAltValue.Text = Math.Round(e.NewValue).ToString();
            }
        }

        // Filtreleri Uygula butonuna basıldığında
        private void BtnApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            // Verileri paketle
            var filterArgs = new FilterEventArgs
            {
                SearchText = TxtSearch.Text.ToLower(),
                ShowGEO = ChkGeo.IsChecked == true,
                ShowMEO = ChkMeo.IsChecked == true,
                ShowLEO = ChkLeo.IsChecked == true,
                MaxAltitude = SldAltitude.Value
            };

            // Paketi MainWindow'a fırlat
            OnFilterApplied?.Invoke(this, filterArgs);
        }
    }
}