
using SnapCalc.Models;

namespace SnapCalc
{
    public partial class MainPage : ContentPage
    {


        public MainPage()
        {
            InitializeComponent();

            Aperture1.SelectedIndex = 2;  // f2.8
            Aperture2.SelectedIndex = 0;
            Shutter1.SelectedIndex  = 10; // 1/80
            Iso1.SelectedIndex      = 0;
            Iso2.SelectedIndex      = 0;

            List<FilterItem> itemsFilter =
            [
                new() { Name = "ND0",    Stops = 0,               Description="no filter"},
                new() { Name = "ND2",    Stops = 1,               Description="1 stop var. filter"},
                new() { Name = "ND4",    Stops = 2,               Description="2 stops"},
                new() { Name = "ND8",    Stops = 3,               Description="3 stops"},
                new() { Name = "ND16",   Stops = 4,               Description="4 stops var. filter"},
                new() { Name = "ND32",   Stops = 5,               Description="5 stops var. filter"},
                new() { Name = "ND64",   Stops = 6,               Description="6 stops"},
                new() { Name = "ND1000", Stops = Math.Log2(1000), Description="~10 stops"},
            ];
            CurrentNd.ItemsSource = itemsFilter;
            NewNd1.ItemsSource = itemsFilter;
            NewNd1.Position = 7; // the ND1000 is default position for this filter
            NewNd2.ItemsSource = itemsFilter;
        }


        private static double GetAperture(string current, string? original = null)
        {
            if (current is null)
            {
                return 1.8;
            }

            if (current is "same aperture")
            {
                if (string.IsNullOrEmpty(original))
                {
                    return 0;
                }

                return double.Parse(original[1..]);
            }

            return double.Parse(current[1..]);
        }


        private static int GetIso(string current, string? original = null)
        {
            if (current is null)
            {
                return 100;
            }

            if (current is "same ISO")
            {
                if (string.IsNullOrEmpty(original))
                {
                    return 0;
                }

                return int.Parse(original[..^4]);
            }

            return int.Parse(current[..^4]);
        }


        private static double ParseShutterSpeed(string shutterSpeed)
        {
            if (shutterSpeed.Contains('/'))
            {
                //  1/250s
                return 1.0 / double.Parse(shutterSpeed[2..^1]);
            }

            // 1.3s
            return double.Parse(shutterSpeed[..^1]);
        }


        private static double GetShutterSpeed(string current)
        {
            if (current is null) 
            { 
                return 1.0; 
            }

            return ParseShutterSpeed(current);
        }


        private double GetEv()
        {
            // https://en.wikipedia.org/wiki/Exposure_value
            var N = GetAperture((string)Aperture1.SelectedItem);
            var S = GetIso((string)Iso1.SelectedItem);
            var t = GetShutterSpeed((string)Shutter1.SelectedItem);

            var EV = Math.Log2((N * N) / t) - Math.Log2(S / 100);
            return EV;
        }

        private double CalculateShutterSpeed(double ev)
        {
            var N = GetAperture((string)Aperture2.SelectedItem, (string)Aperture1.SelectedItem);
            var S = GetIso((string)Iso2.SelectedItem, (string)Iso1.SelectedItem);

            ev += Math.Log2(S / 100); // Compensate for the new ISO
            var t = (N * N) / Math.Pow(2, ev);
            return t;
        }


        private static double CalculateLux(double ev)
        {
            var L = Math.Pow(2, ev - 2.84);
            return L;
        }


        private static double GetNdFilter(CarouselView filter)
        {
            return ((FilterItem)(filter.CurrentItem))?.Stops ?? 0;
        }


        private void OnAnythingChange(object sender, EventArgs e)
        {
            var currentNd = GetNdFilter(CurrentNd);
            var ev = GetEv() + currentNd;

            var newNd1 = GetNdFilter(NewNd1);
            var newNd2 = GetNdFilter(NewNd2);
            var newNdCombined = newNd1 + newNd2;

            var newShutterSpeed = CalculateShutterSpeed(ev - newNdCombined);

            NewShutterSpeed.Text = $"{newShutterSpeed / 60:00}min {newShutterSpeed % 60:00}sec ({newNdCombined:F2} ND stops)";

            Ev.Text = $"{ev:F2} EV ({CalculateLux(ev):F2} lux and {currentNd:F1} stops of ND in original exposure)";
        }


    }

}
