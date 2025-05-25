using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamLab
{
    public class PizzaBuilder
    {
        private Pizza _pizza;

        public PizzaBuilder()
        {
            _pizza = new Pizza();
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
            _pizza.Toppings = toppings;
            return this;
        }

        public PizzaBuilder AddTopping(string topping)
        {
            if (_pizza.Toppings == null)
                _pizza.Toppings = new List<string>();
            _pizza.Toppings.Add(topping);
            return this;
        }

        public Pizza Build()
        {
            return _pizza;
        }
    }

}
