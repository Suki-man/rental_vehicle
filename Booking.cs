namespace Rental_System
{
    public class Booking
    {
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string VehicleType { get; set; }
        public string VehicleModel { get; set; }

        public Booking(string customerName, string phone, string vehicleType, string vehicleModel)
        {
            CustomerName = customerName;
            PhoneNumber = phone;
            VehicleType = vehicleType;
            VehicleModel = vehicleModel;
        }
    }
}
