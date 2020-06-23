﻿// <auto-generated />
using System;
using FFCDemoPaymentService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace FFCDemoPaymentService.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("FFCDemoPaymentService.Models.Payment", b =>
                {
                    b.Property<string>("ClaimId")
                        .HasColumnName("claimId")
                        .HasColumnType("text");

                    b.Property<decimal>("Value")
                        .HasColumnName("value")
                        .HasColumnType("numeric");

                    b.HasKey("ClaimId");

                    b.ToTable("payments");
                });

            modelBuilder.Entity("FFCDemoPaymentService.Models.Schedule", b =>
                {
                    b.Property<int>("ScheduleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("scheduleId")
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

                    b.Property<string>("ClaimId")
                        .HasColumnName("claimId")
                        .HasColumnType("text");

                    b.Property<DateTime>("PaymentDate")
                        .HasColumnName("paymentDate")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("ScheduleId");

                    b.ToTable("schedule");
                });
#pragma warning restore 612, 618
        }
    }
}
