using TeamLab.Services.Delivery.Strategies;

namespace TeamLab.Services.Delivery
{
    public class DeliveryService
    {
        private readonly List<IDeliveryStrategy> strategies;

        public DeliveryService()
        {
            strategies = new List<IDeliveryStrategy>
        {
            new SelfPickUpDelivery(),
            new CourierDelivery(),
            new DroneDelivery()
        };
        }

        public List<IDeliveryStrategy> GetAvailableStrategies(decimal? distanceInKm = null)
        {
            return strategies.Where(s => s.IsAvailable(distanceInKm)).ToList();
        }

        public IDeliveryStrategy? GetStrategyByName(string name)
        {
            return strategies.FirstOrDefault(s => s.GetType().Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
