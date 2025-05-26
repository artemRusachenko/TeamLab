namespace TeamLab.Services.Delivery;

public interface IDeliveryStrategy
{
    decimal Cost { get; }
    bool IsAvailable(decimal? distanceInKm = null);
    decimal CalculateCost(decimal? distanceInKm = null);
    string GetDeliveryInfo();
    TimeSpan GetDeliveryTime(decimal? distanceInKm = null);
}
