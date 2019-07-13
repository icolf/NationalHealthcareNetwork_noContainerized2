using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Organizations.Api.Migrations
{
    public partial class Firstmigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    OrganizationId = table.Column<Guid>(nullable: false),
                    IsHospital = table.Column<bool>(nullable: false),
                    Ssn = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: false),
                    Email = table.Column<string>(maxLength: 50, nullable: true),
                    Description = table.Column<string>(maxLength: 250, nullable: true),
                    Url = table.Column<string>(maxLength: 250, nullable: true),
                    ImagePath = table.Column<string>(maxLength: 250, nullable: true),
                    ActiveAddressId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.OrganizationId);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    AddressId = table.Column<Guid>(nullable: false),
                    OrganizationId = table.Column<Guid>(nullable: false),
                    Address1 = table.Column<string>(maxLength: 250, nullable: false),
                    Address2 = table.Column<string>(maxLength: 250, nullable: true),
                    State = table.Column<string>(maxLength: 250, nullable: false),
                    City = table.Column<string>(maxLength: 250, nullable: false),
                    Country = table.Column<string>(maxLength: 250, nullable: false),
                    Zip = table.Column<string>(maxLength: 250, nullable: false),
                    PostalAddress1 = table.Column<string>(maxLength: 250, nullable: false),
                    PostalAddress2 = table.Column<string>(maxLength: 250, nullable: true),
                    PostalState = table.Column<string>(maxLength: 250, nullable: false),
                    PostalCity = table.Column<string>(maxLength: 250, nullable: false),
                    PostalCountry = table.Column<string>(maxLength: 250, nullable: false),
                    PostalZip = table.Column<string>(maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK_Addresses_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "OrganizationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Phones",
                columns: table => new
                {
                    PhoneId = table.Column<Guid>(nullable: false),
                    PhoneNumber = table.Column<string>(nullable: false),
                    OrganizationId = table.Column<Guid>(nullable: false),
                    Extension = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    IsForDisplay = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phones", x => x.PhoneId);
                    table.ForeignKey(
                        name: "FK_Phones_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "OrganizationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Organizations",
                columns: new[] { "OrganizationId", "ActiveAddressId", "Description", "Email", "ImagePath", "IsHospital", "Name", "Ssn", "Url" },
                values: new object[] { new Guid("123e4567-e89b-12d3-a456-426655440001"), 0, "Description for Organization 1", null, null, false, "Organization1", "000000001", null });

            migrationBuilder.InsertData(
                table: "Organizations",
                columns: new[] { "OrganizationId", "ActiveAddressId", "Description", "Email", "ImagePath", "IsHospital", "Name", "Ssn", "Url" },
                values: new object[] { new Guid("123e4567-e89b-12d3-a456-426655440002"), 0, "Description for Organization 2", null, null, false, "Organization1", "000000002", null });

            migrationBuilder.InsertData(
                table: "Addresses",
                columns: new[] { "AddressId", "Address1", "Address2", "City", "Country", "OrganizationId", "PostalAddress1", "PostalAddress2", "PostalCity", "PostalCountry", "PostalState", "PostalZip", "State", "Zip" },
                values: new object[,]
                {
                    { new Guid("123e4567-e89b-12d3-a456-426655440006"), "Address1 Organization1", null, "Kansas City", "USA", new Guid("123e4567-e89b-12d3-a456-426655440001"), "Address1 Organization1", null, "Kansas City", "USA", "MO", "64158", "MO", "64158" },
                    { new Guid("123e4567-e89b-12d3-a456-426655440007"), "Address1 Organization1", null, "Kansas City", "USA", new Guid("123e4567-e89b-12d3-a456-426655440002"), "Address1 Organization1", null, "Kansas City", "USA", "MO", "64158", "MO", "64158" }
                });

            migrationBuilder.InsertData(
                table: "Phones",
                columns: new[] { "PhoneId", "Extension", "IsForDisplay", "OrganizationId", "PhoneNumber", "Type" },
                values: new object[,]
                {
                    { new Guid("123e4567-e89b-12d3-a456-426655440003"), "123", false, new Guid("123e4567-e89b-12d3-a456-426655440001"), "1234567890", null },
                    { new Guid("123e4567-e89b-12d3-a456-426655440004"), "123", false, new Guid("123e4567-e89b-12d3-a456-426655440001"), "1234567890", null },
                    { new Guid("123e4567-e89b-12d3-a456-426655440005"), "123", false, new Guid("123e4567-e89b-12d3-a456-426655440002"), "1234567890", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_OrganizationId",
                table: "Addresses",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Phones_OrganizationId",
                table: "Phones",
                column: "OrganizationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Phones");

            migrationBuilder.DropTable(
                name: "Organizations");
        }
    }
}
