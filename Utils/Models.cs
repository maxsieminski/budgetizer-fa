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