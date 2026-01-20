namespace Rental_System
{
    public class Vehicle
    {
        public string Model { get; set; }
        public string RegNumber { get; set; }
        public string Color { get; set; }
        public string Fuel { get; set; }
        public string Transmission { get; set; }

        public Vehicle(string model, string reg, string color, string fuel, string transmission)
        {
            Model = model;
            RegNumber = reg;
            Color = color;
            Fuel = fuel;
            Transmission = transmission;
        }
    }
}
