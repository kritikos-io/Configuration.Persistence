﻿// <auto-generated />
using System;
using Kritikos.Samples.CityCensus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Kritikos.Samples.CityCensus.Migrations
{
    [DbContext(typeof(CityCensusTrailDbContext))]
    partial class CityCensusTrailDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.10");

            modelBuilder.Entity("Kritikos.Configuration.Persistence.Entities.AuditRecord", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("TEXT");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Modification")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("NewValues")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("OldValues")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Table")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("AuditRecords");
                });

            modelBuilder.Entity("Kritikos.Samples.CityCensus.Base.OrderedCityEntity<long, Kritikos.Samples.CityCensus.Model.Corporation>", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("Order")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("OrderedCityEntity<long, Corporation>");

                    b.HasDiscriminator<string>("Discriminator").HasValue("OrderedCityEntity<long, Corporation>");
                });

            modelBuilder.Entity("Kritikos.Samples.CityCensus.Joins.CountyCorporation", b =>
                {
                    b.Property<long>("CountyId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("CorporationId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Id")
                        .HasColumnType("INTEGER");

                    b.HasKey("CountyId", "CorporationId");

                    b.HasIndex("CorporationId");

                    b.ToTable("CountyCorporations");
                });

            modelBuilder.Entity("Kritikos.Samples.CityCensus.Model.County", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("Order")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Counties");
                });

            modelBuilder.Entity("Kritikos.Samples.CityCensus.Model.Person", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long?>("CountyId")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UpdatedBy")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CountyId");

                    b.ToTable("People");
                });

            modelBuilder.Entity("Kritikos.Samples.CityCensus.Model.Corporation", b =>
                {
                    b.HasBaseType("Kritikos.Samples.CityCensus.Base.OrderedCityEntity<long, Kritikos.Samples.CityCensus.Model.Corporation>");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasDiscriminator().HasValue("Corporation");
                });

            modelBuilder.Entity("Kritikos.Samples.CityCensus.Joins.CountyCorporation", b =>
                {
                    b.HasOne("Kritikos.Samples.CityCensus.Model.Corporation", "Corporation")
                        .WithMany()
                        .HasForeignKey("CorporationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Kritikos.Samples.CityCensus.Model.County", "County")
                        .WithMany()
                        .HasForeignKey("CountyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Corporation");

                    b.Navigation("County");
                });

            modelBuilder.Entity("Kritikos.Samples.CityCensus.Model.Person", b =>
                {
                    b.HasOne("Kritikos.Samples.CityCensus.Model.County", "County")
                        .WithMany()
                        .HasForeignKey("CountyId");

                    b.Navigation("County");
                });
#pragma warning restore 612, 618
        }
    }
}
