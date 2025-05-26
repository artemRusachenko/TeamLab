namespace TeamLab.Services.PizzaDecorator
{
    public abstract class PizzaDecorator : IPizza
    {
        protected IPizza _pizza;

        public PizzaDecorator(IPizza pizza)
        {
            _pizza = pizza;
        }

        public virtual string GetDescription()
        {
            return _pizza.GetDescription();
        }

        public virtual double GetPrice()
        {
            return _pizza.GetPrice();
        }
    }
}
