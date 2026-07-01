namespace DhoobiGO.Domain.Enums;

public enum OrderStatus
{
    PendingBidding,
    BidSelected,
    PickupScheduled,
    PickedUp,
    InLaundry,
    ReadyForDelivery,
    OutForDelivery,
    Completed,
    Cancelled,
    Disputed,
    Returned
}
