using TeamLab.Domain;

namespace TeamLab.Services.PizzaDecorator
{
    public class ToppingsDecorator : PizzaDecorator
    {
        private readonly List<Ingredient> _additionalToppings;

        public ToppingsDecorator(IPizza pizza, List<Ingredient> additionalToppings) : base(pizza)
        {
            _additionalToppings = additionalToppings;
        }

        public override string GetDescription()
        {
            var baseDesc = base.GetDescription();
            var toppingsDesc = string.Join(", ", _additionalToppings.Select(t => t.Name));
            return $"{baseDesc}\nДодано топінги: {toppingsDesc}";
        }

        public override double GetPrice()
        {
            double extraPrice = _additionalToppings.Sum(t => t.Price);
            return base.GetPrice() + extraPrice;
        }
    }
}
