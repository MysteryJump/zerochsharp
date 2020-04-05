﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ZerochSharp.Models;

namespace ZerochSharp.Migrations
{
    [DbContext(typeof(MainContext))]
    [Migration("20200211223341_somechange-in-acl")]
    partial class somechangeinacl
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("ZerochSharp.Models.Board", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("AutoRemovingPredicate")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("BoardDefaultName")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("BoardKey")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("BoardName")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("BoardSubTitle")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("Boards");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            BoardDefaultName = "以下、名無しにかわりまして裏VIP(´・ω・`)がお送りします",
                            BoardKey = "news7vip",
                            BoardName = "裏VIP"
                        },
                        new
                        {
                            Id = 2,
                            BoardDefaultName = "雑談うんちー",
                            BoardKey = "coffeehouse",
                            BoardName = "雑談ルノワール"
                        });
                });

            modelBuilder.Entity("ZerochSharp.Models.Response", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("HostAddress")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("IsAboned")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Mail")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("ThreadId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Responses");
                });

            modelBuilder.Entity("ZerochSharp.Models.Session", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("Expired")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("SessionToken")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("SettingData")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("ZerochSharp.Models.Thread", b =>
                {
                    b.Property<int>("ThreadId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<bool>("Archived")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("BoardKey")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime(6)");

                    b.Property<long>("DatKey")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("Modified")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("ResponseCount")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("ThreadId");

                    b.ToTable("Threads");
                });

            modelBuilder.Entity("ZerochSharp.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("Authority")
                        .HasColumnType("int");

                    b.Property<int>("CapAuthority")
                        .HasColumnType("int");

                    b.Property<string>("ControllableBoard")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("PasswordSalt")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("SystemAuthority")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ZerochSharp.Models.UserSession", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("Expired")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("SessionToken")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("UserSessions");
                });
#pragma warning restore 612, 618
        }
    }
}