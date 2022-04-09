using Finerd.Api.Data;
using Finerd.Api.Model.Entities;
using Finerd.Api.Model.Responses;
using Microsoft.EntityFrameworkCore;

namespace Finerd.Api.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public TransactionService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<GetTransactionResponse> GetById(int transactionId)
        {
            var transactions = await _applicationDbContext.Transactions.Where(o => o.Id == transactionId).ToListAsync();
            if (transactions.Count == 0)
            {
                return new GetTransactionResponse
                {
                    Success = false,
                    Error = "No transaction found for this user",
                    ErrorCode = "T04"
                };
            }
            return new GetTransactionResponse { Success = true, Transactions = transactions };
        }

        public async Task<GetTransactionResponse> Get(int userId)
        {
            var transactions = await _applicationDbContext.Transactions.Where(o => o.UserId == userId).ToListAsync();
            if (transactions.Count == 0)
            {
                return new GetTransactionResponse
                {
                    Success = false,
                    Error = "No transaction found for this user",
                    ErrorCode = "T04"
                };
            }
            return new GetTransactionResponse { Success = true, Transactions = transactions };
        }

        public async Task<SaveTransactionResponse> Save(Transaction transaction, int userId)
        {
            if (transaction.UserId != userId)
            {
                return new SaveTransactionResponse
                {
                    Success = false,
                    Error = "You don't have access to update this transaction",
                    ErrorCode = "T01"
                };
            }
            if (transaction.Id > 0)
            {
                var model = await _applicationDbContext.Transactions.FirstOrDefaultAsync(o => o.Id == transaction.Id);
                if (model == null)
                {
                    return new SaveTransactionResponse
                    {
                        Success = false,
                        Error = "Transaction doest not exist. Can not update.",
                        ErrorCode = "T02"
                    };
                }
                model.Amount = transaction.Amount;
                model.CategoryId = transaction.CategoryId;
                model.PaymentMethodId = transaction.PaymentMethodId;
                model.TransactionTypeId = transaction.TransactionTypeId;
                if (!string.IsNullOrEmpty(transaction.Photo))
                    model.Photo = transaction.Photo;
                model.UpdatedAt = DateTime.UtcNow;
                _applicationDbContext.Transactions.Update(model);
            }
            else {
                transaction.UpdatedAt = DateTime.UtcNow;
                transaction.CreatedAt = DateTime.UtcNow;
                await _applicationDbContext.Transactions.AddAsync(transaction);
            }
            var saveResponse = await _applicationDbContext.SaveChangesAsync();
            if (saveResponse >= 0)
            {
                return new SaveTransactionResponse
                {
                    Success = true,
                    Transaction = transaction
                };
            }
            return new SaveTransactionResponse
            {
                Success = false,
                Error = "Unable to save transaction",
                ErrorCode = "T05"
            };
        }


        public async Task<DeleteEntityResponse> Delete(int transactionId, int userId)
        {
            var transaction = await _applicationDbContext.Transactions.FindAsync(transactionId);
            if (transaction == null)
            {
                return new DeleteEntityResponse
                {
                    Success = false,
                    Error = "Transaction not found",
                    ErrorCode = "T01"
                };
            }
            if (transaction.UserId != userId)
            {
                return new DeleteEntityResponse
                {
                    Success = false,
                    Error = "You don't have access to delete this transaction",
                    ErrorCode = "T02"
                };
            }
            _applicationDbContext.Transactions.Remove(transaction);
            var saveResponse = await _applicationDbContext.SaveChangesAsync();
            if (saveResponse >= 0)
            {
                return new DeleteEntityResponse
                {
                    Success = true,
                    Id = transaction.Id
                };
            }
            return new DeleteEntityResponse
            {
                Success = false,
                Error = "Unable to delete transaction",
                ErrorCode = "T03"
            };
        }

    }
}
