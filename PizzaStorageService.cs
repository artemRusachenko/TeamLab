using System.Text;
using System.Text.Json;

namespace TeamLab
{
    public class PizzaStorageService
    {
        private readonly string _filePath;

        public PizzaStorageService(string filePath)
        {
            _filePath = filePath;
        }

        public List<Pizza> LoadPizzas()
        {
            if (!File.Exists(_filePath)) return new List<Pizza>();
            var json = File.ReadAllText(_filePath, Encoding.UTF8);
            return JsonSerializer.Deserialize<List<Pizza>>(json);
        }

        /*public void SavePizzas(List<Pizza> pizzas)
        {
            var json = JsonSerializer.Serialize(pizzas, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }*/
    }
}
