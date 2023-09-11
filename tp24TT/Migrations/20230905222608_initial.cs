using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace tp24TT.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Receivables",
                columns: table => new
                {
                    Reference = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssueDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OpeningValue = table.Column<double>(type: "float", nullable: false),
                    PaidValue = table.Column<double>(type: "float", nullable: false),
                    DueDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClosedDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cancelled = table.Column<bool>(type: "bit", nullable: true),
                    DebtorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DebtorReference = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DebtorAddress1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DebtorAddress2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DebtorTown = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DebtorState = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DebtorZip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DebtorCountryCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DebtorRegistrationNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receivables", x => x.Reference);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Receivables");
        }
    }
}
