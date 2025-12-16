namespace EmpDir.Desktop.Layout
{
    // Model classes and factory methods for creating header card information
    public class HeaderCardModels
    {
        // Flexible data model for various page header types
        public class HeadInfoCardModel
        {
            // Core Information
            public string Title { get; set; } = "";
            public string Email { get; set; } = "";

            // Subtitle Fields
            public string Subtitle1 { get; set; } = "";
            public string Subtitle2 { get; set; } = "";
            public string Subtitle3 { get; set; } = "";
            public string Subtitle4 { get; set; } = "";
            public string Subtitle5 { get; set; } = "";
            public string Subtitle6 { get; set; } = "";
            public string Subtitle7 { get; set; } = "";
            public string Subtitle8 { get; set; } = "";

            // Contact Information
            public string Phone { get; set; } = "";
            public string Phone2 { get; set; } = "";
            public string Phone3 { get; set; } = "";

            // Address Information
            public string Address { get; set; } = "";
            public string City { get; set; } = "";
            public string State { get; set; } = "";
            public string Zip { get; set; } = "";

            // Management Information
            public string StoreManager { get; set; } = "";
            public string AreaManager { get; set; } = "";

            // Team/Title Manager Fields
            public string TManager1 { get; set; } = "";
            public string TManager2 { get; set; } = "";
            public string TManager3 { get; set; } = "";
            public string TManager4 { get; set; } = "";
            public string TManager5 { get; set; } = "";
            public string TManager6 { get; set; } = "";
            public string TManager7 { get; set; } = "";
            public string TManager8 { get; set; } = "";

            // Controls back button visibility
            public string ShowBackButton { get; set; } = "true";
        }

        // Creates corporate header with company information
        public static HeadInfoCardModel Corporate()
        {
            return new HeadInfoCardModel
            {
                Title = "Employee Directory",
                Address = "1500 Hamilton Rd",
                City = "Bossier City",
                State = "LA",
                Zip = "71111",
                Subtitle1 = "Corp. Phone",
                Phone = "(888) 245-3696",
                Subtitle2 = "Corp. Email",
                Email = "info@mcelroymetal.com",
                Subtitle3 = "IT Help Desk",
                Phone2 = "(866) 634-5111",
            };
        }

        // Creates department header with contact information
        public static HeadInfoCardModel ForDepartment(string title, string? phone, string? email, string? manager)
        {
            return new HeadInfoCardModel
            {
                Title = title,
                Subtitle1 = "Dept. Phone",
                Phone = phone ?? "",
                Subtitle2 = "Dept. Email",
                Email = email ?? "",
                Subtitle3 = "Dept. Manager",
                Phone2 = manager ?? ""
            };
        }

        // Creates plant header with multiple management roles (up to 6 role/person pairs)
        public static HeadInfoCardModel Plant(string title, string? subtitle1, string? tmanager1, string? subtitle2, string? tmanager2, string? subtitle3, string? tmanager3, string? subtitle4, string? tmanager4, string? subtitle5, string? tmanager5, string? subtitle6, string? tmanager6)
        {
            return new HeadInfoCardModel
            {
                Title = title,
                Subtitle1 = subtitle1 ?? "",
                TManager1 = tmanager1 ?? "",
                Subtitle2 = subtitle2 ?? "",
                TManager2 = tmanager2 ?? "",
                Subtitle3 = subtitle3 ?? "",
                TManager3 = tmanager3 ?? "",
                Subtitle4 = subtitle4 ?? "",
                TManager4 = tmanager4 ?? "",
                Subtitle5 = subtitle5 ?? "",
                TManager5 = tmanager5 ?? "",
                Subtitle6 = subtitle6 ?? "",
                TManager6 = tmanager6 ?? "",
            };
        }

        // Creates simple employee header with just a title
        public static HeadInfoCardModel Employee(string title)
        {
            return new HeadInfoCardModel
            {
                Title = title
            };
        }

        // Creates store header with store and area management information
        public static HeadInfoCardModel Store(string title, string subtitle1, string storemanager, string subtitle2, string areamanager)
        {
            return new HeadInfoCardModel
            {
                Title = title,
                Subtitle1 = subtitle1 ?? "",
                StoreManager = storemanager ?? "",
                Subtitle2 = subtitle2 ?? "",
                AreaManager = areamanager ?? ""
            };
        }

        // Creates area managers header with multiple management roles (up to 6 role/person pairs)
        public static HeadInfoCardModel AreaManagers(string title, string? subtitle1, string? tmanager1, string? subtitle2, string? tmanager2, string? subtitle3, string? tmanager3, string? subtitle4, string? tmanager4, string? subtitle5, string? tmanager5, string? subtitle6, string? tmanager6)
        {
            return new HeadInfoCardModel
            {
                Title = title,
                Subtitle1 = subtitle1 ?? "",
                TManager1 = tmanager1 ?? "",
                Subtitle2 = subtitle2 ?? "",
                TManager2 = tmanager2 ?? "",
                Subtitle3 = subtitle3 ?? "",
                TManager3 = tmanager3 ?? "",
                Subtitle4 = subtitle4 ?? "",
                TManager4 = tmanager4 ?? "",
                Subtitle5 = subtitle5 ?? "",
                TManager5 = tmanager5 ?? "",
                Subtitle6 = subtitle6 ?? "",
                TManager6 = tmanager6 ?? "",
            };
        }
    }
}