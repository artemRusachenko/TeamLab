using TeamLab;
using System.Text.Json;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var pizzasPath = Path.Combine(AppContext.BaseDirectory, "pizzas.json");
var ingredientsPath = Path.Combine(AppContext.BaseDirectory, "ingridients.json");

var pizzaStorage = new PizzaStorageService(pizzasPath);
var allPizzas = pizzaStorage.LoadPizzas();

var ingredientsJson = File.ReadAllText(ingredientsPath);
var allIngredients = JsonSerializer.Deserialize<List<Ingredient>>(ingredientsJson)!;

Console.WriteLine("Оберіть піцу:");
for (int i = 0; i < allPizzas.Count; i++)
{
    Console.WriteLine(i + ":" +  allPizzas[i].ToString());
}

int pizzaChoice = int.Parse(Console.ReadLine()!) - 1;
var basePizza = new BasicPizza(allPizzas[pizzaChoice]);

IPizza customPizza = basePizza;

Console.WriteLine("\nДодайте інгредієнти (введіть номер, 0 щоб завершити):");
while (true)
{
    for (int i = 0; i < allIngredients.Count; i++)
    {
        Console.WriteLine($"{i + 1}. {allIngredients[i].Name} (+{allIngredients[i].Price} грн)");
    }

    Console.Write("Ваш вибір: ");
    var input = Console.ReadLine();
    if (input == "0") break;

    int ingredientIndex = int.Parse(input!) - 1;
    var selectedIngredient = allIngredients[ingredientIndex];
    customPizza = new IngredientDecorator(customPizza, selectedIngredient.Name, selectedIngredient.Price);
}

Console.WriteLine("\n🔸 Ваша піца:");
Console.WriteLine(customPizza.GetDescription());
Console.WriteLine($"💰 Загальна ціна: {customPizza.GetCost()} грн");
