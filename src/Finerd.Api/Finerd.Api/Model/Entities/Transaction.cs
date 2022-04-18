namespace Finerd.Api.Model.Entities
{
    public partial class Transaction: EntityKey
    {
        public int TransactionTypeId { get; set; }
        public int CategoryId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }       
        public string Photo { get; set; }


        //public int? PaymentMethodId { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}

