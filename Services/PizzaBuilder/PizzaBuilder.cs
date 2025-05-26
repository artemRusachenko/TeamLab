using TeamLab.Domain;

namespace TeamLab.Services.PizzaBuilder
{
    public class PizzaBuilder
    {
        private Pizza _pizza;
        private List<Ingredient> _allIngredients;

        public PizzaBuilder(List<Ingredient> allIngredients)
        {
            _pizza = new Pizza { Toppings = new List<string>() };
            _allIngredients = allIngredients;
        }

        public PizzaBuilder SetName(string name)
        {
            _pizza.Name = name;
            return this;
        }

        public PizzaBuilder SetDough(string dough)
        {
            _pizza.Dough = dough;
            return this;
        }

        public PizzaBuilder SetSauce(string sauce)
        {
            _pizza.Sauce = sauce;
            return this;
        }

        public PizzaBuilder SetToppings(List<string> toppings)
        {
            _pizza.Toppings = new List<string>(toppings);
            return this;
        }

        public PizzaBuilder AddTopping(string topping)
        {
            if (_pizza.Toppings == null)
                _pizza.Toppings = new List<string>();
            _pizza.Toppings.Add(topping);
            return this;
        }

        public PizzaBuilder RemoveTopping(string topping)
        {
            _pizza.Toppings.Remove(topping);
            return this;
        }

        private double CalculatePrice()
        {
            double price = 0;

            var dough = _allIngredients.Find(i => i.Name == _pizza.Dough && i.Type == IngredientType.Dough);
            if (dough != null)
                price += dough.Price;

            var sauce = _allIngredients.Find(i => i.Name == _pizza.Sauce && i.Type == IngredientType.Sauce);
            if (sauce != null)
                price += sauce.Price;

            foreach (var toppingName in _pizza.Toppings)
            {
                var topping = _allIngredients.Find(i => i.Name == toppingName && i.Type == IngredientType.Topping);
                if (topping != null)
                    price += topping.Price;
            }

            return price;
        }

        public Pizza Build()
        {
            _pizza.BasePrice = CalculatePrice();

            var result = _pizza;
            _pizza = new Pizza { Toppings = new List<string>() };
            return result;
        }

        public List<string> GetToppings()
        {
            return new List<string>(_pizza.Toppings);
        }
    }
}
