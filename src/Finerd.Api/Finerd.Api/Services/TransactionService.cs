using Finerd.Api.Data;
using Finerd.Api.Model.Entities;
using Finerd.Api.Model.Responses;
using Microsoft.EntityFrameworkCore;

namespace Finerd.Api.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext tasksDbContext;

        public TransactionService(ApplicationDbContext tasksDbContext)
        {
            this.tasksDbContext = tasksDbContext;
        }

        public async Task<DeleteTaskResponse> Delete(int taskId, int userId)
        {
            var task = await tasksDbContext.Transactions.FindAsync(taskId);

            if (task == null)
            {
                return new DeleteTaskResponse
                {
                    Success = false,
                    Error = "Task not found",
                    ErrorCode = "T01"
                };
            }

            if (task.UserId != userId)
            {
                return new DeleteTaskResponse
                {
                    Success = false,
                    Error = "You don't have access to delete this task",
                    ErrorCode = "T02"
                };
            }

            tasksDbContext.Transactions.Remove(task);

            var saveResponse = await tasksDbContext.SaveChangesAsync();

            if (saveResponse >= 0)
            {
                return new DeleteTaskResponse
                {
                    Success = true,
                    TaskId = task.Id
                };
            }

            return new DeleteTaskResponse
            {
                Success = false,
                Error = "Unable to delete task",
                ErrorCode = "T03"
            };
        }

        public async Task<GetTransactionResponse> Get(int userId)
        {
            var tasks = await tasksDbContext.Transactions.Where(o => o.UserId == userId).ToListAsync();

            if (tasks.Count == 0)
            {
                return new GetTransactionResponse
                {
                    Success = false,
                    Error = "No tasks found for this user",
                    ErrorCode = "T04"
                };
            }

            return new GetTransactionResponse { Success = true, Transactions = tasks };

        }

        public async Task<SaveTransactionResponse> Save(Transaction task)
        {
            await tasksDbContext.Transactions.AddAsync(task);

            var saveResponse = await tasksDbContext.SaveChangesAsync();

            if (saveResponse >= 0)
            {
                return new SaveTransactionResponse
                {
                    Success = true,
                    Transaction = task
                };
            }
            return new SaveTransactionResponse
            {
                Success = false,
                Error = "Unable to save task",
                ErrorCode = "T05"
            };
        }
    }
}
