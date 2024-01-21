
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

            // ND filters
            List<FilterItem> itemsFilter =
                [
                    new() { Name = "ND1",    Stops = 0,               Description = "no filter" },
                    new() { Name = "ND2",    Stops = 1,               Description = "1 stop variable" },
                    new() { Name = "ND4",    Stops = 2,               Description = "2 stops" },
                    new() { Name = "ND8",    Stops = 3,               Description = "3 stops" },
                    new() { Name = "ND16",   Stops = 4,               Description = "4 stops variable" },
                    new() { Name = "ND32",   Stops = 5,               Description = "5 stops variable" },
                    new() { Name = "ND64",   Stops = 6,               Description = "6 stops" },
                    new() { Name = "ND1000", Stops = Math.Log2(1000), Description = "~10 stops" },
                ];

            CurrentNd.ItemsSource = itemsFilter;
            NewNd1.ItemsSource = itemsFilter;
            NewNd2.ItemsSource = itemsFilter;
            // The last filter option is the default position for NewNd1
            NewNd1.Position = itemsFilter.Count-1; 

            // Shutter speed
            List<ShutterSpeedItem> itemsShutterSpeed =
                [
                    new ShutterSpeedItem("16000"),
                    new ShutterSpeedItem("12800"),
                    new ShutterSpeedItem("10000"),
                    new ShutterSpeedItem("8000"),
                    new ShutterSpeedItem("6400"),
                    new ShutterSpeedItem("5000"),
                    new ShutterSpeedItem("4000"),
                    new ShutterSpeedItem("3200"),
                    new ShutterSpeedItem("2500"),
                    new ShutterSpeedItem("2000"),
                    new ShutterSpeedItem("1600"),
                    new ShutterSpeedItem("1250"),
                    new ShutterSpeedItem("1000"),
                    new ShutterSpeedItem("800"),
                    new ShutterSpeedItem("640"),
                    new ShutterSpeedItem("500"),
                    new ShutterSpeedItem("400"),
                    new ShutterSpeedItem("320"),
                    new ShutterSpeedItem("250"),
                    new ShutterSpeedItem("200"),
                    new ShutterSpeedItem("160"),
                    new ShutterSpeedItem("125"),
                    new ShutterSpeedItem("100"),
                    new ShutterSpeedItem("80"),
                    new ShutterSpeedItem("60"),
                    new ShutterSpeedItem("50"),
                    new ShutterSpeedItem("40"),
                    new ShutterSpeedItem("30"),
                    new ShutterSpeedItem("25"),
                    new ShutterSpeedItem("20"),
                    new ShutterSpeedItem("15"),
                    new ShutterSpeedItem("13"),
                    new ShutterSpeedItem("10"),
                    new ShutterSpeedItem("8"),
                    new ShutterSpeedItem("6"),
                    new ShutterSpeedItem("5"),
                    new ShutterSpeedItem("4"),
                    new ShutterSpeedItem("0\"3"),
                    new ShutterSpeedItem("0\"4"),
                    new ShutterSpeedItem("0\"5"),
                    new ShutterSpeedItem("0\"6"),
                    new ShutterSpeedItem("0\"8"),
                    new ShutterSpeedItem("1\""),
                    new ShutterSpeedItem("1\"3"),
                    new ShutterSpeedItem("1\"6"),
                    new ShutterSpeedItem("2\""),
                    new ShutterSpeedItem("2\"5"),
                    new ShutterSpeedItem("3\"2"),
                    new ShutterSpeedItem("4\""),
                    new ShutterSpeedItem("5\""),
                    new ShutterSpeedItem("6\""),
                    new ShutterSpeedItem("8\""),
                    new ShutterSpeedItem("10\""),
                    new ShutterSpeedItem("13\""),
                    new ShutterSpeedItem("15\""),
                    new ShutterSpeedItem("20\""),
                    new ShutterSpeedItem("25\""),
                    new ShutterSpeedItem("30\"")
                ];

            CurrentShutter.ItemsSource = itemsShutterSpeed;

            // Default possition is the 1/100 shutter speed
            CurrentShutter.Position = itemsShutterSpeed.FindIndex(i => i.Abbreviation == "100"); 
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


        private double GetEv()
        {
            // https://en.wikipedia.org/wiki/Exposure_value
            var aperture = GetAperture((string)Aperture1.SelectedItem);
            var iso = GetIso((string)Iso1.SelectedItem);
            double shutterTime = ((ShutterSpeedItem)CurrentShutter.CurrentItem)?.Seconds ?? 1;

            var ev = Math.Log2(aperture * aperture / shutterTime) - Math.Log2(iso / 100);
            return ev;
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
            return ((FilterItem)filter.CurrentItem)?.Stops ?? 0;
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
