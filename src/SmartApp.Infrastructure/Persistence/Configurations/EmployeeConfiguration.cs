namespace SmartApp.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartApp.Domain.Entities;

public sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.LastName).HasMaxLength(100).IsRequired();

        builder.Property(e => e.Email).HasMaxLength(200).IsRequired();
        builder.HasIndex(e => e.Email).IsUnique().HasDatabaseName("IX_Employees_Email");

        builder.Property(e => e.Department).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Position).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Salary).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(e => e.JoinedAt).IsRequired();
        builder.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);

        builder.HasIndex(e => e.Department).HasDatabaseName("IX_Employees_Department");
        builder.HasIndex(e => e.IsActive).HasDatabaseName("IX_Employees_IsActive");
        builder.HasIndex(e => new { e.Department, e.IsActive }).HasDatabaseName("IX_Employees_Department_IsActive");
    }
}