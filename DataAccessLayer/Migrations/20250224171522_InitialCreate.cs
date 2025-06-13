using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tour",
                columns: table => new
                {
                    TourID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(225)", maxLength: 225, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Start = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    End = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Transport = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Distance = table.Column<float>(type: "real", nullable: true),
                    EstTime = table.Column<float>(type: "real", nullable: true),
                    RouteMap = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tour", x => x.TourID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tour");
        }
    }
}
