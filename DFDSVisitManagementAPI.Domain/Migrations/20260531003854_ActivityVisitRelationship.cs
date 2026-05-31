using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DFDSVisitManagementAPI.Domain.Migrations
{
    /// <inheritdoc />
    public partial class ActivityVisitRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Visits_Activities_ActivityId",
                table: "Visits");

            migrationBuilder.DropIndex(
                name: "IX_Visits_ActivityId",
                table: "Visits");

            migrationBuilder.AddColumn<Guid>(
                name: "VisitId",
                table: "Activities",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Activities_VisitId",
                table: "Activities",
                column: "VisitId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Visits_VisitId",
                table: "Activities",
                column: "VisitId",
                principalTable: "Visits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Visits_VisitId",
                table: "Activities");

            migrationBuilder.DropIndex(
                name: "IX_Activities_VisitId",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "VisitId",
                table: "Activities");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_ActivityId",
                table: "Visits",
                column: "ActivityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_Activities_ActivityId",
                table: "Visits",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
