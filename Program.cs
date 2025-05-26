using TeamLab.Domain;
using TeamLab.Services.Delivery;
using TeamLab.Services.PizzaBuilder;
using TeamLab.Services.PizzaDecorator;
using TeamLab.Storage;

class Program
{
    static void Main()
    {
        string pizzaFile = Path.Combine(AppContext.BaseDirectory, @"..\..\..\Storage\Data\pizzas.json");
        pizzaFile = Path.GetFullPath(pizzaFile);
        
        string ingredientFile = Path.Combine(AppContext.BaseDirectory, @"..\..\..\Storage\Data\ingridients.json");
        ingredientFile = Path.GetFullPath(ingredientFile);

        var ingredientService = new IngredientStorageService(ingredientFile);
        var allIngredients = ingredientService.LoadIngredients();

        var pizzaService = new PizzaStorageService(pizzaFile);
        var availablePizzas = pizzaService.LoadPizzas();

        Console.WriteLine("Ласкаво просимо до Піцерії!");
        Console.WriteLine("1 - Обрати готову піцу");
        Console.WriteLine("2 - Зібрати піцу самому");
        Console.Write("Ваш вибір: ");
        var choice = Console.ReadLine();

        if (choice == "1")
        {
            ChooseExistingPizza(availablePizzas, ingredientService);
        }
        else if (choice == "2")
        {
            CreateCustomPizza(allIngredients);
        }
        else
        {
            Console.WriteLine("Невірний вибір.");
        }

        Console.Write("Вкажіть дистанцію до вашої адреси (км): ");
        if (decimal.TryParse(Console.ReadLine(), out var distance))
        {
            ChooseDelivery(distance);
        }
        else
        {
            Console.WriteLine("Невірна дистанція.");
        }
    }

    static void ChooseExistingPizza(List<Pizza> pizzas, IngredientStorageService ingredientsService)
    {
        Console.WriteLine("Доступні піци:");
        for (int i = 0; i < pizzas.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {pizzas[i]}");
        }

        Console.Write("Введіть номер піци: ");
        if (int.TryParse(Console.ReadLine(), out int selected) && selected > 0 && selected <= pizzas.Count)
        {
            var basePizza = pizzas[selected - 1];
            Console.WriteLine($"Ви обрали: {basePizza}");

            IPizza decoratedPizza = new BasicPizza(basePizza);

            Console.Write("Бажаєте додати топінги до піци? (так/ні): ");
            string input = Console.ReadLine()?.Trim().ToLower();

            if (input == "так")
            {
                var toppings = ingredientsService.GetToppings();

                Console.WriteLine("Оберіть топінги для додавання (номери через кому):");
                for (int i = 0; i < toppings.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {toppings[i].Name} ({toppings[i].Price} грн)");
                }

                string toppingInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(toppingInput))
                {
                    var selectedToppings = new List<Ingredient>();

                    foreach (var s in toppingInput.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (int.TryParse(s.Trim(), out int index) && index > 0 && index <= toppings.Count)
                        {
                            selectedToppings.Add(toppings[index - 1]);
                        }
                    }

                    if (selectedToppings.Any())
                    {
                        decoratedPizza = new ToppingsDecorator(decoratedPizza, selectedToppings);
                    }
                }
            }

            Console.WriteLine("\nФінальна піца:");
            Console.WriteLine(decoratedPizza.GetDescription());
            Console.WriteLine($"Ціна: {decoratedPizza.GetPrice():F2} грн");
        }
        else
        {
            Console.WriteLine("Невірний номер піци.");
        }
    }


    static void CreateCustomPizza(List<Ingredient> ingredients)
    {
        var builder = new PizzaBuilder(ingredients);

        Console.Write("Введіть назву піци: ");
        builder.SetName(Console.ReadLine());

        // Вибір тіста
        var doughs = ingredients.FindAll(i => i.Type == IngredientType.Dough);
        Console.WriteLine("Оберіть тісто:");
        for (int i = 0; i < doughs.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {doughs[i].Name} ({doughs[i].Price} грн)");
        }
        if (int.TryParse(Console.ReadLine(), out int doughChoice) && doughChoice > 0 && doughChoice <= doughs.Count)
        {
            builder.SetDough(doughs[doughChoice - 1].Name);
        }

        // Вибір соусу
        var sauces = ingredients.FindAll(i => i.Type == IngredientType.Sauce);
        Console.WriteLine("Оберіть соус:");
        for (int i = 0; i < sauces.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {sauces[i].Name} ({sauces[i].Price} грн)");
        }
        if (int.TryParse(Console.ReadLine(), out int sauceChoice) && sauceChoice > 0 && sauceChoice <= sauces.Count)
        {
            builder.SetSauce(sauces[sauceChoice - 1].Name);
        }

        // Вибір топінгів
        var toppings = ingredients.FindAll(i => i.Type == IngredientType.Topping);
        Console.WriteLine("Оберіть топінги (введіть номера через кому, або порожньо для пропуску):");
        for (int i = 0; i < toppings.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {toppings[i].Name} ({toppings[i].Price} грн)");
        }
        var input = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(input))
        {
            var selectedToppings = new List<string>();
            foreach (var s in input.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                if (int.TryParse(s.Trim(), out int toppingNum) && toppingNum > 0 && toppingNum <= toppings.Count)
                {
                    selectedToppings.Add(toppings[toppingNum - 1].Name);
                }
            }
            builder.SetToppings(selectedToppings);
        }

        // Видалення топінгів
        while (true)
        {
            var currentToppings = builder.GetToppings();
            Console.WriteLine("Поточні топінги: " + string.Join(", ", currentToppings));
            Console.WriteLine("Введіть ім'я топінга, щоб видалити, або натисніть Enter для продовження:");
            var toppingToRemove = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(toppingToRemove))
                break;

            if (currentToppings.Contains(toppingToRemove))
            {
                builder.RemoveTopping(toppingToRemove);
                Console.WriteLine($"Топінг '{toppingToRemove}' видалено.");
            }
            else
            {
                Console.WriteLine($"Топінг '{toppingToRemove}' не знайдено в піці.");
            }
        }

        var customPizza = builder.Build();
        Console.WriteLine("Ваша піца готова:");
        Console.WriteLine(customPizza);
    }

    static void ChooseDelivery(decimal? distanceInKm)
    {
        var deliveryService = new DeliveryService();

        var availableStrategies = deliveryService.GetAvailableStrategies(distanceInKm);

        if (!availableStrategies.Any())
        {
            Console.WriteLine("На жаль, доставка недоступна для вашої адреси.");
            return;
        }

        Console.WriteLine("Оберіть спосіб доставки:");
        for (int i = 0; i < availableStrategies.Count; i++)
        {
            var strategy = availableStrategies[i];
            Console.WriteLine($"{i + 1}. {strategy.GetDeliveryInfo()}");
        }

        if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= availableStrategies.Count)
        {
            var selectedStrategy = availableStrategies[choice - 1];
            var cost = selectedStrategy.CalculateCost(distanceInKm);
            var time = selectedStrategy.GetDeliveryTime(distanceInKm);

            Console.WriteLine($"Ви обрали: {selectedStrategy.GetType().Name}");
            Console.WriteLine($"Вартість доставки: {cost} грн");
            Console.WriteLine($"Очікуваний час доставки: {time.TotalMinutes} хв");
        }
        else
        {
            Console.WriteLine("Невірний вибір.");
        }
    }

}
