using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Budgetizer.Utils.Cosmos;
using Budgetizer.Utils.Models;

namespace Budgetizer.Functions 
{
    public class GetExpenses() 
    {
        private static readonly string containerName = "expenses";
        

        [Function("GetExpenses")]
        public static async Task<IActionResult> GetAll([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "expenses")] HttpRequest req) 
        {
            CosmosEntry<Expense>[] expenses = await CosmosDbManager.GetAllItems<Expense>(containerName);

            foreach (var expense in expenses)
            {
                if (expense.item.shopId != null)
                {
                    var expenseShop = await CosmosDbManager.GetItem<Shop>("shops", expense.item.shopId);
                    expense.item.shop = expenseShop.item;
                }
            }

            return new OkObjectResult(expenses);
        }   

        [Function("GetExpense")]
        public static async Task<IActionResult> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "expenses/{id}")] HttpRequest req, string id) 
        {
            try 
            {
                CosmosEntry<Expense> expense = await CosmosDbManager.GetItem<Expense>(containerName, id);

                if (expense.item.shopId != null)
                {
                    var expenseShop = await CosmosDbManager.GetItem<Shop>("shops", expense.item.shopId);
                    expense.item.shop = expenseShop.item;
                }

                foreach (var expenseProduct in expense.item.products)
                {
                    string productId = expenseProduct.productId ?? "";
                    if (productId != "")
                    {
                        var product = await CosmosDbManager.GetItem<Product>("products", productId);
                        expenseProduct.product = product.item;
                    }
                }

                return new OkObjectResult(expense);
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

        [Function("CreateExpense")]
        public static async Task<IActionResult> Create([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "expenses")] HttpRequest req)
        {
            CosmosEntry<Expense> expense = await CosmosDbManager.CreateItem<Expense>(containerName, req.Body);
            return new OkObjectResult(expense);
        }

        [Function("PatchExpense")]
        public static async Task<IActionResult> Patch([HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "expenses/{id}")] HttpRequest req, string id)
        {
            CosmosEntry<Expense> expense = await CosmosDbManager.PatchItem<Expense>(containerName, id, req.Body);
            return new OkObjectResult(expense);
        }

        [Function("DeleteExpense")]
        public static async Task<StatusCodeResult> Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "expenses/{id}")] HttpRequest req, string id)
        {
            await CosmosDbManager.DeleteItem<Expense>(containerName, id);
            return new NoContentResult();
        }
    }
}