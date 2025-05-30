﻿using TeamLab.Domain;
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
        if (ingredients == null || !ingredients.Any())
        {
            Console.WriteLine("Інгредієнти недоступні для створення піци.");
            Console.ReadLine();
            return null;
        }

        var builder = new PizzaBuilder(ingredients);

        // Назва піци
        while (true)
        {
            Console.Write("Введіть назву піци: ");
            var name = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Назва не може бути порожньою. Спробуйте ще раз.");
                continue;
            }
            builder.SetName(name);
            break;
        }

        // Вибір тіста
        var doughs = ingredients.Where(i => i.Type == IngredientType.Dough).ToList();
        if (!doughs.Any())
            Console.WriteLine("Вибір тіста недоступний.");
        else
        {
            Console.WriteLine("Оберіть тісто (0 для випадкового):");
            for (int i = 0; i < doughs.Count; i++)
                Console.WriteLine($"{i + 1}. {doughs[i].Name} ({doughs[i].Price:F2} грн)");
            while (true)
            {
                Console.Write("Ваш вибір: ");
                var input = Console.ReadLine()?.Trim();
                if (!int.TryParse(input, out var idx) || idx < 0 || idx > doughs.Count)
                {
                    Console.WriteLine($"Невірний ввід. Введіть число від 0 до {doughs.Count}.");
                    continue;
                }
                if (idx > 0)
                    builder.SetDough(doughs[idx - 1].Name);
                break;
            }
        }

        // Вибір соусу
        var sauces = ingredients.Where(i => i.Type == IngredientType.Sauce).ToList();
        if (!sauces.Any())
            Console.WriteLine("Вибір соусу недоступний.");
        else
        {
            Console.WriteLine("Оберіть соус (0 для пропуску):");
            for (int i = 0; i < sauces.Count; i++)
                Console.WriteLine($"{i + 1}. {sauces[i].Name} ({sauces[i].Price:F2} грн)");
            while (true)
            {
                Console.Write("Ваш вибір: ");
                var input = Console.ReadLine()?.Trim();
                if (!int.TryParse(input, out var idx) || idx < 0 || idx > sauces.Count)
                {
                    Console.WriteLine($"Невірний ввід. Введіть число від 0 до {sauces.Count}.");
                    continue;
                }
                if (idx > 0)
                    builder.SetSauce(sauces[idx - 1].Name);
                break;
            }
        }
        static bool AskToppings(List<Ingredient> toppings, List<Ingredient> chosen)
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

        // Додавання топінгів
        var toppings = ingredients.Where(i => i.Type == IngredientType.Topping).ToList();
        if (toppings.Any() && AskYesNo("Бажаєте додати топінги? (так/ні): "))
        {
            var selected = new List<Ingredient>();
            while (AskToppings(toppings, selected)) { }
            if (selected.Any())
                builder.SetToppings(selected.Select(t => t.Name).ToList());
        }

        // Видалення топінгів
        if (builder.GetToppings().Any() && AskYesNo("Бажаєте видалити топінги? (так/ні): "))
        {
            while (true)
            {
                var current = builder.GetToppings();
                Console.WriteLine("Поточні топінги: " + string.Join(", ", current));
                Console.Write("Введіть назву топінга для видалення або Enter для завершення: ");
                var rem = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(rem))
                    break;
                if (current.Contains(rem))
                {
                    builder.RemoveTopping(rem);
                    Console.WriteLine($"Топінг '{rem}' видалено.");
                }
                else
                    Console.WriteLine($"Топінг '{rem}' не знайдено.");
            }
        }

        // Побудова піци та повернення через IPizza
        var domainPizza = builder.Build();
        IPizza pizza = new BasicPizza(domainPizza);

        // Підсумок
        Console.WriteLine("Ваша піца готова: ");
    
        Console.WriteLine(pizza.GetDescription());
        Console.WriteLine($"Ціна: {pizza.GetPrice():F2} грн");
        Console.ReadLine();
        return pizza;
    }

    static void ChooseDelivery(decimal? distanceKm, IPizza pizza)
    {
        string path = Path.Combine(AppContext.BaseDirectory, @"..\..\..\Storage\Data\orders.json");
        path = Path.GetFullPath(path);
        var storage = new OrderStorageService(path);

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
        {
            if (pizza != null && time != TimeSpan.Zero && totalPrice != 0)
            {
                storage.SaveOrder(new Order
                {
                    OrderItem = pizza.GetDescription(),
                    DeliveryInfo = strat.GetDeliveryInfo(),
                    DeliveryTime = DateTime.Now + time,
                    TotalPrice = totalPrice
                });
            }
            Console.WriteLine("Замовлення оформлено.");
        }
        else
        {
            Console.WriteLine("Доставка скасована.");
            return;
        }

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
