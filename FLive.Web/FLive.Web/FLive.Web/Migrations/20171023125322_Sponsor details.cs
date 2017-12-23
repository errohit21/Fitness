using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FLive.Web.Migrations
{
    public partial class Sponsordetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SponsorLogo",
                table: "TrainingCategories",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SponsorText",
                table: "TrainingCategories",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SponsorLogo",
                table: "TrainingCategories");

            migrationBuilder.DropColumn(
                name: "SponsorText",
                table: "TrainingCategories");
        }
    }
}
