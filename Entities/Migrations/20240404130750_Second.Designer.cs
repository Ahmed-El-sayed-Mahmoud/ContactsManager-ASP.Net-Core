﻿// <auto-generated />
using System;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Entities.Migrations
{
    [DbContext(typeof(PersonsDbContext))]
    [Migration("20240404130750_Second")]
    partial class Second
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Entities.Country", b =>
                {
                    b.Property<Guid?>("CountryID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CountryName")
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.HasKey("CountryID");

                    b.ToTable("Countries", (string)null);

                    b.HasData(
                        new
                        {
                            CountryID = new Guid("14629847-905a-4a0e-9abe-80b61655c5cb"),
                            CountryName = "Philippines"
                        },
                        new
                        {
                            CountryID = new Guid("56bf46a4-02b8-4693-a0f5-0a95e2218bdc"),
                            CountryName = "Thailand"
                        },
                        new
                        {
                            CountryID = new Guid("12e15727-d369-49a9-8b13-bc22e9362179"),
                            CountryName = "China"
                        },
                        new
                        {
                            CountryID = new Guid("8f30bedc-47dd-4286-8950-73d8a68e5d41"),
                            CountryName = "Palestinian Territory"
                        },
                        new
                        {
                            CountryID = new Guid("501c6d33-1bbe-45f1-8fbd-2275913c6218"),
                            CountryName = "China"
                        });
                });

            modelBuilder.Entity("Entities.Person", b =>
                {
                    b.Property<Guid>("PersonID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Country")
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.Property<Guid?>("CountryID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("Gender")
                        .HasColumnType("int");

                    b.Property<string>("PersonName")
                        .HasMaxLength(40)
                        .HasColumnType("nvarchar(40)");

                    b.Property<bool>("ReceiveNewsLetters")
                        .HasColumnType("bit");

                    b.HasKey("PersonID");

                    b.ToTable("Persons", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
