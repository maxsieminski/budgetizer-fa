using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Budgetizer.Utils.Cosmos;
using Budgetizer.Utils.Models;

namespace Budgetizer.Functions 
{
    public class GetProducts() 
    {
        private static readonly string containerName = "products";
        

        [Function("GetProducts")]
        public static async Task<IActionResult> GetAll([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products")] HttpRequest req) 
        {
            CosmosEntry<Product>[] products = await CosmosDbManager.GetAllItems<Product>(containerName);
            return new OkObjectResult(products);
        }   

        [Function("GetProduct")]
        public static async Task<IActionResult> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products/{id}")] HttpRequest req, string id) 
        {
            try 
            {
                CosmosEntry<Product> product = await CosmosDbManager.GetItem<Product>(containerName, id);
                return new OkObjectResult(product);
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

        [Function("CreateProduct")]
        public static async Task<IActionResult> Create([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "products")] HttpRequest req)
        {
            CosmosEntry<Product> product = await CosmosDbManager.CreateItem<Product>(containerName, req.Body);
            return new OkObjectResult(product);
        }

        [Function("PatchProduct")]
        public static async Task<IActionResult> Patch([HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "products/{id}")] HttpRequest req, string id)
        {
            CosmosEntry<Product> product = await CosmosDbManager.PatchItem<Product>(containerName, id, req.Body);
            return new OkObjectResult(product);
        }

        [Function("DeleteProduct")]
        public static async Task<StatusCodeResult> Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "products/{id}")] HttpRequest req, string id)
        {
            await CosmosDbManager.DeleteItem<Product>(containerName, id);
            return new NoContentResult();
        }
    }
}