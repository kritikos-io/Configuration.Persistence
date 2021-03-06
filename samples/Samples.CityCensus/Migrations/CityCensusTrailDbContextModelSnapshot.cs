﻿// <auto-generated />
using System;
using Kritikos.Samples.CityCensus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Kritikos.Samples.CityCensus.Migrations
{
    [DbContext(typeof(CityCensusTrailDbContext))]
    partial class CityCensusTrailDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.5")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Kritikos.Configuration.Persistence.Entities.AuditRecord", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uuid");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Modification")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("NewValues")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("OldValues")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Table")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("AuditRecords");
                });

            modelBuilder.Entity("Kritikos.Samples.CityCensus.Corporation", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Corporations");
                });

            modelBuilder.Entity("Kritikos.Samples.CityCensus.County", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Counties");
                });

            modelBuilder.Entity("Kritikos.Samples.CityCensus.CountyCorporation", b =>
                {
                    b.Property<long>("CountyId")
                        .HasColumnType("bigint");

                    b.Property<long>("CorporationId")
                        .HasColumnType("bigint");

                    b.HasKey("CountyId", "CorporationId");

                    b.HasIndex("CorporationId");

                    b.ToTable("CountyCorporations");
                });

            modelBuilder.Entity("Kritikos.Samples.CityCensus.CountyCorporation", b =>
                {
                    b.HasOne("Kritikos.Samples.CityCensus.Corporation", "Corporation")
                        .WithMany()
                        .HasForeignKey("CorporationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Kritikos.Samples.CityCensus.County", "County")
                        .WithMany()
                        .HasForeignKey("CountyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Corporation");

                    b.Navigation("County");
                });
#pragma warning restore 612, 618
        }
    }
}
