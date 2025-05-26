using TeamLab.Domain;

namespace TeamLab.Services.PizzaDecorator
{
    public class BasicPizza : IPizza
    {
        private readonly Pizza _pizza;

        public BasicPizza(Pizza pizza)
        {
            _pizza = pizza;
        }

        public string GetDescription()
        {
            return _pizza.ToString();
        }

        public double GetPrice()
        {
            return _pizza.BasePrice;
        }
    }
}
