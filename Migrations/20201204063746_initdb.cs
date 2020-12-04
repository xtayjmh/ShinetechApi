using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace API.Migrations
{
    public partial class initdb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActionLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Who = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Content = table.Column<string>(type: "longtext", nullable: true),
                    IsDelete = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ParentId = table.Column<int>(type: "int", nullable: false),
                    DeptCode = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    DeptName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ResponsibleMan = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    ResponsiblePhone = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Remark = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    IsDelete = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoginTrackings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IP = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    FailedCount = table.Column<int>(type: "int", nullable: false),
                    ValidateionCode = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    IsDelete = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginTrackings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LoginName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Password = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Name = table.Column<string>(type: "varchar(50) CHARACTER SET utf8mb4", maxLength: 50, nullable: true),
                    IsAdmin = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsEnable = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDelete = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Parameter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Value = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    IsDelete = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UpdatedTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parameter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Parameter_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Parameter_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Parameter_CreatedBy",
                table: "Parameter",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Parameter_UpdatedBy",
                table: "Parameter",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActionLogs");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "LoginTrackings");

            migrationBuilder.DropTable(
                name: "Parameter");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
