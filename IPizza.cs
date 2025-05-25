public interface IPizza
{
    string GetDescription();
    double GetCost();
}

public class BasicPizza : IPizza
{
    private readonly Pizza _pizza;

    public BasicPizza(Pizza pizza)
    {
        _pizza = pizza;
    }

    public string GetDescription()
    {
        var toppings = _pizza.Toppings != null ? string.Join(", ", _pizza.Toppings) : "No toppings";
        return $"Pizza: {_pizza.Name}, Dough: {_pizza.Dough}, Sauce: {_pizza.Sauce}, Toppings: {toppings}";
    }

    public double GetCost()
    {
        return _pizza.BasePrice;
    }
}

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

    public virtual double GetCost()
    {
        return _pizza.GetCost();
    }
}

public class IngredientDecorator : PizzaDecorator
{
    private readonly string _ingredientName;
    private readonly double _ingredientPrice;

    public IngredientDecorator(IPizza pizza, string ingredientName, double ingredientPrice)
        : base(pizza)
    {
        _ingredientName = ingredientName;
        _ingredientPrice = ingredientPrice;
    }

    public override string GetDescription()
    {
        return _pizza.GetDescription() + $", {_ingredientName}";
    }

    public override double GetCost()
    {
        return _pizza.GetCost() + _ingredientPrice;
    }
}
