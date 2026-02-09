using LMS.Domain.Purchasing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Persistence.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Product");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.ProductType).HasConversion<int>().IsRequired();
        builder.Property(p => p.ModuleId);
        builder.Property(p => p.ContentItemId);
        builder.Property(p => p.PointsPackageId);
        builder.Property(p => p.Name).HasMaxLength(300).IsRequired();
        builder.Property(p => p.Description).HasColumnType("text");
        builder.Property(p => p.ThumbnailUrl).HasMaxLength(500);
        builder.Property(p => p.PriceEgp).HasPrecision(10, 2).IsRequired();
        builder.Property(p => p.PointsPrice).IsRequired().HasDefaultValue(0);
        builder.Property(p => p.AllowPointsPurchase).IsRequired().HasDefaultValue(true);
        builder.Property(p => p.DiscountedPriceEgp).HasPrecision(10, 2);
        builder.Property(p => p.DiscountStartUtc);
        builder.Property(p => p.DiscountEndUtc);
        builder.Property(p => p.ParentProductId);
        builder.Property(p => p.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(p => p.DisplayOrder).IsRequired().HasDefaultValue(0);
        builder.Property(p => p.CreatedAtUtc).IsRequired();
        builder.Property(p => p.UpdatedAtUtc).IsRequired();
        builder.HasIndex(p => new { p.ProductType, p.IsActive }).HasDatabaseName("IX_Product_Type");
        builder.HasIndex(p => p.ModuleId).HasDatabaseName("IX_Product_Module");
    }
}

public sealed class PromoCodeConfiguration : IEntityTypeConfiguration<PromoCode>
{
    public void Configure(EntityTypeBuilder<PromoCode> builder)
    {
        builder.ToTable("PromoCode");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Code).HasMaxLength(50).IsRequired();
        builder.Property(p => p.DiscountType).HasConversion<int>().IsRequired();
        builder.Property(p => p.DiscountValue).HasPrecision(10, 2).IsRequired();
        builder.Property(p => p.MaxTotalUses);
        builder.Property(p => p.CurrentTotalUses).IsRequired().HasDefaultValue(0);
        builder.Property(p => p.MaxUsesPerUser).IsRequired().HasDefaultValue(1);
        builder.Property(p => p.MinOrderAmountEgp).HasPrecision(10, 2);
        builder.Property(p => p.ValidFromUtc).IsRequired();
        builder.Property(p => p.ValidUntilUtc).IsRequired();
        builder.Property(p => p.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(p => p.CreatedByUserId).IsRequired();
        builder.Property(p => p.CreatedAtUtc).IsRequired();
        builder.Property(p => p.UpdatedAtUtc).IsRequired();
        builder.HasIndex(p => p.Code).IsUnique().HasDatabaseName("UQ_PromoCode_Code");
        builder.HasIndex(p => new { p.IsActive, p.ValidFromUtc, p.ValidUntilUtc }).HasDatabaseName("IX_PromoCode_Valid");
    }
}

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Order");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).HasConversion(id => id.Value, value => OrderId.From(value)).ValueGeneratedNever();
        builder.Property(o => o.OrderNumber).HasMaxLength(50).IsRequired();
        builder.Property(o => o.StudentId).IsRequired();
        builder.Property(o => o.Status).HasConversion<int>().IsRequired();
        builder.Property(o => o.SubTotalEgp).HasPrecision(10, 2).IsRequired();
        builder.Property(o => o.DiscountAmountEgp).HasPrecision(10, 2).IsRequired().HasDefaultValue(0);
        builder.Property(o => o.PointsUsed).IsRequired().HasDefaultValue(0);
        builder.Property(o => o.PointsValueEgp).HasPrecision(10, 2).IsRequired().HasDefaultValue(0);
        builder.Property(o => o.TotalAmountEgp).HasPrecision(10, 2).IsRequired();
        builder.Property(o => o.TotalAmountPaidEgp).HasPrecision(10, 2).IsRequired().HasDefaultValue(0);
        builder.Property(o => o.PromoCodeId);
        builder.Property(o => o.PaymentMethod).HasConversion<int?>();
        builder.Property(o => o.PaymobOrderId).HasMaxLength(100);
        builder.Property(o => o.PaymobTransactionId).HasMaxLength(100);
        builder.Property(o => o.PaidAtUtc);
        builder.Property(o => o.ExpiresAtUtc);
        builder.Property(o => o.Notes).HasColumnType("text");
        builder.Property(o => o.CreatedAtUtc).IsRequired();
        builder.Property(o => o.UpdatedAtUtc).IsRequired();
        builder.HasIndex(o => o.OrderNumber).IsUnique().HasDatabaseName("UQ_Order_OrderNumber");
        builder.HasIndex(o => new { o.StudentId, o.Status }).HasDatabaseName("IX_Order_Student");
        builder.HasIndex(o => o.Status).HasDatabaseName("IX_Order_Status");
        builder.Ignore(o => o.DomainEvents);
    }
}

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItem");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.OrderId).HasConversion(id => id.Value, value => OrderId.From(value)).IsRequired();
        builder.Property(o => o.ProductId).IsRequired();
        builder.Property(o => o.ProductName).HasMaxLength(300).IsRequired();
        builder.Property(o => o.ProductType).HasConversion<int>().IsRequired();
        builder.Property(o => o.UnitPriceEgp).HasPrecision(10, 2).IsRequired();
        builder.Property(o => o.Quantity).IsRequired().HasDefaultValue(1);
        builder.Property(o => o.TotalPriceEgp).HasPrecision(10, 2).IsRequired();
        builder.Property(o => o.CreatedAtUtc).IsRequired();
        builder.Property(o => o.UpdatedAtUtc).IsRequired();
        builder.HasIndex(o => o.OrderId).HasDatabaseName("IX_OrderItem_Order");
        builder.HasIndex(o => o.ProductId).HasDatabaseName("IX_OrderItem_Product");
    }
}

public sealed class StudentEnrollmentConfiguration : IEntityTypeConfiguration<StudentEnrollment>
{
    public void Configure(EntityTypeBuilder<StudentEnrollment> builder)
    {
        builder.ToTable("StudentEnrollment");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.StudentId).IsRequired();
        builder.Property(s => s.ModuleId).IsRequired();
        builder.Property(s => s.OrderId).HasConversion(id => id.Value, value => OrderId.From(value)).IsRequired();
        builder.Property(s => s.IncludesFollowUp).IsRequired().HasDefaultValue(false);
        builder.Property(s => s.Status).HasConversion<int>().IsRequired();
        builder.Property(s => s.AccessStartUtc).IsRequired();
        builder.Property(s => s.AccessEndUtc);
        builder.Property(s => s.CompletedAtUtc);
        builder.Property(s => s.CreatedAtUtc).IsRequired();
        builder.Property(s => s.UpdatedAtUtc).IsRequired();
        builder.HasIndex(s => new { s.StudentId, s.ModuleId }).IsUnique().HasDatabaseName("UQ_StudentEnrollment");
        builder.HasIndex(s => new { s.StudentId, s.Status }).HasDatabaseName("IX_StudentEnrollment_Student");
        builder.HasIndex(s => s.ModuleId).HasDatabaseName("IX_StudentEnrollment_Module");
    }
}
