using System.Text.Json.Serialization;

namespace Finerd.Api.Model.Responses
{
    public class DeleteEntityResponse : BaseResponse
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int Id { get; set; }
    }
}
