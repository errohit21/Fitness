using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FLive.Web.Migrations
{
    public partial class renamedtokentable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PasswordResetRequests",
                table: "PasswordResetRequests");

            migrationBuilder.RenameTable(
                name: "PasswordResetRequests",
                newName: "IdentityVerifications");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdentityVerifications",
                table: "IdentityVerifications",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_IdentityVerifications",
                table: "IdentityVerifications");

            migrationBuilder.RenameTable(
                name: "IdentityVerifications",
                newName: "PasswordResetRequests");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PasswordResetRequests",
                table: "PasswordResetRequests",
                column: "Id");
        }
    }
}
