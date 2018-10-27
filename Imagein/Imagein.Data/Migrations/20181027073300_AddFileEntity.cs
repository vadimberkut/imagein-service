using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Imagein.Data.Migrations
{
    public partial class AddFileEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(39)", nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    FileStoreType = table.Column<int>(nullable: false),
                    Path = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    MimeType = table.Column<string>(nullable: true),
                    Size = table.Column<int>(nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CAST(NOW() at time zone 'utc' AS timestamp)"),
                    UpdatedOnUtc = table.Column<DateTime>(type: "timestamp", nullable: false, defaultValueSql: "CAST(NOW() at time zone 'utc' AS timestamp)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Files");
        }
    }
}
