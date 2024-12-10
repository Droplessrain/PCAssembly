using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PCAssembly.Data;

public partial class ComputerAssemblyContext : IdentityDbContext<User>
{
    public ComputerAssemblyContext()
    {
    }

    public ComputerAssemblyContext(DbContextOptions<ComputerAssemblyContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Assembly> Assemblies { get; set; }

    public virtual DbSet<AssemblyComponent> AssemblyComponents { get; set; }

    public virtual DbSet<Component> Components { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<TypeComponent> TypeComponents { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Name=DefaultConnection");
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Assembly>(entity =>
        {
            entity.HasKey(e => e.AssemblyId).HasName("PK__Assembli__1C02BD4B2F8E9E60");

            entity.Property(e => e.AssemblyId).HasColumnName("AssemblyID");
            entity.Property(e => e.AssemblyName).HasMaxLength(100);
            entity.Property(e => e.Avgrating).HasColumnName("AVGRating");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Assemblies)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Assemblie__UserI__4316F928");
        });

        modelBuilder.Entity<AssemblyComponent>(entity =>
        {
            entity.HasKey(e => e.AssemblyComponentId);

            entity.Property(e => e.AssemblyId).HasColumnName("AssemblyID");
            entity.Property(e => e.ComponentId).HasColumnName("ComponentID");

            entity.HasOne(d => d.Assembly).WithMany()
                .HasForeignKey(d => d.AssemblyId)
                .HasConstraintName("FK__AssemblyC__Assem__440B1D61");

            entity.HasOne(d => d.Component).WithMany()
                .HasForeignKey(d => d.ComponentId)
                .HasConstraintName("FK__AssemblyC__Compo__44FF419A");
        });

        modelBuilder.Entity<Component>(entity =>
        {
            entity.HasKey(e => e.ComponentId).HasName("PK__Componen__D79CF02E31DC1A0D");

            entity.Property(e => e.ComponentId).HasColumnName("ComponentID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TypeComponentsId).HasColumnName("TypeComponentsID");

            entity.HasOne(d => d.TypeComponents).WithMany(p => p.Components)
                .HasForeignKey(d => d.TypeComponentsId)
                .HasConstraintName("FK__Component__TypeC__45F365D3");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__74BC79AE12A3F745");

            entity.ToTable(tb => tb.HasTrigger("trg_UpdateAVGRating"));

            entity.Property(e => e.ReviewId).HasColumnName("ReviewID");
            entity.Property(e => e.AssemblyId).HasColumnName("AssemblyID");
            entity.Property(e => e.ReviewText).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Assembly).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.AssemblyId)
                .HasConstraintName("FK__Reviews__Assembl__46E78A0C");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reviews_Users");
        });

        modelBuilder.Entity<TypeComponent>(entity =>
        {
            entity.HasKey(e => e.TypeComponentsId).HasName("PK__TypeComp__70E8417F814E506F");

            entity.Property(e => e.TypeComponentsId).HasColumnName("TypeComponentsID");
            entity.Property(e => e.TypeName).HasMaxLength(40);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
