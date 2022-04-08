using System.Text.Json.Serialization;

namespace Finerd.Api.Model.Responses
{
    public class DeleteTaskResponse : BaseResponse
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public int TaskId { get; set; }
    }
}
