using Finerd.Api.Model.Entities;
using Finerd.Api.Model.Responses;

namespace Finerd.Api.Services
{
    public interface ITransactionService
    {
        Task<GetTransactionResponse> Get(int userId);
        Task<GetTransactionResponse> GetById(int transactionId);
        Task<SaveTransactionResponse> Save(Transaction transaction, int userId);
        Task<DeleteEntityResponse> Delete(int transactionId, int userId);
    }
}
