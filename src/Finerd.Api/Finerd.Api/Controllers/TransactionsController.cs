using Finerd.Api.Model;
using Finerd.Api.Model.Entities;
using Finerd.Api.Model.Responses;
using Finerd.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Finerd.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : BaseApiController
    {
        private readonly ITransactionService TransactionService;
        private readonly ILogger<HeathController> _logger;

        public TransactionsController(ITransactionService TransactionService, ILogger<HeathController> logger)
        {
            this.TransactionService = TransactionService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var getTasksResponse = await TransactionService.Get(UserID);

            if (!getTasksResponse.Success)
            {
                return UnprocessableEntity(getTasksResponse);
            }

            var tasksResponse = getTasksResponse.Transactions;

            return Ok(getTasksResponse.Transactions);
        }


        [HttpPost]
        public async Task<IActionResult> Post(Transaction transaction)
        {
            transaction.CreatedAt = DateTime.UtcNow;
            transaction.UpdatedAt = DateTime.UtcNow;

            var saveTaskResponse = await TransactionService.Save(transaction);

            if (!saveTaskResponse.Success)
            {
                return UnprocessableEntity(saveTaskResponse);
            }

            return Ok(saveTaskResponse.Transaction);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleteTaskResponse = await TransactionService.Delete(id, UserID);
            if (!deleteTaskResponse.Success)
            {
                return UnprocessableEntity(deleteTaskResponse);
            }

            return Ok(deleteTaskResponse.TaskId);
        }
    }
}
