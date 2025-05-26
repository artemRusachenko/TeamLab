namespace TeamLab.Services.Delivery.Strategies
{
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
}
