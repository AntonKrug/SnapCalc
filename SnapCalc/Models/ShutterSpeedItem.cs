namespace SnapCalc.Models
{
    internal class ShutterSpeedItem
    {

        public string Abbreviation { get; }


        public double Seconds { get; }


        public string SecondsFormated => $"{Seconds:F6}s";


        public ShutterSpeedItem(string abbreviation) 
        { 
            Abbreviation = abbreviation;
            
            if (abbreviation.Contains('"'))
            {
                // The " symbol represnts fraction point .
                Seconds = double.Parse(abbreviation.Replace('"', '.'));
            } 
            else
            {
                // All other numbers x are actually 1 / x
                Seconds = 1 / double.Parse(abbreviation);
            }
        }


    }
}
