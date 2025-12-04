namespace EmpDir.UI.Layout
{
    /// <summary>
    /// Contains model classes and factory methods for creating header card information.
    /// Provides a flexible system for displaying different types of header content
    /// based on the current page context (Corporate, Department, Store, etc.).
    /// </summary>
    public class HeaderCardModels
    {
        /// <summary>
        /// Flexible data model that can represent header information for various page types.
        /// Contains numerous properties to support different display scenarios.
        /// Properties are used conditionally based on the header type being displayed.
        /// </summary>
        public class HeadInfoCardModel
        {
            #region Core Information

            /// <summary>
            /// Primary title displayed in the header (e.g., department name, store name, page title).
            /// Always visible and typically the most prominent text in the header.
            /// </summary>
            public string Title { get; set; } = "";

            /// <summary>
            /// Primary email address for contact information.
            /// Used for department emails, corporate contact, etc.
            /// </summary>
            public string Email { get; set; } = "";

            #endregion

            #region Subtitle Fields

            /// <summary>
            /// Label for the first piece of supplementary information.
            /// Often used for "Dept. Phone", "Corp. Phone", or role titles.
            /// </summary>
            public string Subtitle1 { get; set; } = "";

            /// <summary>
            /// Label for the second piece of supplementary information.
            /// Commonly used for "Dept. Email", "Corp. Email", or secondary role titles.
            /// </summary>
            public string Subtitle2 { get; set; } = "";

            /// <summary>
            /// Label for the third piece of supplementary information.
            /// Often used for "Dept. Manager", "IT Help Desk", or additional role titles.
            /// </summary>
            public string Subtitle3 { get; set; } = "";

            /// <summary>
            /// Label for the fourth piece of supplementary information.
            /// Used in contexts requiring multiple labeled fields (plants, area managers).
            /// </summary>
            public string Subtitle4 { get; set; } = "";

            /// <summary>
            /// Label for the fifth piece of supplementary information.
            /// Used in complex layouts with many organizational roles.
            /// </summary>
            public string Subtitle5 { get; set; } = "";

            /// <summary>
            /// Label for the sixth piece of supplementary information.
            /// Used in complex layouts with many organizational roles.
            /// </summary>
            public string Subtitle6 { get; set; } = "";

            /// <summary>
            /// Label for the seventh piece of supplementary information.
            /// Used in complex layouts requiring extensive role listings.
            /// </summary>
            public string Subtitle7 { get; set; } = "";

            /// <summary>
            /// Label for the eighth piece of supplementary information.
            /// Used in complex layouts requiring extensive role listings.
            /// </summary>
            public string Subtitle8 { get; set; } = "";

            #endregion

            #region Contact Information

            /// <summary>
            /// Primary phone number (department phone, corporate phone, etc.).
            /// Pairs with Subtitle1 in most factory method implementations.
            /// </summary>
            public string Phone { get; set; } = "";

            /// <summary>
            /// Secondary phone number or contact method.
            /// Often used for manager names or additional phone numbers.
            /// Context-dependent based on the header type.
            /// </summary>
            public string Phone2 { get; set; } = "";

            /// <summary>
            /// Tertiary phone number or contact method.
            /// Used when multiple contact options are needed.
            /// </summary>
            public string Phone3 { get; set; } = "";

            #endregion

            #region Address Information

            /// <summary>
            /// Street address for location information.
            /// Primarily used in corporate and store contexts.
            /// </summary>
            public string Address { get; set; } = "";

            /// <summary>
            /// City portion of the address.
            /// Used alongside Address, State, and Zip for complete location info.
            /// </summary>
            public string City { get; set; } = "";

            /// <summary>
            /// State portion of the address.
            /// Used alongside Address, City, and Zip for complete location info.
            /// </summary>
            public string State { get; set; } = "";

            /// <summary>
            /// ZIP code portion of the address.
            /// Used alongside Address, City, and State for complete location info.
            /// </summary>
            public string Zip { get; set; } = "";

            #endregion

            #region Management Information

            /// <summary>
            /// Store manager name for store-specific headers.
            /// Used in store contexts to show key personnel.
            /// </summary>
            public string StoreManager { get; set; } = "";

            /// <summary>
            /// Area manager name for regional management info.
            /// Used in store and area management contexts.
            /// </summary>
            public string AreaManager { get; set; } = "";

            #endregion

            #region Team/Title Manager Fields

            /// <summary>
            /// First manager/title holder name.
            /// Used in contexts requiring multiple management roles (plants, area management).
            /// Pairs with Subtitle1 in Plant and AreaManagers factory methods.
            /// </summary>
            public string TManager1 { get; set; } = "";

            /// <summary>
            /// Second manager/title holder name.
            /// Used in contexts requiring multiple management roles.
            /// Pairs with Subtitle2 in Plant and AreaManagers factory methods.
            /// </summary>
            public string TManager2 { get; set; } = "";

            /// <summary>
            /// Third manager/title holder name.
            /// Used in contexts requiring multiple management roles.
            /// Pairs with Subtitle3 in Plant and AreaManagers factory methods.
            /// </summary>
            public string TManager3 { get; set; } = "";

            /// <summary>
            /// Fourth manager/title holder name.
            /// Used in contexts requiring multiple management roles.
            /// Pairs with Subtitle4 in Plant and AreaManagers factory methods.
            /// </summary>
            public string TManager4 { get; set; } = "";

            /// <summary>
            /// Fifth manager/title holder name.
            /// Used in contexts requiring multiple management roles.
            /// Pairs with Subtitle5 in Plant and AreaManagers factory methods.
            /// </summary>
            public string TManager5 { get; set; } = "";

            /// <summary>
            /// Sixth manager/title holder name.
            /// Used in contexts requiring multiple management roles.
            /// Pairs with Subtitle6 in Plant and AreaManagers factory methods.
            /// </summary>
            public string TManager6 { get; set; } = "";

            /// <summary>
            /// Seventh manager/title holder name.
            /// Available for complex organizational structures.
            /// </summary>
            public string TManager7 { get; set; } = "";

            /// <summary>
            /// Eighth manager/title holder name.
            /// Available for complex organizational structures.
            /// </summary>
            public string TManager8 { get; set; } = "";

            #endregion

            #region UI Control

            /// <summary>
            /// Controls whether a back button should be displayed in the header.
            /// String value ("true"/"false") for easy binding to UI components.
            /// Defaults to "true" to show back button unless explicitly disabled.
            /// </summary>
            public string ShowBackButton { get; set; } = "true";

            #endregion
        }

        #region Factory Methods

        /// <summary>
        /// Creates a corporate header model with McElroy Metal company information.
        /// Displays corporate contact details and IT help desk information.
        /// Used for home page and corporate section headers.
        /// </summary>
        /// <returns>HeadInfoCardModel configured for corporate display</returns>
        public static HeadInfoCardModel Corporate()
        {
            return new HeadInfoCardModel
            {
                Title = "Employee Directory",
                Address = "1500 Hamilton Rd",           // Corporate headquarters address
                City = "Bossier City",
                State = "LA",
                Zip = "71111",
                Subtitle1 = "Corp. Phone",              // Label for main corporate number
                Phone = "(888) 245-3696",               // Main corporate phone number
                Subtitle2 = "Corp. Email",              // Label for corporate email
                Email = "info@mcelroymetal.com",        // Corporate contact email
                Subtitle3 = "IT Help Desk",             // Label for IT support
                Phone2 = "(866) 634-5111",              // IT help desk phone number
            };
        }

        /// <summary>
        /// Creates a department header model with department-specific contact information.
        /// Used for department pages to show department details and management.
        /// </summary>
        /// <param name="title">Department name or title</param>
        /// <param name="phone">Department phone number</param>
        /// <param name="email">Department email address</param>
        /// <param name="manager">Department manager name</param>
        /// <returns>HeadInfoCardModel configured for department display</returns>
        public static HeadInfoCardModel ForDepartment(string title, string? phone, string? email, string? manager)
        {
            return new HeadInfoCardModel
            {
                Title = title,
                Subtitle1 = "Dept. Phone",              // Standard label for department phone
                Phone = phone ?? "",                    // Department phone (null-safe)
                Subtitle2 = "Dept. Email",              // Standard label for department email
                Email = email ?? "",                    // Department email (null-safe)
                Subtitle3 = "Dept. Manager",            // Standard label for department manager
                Phone2 = manager ?? ""                  // Manager name in Phone2 field (reusing field)
            };
        }

        /// <summary>
        /// Creates a plant header model with multiple management roles and titles.
        /// Supports up to 6 different role/person pairs for complex plant hierarchies.
        /// Used for manufacturing plant or facility pages.
        /// </summary>
        /// <param name="title">Plant or facility name</param>
        /// <param name="subtitle1">Label for first role (e.g., "Plant Manager")</param>
        /// <param name="tmanager1">Name of person in first role</param>
        /// <param name="subtitle2">Label for second role</param>
        /// <param name="tmanager2">Name of person in second role</param>
        /// <param name="subtitle3">Label for third role</param>
        /// <param name="tmanager3">Name of person in third role</param>
        /// <param name="subtitle4">Label for fourth role</param>
        /// <param name="tmanager4">Name of person in fourth role</param>
        /// <param name="subtitle5">Label for fifth role</param>
        /// <param name="tmanager5">Name of person in fifth role</param>
        /// <param name="subtitle6">Label for sixth role</param>
        /// <param name="tmanager6">Name of person in sixth role</param>
        /// <returns>HeadInfoCardModel configured for plant display with multiple management roles</returns>
        public static HeadInfoCardModel Plant(string title, string? subtitle1, string? tmanager1, string? subtitle2, string? tmanager2, string? subtitle3, string? tmanager3, string? subtitle4, string? tmanager4, string? subtitle5, string? tmanager5, string? subtitle6, string? tmanager6)
        {
            return new HeadInfoCardModel
            {
                Title = title,
                // Map role titles to subtitle fields and names to TManager fields
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

        /// <summary>
        /// Creates a simple employee header model with just a title.
        /// Used for individual employee pages where minimal header info is needed.
        /// </summary>
        /// <param name="title">Employee name or page title</param>
        /// <returns>HeadInfoCardModel configured for employee display</returns>
        public static HeadInfoCardModel Employee(string title)
        {
            return new HeadInfoCardModel
            {
                Title = title
                // Only title is set - minimal header for employee pages
            };
        }

        /// <summary>
        /// Creates a store header model with store and area management information.
        /// Used for store-specific pages to show key management personnel.
        /// </summary>
        /// <param name="title">Store name or identifier</param>
        /// <param name="subtitle1">Label for store manager (e.g., "Store Manager")</param>
        /// <param name="storemanager">Store manager name</param>
        /// <param name="subtitle2">Label for area manager (e.g., "Area Manager")</param>
        /// <param name="areamanager">Area manager name</param>
        /// <returns>HeadInfoCardModel configured for store display</returns>
        public static HeadInfoCardModel Store(string title, string subtitle1, string storemanager, string subtitle2, string areamanager)
        {
            return new HeadInfoCardModel
            {
                Title = title,
                Subtitle1 = subtitle1 ?? "",            // Store manager label
                StoreManager = storemanager ?? "",       // Store manager name
                Subtitle2 = subtitle2 ?? "",             // Area manager label
                AreaManager = areamanager ?? ""          // Area manager name
            };
        }

        /// <summary>
        /// Creates an area managers header model similar to Plant but for area management.
        /// Supports up to 6 different area management roles and personnel.
        /// Used for regional management pages showing area management hierarchy.
        /// </summary>
        /// <param name="title">Area or region name</param>
        /// <param name="subtitle1">Label for first area role</param>
        /// <param name="tmanager1">Name of person in first area role</param>
        /// <param name="subtitle2">Label for second area role</param>
        /// <param name="tmanager2">Name of person in second area role</param>
        /// <param name="subtitle3">Label for third area role</param>
        /// <param name="tmanager3">Name of person in third area role</param>
        /// <param name="subtitle4">Label for fourth area role</param>
        /// <param name="tmanager4">Name of person in fourth area role</param>
        /// <param name="subtitle5">Label for fifth area role</param>
        /// <param name="tmanager5">Name of person in fifth area role</param>
        /// <param name="subtitle6">Label for sixth area role</param>
        /// <param name="tmanager6">Name of person in sixth area role</param>
        /// <returns>HeadInfoCardModel configured for area management display</returns>
        public static HeadInfoCardModel AreaManagers(string title, string? subtitle1, string? tmanager1, string? subtitle2, string? tmanager2, string? subtitle3, string? tmanager3, string? subtitle4, string? tmanager4, string? subtitle5, string? tmanager5, string? subtitle6, string? tmanager6)
        {
            return new HeadInfoCardModel
            {
                Title = title,
                // Identical structure to Plant method - supports flexible role/person mapping
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

        #endregion
    }
}