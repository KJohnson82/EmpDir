using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmpDir.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Loctypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LoctypeName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loctypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LocName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LocNum = table.Column<int>(type: "INTEGER", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: false),
                    City = table.Column<string>(type: "TEXT", nullable: false),
                    State = table.Column<string>(type: "TEXT", maxLength: 3, nullable: false),
                    Zipcode = table.Column<string>(type: "TEXT", nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    FaxNumber = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    Hours = table.Column<string>(type: "TEXT", nullable: true),
                    Loctype = table.Column<int>(type: "INTEGER", nullable: false),
                    AreaManager = table.Column<string>(type: "TEXT", nullable: true),
                    StoreManager = table.Column<string>(type: "TEXT", nullable: true),
                    RecordAdd = table.Column<DateTime>(type: "DATETIME DEFAULT CURRENT_TIMESTAMP", nullable: true),
                    Active = table.Column<bool>(type: "INTEGER", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locations_Loctypes_Loctype",
                        column: x => x.Loctype,
                        principalTable: "Loctypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeptName = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Location = table.Column<int>(type: "INTEGER", nullable: false),
                    DeptManager = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    DeptPhone = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    DeptEmail = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    DeptFax = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    RecordAdd = table.Column<DateTime>(type: "DATETIME DEFAULT CURRENT_TIMESTAMP", nullable: true),
                    Active = table.Column<bool>(type: "INTEGER", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departments_Locations_Location",
                        column: x => x.Location,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    JobTitle = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    IsManager = table.Column<bool>(type: "INTEGER", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    CellNumber = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    Extension = table.Column<string>(type: "TEXT", maxLength: 8, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    NetworkId = table.Column<string>(type: "TEXT", nullable: true),
                    EmpAvatar = table.Column<string>(type: "TEXT", nullable: true),
                    Location = table.Column<int>(type: "INTEGER", nullable: true),
                    Department = table.Column<int>(type: "INTEGER", nullable: true),
                    RecordAdd = table.Column<DateTime>(type: "DATETIME DEFAULT CURRENT_TIMESTAMP", nullable: true),
                    Active = table.Column<bool>(type: "INTEGER", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Departments_Department",
                        column: x => x.Department,
                        principalTable: "Departments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Employees_Locations_Location",
                        column: x => x.Location,
                        principalTable: "Locations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Location",
                table: "Departments",
                column: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Department",
                table: "Employees",
                column: "Department");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Location",
                table: "Employees",
                column: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_Loctype",
                table: "Locations",
                column: "Loctype");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Loctypes");
        }
    }
}
