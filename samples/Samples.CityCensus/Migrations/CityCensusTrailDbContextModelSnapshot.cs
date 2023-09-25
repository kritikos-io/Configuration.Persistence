﻿// <auto-generated />
using System;
using Kritikos.Samples.CityCensus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Kritikos.Samples.CityCensus.Migrations
{
    [DbContext(typeof(CityCensusTrailDbContext))]
    partial class CityCensusTrailDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.11");

            modelBuilder.Entity("CorporationCounty", b =>
                {
                    b.Property<long>("CorporationsId")
                        .HasColumnType("INTEGER");

                    b.Property<long>("CountiesId")
                        .HasColumnType("INTEGER");

                    b.HasKey("CorporationsId", "CountiesId");

                    b.HasIndex("CountiesId");

                    b.ToTable("CorporationCounty");
                });

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

                    b.Property<int>("Modification")
                        .HasColumnType("INTEGER");

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

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Kritikos.Samples.CityCensus.Joins.CountyCorporation", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long?>("CorporationId")
                        .HasColumnType("INTEGER");

                    b.Property<long?>("CountyId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CorporationId");

                    b.HasIndex("CountyId");

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

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(false);

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

            modelBuilder.Entity("CorporationCounty", b =>
                {
                    b.HasOne("Kritikos.Samples.CityCensus.Model.Corporation", null)
                        .WithMany()
                        .HasForeignKey("CorporationsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Kritikos.Samples.CityCensus.Model.County", null)
                        .WithMany()
                        .HasForeignKey("CountiesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Kritikos.Samples.CityCensus.Joins.CountyCorporation", b =>
                {
                    b.HasOne("Kritikos.Samples.CityCensus.Model.Corporation", "Corporation")
                        .WithMany()
                        .HasForeignKey("CorporationId");

                    b.HasOne("Kritikos.Samples.CityCensus.Model.County", "County")
                        .WithMany()
                        .HasForeignKey("CountyId");

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
