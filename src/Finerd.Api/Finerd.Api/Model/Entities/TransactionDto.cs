namespace Finerd.Api.Model.Entities
{
    public partial class TransactionDto : EntityKey
    {
        public int TransactionTypeId { get; set; }
        public string? Type { get; set; }
        public int CategoryId { get; set; }
        public string? Category { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedAt { get; set; }   
        public string? Photo { get; set; }


        public int? PaymentMethodId { get; set; }
        public string? PaymentMethod { get; set; }

        public int UserId { get; set; }
    }
}

