using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FLive.Web.Migrations
{
    public partial class TrainerSubscriber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          

            migrationBuilder.CreateTable(
                name: "TrainerSubsribers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SubscriberId = table.Column<long>(nullable: false),
                    TrainerId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainerSubsribers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainerSubsribers_Subscribers_SubscriberId",
                        column: x => x.SubscriberId,
                        principalTable: "Subscribers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainerSubsribers_Trainers_TrainerId",
                        column: x => x.TrainerId,
                        principalTable: "Trainers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrainerSubsribers_SubscriberId",
                table: "TrainerSubsribers",
                column: "SubscriberId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainerSubsribers_TrainerId",
                table: "TrainerSubsribers",
                column: "TrainerId");

          
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainerFollowers_Subscribers_SubscriberId",
                table: "TrainerFollowers");

            migrationBuilder.DropTable(
                name: "TrainerSubsribers");

            migrationBuilder.RenameColumn(
                name: "SubscriberId",
                table: "TrainerFollowers",
                newName: "FollowerId");

            migrationBuilder.RenameIndex(
                name: "IX_TrainerFollowers_SubscriberId",
                table: "TrainerFollowers",
                newName: "IX_TrainerFollowers_FollowerId");

            migrationBuilder.CreateTable(
                name: "TrainerSubscribers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SubscriberId = table.Column<long>(nullable: false),
                    TrainerId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainerSubscribers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrainerSubscribers_Subscribers_SubscriberId",
                        column: x => x.SubscriberId,
                        principalTable: "Subscribers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainerSubscribers_Trainers_TrainerId",
                        column: x => x.TrainerId,
                        principalTable: "Trainers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrainerSubscribers_SubscriberId",
                table: "TrainerSubscribers",
                column: "SubscriberId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainerSubscribers_TrainerId",
                table: "TrainerSubscribers",
                column: "TrainerId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainerFollowers_Subscribers_FollowerId",
                table: "TrainerFollowers",
                column: "FollowerId",
                principalTable: "Subscribers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
