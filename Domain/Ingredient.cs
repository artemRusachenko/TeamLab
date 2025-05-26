namespace TeamLab.Domain
{
    public class Ingredient
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public IngredientType Type { get; set; }
    }

    public enum IngredientType
    {
        Dough,
        Sauce,
        Topping
    }
}
