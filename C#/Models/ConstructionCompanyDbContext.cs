using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ConstructionCompany.Models;

public partial class ConstructionCompanyDbContext : DbContext
{
    public ConstructionCompanyDbContext()
    {
    }

    public ConstructionCompanyDbContext(DbContextOptions<ConstructionCompanyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Brigade> Brigades { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Equipment> Equipment { get; set; }

    public virtual DbSet<Material> Materials { get; set; }

    public virtual DbSet<MaterialWarehouse> MaterialWarehouses { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserActionLog> UserActionLogs { get; set; }

    public virtual DbSet<Warehouse> Warehouses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=ConstructionCompany;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brigade>(entity =>
        {
            entity.HasKey(e => e.BrigadeId).HasName("PK__Brigades__5486999272FE20BA");

            entity.Property(e => e.BrigadeId).HasColumnName("brigade_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Specialization)
                .HasMaxLength(100)
                .HasColumnName("specialization");

            entity.HasMany(d => d.Projects).WithMany(p => p.Brigades)
                .UsingEntity<Dictionary<string, object>>(
                    "BrigadeProject",
                    r => r.HasOne<Project>().WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__BrigadePr__proje__7B5B524B"),
                    l => l.HasOne<Brigade>().WithMany()
                        .HasForeignKey("BrigadeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__BrigadePr__briga__7A672E12"),
                    j =>
                    {
                        j.HasKey("BrigadeId", "ProjectId").HasName("PK__BrigadeP__BF410073B2261115");
                        j.ToTable("BrigadeProject");
                        j.IndexerProperty<int>("BrigadeId").HasColumnName("brigade_id");
                        j.IndexerProperty<int>("ProjectId").HasColumnName("project_id");
                    });
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__C52E0BA894BFC3E4");

            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.HireDate).HasColumnName("hire_date");
            entity.Property(e => e.Position)
                .HasMaxLength(100)
                .HasColumnName("position");
            entity.Property(e => e.Salary)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("salary");

            entity.HasMany(d => d.Brigades).WithMany(p => p.Employees)
                .UsingEntity<Dictionary<string, object>>(
                    "EmployeeBrigade",
                    r => r.HasOne<Brigade>().WithMany()
                        .HasForeignKey("BrigadeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__EmployeeB__briga__5535A963"),
                    l => l.HasOne<Employee>().WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__EmployeeB__emplo__5441852A"),
                    j =>
                    {
                        j.HasKey("EmployeeId", "BrigadeId").HasName("PK__Employee__F06662319C22D6ED");
                        j.ToTable("EmployeeBrigade");
                        j.IndexerProperty<int>("EmployeeId").HasColumnName("employee_id");
                        j.IndexerProperty<int>("BrigadeId").HasColumnName("brigade_id");
                    });
        });

        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasKey(e => e.EquipmentId).HasName("PK__Equipmen__197068AFE1D58379");

            entity.Property(e => e.EquipmentId).HasColumnName("equipment_id");
            entity.Property(e => e.Brand)
                .HasMaxLength(100)
                .HasColumnName("brand");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.PurchaseDate).HasColumnName("purchase_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");

            entity.HasMany(d => d.Employees).WithMany(p => p.Equipment)
                .UsingEntity<Dictionary<string, object>>(
                    "EquipmentEmployee",
                    r => r.HasOne<Employee>().WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Equipment__emplo__4D94879B"),
                    l => l.HasOne<Equipment>().WithMany()
                        .HasForeignKey("EquipmentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Equipment__equip__4CA06362"),
                    j =>
                    {
                        j.HasKey("EquipmentId", "EmployeeId").HasName("PK__Equipmen__852288155B0F521A");
                        j.ToTable("EquipmentEmployee");
                        j.IndexerProperty<int>("EquipmentId").HasColumnName("equipment_id");
                        j.IndexerProperty<int>("EmployeeId").HasColumnName("employee_id");
                    });

            entity.HasMany(d => d.Projects).WithMany(p => p.Equipment)
                .UsingEntity<Dictionary<string, object>>(
                    "EquipmentProject",
                    r => r.HasOne<Project>().WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Equipment__proje__5165187F"),
                    l => l.HasOne<Equipment>().WithMany()
                        .HasForeignKey("EquipmentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Equipment__equip__5070F446"),
                    j =>
                    {
                        j.HasKey("EquipmentId", "ProjectId").HasName("PK__Equipmen__F2B7F14E55EE1CC2");
                        j.ToTable("EquipmentProject");
                        j.IndexerProperty<int>("EquipmentId").HasColumnName("equipment_id");
                        j.IndexerProperty<int>("ProjectId").HasColumnName("project_id");
                    });
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.MaterialId).HasName("PK__Material__6BFE1D28AEFAD6AF");

            entity.Property(e => e.MaterialId).HasColumnName("material_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.TotalPrice)
                .HasComputedColumnSql("([total_quantity]*[unit_price])", true)
                .HasColumnType("decimal(29, 2)")
                .HasColumnName("total_price");
            entity.Property(e => e.TotalQuantity).HasColumnName("total_quantity");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("unit_price");
        });

        modelBuilder.Entity<MaterialWarehouse>(entity =>
        {
            entity.HasKey(e => new { e.MaterialId, e.WarehouseId }).HasName("PK__Material__8CCAE343E48238EE");

            entity.ToTable("MaterialWarehouse", tb => tb.HasTrigger("trg_UpdateMaterialTotalQuantity"));

            entity.Property(e => e.MaterialId).HasColumnName("material_id");
            entity.Property(e => e.WarehouseId).HasColumnName("warehouse_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Material).WithMany(p => p.MaterialWarehouses)
                .HasForeignKey(d => d.MaterialId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MaterialW__mater__48CFD27E");

            entity.HasOne(d => d.Warehouse).WithMany(p => p.MaterialWarehouses)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MaterialW__wareh__49C3F6B7");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__Projects__BC799E1F6280231B");

            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Budget)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("budget");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__Tasks__0492148D8E238B41");

            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.BrigadeId).HasColumnName("brigade_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Deadline).HasColumnName("deadline");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");

            entity.HasOne(d => d.Brigade).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.BrigadeId)
                .HasConstraintName("FK__Tasks__brigade_i__45F365D3");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370FB84F7501");

            entity.HasIndex(e => e.Username, "UQ__Users__F3DBC572AFDC24DD").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasColumnName("role");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");

            entity.HasOne(d => d.Employee).WithMany(p => p.Users)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK__Users__employee___59FA5E80");
        });

        modelBuilder.Entity<UserActionLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__UserActi__9E2397E0E69D02F3");

            entity.Property(e => e.LogId).HasColumnName("log_id");
            entity.Property(e => e.ActionDate)
                .HasDefaultValueSql("(CONVERT([date],getdate()))")
                .HasColumnName("action_date");
            entity.Property(e => e.ActionDescription)
                .HasMaxLength(1000)
                .HasColumnName("action_description");
            entity.Property(e => e.ActionTime)
                .HasDefaultValueSql("(CONVERT([time],getdate()))")
                .HasColumnName("action_time");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserActionLogs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserActio__user___5EBF139D");
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.HasKey(e => e.WarehouseId).HasName("PK__Warehous__734FE6BF5B546E3D");

            entity.Property(e => e.WarehouseId).HasColumnName("warehouse_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
