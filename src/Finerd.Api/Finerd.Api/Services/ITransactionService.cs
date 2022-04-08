using Finerd.Api.Model.Entities;
using Finerd.Api.Model.Responses;

namespace Finerd.Api.Services
{
    public interface ITransactionService
    {
        Task<GetTransactionResponse> Get(int userId);
        Task<SaveTransactionResponse> Save(Transaction transaction);
        Task<DeleteTaskResponse> Delete(int transactionId, int userId);
    }
}
