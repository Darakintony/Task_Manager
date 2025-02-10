﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TaskManagementAPI.Data;

#nullable disable

namespace TaskManagementAPI.Migrations
{
    [DbContext(typeof(TaskManagementDbContext))]
    [Migration("20250210164224_AddEnumStringConverter")]
    partial class AddEnumStringConverter
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("TaskManagementAPI.Model.ProjectMagTable", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UsersId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UsersId");

                    b.ToTable("ProjectMagTables");
                });

            modelBuilder.Entity("TaskManagementAPI.Model.TaskMagTable", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DueDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Priority")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ProjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("TaskMagTables");
                });

            modelBuilder.Entity("TaskManagementAPI.Model.UserMagTable", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("UserMagTables");
                });

            modelBuilder.Entity("TaskManagementAPI.Model.ProjectMagTable", b =>
                {
                    b.HasOne("TaskManagementAPI.Model.UserMagTable", "UserMagTable")
                        .WithMany("ProjectMagTables")
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserMagTable");
                });

            modelBuilder.Entity("TaskManagementAPI.Model.TaskMagTable", b =>
                {
                    b.HasOne("TaskManagementAPI.Model.ProjectMagTable", "ProjectMagTable")
                        .WithMany("TaskMagTables")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ProjectMagTable");
                });

            modelBuilder.Entity("TaskManagementAPI.Model.ProjectMagTable", b =>
                {
                    b.Navigation("TaskMagTables");
                });

            modelBuilder.Entity("TaskManagementAPI.Model.UserMagTable", b =>
                {
                    b.Navigation("ProjectMagTables");
                });
#pragma warning restore 612, 618
        }
    }
}
