using EmpDir.Desktop.Components;

namespace EmpDir.Desktop.Navigation
{
    public static class FooterNavigation
    {
        private static readonly FooterNav.FooterItem Home = new(CustomIcons.Home, "Home", "/");
        private static readonly FooterNav.FooterItem Corporate = new(CustomIcons.Corporate, "Corporate", "/corporate");
        private static readonly FooterNav.FooterItem MetalMart = new(CustomIcons.MetalMart, "Metal Mart", "/metalmart");
        private static readonly FooterNav.FooterItem ServiceCenter = new(CustomIcons.ServiceCenter, "Service Center", "/servicecenter");
        private static readonly FooterNav.FooterItem Plant = new(CustomIcons.Plant, "Plant", "/plant");

        // Each section excludes itself from navigation
        public static List<FooterNav.FooterItem> HomeSection => new() { Corporate, MetalMart, ServiceCenter, Plant };
        public static List<FooterNav.FooterItem> CorporateSection => new() { Home, MetalMart, ServiceCenter, Plant };
        public static List<FooterNav.FooterItem> MetalMartSection => new() { Home, Corporate, ServiceCenter, Plant };
        public static List<FooterNav.FooterItem> ServiceCenterSection => new() { Home, Corporate, MetalMart, Plant };
        public static List<FooterNav.FooterItem> PlantSection => new() { Home, Corporate, MetalMart, ServiceCenter };
    }
}
