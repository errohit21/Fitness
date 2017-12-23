using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FLive.Web.Migrations
{
    public partial class address : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddressLine1",
                table: "Trainers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressLine2",
                table: "Trainers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Trainers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostCode",
                table: "Trainers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Suburb",
                table: "Trainers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressLine1",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "AddressLine2",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "PostCode",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "Suburb",
                table: "Trainers");
        }
    }
}
