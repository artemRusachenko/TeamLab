namespace TeamLab.Services.Delivery.Strategies
{
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
}
