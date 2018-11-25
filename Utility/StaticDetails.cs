using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Utility
{
    public class StaticDetails
    {
        public static string DefaultFoodImage = "default_food.png";

        public const  string AdminEndUser = "Admin";

        public static string CustomerEndUser = "Customer";

        public const string StatusSubmitted = "Submitted";
        public const string StatusInProgress = "Being prepared";
        public const string StatusReady = "Ready for pick up";
        public const string StatusCompletted = "Completted";
        public const string StatusCancelled = "Cancelled";

    }
}
