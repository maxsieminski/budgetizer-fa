using Microsoft.Azure.Functions.Worker;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Budgetizer.Utils.Cosmos;
using Budgetizer.Utils.Models;
using Microsoft.Azure.Cosmos;

namespace Budgetizer.Functions 
{
    public class GetShops() 
    {
        private static readonly string containerName = "shops";
        

        [Function("GetShops")]
        public static async Task<IActionResult> GetAll([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "shops")] HttpRequest req) 
        {
            CosmosEntry<Shop>[] shops = await CosmosDbManager.GetAllItems<Shop>(containerName);
            return new OkObjectResult(shops);
        }   

        [Function("GetShop")]
        public static async Task<IActionResult> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "shops/{id}")] HttpRequest req, string id) 
        {
            try 
            {
                CosmosEntry<Shop> shop = await CosmosDbManager.GetItem<Shop>(containerName, id);
                return new OkObjectResult(shop);
            }
            catch (CosmosException exception)
            {
                if (exception.StatusCode.Equals(404))
                {
                    return new NotFoundObjectResult(null);
                }
                return new StatusCodeResult(500);
            }
        }

        [Function("CreateShop")]
        public static async Task<IActionResult> Create([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "shops")] HttpRequest req)
        {
            CosmosEntry<Shop> shop = await CosmosDbManager.CreateItem<Shop>(containerName, req.Body);
            return new OkObjectResult(shop);
        }

        [Function("PatchShop")]
        public static async Task<IActionResult> Patch([HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "shops/{id}")] HttpRequest req, string id)
        {
            CosmosEntry<Shop> shop = await CosmosDbManager.PatchItem<Shop>(containerName, id, req.Body);
            return new OkObjectResult(shop);
        }

        [Function("DeleteShop")]
        public static async Task<StatusCodeResult> Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "shops/{id}")] HttpRequest req, string id)
        {
            await CosmosDbManager.DeleteItem<Shop>(containerName, id);
            return new NoContentResult();
        }
    }
}