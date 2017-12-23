using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FLive.Web.Migrations
{
    public partial class WorkoutPublishStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkoutPublishStatus",
                table: "UpcomingLiveWorkouts",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkoutPublishStatus",
                table: "UpcomingLiveWorkouts");
        }
    }
}
