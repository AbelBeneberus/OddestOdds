﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OddestOdds.Data.Database;

#nullable disable

namespace OddestOdds.Data.Migrations
{
    [DbContext(typeof(FixtureDbContext))]
    partial class FixtureContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("OddestOdds.Data.Models.Fixture", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AwayTeam")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FixtureName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HomeTeam")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Fixtures");
                });

            modelBuilder.Entity("OddestOdds.Data.Models.Market", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("FixtureId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("MarketName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("FixtureId");

                    b.ToTable("Markets");
                });

            modelBuilder.Entity("OddestOdds.Data.Models.MarketSelection", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("MarketId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Odd")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Side")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MarketId");

                    b.ToTable("MarketSelections");
                });

            modelBuilder.Entity("OddestOdds.Data.Models.Market", b =>
                {
                    b.HasOne("OddestOdds.Data.Models.Fixture", null)
                        .WithMany("Markets")
                        .HasForeignKey("FixtureId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OddestOdds.Data.Models.MarketSelection", b =>
                {
                    b.HasOne("OddestOdds.Data.Models.Market", null)
                        .WithMany("Selections")
                        .HasForeignKey("MarketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OddestOdds.Data.Models.Fixture", b =>
                {
                    b.Navigation("Markets");
                });

            modelBuilder.Entity("OddestOdds.Data.Models.Market", b =>
                {
                    b.Navigation("Selections");
                });
#pragma warning restore 612, 618
        }
    }
}
