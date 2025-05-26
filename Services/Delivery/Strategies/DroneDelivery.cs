namespace TeamLab.Services.Delivery.Strategies
{
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
}
