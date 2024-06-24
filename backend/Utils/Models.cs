namespace Budgetizer.Utils.Models 
{
    public class CosmosEntry<T>
    {
        public required string id { get; set; }
        public required T item { get; set; }
    }

    public class Product
    {
        public required string name { get; set; }
        public required string producer { get; set; }
    }

    public class Expense {
        public required string name { get; set; }
        public Shop? shop { get; set; }
        public required string? shopId { get; set; } // cosmos field
        public required decimal value { get; set; }
        public required ExpenseProduct[] products { get; set; }
        public required string timestamp { get; set; }
    }

    public class ExpenseProduct {
        public Product? product { get; set; }
        public string? productId { get; set; } // cosmos field
        public required decimal value { get; set; }
        public required int quantity { get; set; }
    }

    public class Shop
    {
        public required string name { get; set; }
        public required Address address { get; set; }
    }

    public class Address
    {
        public required string street { get; set; }
        public required string number { get; set; }
        public required string zip { get; set; }
        public required string city { get; set; }
        public required string country { get; set; }
    }
}