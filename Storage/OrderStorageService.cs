using System.Text.Encodings.Web;
using System.Text.Json;
using TeamLab.Domain;

namespace TeamLab.Storage
{
    public class OrderStorageService
    {
        private readonly string _filePath;

        public OrderStorageService(string filePath)
        {
            _filePath = filePath;
        }

        public void SaveOrder(Order order)
        {
            List<Order> orders;

            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                orders = JsonSerializer.Deserialize<List<Order>>(json) ?? new List<Order>();
            }
            else
            {
                orders = new List<Order>();
            }

            orders.Add(order);

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            string updatedJson = JsonSerializer.Serialize(orders, options);
            File.WriteAllText(_filePath, updatedJson);
        }
    }

}
