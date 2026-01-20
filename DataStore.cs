using System.Collections.Generic;

namespace Rental_System
{
    public static class DataStore
    {
     

     
        public static Dictionary<string, string> ownerUsers =
            new Dictionary<string, string>();

        public static Dictionary<string, string> renterUsers =
            new Dictionary<string, string>();


       

        public static List<string> carVehicles = new List<string>();
        public static List<string> bikeVehicles = new List<string>();
        public static List<string> pickupVehicles = new List<string>();
        public static List<string> cycleVehicles = new List<string>();



        public static int TotalOwners = 0;
        public static int TotalRenters = 0;
        public static int TotalVehicles = 0;
        public static int TotalBookings = 0;
    }
}


