namespace TeamLab;

public interface IDeliveryStrategy
{
    decimal Cost { get; }
    bool IsAvailable(decimal? distanceInKm = null);
    decimal CalculateCost(decimal? distanceInKm = null);
    string GetDeliveryInfo(); 
    TimeSpan GetDeliveryTime(decimal? distanceInKm = null);
}
public class CourierDelivery : IDeliveryStrategy
{
    private const decimal PricePerKmInUah = 45.0m;
    private const decimal MaxDistanceInKm = 5.0m;
    private const decimal BaseDeliveryTimeMinutes = 15m;
    private const decimal MinutesPerKm = 3m;
    
    public decimal Cost { get; private set; }
    
    public bool IsAvailable(decimal? distanceInKm = null)
    {
        return distanceInKm.HasValue && distanceInKm <= MaxDistanceInKm;
    }
    
    public decimal CalculateCost(decimal? distanceInKm = null)
    {
        if (!distanceInKm.HasValue)
            throw new ArgumentException("Для кур'єрської доставки необхідно вказати дистанцію");
        
        if (distanceInKm > MaxDistanceInKm)
            throw new ArgumentException($"Кур'єрська доставка доступна лише до {MaxDistanceInKm} км");
        
        Cost = distanceInKm.Value * PricePerKmInUah;
        return Cost;
    }
    
    public string GetDeliveryInfo()
    {
        return $"Кур'єрська доставка (до {MaxDistanceInKm} км)\n" +
               $"Вартість: {PricePerKmInUah} грн/км\n" +
               $"Мінімальний час доставки: {BaseDeliveryTimeMinutes} хв";
    }
    
    public TimeSpan GetDeliveryTime(decimal? distanceInKm = null)
    {
        if (!distanceInKm.HasValue || distanceInKm > MaxDistanceInKm)
            return TimeSpan.Zero;
            
        return TimeSpan.FromMinutes((double)(BaseDeliveryTimeMinutes + distanceInKm.Value * MinutesPerKm));
    }
}

public class SelfPickUpDelivery : IDeliveryStrategy
{
    private const string PickupAddress = "вул. Піцерійна, 12";
    private const int PreparationTimeMinutes = 10;
    
    public decimal Cost => 0;
    
    public bool IsAvailable(decimal? distanceInKm = null)
    {
        return true;
    }
    
    public decimal CalculateCost(decimal? distanceInKm = null)
    {
        return Cost;
    }
    
    public string GetDeliveryInfo()
    {
        return $"Самовивіз\n" +
               $"Безкоштовно\n" +
               $"Адреса: {PickupAddress}\n" +
               $"Час приготування: ~{PreparationTimeMinutes} хв";
    }
    
    public TimeSpan GetDeliveryTime(decimal? distanceInKm = null)
    {
        return TimeSpan.FromMinutes(PreparationTimeMinutes);
    }
}

public class DroneDelivery : IDeliveryStrategy
{
    private const decimal PricePerKm = 25.0m;
    private const decimal MaxDistanceInKm = 15.0m;
    private const decimal BaseDroneTimeMinutes = 5m;
    private const decimal MinutesPerKmDrone = 1.5m;
    
    public decimal Cost { get; private set; }
    
    public bool IsAvailable(decimal? distanceInKm = null)
    {
        return distanceInKm.HasValue && distanceInKm <= MaxDistanceInKm;
    }
    
    public decimal CalculateCost(decimal? distanceInKm = null)
    {
        if (!distanceInKm.HasValue)
            throw new ArgumentException("Для доставки дроном необхідно вказати дистанцію");
        
        if (distanceInKm > MaxDistanceInKm)
            throw new ArgumentException($"Доставка дроном доступна лише до {MaxDistanceInKm} км");
        
        Cost = distanceInKm.Value * PricePerKm;
        return Cost;
    }
    
    public string GetDeliveryInfo()
    {
        return $"Доставка дроном (до {MaxDistanceInKm} км)\n" +
               $"Вартість: {PricePerKm} грн/км\n" +
               $"Найшвидший спосіб доставки!";
    }
    
    public TimeSpan GetDeliveryTime(decimal? distanceInKm = null)
    {
        if (!distanceInKm.HasValue || distanceInKm > MaxDistanceInKm)
            return TimeSpan.Zero;
            
        return TimeSpan.FromMinutes((double)(BaseDroneTimeMinutes + distanceInKm.Value * MinutesPerKmDrone));
    }
}