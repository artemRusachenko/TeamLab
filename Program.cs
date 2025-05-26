using TeamLab.Domain;
using TeamLab.Services.Delivery;
using TeamLab.Services.PizzaBuilder;
using TeamLab.Services.PizzaDecorator;
using TeamLab.Storage;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding = System.Text.Encoding.Unicode;

        string pizzaFile = Path.Combine(AppContext.BaseDirectory, @"..\..\..\Storage\Data\pizzas.json");
        pizzaFile = Path.GetFullPath(pizzaFile);
        string ingredientFile = Path.Combine(AppContext.BaseDirectory, @"..\..\..\Storage\Data\ingridients.json");
        ingredientFile = Path.GetFullPath(ingredientFile);

        var ingredientService = new IngredientStorageService(ingredientFile);
        var allIngredients = ingredientService.LoadIngredients() ?? new List<Ingredient>();
        var pizzaService = new PizzaStorageService(pizzaFile);
        var availablePizzas = pizzaService.LoadPizzas() ?? new List<Pizza>();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Ласкаво просимо до Піцерії!");
            Console.WriteLine("1 - Обрати готову піцу");
            Console.WriteLine("2 - Зібрати піцу самому");
            Console.WriteLine("0 - Вийти");
            Console.Write("Ваш вибір: ");
            var choice = Console.ReadLine()?.Trim();

            IPizza selectedPizza = null;
            if (choice == "0")
            {
                Console.WriteLine("До зустрічі! Дякуємо за замовлення!");
                break;
            }
            else if (choice == "1")
            {
                selectedPizza = ChooseExistingPizza(availablePizzas, ingredientService);
            }
            else if (choice == "2")
            {
                selectedPizza = CreateCustomPizza(allIngredients, ingredientService);
            }
            else
            {
                Console.WriteLine("Невірний вибір. Натисніть Enter для повтору...");
                Console.ReadLine();
                continue;
            }

            if (selectedPizza == null)
                continue; // user cancelled

            // Вкажіть дистанцію та оберіть доставку
            decimal distance;
            while (true)
            {
                Console.Write("Вкажіть дистанцію до вашої адреси (км): ");
                var distInput = Console.ReadLine();
                if (decimal.TryParse(distInput, out distance) && distance > 0)
                    break;
                Console.WriteLine("Невірна дистанція. Спробуйте ще раз.");
            }
            ChooseDelivery(distance, selectedPizza);

            Console.WriteLine("\nБажаєте зробити ще одне замовлення? (так/ні): ");
            if (!AskYesNo())
            {
                Console.WriteLine("Дякуємо! Гарного дня!");
                break;
            }
        }
    }

    // Метод повертає вибрану або скомплектовану піцу, або null при відміні
    static IPizza ChooseExistingPizza(List<Pizza> pizzas, IngredientStorageService ingredientsService)
    {
        if (!pizzas.Any())
        {
            Console.WriteLine("Вибраних піц немає.");
            Console.ReadLine();
            return null;
        }

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Доступні піци:");
            for (int i = 0; i < pizzas.Count; i++)
                Console.WriteLine($"{i + 1}. {pizzas[i].Name} - {pizzas[i].BasePrice:F2} грн");
            Console.WriteLine("0. Відміна");
            Console.Write("Ваш вибір: ");
            var inp = Console.ReadLine()?.Trim();
            if (!int.TryParse(inp, out int idx) || idx < 0 || idx > pizzas.Count)
            {
                Console.WriteLine("Невірний вибір. Натисніть Enter для повтору...");
                Console.ReadLine();
                continue;
            }
            if (idx == 0) return null;

            var basePizza = pizzas[idx - 1];
            Console.WriteLine($"Ви обрали: {basePizza}");

            IPizza pizza = new BasicPizza(basePizza);
            if (AskYesNo("Додати топінги? (так/ні): "))
            {
                var toppings = ingredientsService.GetToppings() ?? new List<Ingredient>();
                if (!toppings.Any()) Console.WriteLine("Топінги недоступні.");
                else
                {
                    var chosen = new List<Ingredient>();
                    while (AskTopings(toppings, chosen)) { }
                    if (chosen.Any()) pizza = new ToppingsDecorator(pizza, chosen);
                }
            }
            Console.WriteLine("\nГотова піца:");
            Console.WriteLine(pizza.GetDescription());
            Console.WriteLine($"Ціна: {pizza.GetPrice():F2} грн");
            Console.WriteLine("Натисніть Enter для продовження...");
            Console.ReadLine();
            return pizza;
        }
    }

    static bool AskTopings(List<Ingredient> toppings, List<Ingredient> chosen)
    {
        Console.WriteLine("Доступні топінги:");
        for (int i = 0; i < toppings.Count; i++)
            Console.WriteLine($"{i + 1}. {toppings[i].Name} ({toppings[i].Price:F2} грн)");
        Console.WriteLine("Введіть номери через кому або 0 для завершення:");
        var inp = Console.ReadLine()?.Trim();
        if (inp == "0" || string.IsNullOrWhiteSpace(inp)) return false;
        foreach (var part in inp.Split(','))
        {
            if (int.TryParse(part.Trim(), out int i) && i > 0 && i <= toppings.Count)
            {
                var t = toppings[i - 1];
                if (!chosen.Contains(t)) { chosen.Add(t); Console.WriteLine($"Додано: {t.Name}"); }
            }
            else Console.WriteLine($"Невірний індекс: {part}");
        }
        Console.WriteLine("Додати ще топінги? (так/ні): ");
        return AskYesNo();
    }

    static IPizza CreateCustomPizza(List<Ingredient> ingredients, IngredientStorageService ingredientsService)
    {
        if (!ingredients.Any())
        {
            Console.WriteLine("Інгредієнти недоступні.");
            Console.ReadLine();
            return null;
        }
        var builder = new PizzaBuilder(ingredients);

        // Назва
        while (true)
        {
            Console.Write("Назва піци: ");
            var n = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(n)) { Console.WriteLine("Не може бути порожнім."); continue; }
            builder.SetName(n);
            break;
        }
        var domainPizza = builder.Build();  // повернув тип Pizza
                                            
        IPizza pizza = new BasicPizza(domainPizza);

        // Підсумок
        Console.WriteLine("\nВаша піца:");
        Console.WriteLine(pizza.GetDescription());
        Console.WriteLine($"Ціна: {pizza.GetPrice():F2} грн");
        Console.ReadLine();
        return pizza;
    }

    static void ChooseDelivery(decimal? distanceKm, IPizza pizza)
    {
        Console.Clear();
        Console.WriteLine("Поточна піца для доставки:");
        Console.WriteLine(pizza.GetDescription());
        Console.WriteLine($"Ціна піци: {pizza.GetPrice():F2} грн");

        var service = new DeliveryService();
        var strategies = service.GetAvailableStrategies(distanceKm) ?? new List<IDeliveryStrategy>();
        if (!strategies.Any())
        {
            Console.WriteLine("Доставка недоступна."); Console.ReadLine(); return;
        }

        Console.WriteLine("Оберіть спосіб доставки:");
        for (int i = 0; i < strategies.Count; i++)
            Console.WriteLine($"{i + 1}. {strategies[i].GetDeliveryInfo()}");
        Console.WriteLine("0. Скасувати");

        int choice;
        while (!int.TryParse(Console.ReadLine(), out choice) || choice < 0 || choice > strategies.Count)
            Console.WriteLine("Невірний ввід, спробуйте ще раз.");
        if (choice == 0) return;

        var strat = strategies[choice - 1];
        var costDelivery = strat.CalculateCost(distanceKm);
        var time = strat.GetDeliveryTime(distanceKm);
        Console.WriteLine($"Вартість доставки: {costDelivery:F2} грн, час: {time.TotalMinutes:F0} хв");

        // Загальна вартість
        var pricePizza = pizza.GetPrice();
        var totalPrice = pricePizza + (double)costDelivery;
        Console.WriteLine($"Загальна вартість: {pricePizza:F2} грн ({costDelivery:F2} грн доставка) - {totalPrice:F2} грн");

        if (AskYesNo("Підтвердити доставку? (так/ні): "))
            Console.WriteLine("Замовлення оформлено.");
        else
            Console.WriteLine("Доставка скасована.");

    }
    static bool AskYesNo(string prompt = "(так/ні): ")
    {
        while (true)
        {
            Console.Write(prompt);
            var a = Console.ReadLine()?.Trim().ToLower();
            if (a == "так" || a == "т") return true;
            if (a == "ні" || a == "н") return false;
            Console.WriteLine("Введіть 'так' або 'ні'.");
        }
    }
}
