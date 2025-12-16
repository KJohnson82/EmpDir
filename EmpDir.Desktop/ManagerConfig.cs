

namespace EmpDir.Desktop
{

    /// Static configuration for area/territory managers across different sections.
    /// Data is defined here for simplicity - update this file when managers change.

    public static class ManagerConfig
    {

        /// Metal Mart territory managers mapped by territory name.

        public static class MetalMart
        {
            public const string Northeast = "David Boyte";
            public const string Central = "John Baker";
            public const string SouthCentral = "Beau Brown";
            public const string NewMexico = "Cliff Loveless";
            public const string Southeast = "Michael Northen";
            public const string Southwest = "Shaun Steinemann";

  
            /// Get all territories as subtitle/manager pairs for HeaderCardModels.AreaManagers()
  
            public static (string, string, string, string, string, string, string, string, string, string, string, string) GetAreaManagerParams()
            {
                return (
                    "Northeast", Northeast,
                    "Central", Central,
                    "South Central", SouthCentral,
                    "New Mexico", NewMexico,
                    "Southeast", Southeast,
                    "Southwest", Southwest
                );
            }
        }

 
        /// Service Center area managers and operations management.

        public static class ServiceCenter
        {
            public const string AreaManager1 = "Rickie Furr";
            public const string AreaManager2 = "Mark Rollins";
            public const string AreaManager3 = "John Carter";
            public const string AreaManager4 = "Shaun Steinmen";
            public const string AreaManager5 = "Matthew Snudden";
            public const string SCOpsManager = "Jeff Harrington";

            public static (string, string, string, string, string, string, string, string, string, string, string, string) GetAreaManagerParams()
            {
                return (
                    "Area Manager", AreaManager1,
                    "Area Manager", AreaManager2,
                    "Area Manager", AreaManager3,
                    "Area Manager", AreaManager4,
                    "Area Manager", AreaManager5,
                    "SC Ops Manager", SCOpsManager
                );
            }
        }


        /// Plant manufacturing and operations managers.
 
        public static class Plant
        {
            public const string CorpManufacturingMgr = "Marc Scammerhorn";
            public const string CorpTrafficMgr = "Steve Hunter";
            public const string RegionalOpsMgr1 = "Bruce Mancuso";
            public const string RegionalOpsMgr2 = "Teresa Taylor";
            public const string RegionalOpsMgr3 = "Jeff Hoopes";
            public const string RegionalOpsMgr4 = "Don Jones";

            public static (string, string, string, string, string, string, string, string, string, string, string, string) GetPlantParams()
            {
                return (
                    "Corp Manufacturing Mgr", CorpManufacturingMgr,
                    "Corp Traffic Mgr", CorpTrafficMgr,
                    "Regional Ops Mgr", RegionalOpsMgr1,
                    "Regional Ops Mgr", RegionalOpsMgr2,
                    "Regional Ops Mgr", RegionalOpsMgr3,
                    "Regional Ops Mgr", RegionalOpsMgr4
                );
            }
        }
    }
}
