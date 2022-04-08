using Finerd.Api.Model.Entities;

namespace Finerd.Api.Model.Responses
{
    public class SaveTransactionResponse : BaseResponse
    {
        public Transaction Transaction { get; set; }
    }
}
