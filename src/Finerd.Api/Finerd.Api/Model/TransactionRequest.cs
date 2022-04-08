namespace Finerd.Api.Model
{
    public class TransactionRequest
    {
        public string Name { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime Ts { get; set; }
    }
}
