
using SnapCalc.Models;

namespace SnapCalc
{
    public partial class MainPage : ContentPage
    {
        private const double fallbackAperture = 1.8;

        public MainPage()
        {
            InitializeComponent();

            InitNdFilters();
            InitShutterSpeed();
            InitAperture();

            //Iso1.SelectedIndex      = 0;
            //Iso2.SelectedIndex      = 0;
        }


        private void InitNdFilters()
        {
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
            NewNd1.Position = itemsFilter.Count - 1;
        }


        private void InitShutterSpeed()
        {
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


        private void InitAperture()
        {
            List<string> itemsApertureCurrent =
                [
                    "f1.8",
                    "f2.0",
                    "f2.8",
                    "f3.2",
                    "f3.5",
                    "f4.0",
                    "f4.5",
                    "f5.0",
                    "f5.6",
                    "f6.3",
                    "f7.1",
                    "f8.0",
                    "f9.0",
                    "f10.0",
                    "f11.0",
                    "f13.0"
                ];
            CurrentAperture.ItemsSource = itemsApertureCurrent;

            // Default current aperture is not fully wide open
            CurrentAperture.Position = itemsApertureCurrent.IndexOf("f2.8");

            // The new aperture has extra option as default
            NewAperture.ItemsSource = new List<string> { "same aperture" }.Concat(itemsApertureCurrent);
        }


        private static double GetAperture(CarouselView requestedCarousel, CarouselView? backupCarousel = null)
        {
            var requestedValue = (string)requestedCarousel.CurrentItem;

            if (requestedValue is null)
            {
                return fallbackAperture;
            }

            if (requestedValue is "same aperture")
            {
                if (backupCarousel is null)
                {
                    return fallbackAperture;
                }

                return double.Parse(((string)backupCarousel.CurrentItem)[1..]);
            }

            return double.Parse(requestedValue[1..]);
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
            var aperture = GetAperture(CurrentAperture);
            var iso = GetIso((string)Iso1.SelectedItem);
            double shutterTime = ((ShutterSpeedItem)CurrentShutter.CurrentItem)?.Seconds ?? 1;

            var ev = Math.Log2(aperture * aperture / shutterTime) - Math.Log2(iso / 100);
            return ev;
        }


        private double CalculateShutterSpeed(double ev)
        {
            var aperture = GetAperture(NewAperture, CurrentAperture);
            var iso = GetIso((string)Iso2.SelectedItem, (string)Iso1.SelectedItem);

            ev += Math.Log2(iso / 100); // And compensate for the new ISO
            var shutterSpeed = aperture * aperture / Math.Pow(2, ev);
            return shutterSpeed;
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

            Ev.Text = $"{ev:F1}EV {CalculateLux(ev):F2}lux with ND {currentNd:F1}stops in current exposure";

            NewNdCombined.Text = $"Combined ND is {newNdCombined:F2} stops";

            int hours   = (int)Math.Round(newShutterSpeed / (60 * 60));
            int minutes = (int)Math.Round((newShutterSpeed / 60) % 60);
            int seconds = (int)Math.Round(newShutterSpeed % 60);

            if (hours > 0)
            {
                NewShutterSpeed.Text = $"Bulb {hours:00}:{minutes:00}:{seconds:00} (h:min:sec)";
            }
            else
            {
                NewShutterSpeed.Text = $"Bulb {minutes:00}:{seconds:00} (min:sec)";
            }

        }


    }

}
