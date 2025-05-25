public class Pizza
{
    public string Name { get; set; }
    public string Dough { get; set; }
    public string Sauce { get; set; }
    public List<string> Toppings { get; set; }
    public double BasePrice { get; set; } 

    public override string ToString()
    {
        var toppings = Toppings != null && Toppings.Count > 0 ? string.Join(", ", Toppings) : "None";
        return $"Pizza: {Name}, Dough: {Dough}, Sauce: {Sauce}, Toppings: {toppings}, Base price: {BasePrice} грн";
    }
}
