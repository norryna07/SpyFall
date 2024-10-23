using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SpyFall_project.Models;

namespace SpyFall_project.Services;

public partial class SpyFallDBcontext : DbContext
{
    public SpyFallDBcontext()
    {
    }

    public SpyFallDBcontext(DbContextOptions<SpyFallDBcontext> options)
        : base(options)
    {
    }

    public virtual DbSet<CommonService> CommonServices { get; set; }

    public virtual DbSet<ServiceVerif> ServiceVerifs { get; set; }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //     => optionsBuilder.UseNpgsql("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CommonService>(entity =>
        {
            entity.HasKey(e => e.PortNumber);

            entity.ToTable("commonservices");

            entity.Property(e => e.PortNumber)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("portnumber");
            entity.Property(e => e.ServiceName)
                .HasMaxLength(50)
                .HasDefaultValue("unknown")
                .IsFixedLength()
                .HasColumnName("servicename");
        });

        modelBuilder.Entity<ServiceVerif>(entity =>
        {
            entity.HasKey(e => e.ServId);

            entity.ToTable("serviceverif");

            entity.Property(e => e.ServId).HasColumnName("servid");
            entity.Property(e => e.ContainResponse)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("containresponse");
            entity.Property(e => e.MinLengthResponse).HasColumnName("minlengthresponse");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsFixedLength()
                .HasColumnName("name");
            entity.Property(e => e.SendMessage)
                .HasMaxLength(100)
                .IsFixedLength()
                .HasColumnName("sendmessage");
            entity.Property(e => e.StartResponse)
                .HasMaxLength(50)
                .IsFixedLength()
                .HasColumnName("startresponse");
            entity.Property(e => e.SendSize).HasColumnName("sendsize");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
