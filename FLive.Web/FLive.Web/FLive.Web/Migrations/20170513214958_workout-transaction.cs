using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FLive.Web.Migrations
{
    public partial class workouttransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TransactionId",
                table: "LiveWorkouts",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_LiveWorkoutId",
                table: "Transactions",
                column: "LiveWorkoutId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_LiveWorkouts_LiveWorkoutId",
                table: "Transactions",
                column: "LiveWorkoutId",
                principalTable: "LiveWorkouts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_LiveWorkouts_LiveWorkoutId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_LiveWorkoutId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "LiveWorkouts");
        }
    }
}
