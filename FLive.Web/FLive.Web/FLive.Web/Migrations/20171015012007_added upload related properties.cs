using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FLive.Web.Migrations
{
    public partial class addeduploadrelatedproperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PublishDateTime",
                table: "UpcomingLiveWorkouts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkoutType",
                table: "UpcomingLiveWorkouts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Trainers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "LiveWorkouts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "LiveWorkouts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishDateTime",
                table: "LiveWorkouts",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SponsorId",
                table: "LiveWorkouts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkoutType",
                table: "LiveWorkouts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Sponsors",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    SponsorsMessage = table.Column<string>(nullable: true),
                    TrademarkUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sponsors", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LiveWorkouts_SponsorId",
                table: "LiveWorkouts",
                column: "SponsorId");

            migrationBuilder.AddForeignKey(
                name: "FK_LiveWorkouts_Sponsors_SponsorId",
                table: "LiveWorkouts",
                column: "SponsorId",
                principalTable: "Sponsors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LiveWorkouts_Sponsors_SponsorId",
                table: "LiveWorkouts");

            migrationBuilder.DropTable(
                name: "Sponsors");

            migrationBuilder.DropIndex(
                name: "IX_LiveWorkouts_SponsorId",
                table: "LiveWorkouts");

            migrationBuilder.DropColumn(
                name: "PublishDateTime",
                table: "UpcomingLiveWorkouts");

            migrationBuilder.DropColumn(
                name: "WorkoutType",
                table: "UpcomingLiveWorkouts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "LiveWorkouts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "LiveWorkouts");

            migrationBuilder.DropColumn(
                name: "PublishDateTime",
                table: "LiveWorkouts");

            migrationBuilder.DropColumn(
                name: "SponsorId",
                table: "LiveWorkouts");

            migrationBuilder.DropColumn(
                name: "WorkoutType",
                table: "LiveWorkouts");
        }
    }
}
