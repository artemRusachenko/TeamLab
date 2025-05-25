using System.Text.Json;
using System.Text;
using TeamLab;

public class IngredientStorageService
{
    private readonly string _filePath;

    public IngredientStorageService(string filePath)
    {
        _filePath = filePath;
    }

    public List<Ingredient> LoadIngredients()
    {
        if (!File.Exists(_filePath)) return new List<Ingredient>();
        var json = File.ReadAllText(_filePath, Encoding.UTF8);
        return JsonSerializer.Deserialize<List<Ingredient>>(json);
    }
}
