using Finerd.Api.Model.Entities;

namespace Finerd.Api.Model.Responses
{
    public class GetTransactionResponse : BaseResponse
    {
        public List<Transaction> Transactions { get; set; }
    }
}
