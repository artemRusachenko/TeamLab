using System.Text.Json;
using System.Text;
using TeamLab.Domain;
using System.Text.Json.Serialization;

public class IngredientStorageService
{
    private readonly string _filePath;
    private List<Ingredient> _ingredients;

    public IngredientStorageService(string filePath)
    {
        _filePath = filePath;
        _ingredients = LoadIngredients();
    }

    public List<Ingredient> LoadIngredients()
    {
        if (!File.Exists(_filePath)) return new List<Ingredient>();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        var json = File.ReadAllText(_filePath, Encoding.UTF8);
        return JsonSerializer.Deserialize<List<Ingredient>>(json, options);

    }

    public List<Ingredient> GetDoughs()
    {
        return _ingredients.Where(i => i.Type == IngredientType.Dough).ToList();
    }

    public List<Ingredient> GetSauces()
    {
        return _ingredients.Where(i => i.Type == IngredientType.Sauce).ToList();
    }

    public List<Ingredient> GetToppings()
    {
        return _ingredients.Where(i => i.Type == IngredientType.Topping).ToList();
    }
}
