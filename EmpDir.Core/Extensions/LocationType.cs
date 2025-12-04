namespace EmpDir.Core
{
    namespace EmpDir.Core.Extensions
    {

        /// Location types in the McElroy directory system
        /// Maps to the Loctype field in the database

        public enum LocationType
        {
            Corporate = 1,
            MetalMart = 2,
            ServiceCenter = 3,
            Plant = 4
        }


        /// Extension methods for LocationType enum

        public static class LocationTypeExtensions
        {

            /// Get display name for location type

            public static string GetDisplayName(this LocationType locationType)
            {
                return locationType switch
                {
                    LocationType.Corporate => "Corporate",
                    LocationType.MetalMart => "Metal Mart",
                    LocationType.ServiceCenter => "Service Center",
                    LocationType.Plant => "Plant",
                    _ => "Unknown"
                };
            }


            /// Get JSON key name (matches your existing ExportData format)

            public static string GetJsonKey(this LocationType locationType)
            {
                return locationType switch
                {
                    LocationType.Corporate => "corporate",
                    LocationType.MetalMart => "metal mart",
                    LocationType.ServiceCenter => "service center",
                    LocationType.Plant => "plant",
                    _ => "unknown"
                };
            }


            /// Parse location type from integer

            public static LocationType FromInt(int value)
            {
                return value switch
                {
                    1 => LocationType.Corporate,
                    2 => LocationType.MetalMart,
                    3 => LocationType.ServiceCenter,
                    4 => LocationType.Plant,
                    _ => throw new ArgumentException($"Invalid location type: {value}")
                };
            }
        }
    }
}
