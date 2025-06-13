using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class BaseEntityChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "Patients",
                newName: "ModifiedDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "DoctorSchedules",
                newName: "ModifiedDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "Doctors",
                newName: "ModifiedDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "Departments",
                newName: "ModifiedDate");

            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "Appointments",
                newName: "ModifiedDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastVisitDate",
                table: "Patients",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Patients",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "DoctorSchedules",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Doctors",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Departments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "AppointmentTime",
                table: "Appointments",
                type: "TEXT",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "CancelledBy",
                table: "Appointments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelledDate",
                table: "Appointments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "Appointments",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastVisitDate",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "DoctorSchedules");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "AppointmentTime",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "CancelledBy",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "CancelledDate",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "Patients",
                newName: "UpdatedDate");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "DoctorSchedules",
                newName: "UpdatedDate");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "Doctors",
                newName: "UpdatedDate");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "Departments",
                newName: "UpdatedDate");

            migrationBuilder.RenameColumn(
                name: "ModifiedDate",
                table: "Appointments",
                newName: "UpdatedDate");
        }
    }
}
