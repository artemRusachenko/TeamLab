namespace TeamLab.Domain
{
    public class Pizza
    {
        public string Name { get; set; }
        public string Dough { get; set; }
        public string Sauce { get; set; }
        public List<string> Toppings { get; set; } = new();
        public double BasePrice { get; set; }

        public override string ToString()
        {
            string toppings;

            if (Toppings != null && Toppings.Count > 0)
            {
                toppings = string.Join(", ", Toppings
                    .GroupBy(t => t)
                    .Select(g => g.Count() > 1 ? $"{g.Key} x{g.Count()}" : g.Key));
            }
            else
            {
                toppings = "None";
            }

            return $"Pizza: {Name}, Dough: {Dough}, Sauce: {Sauce}, Toppings: {toppings}, Base price: {BasePrice} грн";
        }

    }
}