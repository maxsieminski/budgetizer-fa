using System.Reflection;
using System.Text.Json;

using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

using Budgetizer.Utils.Models;

namespace Budgetizer.Utils.Cosmos 
{
    public static class CosmosDbManager
    {
        private static readonly CosmosClient cosmosClient = new(Environment.GetEnvironmentVariable("CosmosDbConnectionString"));
        private static readonly string databaseName = Defs.databaseName;
        private static Container? container;

        public static async Task<CosmosEntry<T>> GetItem<T>(string containerName, string id)
        {
            container = cosmosClient.GetContainer(databaseName, containerName);
            var item = await container.ReadItemAsync<CosmosEntry<T>>(id, new PartitionKey(id));
            return item.Resource;
        }

        public static async Task<CosmosEntry<T>[]> GetAllItems<T>(string containerName)
        {
            container = cosmosClient.GetContainer(databaseName, containerName);
            var iterator = container.GetItemLinqQueryable<CosmosEntry<T>>().ToFeedIterator();
            return (CosmosEntry<T>[])await iterator.ReadNextAsync();
        }

        public static async Task<CosmosEntry<T>> CreateItem<T>(string containerName, Stream item) 
        {
            container = cosmosClient.GetContainer(databaseName, containerName);
            T? createItem = await ParseItem<T>(item);

            if (createItem == null)
            {
                throw new JsonException("Invalid item provided for parsing");
            }

            CosmosEntry<T> cosmosItem = new()
            {
                id = Guid.NewGuid().ToString(),
                item = createItem
            };

            return await container.CreateItemAsync(cosmosItem);
        }

        public static async Task<CosmosEntry<T>> PatchItem<T>(string containerName, string id, Stream item) 
        {
            string requestBody = await new StreamReader(item).ReadToEndAsync();
            JsonDocument patchItem = JsonDocument.Parse(requestBody);

            container = cosmosClient.GetContainer(databaseName, containerName);
            CosmosEntry<T> currentItem = await GetItem<T>(containerName, id);

            if (patchItem == null || currentItem == null || currentItem.item == null)
            {
                throw new JsonException("Invalid item provided for parsing or entry not found.");
            }

            CosmosEntry<T> updatedItem = MergeItems<T>(patchItem, currentItem.item);
            return await container.ReplaceItemAsync(updatedItem, currentItem.id, new PartitionKey(currentItem.id));
        }

        public static async Task DeleteItem<T>(string containerName, string id)
        {
            container = cosmosClient.GetContainer(databaseName, containerName);
            await container.DeleteItemAsync<CosmosEntry<T>>(id, new PartitionKey(id));
        }

        private static async Task<T?> ParseItem<T>(Stream item)
        {
            string requestBody = await new StreamReader(item).ReadToEndAsync();
            return JsonSerializer.Deserialize<T>(requestBody);
        }

        private static CosmosEntry<T> MergeItems<T>(JsonDocument newItem, T currentItem)
        {
            Type currentValueType = currentItem.GetType();

            foreach (var newProperty in newItem.RootElement.EnumerateObject())
            {
                PropertyInfo? currentProperty = currentValueType.GetProperty(newProperty.Name);
                if (currentProperty != null) 
                {
                    if (currentProperty.GetType() == typeof(object))
                    {
                        return MergeItems<T>()
                    }

                    if (currentProperty.CanWrite)
                    {
                        object? value = JsonSerializer.Deserialize(newProperty.Value.GetRawText(), currentProperty.PropertyType);
                        if (value != null)
                        {
                            currentProperty.SetValue(currentValue, value);
                        }
                    }
                }
            }
            return currentItem;
        }
    }
}