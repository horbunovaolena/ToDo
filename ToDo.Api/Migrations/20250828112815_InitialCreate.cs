using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDo.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Todos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    IsComplete = table.Column<bool>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    DueDate = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    Tags = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Todos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Todos");
        }
    }
}
