using LMS.Domain.Common;

namespace LMS.Domain.Purchasing;

public sealed class Order : AggregateRoot<OrderId>
{
    public string OrderNumber { get; private set; }
    public Guid StudentId { get; private set; }
    public OrderStatus Status { get; private set; }
    public decimal SubTotalEgp { get; private set; }
    public decimal DiscountAmountEgp { get; private set; }
    public int PointsUsed { get; private set; }
    public decimal PointsValueEgp { get; private set; }
    public decimal TotalAmountEgp { get; private set; }
    public decimal TotalAmountPaidEgp { get; private set; }
    public Guid? PromoCodeId { get; private set; }
    public PaymentMethod? PaymentMethod { get; private set; }
    public string? PaymobOrderId { get; private set; }
    public string? PaymobTransactionId { get; private set; }
    public DateTime? PaidAtUtc { get; private set; }
    public DateTime? ExpiresAtUtc { get; private set; }
    public string? Notes { get; private set; }

    private Order() : base(OrderId.New()) { OrderNumber = string.Empty; }

    public static Result<Order> Create(Guid studentId, string orderNumber, decimal subTotal)
    {
        if (studentId == Guid.Empty || string.IsNullOrWhiteSpace(orderNumber))
            return Result<Order>.Failure(Error.Validation(ErrorCodes.Validation.Required));
        return Result<Order>.Success(new Order { Id = OrderId.New(), StudentId = studentId, OrderNumber = orderNumber, SubTotalEgp = subTotal, TotalAmountEgp = subTotal, Status = OrderStatus.Pending, ExpiresAtUtc = DateTime.UtcNow.AddMinutes(15) });
    }

    public Result ApplyDiscount(decimal discountAmount, Guid? promoCodeId = null)
    {
        if (discountAmount < 0 || discountAmount > SubTotalEgp)
            return Result.Failure(Error.Validation(ErrorCodes.Validation.InvalidInput));
        DiscountAmountEgp = discountAmount;
        PromoCodeId = promoCodeId;
        TotalAmountEgp = SubTotalEgp - DiscountAmountEgp - PointsValueEgp;
        return Result.Success();
    }

    public Result UsePoints(int points, decimal pointsValue)
    {
        if (points < 0 || pointsValue > SubTotalEgp)
            return Result.Failure(Error.Validation(ErrorCodes.Validation.InvalidInput));
        PointsUsed = points;
        PointsValueEgp = pointsValue;
        TotalAmountEgp = SubTotalEgp - DiscountAmountEgp - PointsValueEgp;
        if (TotalAmountEgp <= 0) { TotalAmountEgp = 0; PaymentMethod = Purchasing.PaymentMethod.PointsOnly; }
        return Result.Success();
    }

    public Result MarkAsPaid(PaymentMethod method, decimal amountPaid, string? paymobOrderId = null, string? paymobTransactionId = null)
    {
        if (Status != OrderStatus.Pending && Status != OrderStatus.Processing)
            return Result.Failure(Error.Conflict("ORDER.INVALID_STATUS"));
        Status = OrderStatus.Paid;
        PaymentMethod = method;
        TotalAmountPaidEgp = amountPaid;
        PaymobOrderId = paymobOrderId;
        PaymobTransactionId = paymobTransactionId;
        PaidAtUtc = DateTime.UtcNow;
        return Result.Success();
    }

    public void MarkAsProcessing() => Status = OrderStatus.Processing;
    public void MarkAsFailed() => Status = OrderStatus.Failed;
    public void Cancel() => Status = OrderStatus.Cancelled;
    public void Expire() => Status = OrderStatus.Expired;
}

public sealed class OrderItem : Entity<Guid>
{
    public OrderId OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; }
    public ProductType ProductType { get; private set; }
    public decimal UnitPriceEgp { get; private set; }
    public short Quantity { get; private set; }
    public decimal TotalPriceEgp { get; private set; }

    private OrderItem() : base() { OrderId = OrderId.New(); ProductName = string.Empty; }

    public static Result<OrderItem> Create(OrderId orderId, Guid productId, string productName, ProductType productType, decimal unitPrice, short quantity = 1)
    {
        if (orderId.Value == Guid.Empty || productId == Guid.Empty)
            return Result<OrderItem>.Failure(Error.Validation(ErrorCodes.Validation.Required));
        return Result<OrderItem>.Success(new OrderItem { Id = Guid.NewGuid(), OrderId = orderId, ProductId = productId, ProductName = productName, ProductType = productType, UnitPriceEgp = unitPrice, Quantity = quantity, TotalPriceEgp = unitPrice * quantity });
    }
}

public sealed class StudentEnrollment : Entity<Guid>
{
    public Guid StudentId { get; private set; }
    public Guid ModuleId { get; private set; }
    public OrderId OrderId { get; private set; }
    public bool IncludesFollowUp { get; private set; }
    public EnrollmentStatus Status { get; private set; }
    public DateTime AccessStartUtc { get; private set; }
    public DateTime? AccessEndUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }

    private StudentEnrollment() : base() { OrderId = OrderId.New(); }

    public static Result<StudentEnrollment> Create(Guid studentId, Guid moduleId, OrderId orderId, bool includesFollowUp = false)
    {
        if (studentId == Guid.Empty || moduleId == Guid.Empty || orderId.Value == Guid.Empty)
            return Result<StudentEnrollment>.Failure(Error.Validation(ErrorCodes.Validation.Required));
        return Result<StudentEnrollment>.Success(new StudentEnrollment { Id = Guid.NewGuid(), StudentId = studentId, ModuleId = moduleId, OrderId = orderId, IncludesFollowUp = includesFollowUp, Status = EnrollmentStatus.Active, AccessStartUtc = DateTime.UtcNow });
    }

    public void Complete() { Status = EnrollmentStatus.Completed; CompletedAtUtc = DateTime.UtcNow; }
    public void Pause() => Status = EnrollmentStatus.Paused;
    public void Resume() => Status = EnrollmentStatus.Active;
    public void Cancel() => Status = EnrollmentStatus.Cancelled;
}
